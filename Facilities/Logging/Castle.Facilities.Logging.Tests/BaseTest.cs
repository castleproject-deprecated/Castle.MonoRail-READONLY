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

namespace Castle.Facilities.Logging.Tests
{
	using System;
	using Castle.Core.Configuration;
	using Castle.MicroKernel.SubSystems.Configuration;
	using Castle.Windsor;

	/// <summary>
	/// Summary description for BaseTest.
	/// </summary>
	public abstract class BaseTest
	{
		protected virtual IWindsorContainer CreateConfiguredContainer(LoggerImplementation loggerApi)
		{
			return CreateConfiguredContainer(loggerApi, String.Empty);
		}

		protected virtual IWindsorContainer CreateConfiguredContainer(LoggerImplementation loggerApi, String custom)
		{
			IWindsorContainer container = new WindsorContainer(new DefaultConfigurationStore());

			MutableConfiguration confignode = new MutableConfiguration("facility");

			confignode.Attributes.Add("loggingApi", loggerApi.ToString());
			switch (loggerApi)
			{
				case LoggerImplementation.Custom:
					{
						confignode.Attributes.Add("customLoggerFactory", custom);
						break;
					}
				case LoggerImplementation.Log4net:
					{
						confignode.Attributes.Add("configFile", "log4net.facilities.test.config");
						break;
					}
				case LoggerImplementation.NLog:
					{
						confignode.Attributes.Add("configFile", "NLog.facilities.test.config");
						break;
					}
				case LoggerImplementation.ExtendedLog4net:
					{
						confignode.Attributes.Add("configFile", "log4net.facilities.test.config");
						break;
					}
				case LoggerImplementation.ExtendedNLog:
					{
						confignode.Attributes.Add("configFile", "NLog.facilities.test.config");
						break;
					}
			}

			container.Kernel.ConfigurationStore.AddFacilityConfiguration("logging", confignode);

			container.AddFacility("logging", new LoggingFacility());

			return container;
		}
	}
}