// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace NVelocity.Runtime
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using Commons.Collections;
	using Directive;
	using Log;
	using NVelocity.Exception;
	using NVelocity.Runtime.Parser.Node;
	using NVelocity.Util.Introspection;
	using Resource;
	using Util;

	/// <summary> 
	/// This is the Runtime system for Velocity. It is the
	/// single access point for all functionality in Velocity.
	/// It adheres to the mediator pattern and is the only
	/// structure that developers need to be familiar with
	/// in order to get Velocity to perform.
	/// 
	/// The Runtime will also cooperate with external
	/// systems like Turbine. Runtime properties can
	/// set and then the Runtime is initialized.
	/// 
	/// Turbine for example knows where the templates
	/// are to be loaded from, and where the velocity
	/// log file should be placed.
	/// 
	/// So in the case of Velocity cooperating with Turbine
	/// the code might look something like the following:
	/// 
	/// <code>
	/// Runtime.setProperty(Runtime.FILE_RESOURCE_LOADER_PATH, templatePath);
	/// Runtime.setProperty(Runtime.RUNTIME_LOG, pathToVelocityLog);
	/// Runtime.init();
	/// </code>
	/// 
	/// <pre>
	/// -----------------------------------------------------------------------
	/// N O T E S  O N  R U N T I M E  I N I T I A L I Z A T I O N
	/// -----------------------------------------------------------------------
	/// Runtime.init()
	///
	/// If Runtime.init() is called by itself the Runtime will
	/// initialize with a set of default values.
	/// -----------------------------------------------------------------------
	/// Runtime.init(String/Properties)
	/// 
	/// In this case the default velocity properties are layed down
	/// first to provide a solid base, then any properties provided
	/// in the given properties object will override the corresponding
	/// default property.
	/// -----------------------------------------------------------------------
	/// </pre>
	/// 
	/// </summary>
	public class RuntimeInstance : IRuntimeServices
	{
		private DefaultTraceListener debugOutput = new DefaultTraceListener();

		/// <summary>
		/// VelocimacroFactory object to manage VMs
		/// </summary>
		private VelocimacroFactory vmFactory = null;

		/// <summary>
		/// The Runtime parser pool
		/// </summary>
		private SimplePool<Parser.Parser> parserPool;

		/// <summary>
		/// Indicate whether the Runtime has been fully initialized.
		/// </summary>
		private bool initialized;

		/// <summary>
		/// These are the properties that are laid down over top
		/// of the default properties when requested.
		/// </summary>
		private ExtendedProperties overridingProperties = null;

		/// <summary>
		/// Object that houses the configuration options for
		/// the velocity runtime. The ExtendedProperties object allows
		/// the convenient retrieval of a subset of properties.
		/// For example all the properties for a resource loader
		/// can be retrieved from the main ExtendedProperties object
		/// using something like the following:
		/// 
		/// <code>
		/// ExtendedProperties loaderConfiguration =
		/// configuration.subset(loaderID);
		/// </code>
		/// 
		/// And a configuration is a lot more convenient to deal
		/// with then conventional properties objects, or Maps.
		/// </summary>
		private readonly ExtendedProperties configuration;

		private IResourceManager resourceManager = null;

		/// <summary>
		/// Each runtime instance has it's own introspector
		/// to ensure that each instance is completely separate.
		/// </summary>
		private readonly Introspector introspector = null;


		/// <summary>
		/// Opaque reference to something specified by the 
		/// application for use in application supplied/specified
		/// pluggable components.
		/// </summary>
		private readonly Hashtable applicationAttributes = null;

		private IUberspect uberSpect;

		private IDirectiveManager directiveManager;

		public RuntimeInstance()
		{
			// logSystem = new PrimordialLogSystem();
			configuration = new ExtendedProperties();

			// create a VM factory, resource manager
			// and introspector
			vmFactory = new VelocimacroFactory(this);

			// make a new introspector and initialize it
			introspector = new Introspector(this);

			// and a store for the application attributes
			applicationAttributes = new Hashtable();
		}

		public ExtendedProperties Configuration
		{
			get { return configuration; }

			set
			{
				if (overridingProperties == null)
				{
					overridingProperties = value;
				}
				else
				{
					// Avoid possible ConcurrentModificationException
					if (overridingProperties != value)
					{
						overridingProperties.Combine(value);
					}
				}
			}
		}

		public Introspector Introspector
		{
			get { return introspector; }
		}

		public void Init()
		{
			lock(this)
			{
				if (initialized == false)
				{
					initializeProperties();
					initializeLogger();
					initializeResourceManager();
					initializeDirectives();
					initializeParserPool();
					initializeIntrospection();

					// initialize the VM Factory.  It will use the properties 
					// accessible from Runtime, so keep this here at the end.
					vmFactory.InitVelocimacro();

					initialized = true;
				}
			}
		}

		/// <summary>
		/// Gets the classname for the Uberspect introspection package and
		/// instantiates an instance.
		/// </summary>
		private void initializeIntrospection()
		{
			String rm = GetString(RuntimeConstants.UBERSPECT_CLASSNAME);

			if (rm != null && rm.Length > 0)
			{
				Object o;

				try
				{
					o = SupportClass.CreateNewInstance(Type.GetType(rm));
				}
				catch(System.Exception)
				{
					String err =
						string.Format(
							"The specified class for Uberspect ({0}) does not exist (or is not accessible to the current classlaoder.", rm);
					Error(err);
					throw new System.Exception(err);
				}

				if (!(o is IUberspect))
				{
					String err =
						string.Format(
							"The specified class for Uberspect ({0}) does not implement org.apache.velocity.util.introspector.Uberspect. Velocity not initialized correctly.",
							rm);

					Error(err);
					throw new System.Exception(err);
				}

				uberSpect = (IUberspect) o;

				if (uberSpect is UberspectLoggable)
				{
					((UberspectLoggable) uberSpect).RuntimeLogger = this;
				}

				uberSpect.Init();
			}
			else
			{
				// someone screwed up.  Lets not fool around...
				String err =
					"It appears that no class was specified as the Uberspect.  Please ensure that all configuration information is correct.";

				Error(err);
				throw new System.Exception(err);
			}
		}

		/// <summary>
		/// Initializes the Velocity Runtime with properties file.
		/// The properties file may be in the file system proper,
		/// or the properties file may be in the classpath.
		/// </summary>
		private void setDefaultProperties()
		{
			try
			{
				// TODO: this was modified in v1.4 to use the classloader
				configuration.Load(
					Assembly.GetExecutingAssembly().GetManifestResourceStream(RuntimeConstants.DEFAULT_RUNTIME_PROPERTIES));
			}
			catch(System.Exception ex)
			{
				debugOutput.WriteLine(string.Format("Cannot get NVelocity Runtime default properties!\n{0}", ex.Message));
				debugOutput.Flush();
			}
		}

		/// <summary>
		/// Allows an external system to set a property in
		/// the Velocity Runtime.
		/// </summary>
		/// <param name="key">property key </param>
		/// <param name="value">property value</param>
		public void SetProperty(String key, Object value)
		{
			if (overridingProperties == null)
			{
				overridingProperties = new ExtendedProperties();
			}

			overridingProperties.SetProperty(key, value);
		}

		/// <summary>
		/// Add a property to the configuration. If it already
		/// exists then the value stated here will be added
		/// to the configuration entry.
		/// <remarks>
		/// For example, if
		/// <c>resource.loader = file</c>
		/// is already present in the configuration and you
		/// <c>addProperty("resource.loader", "classpath")</c>
		/// 
		/// Then you will end up with a <see cref="IList"/> like the
		/// following:
		/// 
		/// <c>["file", "classpath"]</c>
		/// </remarks>
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		public void AddProperty(String key, Object value)
		{
			if (overridingProperties == null)
			{
				overridingProperties = new ExtendedProperties();
			}

			overridingProperties.AddProperty(key, value);
		}

		/// <summary>
		/// Clear the values pertaining to a particular
		/// property.
		/// </summary>
		/// <param name="key">key of property to clear</param>
		public void ClearProperty(String key)
		{
			if (overridingProperties != null)
			{
				overridingProperties.ClearProperty(key);
			}
		}

		/// <summary>
		/// Allows an external caller to get a property.
		/// <remarks>
		/// The calling routine is required to know the type, as this routine
		/// will return an Object, as that is what properties can be.
		/// </remarks>
		/// </summary>
		/// <param name="key">property to return</param>
		public Object GetProperty(String key)
		{
			return configuration.GetProperty(key);
		}

		/// <summary>
		/// Initialize Velocity properties, if the default
		/// properties have not been laid down first then
		/// do so. Then proceed to process any overriding
		/// properties. Laying down the default properties
		/// gives a much greater chance of having a
		/// working system.
		/// </summary>
		private void initializeProperties()
		{
			// Always lay down the default properties first as
			// to provide a solid base.
			if (configuration.IsInitialized() == false)
			{
				setDefaultProperties();
			}

			if (overridingProperties != null)
			{
				configuration.Combine(overridingProperties);
			}
		}

		/// <summary>
		/// Initialize the Velocity Runtime with a Properties
		/// object.
		/// </summary>
		/// <param name="p">Properties</param>
		public void Init(ExtendedProperties p)
		{
			overridingProperties = ExtendedProperties.ConvertProperties(p);
			Init();
		}

		/// <summary>
		/// Initialize the Velocity Runtime with the name of
		/// ExtendedProperties object.
		/// </summary>
		/// <param name="configurationFile">Properties</param>
		public void Init(String configurationFile)
		{
			overridingProperties = new ExtendedProperties(configurationFile);
			Init();
		}

		private void initializeResourceManager()
		{
			// Which resource manager?
			IResourceManager rmInstance = (IResourceManager)
			                              applicationAttributes[RuntimeConstants.RESOURCE_MANAGER_CLASS];

			String rm = GetString(RuntimeConstants.RESOURCE_MANAGER_CLASS);

			if (rmInstance == null && rm != null && rm.Length > 0)
			{
				// if something was specified, then make one.
				// if that isn't a ResourceManager, consider
				// this a huge error and throw
				Object o;

				try
				{
					Type rmType = Type.GetType(rm);
					o = Activator.CreateInstance(rmType);
				}
				catch(System.Exception)
				{
					String err = string.Format("The specified class for ResourceManager ({0}) does not exist.", rm);
					Error(err);
					throw new System.Exception(err);
				}

				if (!(o is IResourceManager))
				{
					String err =
						string.Format(
							"The specified class for ResourceManager ({0}) does not implement ResourceManager. NVelocity not initialized correctly.",
							rm);
					Error(err);
					throw new System.Exception(err);
				}

				resourceManager = (IResourceManager) o;

				resourceManager.Initialize(this);
			}
			else if (rmInstance != null)
			{
				resourceManager = rmInstance;
				resourceManager.Initialize(this);
			}
			else
			{
				// someone screwed up.  Lets not fool around...
				String err =
					"It appears that no class was specified as the ResourceManager.  Please ensure that all configuration information is correct.";
				Error(err);
				throw new System.Exception(err);
			}
		}

		/// <summary> Initialize the Velocity logging system.
		/// *
		/// @throws Exception
		/// </summary>
		private void initializeLogger()
		{
			/*
			 * Initialize the logger. We will eventually move all
			 * logging into the logging manager.
			 */

//			if (logSystem is PrimordialLogSystem)
//			{
//				PrimordialLogSystem pls = (PrimordialLogSystem) logSystem;
//				// logSystem = LogManager.createLogSystem(this);
//				logSystem = new NullLogSystem();
//
//				/*
//				 * in the event of failure, lets do something to let it 
//				 * limp along.
//				 */
//				if (logSystem == null)
//				{
//					logSystem = new NullLogSystem();
//				}
//				else
//				{
//					pls.DumpLogMessages(logSystem);
//				}
//			}
		}


		/// <summary> This methods initializes all the directives
		/// that are used by the Velocity Runtime. The
		/// directives to be initialized are listed in
		/// the RUNTIME_DEFAULT_DIRECTIVES properties
		/// file.
		///
		/// @throws Exception
		/// </summary>
		private void initializeDirectives()
		{
			initializeDirectiveManager();

			/*
			* Initialize the runtime directive table.
			* This will be used for creating parsers.
			*/
			// runtimeDirectives = new Hashtable();

			ExtendedProperties directiveProperties = new ExtendedProperties();

			/*
			* Grab the properties file with the list of directives
			* that we should initialize.
			*/

			try
			{
				directiveProperties.Load(
					Assembly.GetExecutingAssembly().GetManifestResourceStream(RuntimeConstants.DEFAULT_RUNTIME_DIRECTIVES));
			}
			catch(System.Exception ex)
			{
				throw new System.Exception(
					string.Format(
						"Error loading directive.properties! Something is very wrong if these properties aren't being located. Either your Velocity distribution is incomplete or your Velocity jar file is corrupted!\n{0}",
						ex.Message));
			}

			/*
			* Grab all the values of the properties. These
			* are all class names for example:
			*
			* NVelocity.Runtime.Directive.Foreach
			*/
			IEnumerator directiveClasses = directiveProperties.Values.GetEnumerator();

			while(directiveClasses.MoveNext())
			{
				String directiveClass = (String) directiveClasses.Current;
				// loadDirective(directiveClass);
				directiveManager.Register(directiveClass);
			}

			/*
			*  now the user's directives
			*/
			String[] userdirective = configuration.GetStringArray("userdirective");
			for(int i = 0; i < userdirective.Length; i++)
			{
				// loadDirective(userdirective[i]);
				directiveManager.Register(userdirective[i]);
			}
		}

		private void initializeDirectiveManager()
		{
			String directiveManagerTypeName = configuration.GetString("directive.manager");

			if (directiveManagerTypeName == null)
			{
				throw new System.Exception("Looks like there's no 'directive.manager' configured. NVelocity can't go any further");
			}

			directiveManagerTypeName = directiveManagerTypeName.Replace(';', ',');

			Type dirMngType = Type.GetType(directiveManagerTypeName, false, false);

			if (dirMngType == null)
			{
				throw new System.Exception(
					String.Format("The type {0} could not be resolved", directiveManagerTypeName));
			}

			directiveManager = (IDirectiveManager) Activator.CreateInstance(dirMngType);
		}

		/// <summary> Initializes the Velocity parser pool.
		/// This still needs to be implemented.
		/// </summary>
		private void initializeParserPool()
		{
			int numParsers = GetInt(RuntimeConstants.PARSER_POOL_SIZE, RuntimeConstants.NUMBER_OF_PARSERS);

			parserPool = new SimplePool<Parser.Parser>(numParsers);

			for(int i = 0; i < numParsers; i++)
			{
				parserPool.put(CreateNewParser());
			}
		}

		/// <summary> Returns a JavaCC generated Parser.
		/// </summary>
		/// <returns>Parser javacc generated parser
		/// </returns>
		public Parser.Parser CreateNewParser()
		{
			Parser.Parser parser = new Parser.Parser(this);
			parser.Directives = directiveManager;
			return parser;
		}

		/// <summary>
		/// Parse the input and return the root of
		/// AST node structure.
		/// <remarks>
		/// In the event that it runs out of parsers in the
		/// pool, it will create and let them be GC'd
		/// dynamically, logging that it has to do that.  This
		/// is considered an exceptional condition.  It is
		/// expected that the user will set the
		/// <c>PARSER_POOL_SIZE</c> property appropriately for their
		/// application.  We will revisit this.
		/// </remarks>
		/// </summary>
		/// <param name="reader">inputstream retrieved by a resource loader</param>
		/// <param name="templateName">name of the template being parsed</param>
		public SimpleNode Parse(TextReader reader, String templateName)
		{
			// do it and dump the VM namespace for this template
			return Parse(reader, templateName, true);
		}

		/// <summary>
		/// Parse the input and return the root of the AST node structure.
		/// </summary>
		/// <param name="reader">inputstream retrieved by a resource loader</param>
		/// <param name="templateName">name of the template being parsed</param>
		/// <param name="dumpNamespace">flag to dump the Velocimacro namespace for this template</param>
		public SimpleNode Parse(TextReader reader, String templateName, bool dumpNamespace)
		{
			SimpleNode ast = null;
			Parser.Parser parser = (Parser.Parser) parserPool.get();
			bool madeNew = false;

			if (parser == null)
			{
				// if we couldn't get a parser from the pool
				// make one and log it.
				Error(
					"Runtime : ran out of parsers. Creating new.  Please increment the parser.pool.size property. The current value is too small.");

				parser = CreateNewParser();

				if (parser != null)
				{
					madeNew = true;
				}
			}

			// now, if we have a parser
			if (parser == null)
			{
				Error("Runtime : ran out of parsers and unable to create more.");
			}
			else
			{
				try
				{
					// dump namespace if we are told to.  Generally, you want to 
					// do this - you don't in special circumstances, such as 
					// when a VM is getting init()-ed & parsed
					if (dumpNamespace)
					{
						DumpVMNamespace(templateName);
					}

					ast = parser.Parse(reader, templateName);
				}
				finally
				{
					// if this came from the pool, then put back
					if (!madeNew)
					{
						parserPool.put(parser);
					}
				}
			}
			return ast;
		}

		/// <summary>
		/// Returns a <code>Template</code> from the resource manager.
		/// This method assumes that the character encoding of the
		/// template is set by the <code>input.encoding</code>
		/// property.  The default is "ISO-8859-1"
		/// </summary>
		/// <param name="name">The file name of the desired template.
		/// </param>
		/// <returns>The template.</returns>
		/// <exception cref="ResourceNotFoundException">
		/// if template not found from any available source
		/// </exception>
		/// <exception cref="ParseErrorException">
		/// if template cannot be parsed due to syntax (or other) error.
		/// </exception>
		/// <exception cref="Exception">
		/// if an error occurs in template initialization
		/// </exception>
		public Template GetTemplate(String name)
		{
			return GetTemplate(name, GetString(RuntimeConstants.INPUT_ENCODING, RuntimeConstants.ENCODING_DEFAULT));
		}

		/// <summary>
		/// Returns a <code>Template</code> from the resource manager
		/// </summary>
		/// <param name="name">The name of the desired template.</param>
		/// <param name="encoding">Character encoding of the template</param>
		/// <returns>The template.</returns>
		/// <exception cref="ResourceNotFoundException">
		/// if template not found from any available source.
		/// </exception>
		/// <exception cref="ParseErrorException">
		/// if template cannot be parsed due to syntax (or other) error.
		/// </exception>
		/// <exception cref="Exception">
		/// if an error occurs in template initialization
		/// </exception>
		public Template GetTemplate(String name, String encoding)
		{
			return (Template) resourceManager.GetResource(name, ResourceType.Template, encoding);
		}

		/// <summary>
		/// Returns a static content resource from the
		/// resource manager.  Uses the current value
		/// if <c>INPUT_ENCODING</c> as the character encoding.
		/// </summary>
		/// <param name="name">Name of content resource to get</param>
		/// <returns>ContentResource object ready for use</returns>
		/// <exception cref="ResourceNotFoundException">
		/// if template not found from any available source.
		/// </exception>
		public ContentResource GetContent(String name)
		{
			// the encoding is irrelevant as we don't do any conversation
			// the bytestream should be dumped to the output stream
			return GetContent(name, GetString(RuntimeConstants.INPUT_ENCODING, RuntimeConstants.ENCODING_DEFAULT));
		}

		/// <summary>
		/// Returns a static content resource from the
		/// resource manager.
		/// </summary>
		/// <param name="name">Name of content resource to get</param>
		/// <param name="encoding">Character encoding to use</param>
		/// <returns>ContentResource object ready for use</returns>
		/// <exception cref="ResourceNotFoundException">
		/// if template not found from any available source.
		/// </exception>
		public ContentResource GetContent(String name, String encoding)
		{
			return (ContentResource) resourceManager.GetResource(name, ResourceType.Content, encoding);
		}


		/// <summary>
		/// Determines is a template exists, and returns name of the loader that
		/// provides it.  This is a slightly less hokey way to support
		/// the <c>Velocity.templateExists()</c> utility method, which was broken
		/// when per-template encoding was introduced.  We can revisit this.
		/// </summary>
		/// <param name="resourceName">Name of template or content resource</param>
		/// <returns>class name of loader than can provide it</returns>
		public String GetLoaderNameForResource(String resourceName)
		{
			return resourceManager.GetLoaderNameForResource(resourceName);
		}

		/// <summary>
		/// Added this to check and make sure that the configuration
		/// is initialized before trying to get properties from it.
		/// This occurs when there are errors during initialization
		/// and the default properties have yet to be layed down.
		/// </summary>
		private bool showStackTrace()
		{
			if (configuration.IsInitialized())
			{
				return GetBoolean(RuntimeConstants.RUNTIME_LOG_WARN_STACKTRACE, false);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Handle logging.
		/// </summary>
		/// <param name="level">log level</param>
		/// <param name="message">message to log</param>
		private void Log(LogLevel level, Object message)
		{
// 			String output = message.ToString();

			// just log it, as we are guaranteed now to have some
			// kind of logger - save the if()
//			logSystem.LogVelocityMessage(level, output);
		}

		/// <summary>
		/// Log a warning message.
		/// </summary>
		/// <param name="message">message to log</param>
		public void Warn(Object message)
		{
			Log(LogLevel.Warn, message);
		}

		/// <summary>
		/// Log an info message.
		/// </summary>
		/// <param name="message">message to log</param>
		public void Info(Object message)
		{
			Log(LogLevel.Info, message);
		}

		/// <summary>
		/// Log an error message.
		/// </summary>
		/// <param name="message">message to log</param>
		public void Error(Object message)
		{
			Log(LogLevel.Error, message);
		}

		/// <summary>
		/// Log a debug message.
		/// </summary>
		/// <param name="message">message to log</param>
		public void Debug(Object message)
		{
			Log(LogLevel.Debug, message);
		}

		/// <summary>
		/// String property accessor method with default to hide the
		/// configuration implementation.
		/// </summary>
		/// <param name="key">key property key</param>
		/// <param name="defaultValue">default value to return if key not found in resource manager.</param>
		/// <returns>String  value of key or default</returns>
		public String GetString(String key, String defaultValue)
		{
			return configuration.GetString(key, defaultValue);
		}

		/// <summary>
		/// Returns the appropriate VelocimacroProxy object if strVMname
		/// is a valid current Velocimacro.
		/// </summary>
		/// <param name="vmName">Name of velocimacro requested</param>
		/// <param name="templateName">Name of template</param>
		/// <returns>VelocimacroProxy</returns>
		public Directive.Directive GetVelocimacro(String vmName, String templateName)
		{
			return vmFactory.GetVelocimacro(vmName, templateName);
		}

		/// <summary>
		/// Adds a new Velocimacro. Usually called by Macro only while parsing.
		/// </summary>
		/// <param name="name">Name of velocimacro</param>
		/// <param name="macro">String form of macro body</param>
		/// <param name="argArray">Array of strings, containing the #macro() arguments.  the 0th is the name.</param>
		/// <param name="sourceTemplate">Name of template</param>
		/// <returns>
		/// True if added, false if rejected for some
		/// reason (either parameters or permission settings)
		/// </returns>
		public bool AddVelocimacro(String name, String macro, String[] argArray, String sourceTemplate)
		{
			return vmFactory.AddVelocimacro(name, macro, argArray, sourceTemplate);
		}

		/// <summary>
		/// Checks to see if a VM exists
		/// </summary>
		/// <param name="vmName">Name of velocimacro</param>
		/// <param name="templateName">Name of template</param>
		/// <returns>
		/// True if VM by that name exists, false if not
		/// </returns>
		public bool IsVelocimacro(String vmName, String templateName)
		{
			return vmFactory.IsVelocimacro(vmName, templateName);
		}

		/// <summary>
		/// Tells the vmFactory to dump the specified namespace.
		/// This is to support clearing the VM list when in 
		/// <c>inline-VM-local-scope</c> mode.
		/// </summary>
		public bool DumpVMNamespace(String ns)
		{
			return vmFactory.DumpVMNamespace(ns);
		}

		/* --------------------------------------------------------------------
		* R U N T I M E  A C C E S S O R  M E T H O D S
		* --------------------------------------------------------------------
		* These are the getXXX() methods that are a simple wrapper
		* around the configuration object. This is an attempt
		* to make a the Velocity Runtime the single access point
		* for all things Velocity, and allow the Runtime to
		* adhere as closely as possible the the Mediator pattern
		* which is the ultimate goal.
		* --------------------------------------------------------------------
		*/

		/// <summary>
		/// String property accessor method to hide the configuration implementation
		/// </summary>
		/// <param name="key">property key</param>
		/// <returns>value of key or null</returns>
		public String GetString(String key)
		{
			return configuration.GetString(key);
		}

		/// <summary>
		///	Int property accessor method to hide the configuration implementation.
		/// </summary>
		/// <param name="key">property key</param>
		/// <returns>value</returns>
		public int GetInt(String key)
		{
			return configuration.GetInt(key);
		}

		/// <summary>
		/// Int property accessor method to hide the configuration implementation.
		/// </summary>
		/// <param name="key">property key</param>
		/// <param name="defaultValue">default value</param>
		/// <returns>value</returns>
		public int GetInt(String key, int defaultValue)
		{
			return configuration.GetInt(key, defaultValue);
		}

		/// <summary>
		/// Boolean property accessor method to hide the configuration implementation.
		/// </summary>
		/// <param name="key">property key</param>
		/// <param name="def">default value if property not found</param>
		/// <returns>boolean  value of key or default value</returns>
		public bool GetBoolean(String key, bool def)
		{
			return configuration.GetBoolean(key, def);
		}

		/// <summary>
		/// Return the velocity runtime configuration object.
		/// </summary>
		/// <returns>
		/// ExtendedProperties configuration object which houses
		/// the velocity runtime properties.
		/// </returns>
		public Object GetApplicationAttribute(Object key)
		{
			return applicationAttributes[key];
		}

		public Object SetApplicationAttribute(Object key, Object o)
		{
			return applicationAttributes[key] = o;
		}

		/// <summary>
		///	Return the Introspector for this instance
		/// </summary>
		public IUberspect Uberspect
		{
			get { return uberSpect; }
		}
	}
}