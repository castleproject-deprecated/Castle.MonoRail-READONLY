#region License
/// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
///  
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///  
/// http://www.apache.org/licenses/LICENSE-2.0
///  
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// 
/// -- 
/// 
/// This facility was a contribution kindly 
/// donated by Gilles Bayon <gilles.bayon@gmail.com>
/// 
/// --
#endregion

namespace Castle.Facilities.IBatisNetIntegration
{
	using System;
	using System.Configuration;

	using Castle.Model;
	using Castle.Model.Configuration;
	using Castle.MicroKernel.Facilities;
	using Castle.Services.Transaction;

	using IBatisNet.Common.Logging;
	using IBatisNet.DataMapper;

	public class IBatisNetFacility : AbstractFacility
	{
		public static readonly String MAPPER_CONFIG_FILE = "_IBATIS_MAPPER_CONFIG_FILE_";
		public static readonly String MAPPER_CONFIG_EMBEDDED = "_IBATIS_MAPPER_CONFIG_EMBEDDED_";
		public static readonly String MAPPER_CONFIG_CONNECTION_STRING = "_IBATIS_MAPPER_CONFIG_CONNECTIONSTRING_";

		private static readonly ILog _logger = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

		public IBatisNetFacility()
		{
		}

		#region IFacility Members

		protected override void Init()
		{
			if (FacilityConfig == null)
			{
				throw new ConfigurationException( "The IBatisNetFacility requires an external configuration" );
			}

			Kernel.ComponentModelBuilder.AddContributor( new AutomaticSessionInspector() );
			Kernel.AddComponent( "IBatis.session.interceptor", typeof(AutomaticSessionInterceptor) );
			Kernel.AddComponent( "IBatis.transaction.manager", typeof(ITransactionManager), typeof(DataMapperTransactionManager) );

			int factories = 0;

			foreach( IConfiguration factoryConfig in FacilityConfig.Children)
			{
				if( factoryConfig.Name == "sqlMap")
				{
					ConfigureFactory(factoryConfig);
					factories++;
				}
			}
			
			if ( factories == 0)
			{
				throw new ConfigurationException( "You need to configure at least one sqlMap for IBatisNetFacility" );
			}
		}

		#endregion

		private void ConfigureFactory( IConfiguration config )
		{
			String id = config.Attributes["id"]; 
			if(id==string.Empty)
			{
				throw new ConfigurationException( "The IBatisNetFacility requires each SqlMapper to have an ID." );
			}
			else
			{
				if(_logger.IsDebugEnabled)
				{
					_logger.Debug(string.Format("[{0}] was specified as the SqlMapper ID.", id));
				}
			}
			
			String fileName = config.Attributes["config"];
			if ( fileName == String.Empty )
			{
				if(_logger.IsDebugEnabled)
				{
					_logger.Debug("No filename was specified, using [sqlMap.config].");
				}
				fileName = "sqlMap.config"; // default name
			}

			String connectionString = config.Attributes["connectionString"];
			
			bool isEmbedded = false;
			String embedded = config.Attributes["embedded"];
			if ( embedded != null )
			{
				try
				{
					isEmbedded = Convert.ToBoolean( embedded );
					if(_logger.IsDebugEnabled)
					{
						_logger.Debug("The SqlMap.config was set to embedded.");
					}
				}
				catch( System.Exception ex )
				{
					if(_logger.IsWarnEnabled)
					{
						_logger.Warn(string.Format("The SqlMap.config had a value set for embedded, [{0}], but it was not able to parsed as a Boolean.", embedded.ToString()), ex);
					}
					isEmbedded = false;
				}
			}

			ComponentModel model = new ComponentModel(id, typeof(SqlMapper), null);
			model.ExtendedProperties.Add( MAPPER_CONFIG_FILE, fileName );
			model.ExtendedProperties.Add( MAPPER_CONFIG_EMBEDDED, isEmbedded );
			model.ExtendedProperties.Add( MAPPER_CONFIG_CONNECTION_STRING, connectionString );
			model.LifestyleType = LifestyleType.Singleton;
			model.CustomComponentActivator = typeof( SqlMapActivator );

			Kernel.AddCustomComponent( model );
		}
	}
}
