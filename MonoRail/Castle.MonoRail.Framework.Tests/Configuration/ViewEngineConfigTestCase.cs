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

namespace Castle.MonoRail.Framework.Tests.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Xml;
	using Castle.MonoRail.Framework.Configuration;
	using NUnit.Framework;

	[TestFixture]
	public class ViewEngineConfigTestCase
	{
		private string viewFolder = AppDomain.CurrentDomain.BaseDirectory;

		[Test]
		public void ShouldProcessAdditonalSourcesElement_IfConfiguringSingleViewEngine()
		{
			string configXml =
				@"
			<monorail>
    <controllers>
      <assembly>Castle.MonoRail.Framework.Tests</assembly>
    </controllers>

    <viewEngine viewPathRoot=""" +
				viewFolder +
				@""">

      <additionalSources>
        <assembly name=""Castle.MonoRail.Framework.Tests"" namespace=""Castle.MonoRail.Framework.Tests.Content"" />
      </additionalSources>
    </viewEngine>
  </monorail>";

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(configXml);
			ViewEngineConfig config = new ViewEngineConfig();
			config.Deserialize(doc.DocumentElement);

			Assert.IsTrue(config.Sources.Count > 0, "additonal sources not loaded");
		}

		[Test]
		public void ShouldProcessAdditionalSourcesElement_IfConfiguringMultipleViewEngines()
		{
			string configXml =
				@"
			<monorail>
    <controllers>
      <assembly>Castle.MonoRail.Framework.Tests</assembly>
    </controllers>

    <viewEngines viewPathRoot=""" +
				viewFolder +
				@""">
		<add type=""Castle.MonoRail.Framework.Tests.Configuration.TestViewEngine, Castle.MonoRail.Framework.Tests"" />
      <additionalSources>
        <assembly name=""Castle.MonoRail.Framework.Tests"" namespace=""Castle.MonoRail.Framework.Tests.Content"" />
      </additionalSources>
    </viewEngines>
  </monorail>";

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(configXml);
			ViewEngineConfig config = new ViewEngineConfig();
			config.Deserialize(doc.DocumentElement);

			Assert.IsTrue(config.Sources.Count > 0, "Additional sources not loaded");
		}
	}

	public class TestViewEngine : ViewEngineBase
	{
		private bool _supportsJSGeneration;
		private string _viewFileExtension;
		private string _jsGeneratorFileExtension;

		public TestViewEngine()
		{
			_supportsJSGeneration = false;
			_viewFileExtension = "test";
			_jsGeneratorFileExtension = "testjs";
		}

		public override bool SupportsJSGeneration
		{
			get { return _supportsJSGeneration; }
		}

		public override string ViewFileExtension
		{
			get { return _viewFileExtension; }
		}

		public override string JSGeneratorFileExtension
		{
			get { return _jsGeneratorFileExtension; }
		}

		public override object CreateJSGenerator(JSCodeGeneratorInfo generatorInfo,
		                                         IEngineContext context, IController controller,
		                                         IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		public override void GenerateJS(string templateName, TextWriter output, JSCodeGeneratorInfo generatorInfo,
		                                IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		public override void Process(string templateName, TextWriter output, IEngineContext context, IController controller,
		                             IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		public override void Process(string templateName, string layoutName, TextWriter output,
		                             IDictionary<string, object> parameters)
		{
			throw new NotImplementedException();
		}

		public override void ProcessPartial(string partialName, TextWriter output, IEngineContext context,
		                                    IController controller, IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		public override void RenderStaticWithinLayout(string contents, IEngineContext context, IController controller,
		                                              IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}
	}
}