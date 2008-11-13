using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core.Configuration;
using Castle.Facilities.NHibernateIntegration.Internal;
using NUnit.Framework;

namespace Castle.Facilities.NHibernateIntegration.Tests.Issues.Facilities106
{
	[TestFixture]
	public class Fixture : IssueTestCase
	{
		protected override string ConfigurationFile
		{
			get
			{
				return "EmptyConfiguration.xml";
			}
		}
		[Test]
		public void CanReadNHConfigFileAsTheSourceOfSessionFactory()
		{
			IConfiguration castleConfiguration = new MutableConfiguration("myConfig");
			castleConfiguration.Attributes["nhibernateConfigFile"] = ConfigHelper.ResolvePath("Issues/Facilities106/factory1.xml");
			XmlConfigurationBuilder b = new XmlConfigurationBuilder();
			NHibernate.Cfg.Configuration cfg = b.GetConfiguration(castleConfiguration);
			Assert.IsNotNull(cfg);
			string str = cfg.Properties["connection.provider"];
			Assert.AreEqual("DummyProvider", str);

		}
	}
}

