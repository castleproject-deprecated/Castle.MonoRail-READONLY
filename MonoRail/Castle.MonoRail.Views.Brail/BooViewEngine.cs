// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Views.Brail
{
	using System;
	using System.Collections;
	using System.Configuration;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Web;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.IO;
	using Boo.Lang.Compiler.Pipelines;
	using Boo.Lang.Compiler.Steps;
	using Boo.Lang.Parser;
	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Views;

    public class BooViewEngine : ViewEngineBase, IInitializable
	{
		// This field holds all the cache of all the compiled types (not instances)
		// of all the views that Brail nows of.
		private Hashtable compilations = Hashtable.Synchronized(
			new Hashtable(
#if DOTNET2
				StringComparer.InvariantCultureIgnoreCase
#else
				CaseInsensitiveHashCodeProvider.Default,
				CaseInsensitiveComparer.Default
#endif
				));

		// used to hold the constructors of types, so we can avoid using
		// Activator (which takes a long time
		private Hashtable constructors = new Hashtable();

		// This is used to add a reference to the common scripts for each compiled scripts
		private Assembly common;
		private ILogger logger;
		private static BooViewEngineOptions options;
		private string baseSavePath;

		private static void InitializeConfig()
		{
			InitializeConfig("brail");
			if (options == null)
				InitializeConfig("Brail");
			if (options == null)
				options = new BooViewEngineOptions();
		}

		private static void InitializeConfig(string sectionName)
		{
#if DOTNET2
			options = ConfigurationManager.GetSection(sectionName) as BooViewEngineOptions;
#else
			options = System.Configuration.ConfigurationSettings.GetConfig(sectionName) as BooViewEngineOptions;
#endif
		}

		private void Log(string msg, params object[] items)
		{
			if (logger == null || logger.IsDebugEnabled == false)
				return;
			logger.DebugFormat(msg, items);
		}

		public void Initialize()
		{
			if (options == null)
				InitializeConfig();
			string baseDir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
			Log("Base Directory: " + baseDir);
			baseSavePath = Path.Combine(baseDir, options.SaveDirectory);
			Log("Base Save Path: " + baseSavePath);

			if (options.SaveToDisk && !Directory.Exists(baseSavePath))
			{
				Directory.CreateDirectory(baseSavePath);
				Log("Created directory " + baseSavePath);
			}

			CompileCommonScripts();

			ViewSourceLoader.ViewChanged += new FileSystemEventHandler(OnViewChanged);
		}

		private void OnViewChanged(object sender, FileSystemEventArgs e)
		{
			if (e.FullPath.IndexOf(options.CommonScriptsDirectory) != -1)
			{
				Log("Detected a change in commons scripts directory " + options.CommonScriptsDirectory + ", recompiling site");
				// need to invalidate the entire CommonScripts assembly
				// not worrying about concurrency here, since it is assumed
				// that changes here are rare. Note, this force a recompile of the 
				// whole site.
				CompileCommonScripts();
				return;
			}
			Log("Detected a change in ${e.Name}, removing from complied cache");
			// Will cause a recompilation
			compilations[e.Name] = null;
		}

		// Get configuration options if they exists, if they do not exist, load the default ones
		// Create directory to save the compiled assemblies if required.
		// pre-compile the common scripts
		public override void Service(IServiceProvider serviceProvider)
		{
			base.Service(serviceProvider);
			ILoggerFactory loggerFactory = serviceProvider.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
			if (loggerFactory == null)
				logger = loggerFactory.Create(GetType().Name);
		}

		public override bool HasTemplate(string templateName)
		{
			return ViewSourceLoader.HasTemplate(GetTemplateName(templateName));
		}

		// Process a template name and output the results to the user
		// This may throw if an error occured and the user is not local (which would 
		// cause the yellow screen of death)
		public override void Process(IRailsEngineContext context, Controller controller, string templateName)
		{
			Process(context.Response.Output, context, controller, templateName);
		}

		public override void Process(TextWriter output, IRailsEngineContext context, Controller controller,
		                             string templateName)
		{
			Log("Starting to process request for {0}", templateName);
			string file = GetTemplateName(templateName);
			BrailBase view;
			// Output may be the layout's child output if a layout exists
			// or the context.Response.Output if the layout is null
			LayoutViewOutput layoutViewOutput = GetOutput(output, context, controller);
			// Will compile on first time, then save the assembly on the cache.
			view = GetCompiledScriptInstance(file, layoutViewOutput.Output, context, controller);
			controller.PreSendView(view);
			Log("Executing view {0}", templateName);
			view.Run();
			if (layoutViewOutput.Layout != null)
			{
				layoutViewOutput.Layout.SetParent(view);
				layoutViewOutput.Layout.Run();
			}
			Log("Finished executing view {0}", templateName);
			controller.PostSendView(view);
		}

		// Send the contents text directly to the user, only adding the layout if neccecary
		public override void ProcessContents(IRailsEngineContext context, Controller controller, string contents)
		{
			LayoutViewOutput layoutViewOutput = GetOutput(controller.Response.Output, context, controller);
			layoutViewOutput.Output.Write(contents);
			// here we don't need to pass parameters from the layout to the view, 
			if (layoutViewOutput.Layout != null)
				layoutViewOutput.Layout.Run();
		}

		// Check if a layout has been defined. If it was, then the layout would be created
		// and will take over the output, otherwise, the context.Reposne.Output is used, 
		// and layout is null
		private LayoutViewOutput GetOutput(TextWriter output, IRailsEngineContext context, Controller controller)
		{
			BrailBase layout = null;
			if (controller.LayoutName != null)
			{
				string layoutTemplate = "layouts\\" + controller.LayoutName;
				string layoutFilename = GetTemplateName(layoutTemplate);
				layout = GetCompiledScriptInstance(layoutFilename, output,
				                                   context, controller);
				output = layout.ChildOutput = new StringWriter();
			}
			return new LayoutViewOutput(output, layout);
		}

		public string GetTemplateName(string templateName)
		{
			return templateName + options.Extention;
		}

		// This takes a filename and return an instance of the view ready to be used.
		// If the file does not exist, an exception is raised
		// The cache is checked to see if the file has already been compiled, and it had been
		// a check is made to see that the compiled instance is newer then the file's modification date.
		// If the file has not been compiled, or the version on disk is newer than the one in memory, a new
		// version is compiled.
		// Finally, an instance is created and returned	
		public BrailBase GetCompiledScriptInstance(string file,
		                                           TextWriter output,
		                                           IRailsEngineContext context,
		                                           Controller controller)
		{
			bool batch = options.BatchCompile;
			// normalize filename - replace / or \ to the system path seperator
			string filename = file.Replace('/', Path.DirectorySeparatorChar)
				.Replace('\\', Path.DirectorySeparatorChar);
			Log("Getting compiled instnace of {0}", filename);
			Type type;
			if (compilations.ContainsKey(filename))
			{
				type = (Type) compilations[filename];
				if (type != null)
				{
					Log("Got compiled instnace of ${filename} from cache");
					return CreateBrailBase(context, controller, output, type);
				}
				// if file is in compilations and the type is null,
				// this means that we need to recompile. Since this usually means that 
				// the file was changed, we'll set batch to false and procceed to compile just
				// this file.
				Log("Cache miss! Need to recompile {0}", filename);
				batch = false;
			}
			type = CompileScript(filename, batch);
			if (type == null)
			{
				throw new RailsException("Could not find a view with path " + filename);
			}
			return CreateBrailBase(context, controller, output, type);
		}

		private BrailBase CreateBrailBase(IRailsEngineContext context, Controller controller, TextWriter output, Type type)
		{
			ConstructorInfo constructor = (ConstructorInfo) constructors[type];
			BrailBase self = (BrailBase) FormatterServices.GetUninitializedObject(type);
			constructor.Invoke(self, new object[] {this, output, context, controller});
			return self;
		}

		// Compile a script (or all scripts in a directory), save the compiled result
		// to the cache and return the compiled type.
		// If an error occurs in batch compilation, then an attempt is made to compile just the single
		// request file.
		public Type CompileScript(string filename, bool batch)
		{
            ICompilerInput[] inputs = GetInput(filename, batch);
			string name = NormalizeName(filename);
			Log("Compiling {0} to {1} with batch: {2}", filename, name, batch);
			CompilationResult result = DoCompile(inputs, name);
			
			if (result.Context.Errors.Count > 0)
			{
				if (batch == false)
				{
					string errors = result.Context.Errors.ToString(true);
					Log("Failed to compile ${0} because ${1}", filename, errors);
					StringBuilder msg = new StringBuilder();
					msg.Append("Error during compile:")
						.Append(Environment.NewLine)
						.Append(errors)
						.Append(Environment.NewLine);

                    foreach (ICompilerInput input in inputs)
					{
                        msg.Append("Input (").Append(input.Name).Append(")")
                            .Append(Environment.NewLine);
						msg.Append(result.Processor.GetInputCode(input))
						    .Append(Environment.NewLine);
					}
					throw new RailsException(msg.ToString());
				}
				//error compiling a batch, let's try a single file
				return CompileScript(filename, false);
			}
			Type type;
            foreach (ICompilerInput input in inputs)
			{
				string typeName = Path.GetFileNameWithoutExtension(input.Name) + "_BrailView";
				type = result.Context.GeneratedAssembly.GetType(typeName);
				Log("Adding ${type.FullName} to the cache");
                compilations[input.Name] = type;
				constructors[type] = type.GetConstructor(new Type[]
				                                         	{
				                                         		typeof(BooViewEngine),
				                                         		typeof(TextWriter),
				                                         		typeof(IRailsEngineContext),
				                                         		typeof(Controller)
				                                         	});
			}
			type = (Type) compilations[filename];
			return type;
		}

		// If batch compilation is set to true, this would return all the view scripts
		// in the director (not recursive!)
		// Otherwise, it would return just the single file
        private ICompilerInput[] GetInput(string filename, bool batch)
		{
			if (batch == false)
                return new ICompilerInput[] { CreateInput(filename) };
			ArrayList inputs = new ArrayList();
			// use the System.IO.Path to get the folder name even though
			// we are using the ViewSourceLoader to load the actual file
			string directory = Path.GetDirectoryName(filename);
			foreach(string file in ViewSourceLoader.ListViews(directory))
			{
                inputs.Add(CreateInput(file));
			}
            return (ICompilerInput[])inputs.ToArray(typeof(ICompilerInput));
		}

		// create an input from a resource name
		public ICompilerInput CreateInput(string name)
		{
			IViewSource viewSrc = ViewSourceLoader.GetViewSource(name);
			if (viewSrc == null)
			{
				throw new RailsException("{0} is not a valid view", name);
			}
			// I need to do it this way because I can't tell 
			// when to dispose of the stream. 
			// It is not expected that this will be a big problem, the string
			// will go away after the compile is done with them.
			using(StreamReader stream = new StreamReader(viewSrc.OpenViewStream()))
			{ 
                return new StringInput(name, stream.ReadToEnd());
			}
		}

		// Perform the actual compilation of the scripts
		// Things to note here:
		// * The generated assembly reference the Castle.MonoRail.MonoRailBrail and Castle.MonoRail.Framework assemblies
		// * If a common scripts assembly exist, it is also referenced
		// * The AddBrailBaseClassStep compiler step is added - to create a class from the view's code
		// * The ProcessMethodBodiesWithDuckTyping is replaced with ReplaceUknownWithParameters
		//   this allows to use naked parameters such as (output context.IsLocal) without using 
		//   any special syntax
		// * The IntroduceGlobalNamespaces step is removed, to allow to use common variables such as 
		//   date & list without accidently using the Boo.Lang.BuiltIn versions
        private CompilationResult DoCompile(ICompilerInput[] files, string name)
		{
			BooCompiler compiler = SetupCompiler(files);
			string filename = Path.Combine(baseSavePath, name);
			compiler.Parameters.OutputAssembly = filename;
			// this is here and not in SetupCompiler since CompileCommon is also
			// using SetupCompiler, and we don't want reference to the old common from the new one
			if (common != null)
				compiler.Parameters.References.Add(common);
			// pre procsssor needs to run before the parser
			BrailPreProcessor processor = new BrailPreProcessor();
			compiler.Parameters.Pipeline.Insert(0, processor);
			// inserting the add class step after the parser
			compiler.Parameters.Pipeline.Insert(2, new TransformToBrailStep());
			compiler.Parameters.Pipeline.Replace(typeof(ProcessMethodBodiesWithDuckTyping), new ReplaceUknownWithParameters());
			compiler.Parameters.Pipeline.Replace(typeof(InitializeTypeSystemServices), new InitializeCustomTypeSystem());
			compiler.Parameters.Pipeline.RemoveAt(compiler.Parameters.Pipeline.Find(typeof(IntroduceGlobalNamespaces)));

			return new CompilationResult(compiler.Run(), processor);
		}

		// Return the output filename for the generated assembly
		// The filename is dependant on whatever we are doing a batch
		// compile or not, if it's a batch compile, then the directory name
		// is used, if it's just a single file, we're using the file's name.
		// '/' and '\' are replaced with '_', I'm not handling ':' since the path
		// should never include it since I'm converting this to a relative path
		public string NormalizeName(string filename)
		{
			string name = filename;
			name = name.Replace(Path.AltDirectorySeparatorChar, '_');
			name = name.Replace(Path.DirectorySeparatorChar, '_');

			return name + "_BrailView.dll";
		}

		// Compile all the common scripts to a common assemblies
		// an error in the common scripts would raise an exception.
		public bool CompileCommonScripts()
		{
			if (options.CommonScriptsDirectory == null)
				return false;

			// the demi.boo is stripped, but GetInput require it.
			string demiFile = Path.Combine(options.CommonScriptsDirectory, "demi.boo");
			ICompilerInput[] inputs = GetInput(demiFile, true);
			BooCompiler compiler = SetupCompiler(inputs);
			string outputFile = Path.Combine(baseSavePath, "CommonScripts.dll");
			compiler.Parameters.OutputAssembly = outputFile;
			CompilerContext result = compiler.Run();
			if (result.Errors.Count > 0)
				throw new RailsException(result.Errors.ToString(true));
			common = result.GeneratedAssembly;
			compilations.Clear();
			return true;
		}

		// common setup for the compiler
        private BooCompiler SetupCompiler(ICompilerInput[] files)
		{
			BooCompiler compiler = new BooCompiler();
			compiler.Parameters.Ducky = true;
			compiler.Parameters.Debug = options.Debug;
			if (options.SaveToDisk)
				compiler.Parameters.Pipeline = new CompileToFile();
			else
				compiler.Parameters.Pipeline = new CompileToMemory();
			// replace the normal parser with white space agnostic one.
			compiler.Parameters.Pipeline.RemoveAt(0);
			compiler.Parameters.Pipeline.Insert(0, new WSABooParsingStep());
            foreach (ICompilerInput file in files)
			{
				compiler.Parameters.Input.Add(file);
			}
			foreach(Assembly assembly in options.AssembliesToReference)
			{
				compiler.Parameters.References.Add(assembly);
			}
			compiler.Parameters.OutputType = CompilerOutputType.Library;
			return compiler;
		}

		public string ViewRootDir
		{
			get { return ViewSourceLoader.ViewRootDir; }
		}

		public BooViewEngineOptions Options
		{
			get { return options; }
		}

		private class LayoutViewOutput
		{
			private BrailBase layout;
			private TextWriter output;

			public LayoutViewOutput(TextWriter output, BrailBase layout)
			{
				this.layout = layout;
				this.output = output;
			}

			public BrailBase Layout
			{
				get { return layout; }
			}

			public TextWriter Output
			{
				get { return output; }
			}
		}
		
		private class CompilationResult
		{
			CompilerContext context;
			BrailPreProcessor processor;

			public CompilerContext Context
			{
				get { return context; }
			}

			public BrailPreProcessor Processor
			{
				get { return processor; }
			}

			public CompilationResult(CompilerContext context, BrailPreProcessor processor)
			{
				this.context = context;
				this.processor = processor;
			}
		}
	}
}