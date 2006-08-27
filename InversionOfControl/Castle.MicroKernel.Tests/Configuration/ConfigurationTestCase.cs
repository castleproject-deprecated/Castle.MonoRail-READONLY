// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Tests.Configuration
{
	using System;
	using NUnit.Framework;
	using Castle.Core.Configuration;
	using Castle.MicroKernel.Resolvers;
	using Castle.MicroKernel.Tests.Configuration.Components;
	using Castle.MicroKernel.Tests.ClassComponents;

	[TestFixture]
	public class ConfigurationTestCase
	{
		private IKernel kernel;

		[SetUp]
		public void Init()
		{
			kernel = new DefaultKernel();
		}

		[TearDown]
		public void Dispose()
		{
			kernel.Dispose();
		}

		[Test]
		[ExpectedException(typeof(DependencyResolverException))]
		public void ConstructorWithUnsatisfiedParameters()
		{
			kernel.AddComponent("key", typeof(ClassWithConstructors));
			object res = kernel["key"];
		}

		[Test]
		public void ConstructorWithStringParameters()
		{
			MutableConfiguration confignode = new MutableConfiguration("key");

			IConfiguration parameters =
				confignode.Children.Add(new MutableConfiguration("parameters"));

			parameters.Children.Add(new MutableConfiguration("host", "castleproject.org"));

			kernel.ConfigurationStore.AddComponentConfiguration("key", confignode);

			kernel.AddComponent("key", typeof(ClassWithConstructors));

			ClassWithConstructors instance = (ClassWithConstructors) kernel["key"];
			Assert.IsNotNull(instance);
			Assert.IsNotNull(instance.Host);
			Assert.AreEqual("castleproject.org", instance.Host);
		}

		[Test]
		public void ServiceOverride()
		{
			MutableConfiguration confignode = new MutableConfiguration("key");

			IConfiguration parameters =
				confignode.Children.Add(new MutableConfiguration("parameters"));

			parameters.Children.Add(new MutableConfiguration("common", "${commonservice2}"));

			kernel.ConfigurationStore.AddComponentConfiguration("commonserviceuser", confignode);

			kernel.AddComponent("commonservice1", typeof(ICommon), typeof(CommonImpl1));
			kernel.AddComponent("commonservice2", typeof(ICommon), typeof(CommonImpl2));

			kernel.AddComponent("commonserviceuser", typeof(CommonServiceUser));

			CommonServiceUser instance = (CommonServiceUser) kernel["commonserviceuser"];

			Assert.IsNotNull(instance);
			Assert.AreEqual(typeof(CommonImpl2), instance.CommonService.GetType());
		}

		[Test]
		public void ServiceOverrideUsingProperties()
		{
			MutableConfiguration confignode = new MutableConfiguration("key");

			IConfiguration parameters =
				confignode.Children.Add(new MutableConfiguration("parameters"));

			parameters.Children.Add(new MutableConfiguration("CommonService", "${commonservice2}"));

			kernel.ConfigurationStore.AddComponentConfiguration("commonserviceuser", confignode);

			kernel.AddComponent("commonservice1", typeof(ICommon), typeof(CommonImpl1));
			kernel.AddComponent("commonservice2", typeof(ICommon), typeof(CommonImpl2));

			kernel.AddComponent("commonserviceuser", typeof(CommonServiceUser2));

			CommonServiceUser2 instance = (CommonServiceUser2) kernel["commonserviceuser"];

			Assert.IsNotNull(instance);
			Assert.AreEqual(typeof(CommonImpl2), instance.CommonService.GetType());
		}

		[Test]
		public void ConstructorWithArrayParameter()
		{
			MutableConfiguration confignode = new MutableConfiguration("key");

			IConfiguration parameters =
				confignode.Children.Add(new MutableConfiguration("parameters"));

			IConfiguration hosts = parameters.Children.Add(new MutableConfiguration("hosts"));
			IConfiguration array = hosts.Children.Add(new MutableConfiguration("array"));
			array.Children.Add(new MutableConfiguration("item", "castle"));
			array.Children.Add(new MutableConfiguration("item", "uol"));
			array.Children.Add(new MutableConfiguration("item", "folha"));

			kernel.ConfigurationStore.AddComponentConfiguration("key", confignode);

			kernel.AddComponent("key", typeof(ClassWithConstructors));

			ClassWithConstructors instance = (ClassWithConstructors) kernel["key"];
			Assert.IsNotNull(instance);
			Assert.IsNull(instance.Host);
			Assert.AreEqual("castle", instance.Hosts[0]);
			Assert.AreEqual("uol", instance.Hosts[1]);
			Assert.AreEqual("folha", instance.Hosts[2]);
		}

		[Test]
		public void ConstructorWithListParameterAndCustomType()
		{
			MutableConfiguration confignode = new MutableConfiguration("key");

			IConfiguration parameters =
				confignode.Children.Add(new MutableConfiguration("parameters"));

			IConfiguration services = parameters.Children.Add(new MutableConfiguration("services"));
			MutableConfiguration list = (MutableConfiguration)
			                            services.Children.Add(new MutableConfiguration("list"));
			list.Attributes.Add("type", "Castle.MicroKernel.Tests.ClassComponents.ICommon, Castle.MicroKernel.Tests");

			list.Children.Add(new MutableConfiguration("item", "${commonservice1}"));
			list.Children.Add(new MutableConfiguration("item", "${commonservice2}"));

			kernel.ConfigurationStore.AddComponentConfiguration("key", confignode);
			kernel.AddComponent("key", typeof(ClassWithListConstructor));

			kernel.AddComponent("commonservice1", typeof(ICommon), typeof(CommonImpl1));
			kernel.AddComponent("commonservice2", typeof(ICommon), typeof(CommonImpl2));

			ClassWithListConstructor instance = (ClassWithListConstructor) kernel["key"];
			Assert.IsNotNull(instance.Services);
			Assert.AreEqual(2, instance.Services.Count);
			Assert.AreEqual("CommonImpl1", instance.Services[0].GetType().Name);
			Assert.AreEqual("CommonImpl2", instance.Services[1].GetType().Name);
		}

		[Test]
		public void ConstructorWithArrayParameterAndCustomType()
		{
			MutableConfiguration confignode = new MutableConfiguration("key");

			IConfiguration parameters =
				confignode.Children.Add(new MutableConfiguration("parameters"));

			IConfiguration services = parameters.Children.Add(new MutableConfiguration("services"));
			MutableConfiguration array = (MutableConfiguration)
			                             services.Children.Add(new MutableConfiguration("array"));
			//list.Attributes.Add("type", "Castle.MicroKernel.Tests.ClassComponents.ICommon, Castle.MicroKernel.Tests");

			array.Children.Add(new MutableConfiguration("item", "${commonservice1}"));
			array.Children.Add(new MutableConfiguration("item", "${commonservice2}"));

			kernel.ConfigurationStore.AddComponentConfiguration("key", confignode);

			kernel.AddComponent("key", typeof(ClassWithArrayConstructor));

			kernel.AddComponent("commonservice1", typeof(ICommon), typeof(CommonImpl1));
			kernel.AddComponent("commonservice2", typeof(ICommon), typeof(CommonImpl2));

			ClassWithArrayConstructor instance = (ClassWithArrayConstructor) kernel["key"];
			Assert.IsNotNull(instance.Services);
			Assert.AreEqual(2, instance.Services.Length);
			Assert.AreEqual("CommonImpl1", instance.Services[0].GetType().Name);
			Assert.AreEqual("CommonImpl2", instance.Services[1].GetType().Name);
		}

		[Test]
		public void CustomLifestyleManager()
		{
			string key = "key";

			MutableConfiguration confignode = new MutableConfiguration(key);
			confignode.Attributes.Add("lifestyle", "custom");

			confignode.Attributes.Add("customLifestyleType",
			                          "Castle.MicroKernel.Tests.ClassComponents.CustomLifestyleManager, Castle.MicroKernel.Tests");

			kernel.ConfigurationStore.AddComponentConfiguration(key, confignode);
			kernel.AddComponent(key, typeof(ICommon), typeof(CommonImpl1));

			ICommon instance = (ICommon) kernel[key];
			IHandler handler = kernel.GetHandler(key);

			Assert.IsNotNull(instance);
			Assert.AreEqual(Core.LifestyleType.Custom, handler.ComponentModel.LifestyleType);
			Assert.AreEqual(typeof(CustomLifestyleManager), handler.ComponentModel.CustomLifestyle);
		}
	}
}