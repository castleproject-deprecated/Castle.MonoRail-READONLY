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

namespace Castle.Facilities.NHibernateIntegration.Tests.Internals
{
	using Castle.Core.Configuration;
	using Castle.Core.Resource;
	using Castle.Windsor.Configuration.Interpreters;
	using Internal;
	using NHibernate;
	using NHibernate.Cfg;
	using NUnit.Framework;
	using Windsor;

	public partial class CustomConfigurationBuilder : IConfigurationBuilder
	{
		private int _configurationsCreated;

		public int ConfigurationsCreated
		{
			get { return _configurationsCreated; }
		}

		public Configuration GetConfiguration(IConfiguration config)
		{
			_configurationsCreated++;
			return new DefaultConfigurationBuilder().GetConfiguration(config);
		}
	}

	public class CustomNHibernateFacility : NHibernateFacility
	{
		public CustomNHibernateFacility()
			: base(new CustomConfigurationBuilder())
		{
		}
	}

	public abstract class AbstractCusomConfigurationBuilderTestCase
	{
		protected WindsorContainer container;

		[TestFixtureSetUp]
		protected abstract void containerInit();

		[Test]
		public void Invoked()
		{
			ISession session = container.Resolve<ISessionManager>().OpenSession();
			CustomConfigurationBuilder configurationBuilder = (CustomConfigurationBuilder) container.Resolve<IConfigurationBuilder>();
			Assert.AreEqual(1, configurationBuilder.ConfigurationsCreated);
			session.Close();
		}
	}

	[TestFixture]
	public class CustomConfigurationBuilderTestCase : AbstractCusomConfigurationBuilderTestCase
	{
		protected override void containerInit()
		{
			container =
				new WindsorContainer(
					new XmlInterpreter(
						new AssemblyResource(
							"Castle.Facilities.NHibernateIntegration.Tests/customConfigurationBuilder.xml")));
		}
	}

	[TestFixture]
	public class CustomConfigurationBulderRegressionTestCase : AbstractCusomConfigurationBuilderTestCase
	{
		protected override void containerInit()
		{
			container =
				new WindsorContainer(
					new XmlInterpreter(
						new AssemblyResource(
							"Castle.Facilities.NHibernateIntegration.Tests/configurationBuilderRegression.xml")));
		}
	}
}
