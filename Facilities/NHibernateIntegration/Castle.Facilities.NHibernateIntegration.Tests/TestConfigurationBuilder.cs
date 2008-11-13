using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Castle.Facilities.NHibernateIntegration.Internal;

namespace Castle.Facilities.NHibernateIntegration.Tests
{
	public class TestConfigurationBuilder : IConfigurationBuilder
	{
		public TestConfigurationBuilder()
		{
			this.defaultConfigurationBuilder = new DefaultConfigurationBuilder();
		}

		private readonly IConfigurationBuilder defaultConfigurationBuilder;
		#region IConfigurationBuilder Members

		public NHibernate.Cfg.Configuration GetConfiguration(Castle.Core.Configuration.IConfiguration config)
		{
			NHibernate.Cfg.Configuration nhConfig = defaultConfigurationBuilder.GetConfiguration(config);
			nhConfig.Properties["dialect"] = ConfigurationManager.AppSettings["ar.dialect"];
			nhConfig.Properties["connection.driver_class"] = ConfigurationManager.AppSettings["ar.connection.driver_class"];
			nhConfig.Properties["connection.provider"] = ConfigurationManager.AppSettings["ar.connection.provider"];
			nhConfig.Properties["connection.connection_string"] = ConfigurationManager.AppSettings["ar.connection.connection_string"];
			return nhConfig;
		}

		#endregion
	}
}