﻿// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Services
{
	using ActiveRecordSupport;
	using Castle.MonoRail.Framework.Services.AjaxProxyGenerator;
	using JSON;
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class PrototypeAjaxProxyGeneratorTestCase
	{
		private PrototypeAjaxProxyGenerator generator;
		private StubEngineContext engineContext;
		private StubViewEngineManager engStubViewEngineManager;
		private StubResponse response;

		[SetUp]
		public void Init()
		{
			StubRequest request = new StubRequest();
			response = new StubResponse();
			StubMonoRailServices services = new StubMonoRailServices();
			engStubViewEngineManager = new StubViewEngineManager();
			services.ViewEngineManager = engStubViewEngineManager;
			engineContext = new StubEngineContext(request, response, services, new UrlInfo("area", "controller", "action"));

			generator = new PrototypeAjaxProxyGenerator();
			generator.ControllerTree = services.ControllerTree;
			generator.ControllerDescriptorBuilder = services.ControllerDescriptorProvider;

			services.ControllerTree.AddController("area", "controller1", typeof(NoAjaxController));
			services.ControllerTree.AddController("", "controller2", typeof(AjaxController));
		}

		[Test]
		public void GenerateJSProxy_DoesNotGenerateAnythingForControllerThatHasNoAjaxAction()
		{
			string js = generator.GenerateJSProxy(engineContext, "proxyName", "area", "controller1");

			Assert.AreEqual("\r\n<script type=\"text/javascript\">\r\n" +
				"var proxyName =\r\n{\r\n};\r\n</script>\r\n", js);
		}

		[Test]
		public void GenerateJSProxy_GeneratesProxyOnlyForAjaxActions()
		{
			string js = generator.GenerateJSProxy(engineContext, "proxyName", "", "controller2");

			Assert.AreEqual("\r\n<script type=\"text/javascript\">\r\n" +
				"var proxyName =\r\n{\r\n\t" +

				"action1: function(callback)\r\n\t{\r\n\t\t" + 
				"var r=new Ajax.Request('/controller2/Action1', " +
				"{method: 'get', asynchronous: !!callback, onComplete: callback, parameters: '_='}); \r\n\t\t" +
				"if(!callback) return r.transport.responseText;\r\n\t}\r\n,\r\n\t" +
				
				"action2: function(name, age, callback)\r\n\t{\r\n\t\t" +
				"var r=new Ajax.Request('/controller2/Action2', {method: 'post', asynchronous: !!callback, onComplete: callback, " +
				"parameters: '_=&name='+name+'&age='+age+''}); \r\n\t\t" + 
				"if(!callback) return r.transport.responseText;\r\n\t}\r\n,\r\n\t" +

				"actionWithARFetch: function(personId, age, callback)\r\n\t{\r\n\t\t" +
				"var r=new Ajax.Request('/controller2/ActionWithARFetch', {method: 'post', asynchronous: !!callback, onComplete: callback, " +
				"parameters: '_=&personId='+personId+'&age='+age+''}); \r\n\t\t" +
				"if(!callback) return r.transport.responseText;\r\n\t}\r\n,\r\n\t" +

				"actionReturnJSON: function(test, callback)\r\n\t{\r\n\t\t" +
				"var r=new Ajax.Request('/controller2/ActionReturnJSON', {method: 'get', asynchronous: !!callback, onComplete: callback, parameters: '_=&test='+Object.toJSON(test)+''}); \r\n\t\t" +
				"if(!callback) return r.transport.responseText;\r\n\t}\r\n,\r\n\t" +

				"actionReturnJSONWithEntryKey: function(test, callback)\r\n\t{\r\n\t\t" +
				"var r=new Ajax.Request('/controller2/ActionReturnJSONWithEntryKey', {method: 'get', asynchronous: !!callback, onComplete: callback, parameters: '_=&entryKey='+Object.toJSON(test)+''}); \r\n\t\t" +
				"if(!callback) return r.transport.responseText;\r\n" +
				"\t}\r\n};\r\n" +

				"</script>\r\n", js);
		}

		#region Controllers

		internal class NoAjaxController : Controller
		{
			public void Index()
			{
			}
		}

		internal class AjaxController : Controller
		{
			public void Index()
			{
			}

			[AjaxAction]
			public void Action1()
			{
			}

			[AjaxAction, AccessibleThrough(Verb.Post)]
			public void Action2(string name, int age)
			{
			}

			[AjaxAction, AccessibleThrough(Verb.Post)]
			public void ActionWithARFetch([ARFetch("personId")] Person person, int age)
			{
			}

			[AjaxAction]
			public void ActionReturnJSON([JSONBinder]string test) {
			}

			[AjaxAction]
			public void ActionReturnJSONWithEntryKey([JSONBinder("entryKey")]string test) {
			}

		}

		#endregion
	}
}
