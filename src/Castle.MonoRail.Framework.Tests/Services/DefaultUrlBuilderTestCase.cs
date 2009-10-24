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

namespace Castle.MonoRail.Framework.Tests.Services
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Routing;
	using Castle.MonoRail.Framework.Services;
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class DefaultUrlBuilderTestCase
	{
		private DefaultUrlBuilder urlBuilder;

		[SetUp]
		public void Init()
		{
			urlBuilder = new DefaultUrlBuilder();
			urlBuilder.ServerUtil = new StubServerUtility();
			urlBuilder.RoutingEngine = new RoutingEngine();
		}

		[Test]
		public void InheritsControllerAndAreaWhenCreatingUrl()
		{
			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			Assert.AreEqual("/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new")));
		}

		[Test]
		public void OverridingController()
		{
			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			Assert.AreEqual("/cars/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("controller=cars", "action=new")));
		}

		[Test]
		public void OverridingArea()
		{
			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			Assert.AreEqual("/admin/cars/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("area=admin", "controller=cars", "action=new")));
		}

		[Test]
		public void UsesAppPath()
		{
			UrlInfo url = new UrlInfo("", "controller", "action", "/app", ".castle");

			Assert.AreEqual("/app/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new")));
		}

		[Test]
		public void UsesMoreThanASingleLevelAppPath()
		{
			UrlInfo url = new UrlInfo("", "controller", "action", "/app/some", ".castle");

			Assert.AreEqual("/app/some/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new")));
		}

		[Test]
		public void CanHandleEmptyAppPath()
		{
			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			Assert.AreEqual("/controller/edit.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=edit")));
		}

		[Test]
		public void TurningOffUseExtensions()
		{
			urlBuilder.UseExtensions = false;

			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			Assert.AreEqual("/controller/edit",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=edit")));
		}

		[Test]
		public void SupportsQueryInfoAsString()
		{
			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			Assert.AreEqual("/controller/new.castle?something=1",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "querystring=something=1")));
		}

		[Test]
		public void SupportsPathInfoAsDictionary()
		{
			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			HybridDictionary parameters = new HybridDictionary(true);
			parameters["action"] = "new";
			parameters["querystring"] = DictHelper.Create("id=1", "name=john doe");

			Assert.AreEqual("/controller/new.castle?id=1&name=john+doe",
							urlBuilder.BuildUrl(url, parameters));
		}

		[Test]
		public void SupportsPathInfoAsNameValueCollection()
		{
			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			NameValueCollection namedParams = new NameValueCollection();
			namedParams["id"] = "1";
			namedParams["name"] = "john doe";

			HybridDictionary parameters = new HybridDictionary(true);
			parameters["action"] = "new";
			parameters["querystring"] = namedParams;

			Assert.AreEqual("/controller/new.castle?id=1&name=john+doe",
							urlBuilder.BuildUrl(url, parameters));
		}

		[Test]
		public void SupportsSettingPathInfo()
		{
			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			Assert.AreEqual("/controller/new.castle/id/1/name/doe",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "pathinfo=id/1/name/doe")));
		}

		[Test]
		public void SupportsAbsolutePaths()
		{
			UrlInfo url = new UrlInfo("localhost", "", "", "https", 443, "", "area", "controller", "action", ".castle", "");
			Assert.AreEqual("https://localhost/area/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true")));

			url = new UrlInfo("localhost", "", "/app", "https", 443, "", "area", "controller", "action", ".castle", "");
			Assert.AreEqual("https://localhost/app/area/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true")));
		}

		[Test]
		public void SupportsAbsolutePathsWithSubDomains()
		{
			UrlInfo url = new UrlInfo("vpn", "staging", "", "https", 443, "", "area", "controller", "action", ".castle", "");
			Assert.AreEqual("https://staging.vpn/area/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true")));
		}

		[Test]
		public void CanOverrideSubDomain()
		{
			UrlInfo url = new UrlInfo("vpn", "staging", "", "https", 443, "", "area", "controller", "action", ".castle", "");
			Assert.AreEqual("https://intranet.vpn/area/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "subdomain=intranet", "absolute=true")));
		}

		[Test]
		public void CanOverrideDomain()
		{
			UrlInfo url = new UrlInfo("vpn", "staging", "", "https", 443, "", "area", "controller", "action", ".castle", "");
			Assert.AreEqual("https://staging.intranet/area/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "domain=intranet", "absolute=true")));
		}

		[Test]
		public void EncodesToCreateValidHtmlContent()
		{
			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			HybridDictionary parameters = new HybridDictionary(true);
			parameters["action"] = "new";
			parameters["encode"] = "true";
			parameters["querystring"] = DictHelper.Create("id=1", "name=john doe");

			Assert.AreEqual("/controller/new.castle?id=1&amp;name=john+doe",
							urlBuilder.BuildUrl(url, parameters));

			Assert.AreEqual("/controller/new.castle?id=1&amp;name=john+doe",
							urlBuilder.BuildUrl(url,
								DictHelper.Create("encode=true", "action=new", "querystring=id=1&name=john doe")));
		}

		[Test]
		public void PortsAreSkippedForDefaults()
		{
			UrlInfo url = new UrlInfo("localhost", "", "", "https", 443, "", "", "controller", "action", ".castle", "");
			Assert.AreEqual("https://localhost/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true")));

			url = new UrlInfo("localhost", "", "", "http", 80, "", "", "controller", "action", ".castle", "");
			Assert.AreEqual("http://localhost/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true")));

			url = new UrlInfo("localhost", "", "", "http", 8080, "", "", "controller", "action", ".castle", "");
			Assert.AreEqual("http://localhost:8080/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true")));

			url = new UrlInfo("localhost", "", "", "https", 441, "", "", "controller", "action", ".castle", "");
			Assert.AreEqual("https://localhost:441/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true")));
		}

		[Test]
		public void ParameterPortOverridesCurrentPort()
		{
			UrlInfo url = new UrlInfo("localhost", "", "", "http", 80, "", "", "controller", "action", ".castle", "");
			Assert.AreEqual("https://localhost/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true", "protocol=https", "port=443")));

			url = new UrlInfo("localhost", "", "", "http", 80, "", "", "controller", "action", ".castle", "");
			Assert.AreEqual("https://localhost:441/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true", "protocol=https", "port=441")));

			url = new UrlInfo("localhost", "", "", "https", 443, "", "", "controller", "action", ".castle", "");
			Assert.AreEqual("https://localhost/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true", "protocol=https")));

			url = new UrlInfo("localhost", "", "", "https", 443, "", "", "controller", "action", ".castle", "");
			Assert.AreEqual("https://localhost:441/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true", "protocol=https", "port=441")));

			url = new UrlInfo("localhost", "", "", "https", 443, "", "", "controller", "action", ".castle", "");
			Assert.AreEqual("https://localhost/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true")));

			url = new UrlInfo("localhost", "", "", "https", 443, "", "", "controller", "action", ".castle", "");
			Assert.AreEqual("http://localhost/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true", "protocol=http", "port=80")));

			url = new UrlInfo("localhost", "", "", "https", 443, "", "", "controller", "action", ".castle", "");
			Assert.AreEqual("http://localhost:8080/controller/new.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("action=new", "absolute=true", "protocol=http", "port=8080")));
		}

		[Test]
		public void UseBasePathMustDiscardTheAppVirtualDirInfo()
		{
			UrlInfo url = new UrlInfo("area", "controller", "action", "/app", ".castle");

			Assert.AreEqual("http://localhost/theArea/home/index.castle",
							urlBuilder.BuildUrl(url, DictHelper.Create("basepath=http://localhost/",
																		   "area=theArea", "controller=home",
																		   "action=index")));

			Assert.AreEqual("http://localhost/theArea/home/index.castle",
							urlBuilder.BuildUrl(url,
												DictHelper.Create(
													"basepath=http://localhost",
													"area=theArea", "controller=home",
													"action=index")));
		}

		[Test]
		public void UseBasePathMustDiscardTheAreaIfTheValueIsDuplicated()
		{
			UrlInfo url = new UrlInfo("theArea", "controller", "action", "/app", ".castle");

			Assert.AreEqual("http://localhost/theArea/home/index.castle",
							urlBuilder.BuildUrl(url,
												DictHelper.Create(
													"basepath=http://localhost/theArea",
													"area=theArea", "controller=home",
													"action=index")));

			Assert.AreEqual("http://localhost/theArea/home/index.castle",
							urlBuilder.BuildUrl(url,
												DictHelper.Create(
													"basepath=http://localhost/theArea/",
													"area=theArea", "controller=home",
													"action=index")));
		}

		[Test]
		public void UseBasePathWithQuerystring()
		{
			UrlInfo url = new UrlInfo("area", "controller", "action", "/app", ".castle");

			Assert.AreEqual("http://localhost/theArea/home/index.castle?key=value",
							urlBuilder.BuildUrl(url,
												DictHelper.Create(
													"basepath=http://localhost/theArea",
													"area=theArea",
													"controller=home",
													"action=index",
													"querystring=key=value")));
		}

		[Test]
		public void UseAbsPathWithWWW()
		{
			DefaultUrlTokenizer tokenizer = new DefaultUrlTokenizer();
			UrlInfo urlinfo = tokenizer.TokenizeUrl("/area/home/index.castle", null,
								  new Uri("http://www.castleproject.org"), true, string.Empty);


			UrlBuilderParameters parameters = new UrlBuilderParameters("test", "action");
			parameters.CreateAbsolutePath = true;

			Assert.AreEqual("http://www.castleproject.org/area/test/action.castle",
				urlBuilder.BuildUrl(urlinfo, parameters));
		}

		[Test]
		public void RouteParametersShouldBePersistedDuringCreateUrlPartsWhenNoneSpecifiedInParameters()
		{
			UrlInfo urlInfo = new UrlInfo("i", "shouldbe", "overridden", "/", ".castle");

			UrlBuilderParameters parameters = new UrlBuilderParameters();//empty collection
			IDictionary routeParameters = new HybridDictionary();
			routeParameters.Add("area", "routearea");
			routeParameters.Add("controller", "routecontroller");
			routeParameters.Add("action", "routeaction");

			IRoutingEngine routingEngine = new StubRoutingEngine();
			routingEngine.Add(new PatternRoute("default", "<area>/<controller>/<action>"));//keep routing engine from being empty
			urlBuilder.RoutingEngine = routingEngine;

			Assert.AreEqual("/routearea/routecontroller/routeaction",
				urlBuilder.BuildUrl(urlInfo, parameters, routeParameters));

		}

		[Test]
		public void UseCurrentRouteParamsShouldBeHonored()
		{
			UrlInfo urlInfo = new UrlInfo("Services", "CreateServiceWizard", "Step1", String.Empty, String.Empty);

			UrlBuilderParameters parameters = new UrlBuilderParameters();
			parameters.RouteMatch = new RouteMatch();
			parameters.RouteMatch.AddNamed("serviceArea", "Marketing");
			parameters.RouteMatch.AddNamed("action", "Step1");
			parameters.RouteMatch.AddNamed("controller", "CreateServiceWizard");
			parameters.RouteMatch.AddNamed("area", "Services");
			parameters.UseCurrentRouteParams = true;

			IDictionary routeParameters = new HybridDictionary();
			routeParameters.Add("action", "Step2");

			IRoutingEngine routingEngine = new StubRoutingEngine();
			routingEngine.Add(
				new PatternRoute("ServiceWizardCreate", "/Services/<serviceArea>/AddWizard/[action]")
					.DefaultForController().Is("CreateServiceWizard")
					.DefaultForArea().Is("Services")
					.DefaultForAction().Is("start")
				);

			routingEngine.Add(
				new PatternRoute("ServiceWizardModify", "/Services/<serviceArea>/ModifyWizard/[action]")
					.DefaultForController().Is("ModifyServiceWizard")
					.DefaultForArea().Is("Services")
					.DefaultForAction().Is("start")
				);
			urlBuilder.RoutingEngine = routingEngine;

			Assert.AreEqual("/Services/Marketing/AddWizard/Step2",
				urlBuilder.BuildUrl(urlInfo, parameters, routeParameters));
		}

		[Test]
		public void UseCurrentRouteParamsShouldBeHonoredRegardlessOfTheRoutingOrder()
		{
			UrlInfo urlInfo = new UrlInfo("Services", "ModifyServiceWizard", "Step1", String.Empty, String.Empty);

			UrlBuilderParameters parameters = new UrlBuilderParameters();
			parameters.RouteMatch = new RouteMatch { Name = "ServiceWizardModify" };
			parameters.RouteMatch.AddNamed("serviceArea", "Marketing");
			parameters.RouteMatch.AddNamed("action", "Step1");
			parameters.RouteMatch.AddNamed("controller", "ModifyServiceWizard");
			parameters.RouteMatch.AddNamed("area", "Services");
			parameters.UseCurrentRouteParams = true;

			IDictionary routeParameters = new HybridDictionary();
			routeParameters.Add("action", "Step2");

			IRoutingEngine routingEngine = new StubRoutingEngine();
			routingEngine.Add(
				new PatternRoute("ServiceWizardCreate", "/Services/<serviceArea>/AddWizard/[action]")
					.DefaultForController().Is("CreateServiceWizard")
					.DefaultForArea().Is("Services")
					.DefaultForAction().Is("start")
				);

			routingEngine.Add(
				new PatternRoute("ServiceWizardModify", "/Services/<serviceArea>/ModifyWizard/[action]")
					.DefaultForController().Is("ModifyServiceWizard")
					.DefaultForArea().Is("Services")
					.DefaultForAction().Is("start")
				);
			urlBuilder.RoutingEngine = routingEngine;

			Assert.AreEqual("/Services/Marketing/ModifyWizard/Step2",
				urlBuilder.BuildUrl(urlInfo, parameters, routeParameters));
		}

		[Test]
		public void If_Route_Name_Is_Specified_It_Should_Be_Used_Even_If_UseCurrentRouteParams_Is_True()
		{
			UrlInfo urlInfo = new UrlInfo("", "Car", "View", String.Empty, String.Empty);

			UrlBuilderParameters parameters = new UrlBuilderParameters();
			parameters.RouteMatch = new RouteMatch { Name = "CarRoute" };
			parameters.RouteMatch.AddNamed("carName", "Ford");
			parameters.RouteMatch.AddNamed("action", "View");
			parameters.RouteMatch.AddNamed("controller", "Car");
			parameters.UseCurrentRouteParams = true;
			parameters.RouteName = "CarAddOptionWizard";

			IRoutingEngine routingEngine = new StubRoutingEngine();
			routingEngine.Add(
				new PatternRoute("CarRoute", "/Car/<carName>/[action]")
					.DefaultForController().Is("Car")
					.DefaultForAction().Is("VIew")
				);

			routingEngine.Add(
				new PatternRoute("CarAddOptionWizard", "/Car/<carName>/AddOption/[action]")
					.DefaultForController().Is("CarAddOptionWizard")
					.DefaultForAction().Is("start")
				);
			urlBuilder.RoutingEngine = routingEngine;

			Assert.AreEqual("/Car/Ford/AddOption",
				urlBuilder.BuildUrl(urlInfo, parameters));
		}

		[Test]
		public void ShouldWorkForSingleLetterAppVirtualDir()
		{
			DefaultUrlTokenizer tokenizer = new DefaultUrlTokenizer();
			UrlInfo urlinfo = tokenizer.TokenizeUrl("/v/area/controller/action.castle", null,
								  new Uri("http://www.castleproject.org/v/area/controller/action.castle"), true, "/v");

			UrlBuilderParameters parameters = new UrlBuilderParameters();

			Assert.AreEqual("/v/area/controller/action.castle", urlBuilder.BuildUrl(urlinfo, parameters));
		}
	}
}
