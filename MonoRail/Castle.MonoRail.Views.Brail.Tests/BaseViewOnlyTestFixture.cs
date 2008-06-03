// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.Brail.Tests
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using Castle.MonoRail.Framework.Descriptors;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.JSGeneration;
	using Castle.MonoRail.Framework.JSGeneration.Prototype;
	using Castle.MonoRail.Framework.Resources;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Test;
	using Castle.MonoRail.Views.Brail.TestSite.Controllers;
	using Framework;
	using NUnit.Framework;

	public abstract class BaseViewOnlyTestFixture
	{
		protected string viewSourcePath;
		protected ControllerContext ControllerContext;
		protected HelperDictionary Helpers;
		private string lastOutput;
		protected string[] Layouts;
		protected MockEngineContext MockEngineContext;
		protected Hashtable PropertyBag;
		protected string Area = null;
		protected string ControllerName = "test_controller";
		protected string Action = "test_action";
		protected DefaultViewComponentFactory ViewComponentFactory;
		protected BooViewEngine BooViewEngine;

		protected string Layout
		{
			set { Layouts = new string[] { value }; }
		}

		public BaseViewOnlyTestFixture()
			: this(ViewLocations.TestSiteBrail)
		{
		}

		public BaseViewOnlyTestFixture(string viewSource)
		{
			viewSourcePath = viewSource;
		}


		public string ViewSourcePath
		{
			get { return viewSourcePath; }
		}

		[SetUp]
		public void SetUp()
		{
			Layout = null;
			PropertyBag = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			Helpers = new HelperDictionary();
			MockServices services = new MockServices();
			services.UrlBuilder = new DefaultUrlBuilder(new MockServerUtility(), new MockRoutingEngine());
			services.UrlTokenizer = new DefaultUrlTokenizer();
			UrlInfo urlInfo = new UrlInfo(
				"example.org", "test", "/TestBrail", "http", 80,
				"http://test.example.org/test_area/test_controller/test_action.tdd",
				Area, ControllerName, Action, "tdd", "no.idea");
			MockEngineContext = new MockEngineContext(new MockRequest(), new MockResponse(), services,
													  urlInfo);
			MockEngineContext.AddService<IUrlBuilder>(services.UrlBuilder);
			MockEngineContext.AddService<IUrlTokenizer>(services.UrlTokenizer);

			ViewComponentFactory = new DefaultViewComponentFactory();
			ViewComponentFactory.Service(MockEngineContext);
			ViewComponentFactory.Initialize();

			MockEngineContext.AddService<IViewComponentFactory>(ViewComponentFactory);
			ControllerContext = new ControllerContext();
			ControllerContext.Helpers = Helpers;
			ControllerContext.PropertyBag = PropertyBag;
			MockEngineContext.CurrentControllerContext = ControllerContext;


			Helpers["urlhelper"] = Helpers["url"] = new UrlHelper(MockEngineContext);
			Helpers["htmlhelper"] = Helpers["html"] = new HtmlHelper(MockEngineContext);
			Helpers["dicthelper"] = Helpers["dict"] = new DictHelper(MockEngineContext);
			Helpers["DateFormatHelper"] = Helpers["DateFormat"] = new DateFormatHelper(MockEngineContext);

			string viewPath = Path.Combine(viewSourcePath, "Views");

			FileAssemblyViewSourceLoader loader = new FileAssemblyViewSourceLoader(viewPath);
			loader.AddAssemblySource(
				new AssemblySourceInfo(Assembly.GetExecutingAssembly().FullName,
									   "Castle.MonoRail.Views.Brail.Tests.ResourcedViews"));

			BooViewEngine = new BooViewEngine();
			BooViewEngine.Options = new BooViewEngineOptions();
			BooViewEngine.Options.SaveDirectory = Environment.CurrentDirectory;
			BooViewEngine.Options.SaveToDisk = false;
			BooViewEngine.Options.Debug = true;
			BooViewEngine.Options.BatchCompile = false;

			BooViewEngine.SetViewSourceLoader(loader);
			BooViewEngine.Initialize();

			BeforEachTest();
		}

		protected virtual void BeforEachTest()
		{

		}


		public void ProcessView_StripRailsExtension(string url)
		{
			ProcessView(url.Replace(".rails", ""));
		}

		protected string ProcessView(string templatePath)
		{
			StringWriter sw = new StringWriter();
			ControllerContext.LayoutNames = Layouts;
			MockEngineContext.CurrentControllerContext = ControllerContext;
			BooViewEngine.Process(templatePath, sw, MockEngineContext, null, ControllerContext);
			lastOutput = sw.ToString();
			return lastOutput;
		}

		protected string ProcessViewJS(string templatePath)
		{
			StringWriter sw = new StringWriter();
			ControllerContext.LayoutNames = Layouts;
			MockEngineContext.CurrentControllerContext = ControllerContext;
			DefaultViewEngineManager engineManager = new DefaultViewEngineManager();
			engineManager.RegisterEngineForView(BooViewEngine);
			engineManager.RegisterEngineForExtesionLookup((BooViewEngine));
			JSCodeGenerator codeGenerator =
				  new JSCodeGenerator(MockEngineContext.Server,
					  engineManager,
					  MockEngineContext, null, ControllerContext, MockEngineContext.Services.UrlBuilder);

			IJSGenerator jsGen = new PrototypeGenerator(codeGenerator);

			codeGenerator.JSGenerator = jsGen;

			JSCodeGeneratorInfo info = new JSCodeGeneratorInfo(codeGenerator, jsGen,
				new object[] { new ScriptaculousExtension(codeGenerator) },
				new object[] { new ScriptaculousExtension(codeGenerator) });

			BooViewEngine.GenerateJS(templatePath, sw, info, MockEngineContext, null, ControllerContext);
			lastOutput = sw.ToString();
			return lastOutput;
		}

		protected void AddResource(string name, string resourceName, Assembly asm)
		{
			IResourceFactory resourceFactory = new DefaultResourceFactory();
			ResourceDescriptor descriptor = new ResourceDescriptor(
				null,
				name,
				resourceName,
				null,
				null);
			IResource resource = resourceFactory.Create(
				descriptor,
				asm);
			ControllerContext.Resources.Add(name, resource);
		}

		protected string RenderStaticWithLayout(string staticText)
		{
			ControllerContext.LayoutNames = Layouts;
			MockEngineContext.CurrentControllerContext = ControllerContext;

			BooViewEngine.RenderStaticWithinLayout(staticText, MockEngineContext, null, ControllerContext);
			lastOutput = ((StringWriter)MockEngineContext.Response.Output)
				.GetStringBuilder().ToString();
			return lastOutput;
		}

		public void AssertReplyEqualTo(string expected)
		{
			Assert.AreEqual(expected, lastOutput);
		}

		public void AssertReplyContains(string contained)
		{
			StringAssert.Contains(contained, lastOutput);
		}
	}
}