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

namespace Castle.Facilities.FactorySupport.Tests
{
	using System;

	using Castle.Windsor;
	using Castle.Facilities.FactorySupport.Tests.Components;

	using NUnit.Framework;


	[TestFixture]
	public class FactoryTestCase
	{
		[Test]
		public void FactoryTest1()
		{
			IWindsorContainer container = new WindsorContainer("../configfactory.xml");

			object instance = container[ typeof(MyComp) ];
			
			Assert.IsNotNull(instance);
		}

		[Test]
		public void FactoryTest2()
		{
			IWindsorContainer container = new WindsorContainer("../configfactorywithparameters.xml");

			MyComp instance = (MyComp) container[ typeof(MyComp) ];
			
			Assert.IsNotNull(instance);
			Assert.IsNotNull(instance.StoreName);
			Assert.IsNotNull(instance.Props);

			Assert.AreEqual("MyStore", instance.StoreName);
			Assert.AreEqual("item1", instance.Props["key1"]);
			Assert.AreEqual("item2", instance.Props["key2"]);
		}

		[Test]
		public void FactoryTest3()
		{
			IWindsorContainer container = new WindsorContainer("../configfactorywithparameters2.xml");

			MyComp instance = (MyComp) container[ typeof(MyComp) ];
			
			Assert.IsNotNull(instance);
			Assert.IsNotNull(instance.Service);
		}
	}
}
