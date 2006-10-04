// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Configuration;
	using Castle.Core;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Views.Aspx;

	/// <summary>
	/// 
	/// </summary>
	public class MonoRailServiceContainer : AbstractServiceContainer
	{
		/// <summary>The only one Extension Manager</summary>
		protected internal ExtensionManager extensionManager;

		/// <summary>Prevents GC from collecting the extensions</summary>
		private IList extensions = new ArrayList();

		/// <summary>
		/// 
		/// </summary>
		public void Start()
		{
			InitConfiguration();

			MonoRailConfiguration config = ObtainConfiguration();

			InitExtensions(config);
			InitServices(config);
			// InitApplicationHooks(context);
		}

		/// <summary>
		/// Reads the configuration and initializes
		/// registered extensions.
		/// </summary>
		/// <param name="config">The configuration object</param>
		private void InitExtensions(MonoRailConfiguration config)
		{
			extensionManager = new ExtensionManager(this);

			foreach (ExtensionEntry entry in config.ExtensionEntries)
			{
				AssertImplementsService(typeof(IMonoRailExtension), entry.ExtensionType);

				IMonoRailExtension extension = (IMonoRailExtension)ActivateService(entry.ExtensionType);

				extension.SetExtensionConfigNode(entry.ExtensionNode);

				extensions.Add(extension);
			}
		}

		/// <summary>
		/// Coordinates the instantiation, registering and initialization (lifecycle-wise)
		/// of the services used by MonoRail.
		/// </summary>
		/// <param name="config">The configuration object</param>
		private void InitServices(MonoRailConfiguration config)
		{
			AddService(typeof(ExtensionManager), extensionManager);
			AddService(typeof(MonoRailConfiguration), config);

			IList services = InstantiateAndRegisterServices(config.ServiceEntries);

			LifecycleService(services);
			LifecycleService(extensions);

			LifecycleInitialize(services);
			LifecycleInitialize(extensions);
		}

		/// <summary>
		/// Checks for services that implements <see cref="IInitializable"/>
		/// or <see cref="ISupportInitialize"/> and initialize them through the interface
		/// </summary>
		/// <param name="services">List of MonoRail's services</param>
		private void LifecycleInitialize(IList services)
		{
			foreach (object instance in services)
			{
				IInitializable initializable = instance as IInitializable;

				if (initializable != null)
				{
					initializable.Initialize();
				}

				ISupportInitialize suppInitialize = instance as ISupportInitialize;

				if (suppInitialize != null)
				{
					suppInitialize.BeginInit();
					suppInitialize.EndInit();
				}
			}
		}

		/// <summary>
		/// Checks for services that implements <see cref="IServiceEnabledComponent"/>
		/// and invoke <see cref="IServiceEnabledComponent.Service"/> on them
		/// </summary>
		/// <param name="services">List of MonoRail's services</param>
		private void LifecycleService(IList services)
		{
			foreach (object instance in services)
			{
				IServiceEnabledComponent serviceEnabled = instance as IServiceEnabledComponent;

				if (serviceEnabled != null)
				{
					serviceEnabled.Service(this);
				}
			}
		}

		/// <summary>
		/// Instantiates and registers the services used by MonoRail.
		/// </summary>
		/// <param name="services">The service's registry</param>
		/// <returns>List of service's instances</returns>
		private IList InstantiateAndRegisterServices(ServiceEntryCollection services)
		{
			IList instances = new ArrayList();

			// Builtin services

			foreach(DictionaryEntry entry in services.ServiceImplMap)
			{
				Type service = (Type)entry.Key;
				Type impl = (Type)entry.Value;

				AssertImplementsService(service, impl);

				object instance = ActivateService(impl);

				AddService(service, instance);

				instances.Add(instance);
			}

			// Custom services
			
			foreach(Type type in services.CustomServices)
			{
				object instance = ActivateService(type);

				AddService(type, instance);

				instances.Add(instance);
			}

			return instances;
		}
		
		/// <summary>
		/// Registers the default implementation of services, if 
		/// they are not registered
		/// </summary>
		private void InitConfiguration()
		{
			MonoRailConfiguration config = MonoRailConfiguration.GetConfig();

			RegisterMissingServices(config);
		}

		/// <summary>
		/// Checks whether non-optional services were supplied 
		/// through the configuration, and if not, register the 
		/// default implementation.
		/// </summary>
		/// <param name="config">The configuration object</param>
		private void RegisterMissingServices(MonoRailConfiguration config)
		{
			ServiceEntryCollection services = config.ServiceEntries;

			if (!services.HasService(ServiceIdentification.ViewSourceLoader))
			{
				services.RegisterService(ServiceIdentification.ViewSourceLoader,
										 typeof(FileAssemblyViewSourceLoader));
			}
			if (!services.HasService(ServiceIdentification.ViewEngine))
			{
				Type viewEngineType = config.ViewEngineConfig.CustomEngine;

				if (viewEngineType == null)
				{
					viewEngineType = typeof(WebFormsViewEngine);
				}

				services.RegisterService(ServiceIdentification.ViewEngine, viewEngineType);
			}
			if (!services.HasService(ServiceIdentification.ScaffoldingSupport))
			{
				Type defaultScaffoldingType =
					TypeLoadUtil.GetType(
						TypeLoadUtil.GetEffectiveTypeName(
							"Castle.MonoRail.ActiveRecordScaffold.ScaffoldingSupport, Castle.MonoRail.ActiveRecordScaffold"), true);

				if (defaultScaffoldingType != null)
				{
					services.RegisterService(ServiceIdentification.ScaffoldingSupport, defaultScaffoldingType);
				}
			}
			if (!services.HasService(ServiceIdentification.ControllerFactory))
			{
				if (config.ControllersConfig.CustomControllerFactory != null)
				{
					services.RegisterService(ServiceIdentification.ControllerFactory,
											 config.ControllersConfig.CustomControllerFactory);
				}
				else
				{
					services.RegisterService(ServiceIdentification.ControllerFactory,
											 typeof(DefaultControllerFactory));
				}
			}
			if (!services.HasService(ServiceIdentification.ViewComponentFactory))
			{
				if (config.ViewComponentsConfig.CustomViewComponentFactory != null)
				{
					services.RegisterService(ServiceIdentification.ViewComponentFactory,
											 config.ViewComponentsConfig.CustomViewComponentFactory);
				}
				else
				{
					services.RegisterService(ServiceIdentification.ViewComponentFactory,
											 typeof(DefaultViewComponentFactory));
				}
			}
			if (!services.HasService(ServiceIdentification.FilterFactory))
			{
				if (config.CustomFilterFactory != null)
				{
					services.RegisterService(ServiceIdentification.FilterFactory,
											 config.CustomFilterFactory);
				}
				else
				{
					services.RegisterService(ServiceIdentification.FilterFactory,
											 typeof(DefaultFilterFactory));
				}
			}
			if (!services.HasService(ServiceIdentification.ResourceFactory))
			{
				services.RegisterService(ServiceIdentification.ResourceFactory, typeof(DefaultResourceFactory));
			}
			if (!services.HasService(ServiceIdentification.EmailSender))
			{
				services.RegisterService(ServiceIdentification.EmailSender, typeof(MonoRailSmtpSender));
			}
			if (!services.HasService(ServiceIdentification.ControllerDescriptorProvider))
			{
				services.RegisterService(ServiceIdentification.ControllerDescriptorProvider, typeof(DefaultControllerDescriptorProvider));
			}
			if (!services.HasService(ServiceIdentification.ResourceDescriptorProvider))
			{
				services.RegisterService(ServiceIdentification.ResourceDescriptorProvider, typeof(DefaultResourceDescriptorProvider));
			}
			if (!services.HasService(ServiceIdentification.RescueDescriptorProvider))
			{
				services.RegisterService(ServiceIdentification.RescueDescriptorProvider, typeof(DefaultRescueDescriptorProvider));
			}
			if (!services.HasService(ServiceIdentification.LayoutDescriptorProvider))
			{
				services.RegisterService(ServiceIdentification.LayoutDescriptorProvider, typeof(DefaultLayoutDescriptorProvider));
			}
			if (!services.HasService(ServiceIdentification.HelperDescriptorProvider))
			{
				services.RegisterService(ServiceIdentification.HelperDescriptorProvider, typeof(DefaultHelperDescriptorProvider));
			}
			if (!services.HasService(ServiceIdentification.FilterDescriptorProvider))
			{
				services.RegisterService(ServiceIdentification.FilterDescriptorProvider, typeof(DefaultFilterDescriptorProvider));
			}
			if (!services.HasService(ServiceIdentification.EmailTemplateService))
			{
				services.RegisterService(ServiceIdentification.EmailTemplateService, typeof(EmailTemplateService));
			}
			if (!services.HasService(ServiceIdentification.ControllerTree))
			{
				services.RegisterService(ServiceIdentification.ControllerTree, typeof(DefaultControllerTree));
			}
			if (!services.HasService(ServiceIdentification.CacheProvider))
			{
				services.RegisterService(ServiceIdentification.CacheProvider, typeof(DefaultCacheProvider));
			}
		}

		private object ActivateService(Type type)
		{
			try
			{
				return Activator.CreateInstance(type);
			}
			catch (Exception ex)
			{
				String message = String.Format("Initialization Exception: " +
					"Could not instantiate {0}", type.FullName);
#if DOTNET2
				throw new ConfigurationErrorsException(message, ex);
#else
				throw new ConfigurationException(message, ex);
#endif
			}
		}

		private MonoRailConfiguration ObtainConfiguration()
		{
			return MonoRailConfiguration.GetConfig();
		}

		private void AssertImplementsService(Type service, Type impl)
		{
			if (!service.IsAssignableFrom(impl))
			{
				String message = String.Format("Initialization Exception: " +
					"Service {0} does not implement or extend {1}", impl.FullName, service.FullName);
#if DOTNET2
				throw new ConfigurationErrorsException(message);
#else
				throw new ConfigurationException(message);
#endif
			}
		}
	}
}
