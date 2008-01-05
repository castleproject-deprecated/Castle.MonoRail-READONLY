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

namespace Castle.Windsor.Tests.Configuration2
{
	using System;
	using System.IO;
	using Castle.Windsor.Configuration.Interpreters;
	using Castle.Windsor.Tests.Components;
	using NUnit.Framework;

	[TestFixture]
	public class ConfigurationEnvTestCase
	{
		private string dir = ConfigHelper.ResolveConfigPath("Configuration2/");

		[Test]
		public void AssertDefineIsSetBasedOnEnvironmentInformation()
		{
			string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir +
			                                                                  "env_config.xml");

			WindsorContainer container = new WindsorContainer(new XmlInterpreter(file), new CustomEnv(true));

			ComponentWithStringProperty prop =
				(ComponentWithStringProperty) container.Resolve("component");

			Assert.AreEqual("John Doe", prop.Name);

			container = new WindsorContainer(new XmlInterpreter(file), new CustomEnv(false));

			prop = (ComponentWithStringProperty) container.Resolve("component");

			Assert.AreEqual("Hammett", prop.Name);
		}
	}

	internal class CustomEnv : IEnvironmentInfo
	{
		private readonly bool isDevelopment;

		public CustomEnv(bool isDevelopment)
		{
			this.isDevelopment = isDevelopment;
		}

		public string GetEnvironmentName()
		{
			return isDevelopment ? "devel" : "test";
		}
	}
}