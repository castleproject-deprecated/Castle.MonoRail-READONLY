// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using System.Reflection;
	using Castle.MonoRail.Framework.Services;
	using NUnit.Framework;


	[TestFixture]
	public class DefaultControllerFactoryTestCase
	{
		private readonly String extension = "rails";

		private DefaultControllerFactory factory;

		[TestFixtureSetUp]
		public void Init()
		{
			factory = new DefaultControllerFactory();
			factory.Service(new TestServiceContainer());
			factory.Inspect(Assembly.GetExecutingAssembly());
		}

		[Test]
		public void EmptyArea()
		{
			Controller controller = factory.CreateController(new UrlInfo("domain", "sub", "", "http://", 80, "", "", "home", "", extension));

			Assert.IsNotNull(controller);
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.HomeController",
			                controller.GetType().FullName);
		}

		[Test]
		public void OneLevelArea()
		{
			Controller controller =
				factory.CreateController(new UrlInfo("domain", "sub", "", "http://", 80, "", "clients", "home", "", extension));

			Assert.IsNotNull(controller);
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.Clients.ClientHomeController",
			                controller.GetType().FullName);

			controller = factory.CreateController(new UrlInfo("domain", "sub", "", "http://", 80, "", "clients", "hire-us", "", extension));

			Assert.IsNotNull(controller);
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.Clients.OtherController",
			                controller.GetType().FullName);

			controller =
				factory.CreateController(new UrlInfo("domain", "sub", "", "http://", 80, "", "ourproducts", "shoppingcart", "", extension));

			Assert.IsNotNull(controller);
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.Products.CartController",
			                controller.GetType().FullName);

			controller =
				factory.CreateController(new UrlInfo("domain", "sub", "", "http://", 80, "", "ourproducts", "lista", "", extension));

			Assert.IsNotNull(controller);
			Assert.AreEqual("Castle.MonoRail.Framework.Tests.Controllers.Products.ListController",
			                controller.GetType().FullName);
		}
	}
}