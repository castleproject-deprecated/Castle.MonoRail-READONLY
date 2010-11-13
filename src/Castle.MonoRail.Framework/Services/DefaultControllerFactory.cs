// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	
	using Core.Logging;
	using Configuration;
	using Descriptors;
	using Utils;

	/// <summary>
	/// Standard implementation of <see cref="IControllerFactory"/>.
	/// It inspects assemblies looking for concrete classes
	/// that extend <see cref="IController"/>.
	/// </summary>
	public class DefaultControllerFactory : AbstractControllerFactory
	{
		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;

		private List<string> assemblies;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultControllerFactory"/> class.
		/// </summary>
		public DefaultControllerFactory()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultControllerFactory"/> class.
		/// </summary>
		/// <param name="tree">The tree.</param>
		public DefaultControllerFactory(IControllerTree tree) : base(tree)
		{
		}

		/// <summary>
		/// Invoked by the framework in order to initialize the state
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			
			if (assemblies != null)
			{
				foreach(var assembly in assemblies)
				{
					Inspect(assembly);
				}
			}
			
			assemblies = null;
		}
		
		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public override void Service(IServiceProvider provider)
		{
			base.Service(provider);
			
			var loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));
			
			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(AbstractControllerFactory));
			}
			
			var config = (IMonoRailConfiguration) provider.GetService(typeof(IMonoRailConfiguration));
			
			if (config != null)
			{
				assemblies = config.ControllersConfig.Assemblies;
				
				if (assemblies.Count == 0)
				{
					throw new System.Configuration.ConfigurationErrorsException("No assembly was informed on the configuration file. " +
						"Unfortunately this cannot be inferred (we tried)");
				}
			}

			Initialize();
		}

		/// <summary>
		/// Loads the assembly and inspect its public types.
		/// </summary>
		/// <param name="assemblyFileName"></param>
		public void Inspect(String assemblyFileName)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Inspecting assembly '{0}'", assemblyFileName);
			}
			
			var assembly = Assembly.Load( assemblyFileName );

			Inspect(assembly);
		}

		/// <summary>
		/// Inspect the assembly's public types.
		/// </summary>
		public void Inspect(Assembly assembly)
		{
			var types = assembly.GetExportedTypes();

			foreach(var type in types)
			{
				if (!type.IsPublic || type.IsAbstract || type.IsInterface || type.IsValueType)
				{
					continue;
				}

				if (typeof(IController).IsAssignableFrom(type))
				{
					var contrDesc = ControllerInspectionUtil.Inspect(type);
					
					RegisterController(contrDesc);
				}
			}
		}

		/// <summary>
		/// Registers the controller.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		private void RegisterController(ControllerDescriptor descriptor)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Registering controller descriptor for Area: '{0}' Name: '{1}'", 
				                   descriptor.Area, descriptor.Name);
			}

			Tree.AddController(descriptor.Area, descriptor.Name, descriptor.ControllerType);
		}
	}
}