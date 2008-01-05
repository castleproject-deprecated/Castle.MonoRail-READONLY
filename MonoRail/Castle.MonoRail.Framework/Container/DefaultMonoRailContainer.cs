﻿// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Container
{
	using System;
	using System.Configuration;
	using Castle.Components.Common.EmailSender;
	using Castle.Components.Validator;
	using Castle.Core;
	using Castle.Core.Configuration;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Providers;
	using Castle.MonoRail.Framework.Resources;
	using Castle.MonoRail.Framework.Services.AjaxProxyGenerator;

	/// <summary>
	/// Pendent
	/// </summary>
	public class DefaultMonoRailContainer : AbstractServiceContainer, IMonoRailContainer
	{
		#region ServiceIdentification enum

		/// <summary>
		/// Enum for all known MonoRail services.
		/// </summary>
		public enum ServiceIdentification
		{
			/// <summary>
			/// Default value
			/// </summary>
			Undefined,
			/// <summary>
			/// Custom ( not know service )
			/// </summary>
			Custom,
			/// <summary>
			/// The <see cref="IControllerFactory"/> service
			/// </summary>
			ControllerFactory,
			/// <summary>
			/// The <see cref="IControllerContextFactory"/> service
			/// </summary>
			ControllerContextFactory,
			/// <summary>
			/// The <see cref="IUrlBuilder"/> service
			/// </summary>
			UrlBuilder,
			/// <summary>
			/// The <see cref="IUrlTokenizer"/> service
			/// </summary>
			UrlTokenizer,
			/// <summary>
			/// The <see cref="IServerUtility"/> service
			/// </summary>
			ServerUtility,
			/// <summary>
			/// The <see cref="IControllerTree"/> service
			/// </summary>
			ControllerTree,
			/// <summary>
			/// The <see cref="ICacheProvider"/> service
			/// </summary>
			CacheProvider,
			/// <summary>
			/// The <see cref="IViewSourceLoader"/> service.
			/// </summary>
			ViewSourceLoader,
			/// <summary>
			/// The <see cref="IFilterFactory"/> service.
			/// </summary>
			FilterFactory,
			/// <summary>
			/// The <see cref="IViewComponentFactory"/> service
			/// </summary>
			ViewComponentFactory,
			/// <summary>
			/// The <see cref="IEmailSender"/> service.
			/// </summary>
			EmailSender,
			/// <summary>
			/// The <see cref="IControllerDescriptorProvider"/> service
			/// </summary>
			ControllerDescriptorProvider,
			/// <summary>
			/// The <see cref="IResourceDescriptorProvider"/> service
			/// </summary>
			ResourceDescriptorProvider,
			/// <summary>
			/// The <see cref="IViewComponentDescriptorProvider"/> service
			/// </summary>
			ViewComponentDescriptorProvider,
			/// <summary>
			/// The <see cref="IRescueDescriptorProvider"/> service
			/// </summary>
			RescueDescriptorProvider,
			/// <summary>
			/// The <see cref="ILayoutDescriptorProvider"/> service
			/// </summary>
			LayoutDescriptorProvider,
			/// <summary>
			/// The <see cref="IHelperDescriptorProvider"/> service
			/// </summary>
			HelperDescriptorProvider,
			/// <summary>
			/// The <see cref="IFilterDescriptorProvider"/> service
			/// </summary>
			FilterDescriptorProvider,
			/// <summary>
			/// The <see cref="IResourceFactory"/> service
			/// </summary>
			ResourceFactory,
			/// <summary>
			/// The <see cref="IEmailTemplateService"/> service
			/// </summary>
			EmailTemplateService,
			/// <summary>
			/// The <see cref="IScaffoldingSupport"/> service
			/// </summary>
			ScaffoldingSupport,
			/// <summary>
			/// The <see cref="ITransformFilterDescriptorProvider"/> service
			/// </summary>
			TransformFilterDescriptorProvider,
//			/// <summary>
//			/// The <see cref="ITransformFilterFactory"/> service
//			/// </summary>
//			TransformationFilterFactory,
			/// <summary>
			/// The <see cref="IViewEngineManager"/> service
			/// </summary>
			ViewEngineManager,
			/// <summary>
			/// The <see cref="IValidatorRegistry"/> service
			/// </summary>
			ValidatorRegistry,
			/// <summary>
			/// The <see cref="IAjaxProxyGenerator"/> service
			/// </summary>
			AjaxProxyGenerator
		}

		#endregion

		private IUrlTokenizer urlTokenizerCached;
		private IUrlBuilder urlBuilderCached;
		private ICacheProvider cacheProviderCached;
		private IEngineContextFactory engContextFactoryCached;
		private IControllerFactory controllerFactoryCached;
		private IControllerContextFactory controllerContextFactoryCached;
		private IControllerTree controllerTreeCached;
		private IViewSourceLoader viewSourceLoaderCached;
		private IFilterFactory filterFactoryCached;
		private IControllerDescriptorProvider controllerDescriptorProviderCached;
		private IViewEngineManager viewEngineManagerCached;
		private IValidatorRegistry validatorRegistryCached;
		private IActionSelector actionSelectorCached;
		private IScaffoldingSupport scaffoldSupportCached;
		private IJSONSerializer jsonSerializerCached;
		private IStaticResourceRegistry staticResourceRegCached;
		private IEmailTemplateService emailTemplateServiceCached;
		private IEmailSender emailSenderCached;
		private IResourceFactory resourceFactoryCached;
		private ExtensionManager extensionManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultMonoRailContainer"/> class.
		/// </summary>
		/// <param name="parentContainer">The parent container.</param>
		public DefaultMonoRailContainer(IServiceProvider parentContainer) : base(parentContainer)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultMonoRailContainer"/> class.
		/// </summary>
		public DefaultMonoRailContainer()
		{
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public void UseServicesFromParent()
		{
			if (Parent == null)
			{
				return;
			}

			IUrlTokenizer urlTokenizer = (IUrlTokenizer) Parent.GetService(typeof(IUrlTokenizer));
			if (urlTokenizer != null)
			{
				AddService(typeof(IUrlTokenizer), urlTokenizer);
			}

			IUrlBuilder urlBuilder = (IUrlBuilder) Parent.GetService(typeof(IUrlBuilder));
			if (urlBuilder != null)
			{
				AddService(typeof(IUrlBuilder), urlBuilder);
			}

			ICacheProvider cacheProvider = (ICacheProvider) Parent.GetService(typeof(ICacheProvider));
			if (cacheProvider != null)
			{
				AddService(typeof(ICacheProvider), cacheProvider);
			}

			IEngineContextFactory engContextFactory = (IEngineContextFactory) Parent.GetService(typeof(IEngineContextFactory));
			if (engContextFactory != null)
			{
				AddService(typeof(IEngineContextFactory), engContextFactory);
			}

			IControllerFactory controllerFactory = (IControllerFactory) Parent.GetService(typeof(IControllerFactory));
			if (controllerFactory != null)
			{
				AddService(typeof(IControllerFactory), controllerFactory);
			}

			IControllerContextFactory controllerCtxFactory =
				(IControllerContextFactory) Parent.GetService(typeof(IControllerContextFactory));
			if (controllerCtxFactory != null)
			{
				AddService(typeof(IControllerContextFactory), controllerCtxFactory);
			}

			IControllerTree controllerTree = (IControllerTree) Parent.GetService(typeof(IControllerTree));
			if (controllerTree != null)
			{
				AddService(typeof(IControllerTree), controllerTree);
			}

			IViewSourceLoader viewSourceLoader = (IViewSourceLoader) Parent.GetService(typeof(IViewSourceLoader));
			if (viewSourceLoader != null)
			{
				AddService(typeof(IViewSourceLoader), viewSourceLoader);
			}

			IFilterFactory filterFactory = (IFilterFactory) Parent.GetService(typeof(IFilterFactory));
			if (filterFactory != null)
			{
				AddService(typeof(IFilterFactory), filterFactory);
			}

			IControllerDescriptorProvider controllerDescriptorProvider =
				(IControllerDescriptorProvider) Parent.GetService(typeof(IControllerDescriptorProvider));
			if (controllerDescriptorProvider != null)
			{
				AddService(typeof(IControllerDescriptorProvider), controllerDescriptorProvider);
			}

			ITransformFilterDescriptorProvider transformFilterDescriptorProvider =
				(ITransformFilterDescriptorProvider) Parent.GetService(typeof(ITransformFilterDescriptorProvider));
			if (transformFilterDescriptorProvider != null)
			{
				AddService(typeof(ITransformFilterDescriptorProvider), transformFilterDescriptorProvider);
			}

			IViewEngineManager viewEngManager = (IViewEngineManager) Parent.GetService(typeof(IViewEngineManager));
			if (viewEngManager != null)
			{
				AddService(typeof(IViewEngineManager), viewEngManager);
			}

			IViewComponentFactory viewComponentFactory = (IViewComponentFactory) Parent.GetService(typeof(IViewComponentFactory));
			if (viewComponentFactory != null)
			{
				AddService(typeof(IViewComponentFactory), viewComponentFactory);
			}

			IViewComponentRegistry viewComponentRegistry = (IViewComponentRegistry) Parent.GetService(typeof(IViewComponentRegistry));
			if (viewComponentRegistry != null)
			{
				AddService(typeof(IViewComponentRegistry), viewComponentRegistry);
			}

			IValidatorRegistry validatorRegistry = (IValidatorRegistry) Parent.GetService(typeof(IValidatorRegistry));
			if (validatorRegistry != null)
			{
				AddService(typeof(IValidatorRegistry), validatorRegistry);
			}

			IJSONSerializer jsonSerializer = (IJSONSerializer) Parent.GetService(typeof(IJSONSerializer));
			if (jsonSerializer != null)
			{
				AddService(typeof(IJSONSerializer), jsonSerializer);
			}

			IStaticResourceRegistry staticResourceRegistry =
				(IStaticResourceRegistry) Parent.GetService(typeof(IStaticResourceRegistry));
			if (staticResourceRegistry != null)
			{
				AddService(typeof(IStaticResourceRegistry), staticResourceRegistry);
			}

			IEmailTemplateService emailTemplateService =
				(IEmailTemplateService) Parent.GetService(typeof(IEmailTemplateService));
			if (emailTemplateService != null)
			{
				AddService(typeof(IEmailTemplateService), emailTemplateService);
			}

			IEmailSender emailSender =
				(IEmailSender) Parent.GetService(typeof(IEmailSender));
			if (emailSender != null)
			{
				AddService(typeof(IEmailSender), emailSender);
			}

			IResourceFactory resourceFactory =
				(IResourceFactory) Parent.GetService(typeof(IResourceFactory));
			if (resourceFactory != null)
			{
				AddService(typeof(IResourceFactory), resourceFactory);
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public void InstallMissingServices()
		{
			if (!HasService<IControllerTree>())
			{
				AddService<IControllerTree>(CreateService<DefaultControllerTree>());
			}
			if (!HasService<IUrlTokenizer>())
			{
				AddService<IUrlTokenizer>(CreateService<DefaultUrlTokenizer>());
			}
			if (!HasService<IUrlBuilder>())
			{
				AddService<IUrlBuilder>(CreateService<DefaultUrlBuilder>());
			}
			if (!HasService<ICacheProvider>())
			{
				AddService<ICacheProvider>(CreateService<DefaultCacheProvider>());
			}
			if (!HasService<IEngineContextFactory>())
			{
				AddService<IEngineContextFactory>(CreateService<DefaultEngineContextFactory>());
			}
			if (!HasService<IControllerContextFactory>())
			{
				AddService<IControllerContextFactory>(CreateService<DefaultControllerContextFactory>());
			}
			if (!HasService<IControllerFactory>())
			{
				AddService<IControllerFactory>(CreateService<DefaultControllerFactory>());
			}
			if (!HasService<IViewSourceLoader>())
			{
				AddService<IViewSourceLoader>(CreateService<FileAssemblyViewSourceLoader>());
			}
			if (!HasService<IFilterFactory>())
			{
				AddService<IFilterFactory>(CreateService<DefaultFilterFactory>());
			}
			if (!HasService<IResourceDescriptorProvider>())
			{
				AddService<IResourceDescriptorProvider>(CreateService<DefaultResourceDescriptorProvider>());
			}
			if (!HasService<IRescueDescriptorProvider>())
			{
				AddService<IRescueDescriptorProvider>(CreateService<DefaultRescueDescriptorProvider>());
			}
			if (!HasService<IResourceFactory>())
			{
				AddService<IResourceFactory>(CreateService<DefaultResourceFactory>());
			}
			if (!HasService<IFilterDescriptorProvider>())
			{
				AddService<IFilterDescriptorProvider>(CreateService<DefaultFilterDescriptorProvider>());
			}
			if (!HasService<IHelperDescriptorProvider>())
			{
				AddService<IHelperDescriptorProvider>(CreateService<DefaultHelperDescriptorProvider>());
			}
			if (!HasService<ILayoutDescriptorProvider>())
			{
				AddService<ILayoutDescriptorProvider>(CreateService<DefaultLayoutDescriptorProvider>());
			}
			if (!HasService<ITransformFilterDescriptorProvider>())
			{
				AddService<ITransformFilterDescriptorProvider>(CreateService<DefaultTransformFilterDescriptorProvider>());
			}
			if (!HasService<IControllerDescriptorProvider>())
			{
				AddService<IControllerDescriptorProvider>(CreateService<DefaultControllerDescriptorProvider>());
			}
			if (!HasService<IViewEngineManager>())
			{
				AddService<IViewEngineManager>(CreateService<DefaultViewEngineManager>());
			}
			if (!HasService<IValidatorRegistry>())
			{
				AddService<IValidatorRegistry>(CreateService<CachedValidationRegistry>());
			}
			if (!HasService<IActionSelector>())
			{
				AddService<IActionSelector>(CreateService<DefaultActionSelector>());
			}
			if (!HasService<IJSONSerializer>())
			{
				AddService<IJSONSerializer>(CreateService<NewtonsoftJSONSerializer>());
			}
			if (!HasService<IStaticResourceRegistry>())
			{
				AddService<IStaticResourceRegistry>(CreateService<DefaultStaticResourceRegistry>());
			}
			if (!HasService<IViewComponentRegistry>())
			{
				AddService<IViewComponentRegistry>(CreateService<DefaultViewComponentRegistry>());
			}
			if (!HasService<IViewComponentFactory>())
			{
				AddService<IViewComponentFactory>(CreateService<DefaultViewComponentFactory>());
			}
			if (!HasService<IViewComponentDescriptorProvider>())
			{
				AddService<IViewComponentDescriptorProvider>(CreateService<DefaultViewComponentDescriptorProvider>());
			}
			if (!HasService<IEmailTemplateService>())
			{
				AddService<IEmailTemplateService>(CreateService<EmailTemplateService>());
			}
			if (!HasService<IEmailSender>())
			{
				AddService<IEmailSender>(CreateService<MonoRailSmtpSender>());
			}
			if (!HasService<IAjaxProxyGenerator>())
			{
				AddService<IAjaxProxyGenerator>(CreateService<PrototypeAjaxProxyGenerator>());
			}
		}

		/// <summary>
		/// Initialize extensions and start the extension manager.
		/// </summary>
		public void StartExtensionManager()
		{
			extensionManager = new ExtensionManager(this);

			AddService(typeof(ExtensionManager), extensionManager);

			IMonoRailConfiguration config = GetService<IMonoRailConfiguration>();

			foreach(ExtensionEntry entry in config.ExtensionEntries)
			{
				AssertImplementsService(typeof(IMonoRailExtension), entry.ExtensionType);

				IMonoRailExtension extension = (IMonoRailExtension) CreateService(entry.ExtensionType);

				extension.SetExtensionConfigNode(entry.ExtensionNode);

				extensionManager.Extensions.Add(extension);
			}
		}

		/// <summary>
		/// Configures the specified config.
		/// </summary>
		/// <param name="config">The config.</param>
		public void Configure(IMonoRailConfiguration config)
		{
			if (config == null) throw new ArgumentNullException("config");

			AddService<IMonoRailConfiguration>(config);

			if (config.ServicesConfig != null)
			{
				foreach(IConfiguration serviceConfig in config.ServicesConfig.Children)
				{
					RegisterServiceOverrideFromConfigurationNode(serviceConfig);
				}
			}
		}

		/// <summary>
		/// Creates the specified service running all life cycles for it.
		/// </summary>
		/// <param name="type">The service type.</param>
		/// <returns></returns>
		protected override object CreateService(Type type)
		{
			object instance = Activator.CreateInstance(type);

			IMRServiceEnabled mrServiceEnabled = instance as IMRServiceEnabled;

			if (mrServiceEnabled != null)
			{
				mrServiceEnabled.Service(this);
			}

			IServiceEnabledComponent serviceEnabled = instance as IServiceEnabledComponent;

			if (serviceEnabled != null)
			{
				serviceEnabled.Service(this);
			}

			IInitializable initializable = instance as IInitializable;

			if (initializable != null)
			{
				initializable.Initialize();
			}

			return instance;
		}

		#region Services

		/// <summary>
		/// Gets or sets the URL tokenizer.
		/// </summary>
		/// <value>The URL tokenizer.</value>
		public IUrlTokenizer UrlTokenizer
		{
			get
			{
				if (urlTokenizerCached == null)
				{
					urlTokenizerCached = GetService<IUrlTokenizer>();
				}
				return urlTokenizerCached;
			}
			set { urlTokenizerCached = value; }
		}

		/// <summary>
		/// Gets or sets the URL builder.
		/// </summary>
		/// <value>The URL builder.</value>
		public IUrlBuilder UrlBuilder
		{
			get
			{
				if (urlBuilderCached == null)
				{
					urlBuilderCached = GetService<IUrlBuilder>();
				}
				return urlBuilderCached;
			}
			set { urlBuilderCached = value; }
		}

		/// <summary>
		/// Gets or sets the cache provider.
		/// </summary>
		/// <value>The cache provider.</value>
		public ICacheProvider CacheProvider
		{
			get
			{
				if (cacheProviderCached == null)
				{
					cacheProviderCached = GetService<ICacheProvider>();
				}
				return cacheProviderCached;
			}
			set { cacheProviderCached = value; }
		}

		/// <summary>
		/// Gets or sets the engine context factory.
		/// </summary>
		/// <value>The engine context factory.</value>
		public IEngineContextFactory EngineContextFactory
		{
			get
			{
				if (engContextFactoryCached == null)
				{
					engContextFactoryCached = GetService<IEngineContextFactory>();
				}
				return engContextFactoryCached;
			}
			set { engContextFactoryCached = value; }
		}

		/// <summary>
		/// Gets or sets the controller factory.
		/// </summary>
		/// <value>The controller factory.</value>
		public IControllerFactory ControllerFactory
		{
			get
			{
				if (controllerFactoryCached == null)
				{
					controllerFactoryCached = GetService<IControllerFactory>();
				}
				return controllerFactoryCached;
			}
			set { controllerFactoryCached = value; }
		}

		/// <summary>
		/// Gets or sets the controller context factory.
		/// </summary>
		/// <value>The controller context factory.</value>
		public IControllerContextFactory ControllerContextFactory
		{
			get
			{
				if (controllerContextFactoryCached == null)
				{
					controllerContextFactoryCached = GetService<IControllerContextFactory>();
				}
				return controllerContextFactoryCached;
			}
			set { controllerContextFactoryCached = value; }
		}

		/// <summary>
		/// Gets or sets the controller tree.
		/// </summary>
		/// <value>The controller tree.</value>
		public IControllerTree ControllerTree
		{
			get
			{
				if (controllerTreeCached == null)
				{
					controllerTreeCached = GetService<IControllerTree>();
				}
				return controllerTreeCached;
			}
			set { controllerTreeCached = value; }
		}

		/// <summary>
		/// Gets or sets the view source loader.
		/// </summary>
		/// <value>The view source loader.</value>
		public IViewSourceLoader ViewSourceLoader
		{
			get
			{
				if (viewSourceLoaderCached == null)
				{
					viewSourceLoaderCached = GetService<IViewSourceLoader>();
				}
				return viewSourceLoaderCached;
			}
			set { viewSourceLoaderCached = value; }
		}

		/// <summary>
		/// Gets or sets the filter factory.
		/// </summary>
		/// <value>The filter factory.</value>
		public IFilterFactory FilterFactory
		{
			get
			{
				if (filterFactoryCached == null)
				{
					filterFactoryCached = GetService<IFilterFactory>();
				}
				return filterFactoryCached;
			}
			set { filterFactoryCached = value; }
		}

		/// <summary>
		/// Gets or sets the controller descriptor provider.
		/// </summary>
		/// <value>The controller descriptor provider.</value>
		public IControllerDescriptorProvider ControllerDescriptorProvider
		{
			get
			{
				if (controllerDescriptorProviderCached == null)
				{
					controllerDescriptorProviderCached = GetService<IControllerDescriptorProvider>();
				}
				return controllerDescriptorProviderCached;
			}
			set { controllerDescriptorProviderCached = value; }
		}

		/// <summary>
		/// Gets or sets the view engine manager.
		/// </summary>
		/// <value>The view engine manager.</value>
		public IViewEngineManager ViewEngineManager
		{
			get
			{
				if (viewEngineManagerCached == null)
				{
					viewEngineManagerCached = GetService<IViewEngineManager>();
				}
				return viewEngineManagerCached;
			}
			set { viewEngineManagerCached = value; }
		}

		/// <summary>
		/// Gets or sets the validator registry.
		/// </summary>
		/// <value>The validator registry.</value>
		public IValidatorRegistry ValidatorRegistry
		{
			get
			{
				if (validatorRegistryCached == null)
				{
					validatorRegistryCached = GetService<IValidatorRegistry>();
				}
				return validatorRegistryCached;
			}
			set { validatorRegistryCached = value; }
		}

		/// <summary>
		/// Gets or sets the action selector.
		/// </summary>
		/// <value>The action selector.</value>
		public IActionSelector ActionSelector
		{
			get
			{
				if (actionSelectorCached == null)
				{
					actionSelectorCached = GetService<IActionSelector>();
				}
				return actionSelectorCached;
			}
			set { actionSelectorCached = value; }
		}

		/// <summary>
		/// Gets or sets the scaffold support.
		/// </summary>
		/// <value>The scaffold support.</value>
		public IScaffoldingSupport ScaffoldSupport
		{
			get
			{
				if (scaffoldSupportCached == null)
				{
					scaffoldSupportCached = GetService<IScaffoldingSupport>();
				}
				return scaffoldSupportCached;
			}
			set { scaffoldSupportCached = value; }
		}

		/// <summary>
		/// Gets or sets the JSON serializer.
		/// </summary>
		/// <value>The JSON serializer.</value>
		public IJSONSerializer JSONSerializer
		{
			get
			{
				if (jsonSerializerCached == null)
				{
					jsonSerializerCached = GetService<IJSONSerializer>();
				}
				return jsonSerializerCached;
			}
			set { jsonSerializerCached = value; }
		}

		/// <summary>
		/// Gets or sets the static resource registry service.
		/// </summary>
		/// <value>The static resource registry.</value>
		public IStaticResourceRegistry StaticResourceRegistry
		{
			get
			{
				if (staticResourceRegCached == null)
				{
					staticResourceRegCached = GetService<IStaticResourceRegistry>();
				}
				return staticResourceRegCached;
			}
			set { staticResourceRegCached = value; }
		}

		/// <summary>
		/// Gets or sets the email template service.
		/// </summary>
		/// <value>The email template service.</value>
		public IEmailTemplateService EmailTemplateService
		{
			get
			{
				if (emailTemplateServiceCached == null)
				{
					emailTemplateServiceCached = GetService<IEmailTemplateService>();
				}
				return emailTemplateServiceCached;
			}
			set { emailTemplateServiceCached = value; }
		}

		/// <summary>
		/// Gets or sets the email sender.
		/// </summary>
		/// <value>The email sender.</value>
		public IEmailSender EmailSender
		{
			get
			{
				if (emailSenderCached == null)
				{
					emailSenderCached = GetService<IEmailSender>();
				}
				return emailSenderCached;
			}
			set { emailSenderCached = value; }
		}

		/// <summary>
		/// Gets or sets the extension manager.
		/// </summary>
		/// <value>The extension manager.</value>
		public ExtensionManager ExtensionManager
		{
			get { return extensionManager; }
			set { extensionManager = value; }
		}

		/// <summary>
		/// Gets or sets the resource factory.
		/// </summary>
		/// <value>The resource factory.</value>
		public IResourceFactory ResourceFactory
		{
			get
			{
				if (resourceFactoryCached == null)
				{
					resourceFactoryCached = GetService<IResourceFactory>();
				}
				return resourceFactoryCached;
			}
			set { resourceFactoryCached = value; }
		}

		#endregion

		private void RegisterServiceOverrideFromConfigurationNode(IConfiguration serviceConfig)
		{
			if (serviceConfig == null) throw new ArgumentNullException("serviceConfig");

			string id = serviceConfig.Attributes["id"];
			string _interface = serviceConfig.Attributes["interface"];
			string type = serviceConfig.Attributes["type"];

			ServiceIdentification servId = (ServiceIdentification) Enum.Parse(typeof(ServiceIdentification), id, true);

			if (servId == ServiceIdentification.Undefined)
			{
				throw new MonoRailException("Invalid service id: '" + id + "'. " +
				                            "Check your configuration file, services node under MonoRail configuration.");
			}
			if (string.IsNullOrEmpty(type))
			{
				throw new MonoRailException("No type specified for service: '" + id + "'. You must add a 'type' attribute. " +
				                            "Check your configuration file, services node under MonoRail configuration.");
			}

			Type service;

			if (!string.IsNullOrEmpty(_interface))
			{
				service = Type.GetType(_interface, false, false);

				if (service == null)
				{
					throw new MonoRailException("Could not load service type: '" + _interface + "'.");
				}
			}
			else
			{
				service = InferServiceFromId(servId);
			}

			Type impl = Type.GetType(type, false, false);

			if (impl == null)
			{
				throw new MonoRailException("Could not load service implementation: '" + impl + "'.");
			}

			object instance;

			try
			{
				instance = CreateService(impl);
			}
			catch(Exception ex)
			{
				throw new MonoRailException("Could not create implementation: '" + impl + "'.", ex);
			}

			AddService(service, instance);
		}

		private static Type InferServiceFromId(ServiceIdentification id)
		{
			switch(id)
			{
				case ServiceIdentification.ControllerFactory:
					return typeof(IControllerFactory);
				case ServiceIdentification.ControllerContextFactory:
					return typeof(IControllerContextFactory);
				case ServiceIdentification.ControllerTree:
					return typeof(IControllerTree);
				case ServiceIdentification.CacheProvider:
					return typeof(ICacheProvider);
				case ServiceIdentification.UrlBuilder:
					return typeof(IUrlBuilder);
				case ServiceIdentification.UrlTokenizer:
					return typeof(IUrlTokenizer);
				case ServiceIdentification.ServerUtility:
					return typeof(IServerUtility);
				case ServiceIdentification.FilterFactory:
					return typeof(IFilterFactory);
				case ServiceIdentification.ControllerDescriptorProvider:
					return typeof(IControllerDescriptorProvider);
				case ServiceIdentification.ResourceDescriptorProvider:
					return typeof(IResourceDescriptorProvider);
				case ServiceIdentification.RescueDescriptorProvider:
					return typeof(IRescueDescriptorProvider);
				case ServiceIdentification.LayoutDescriptorProvider:
					return typeof(ILayoutDescriptorProvider);
				case ServiceIdentification.HelperDescriptorProvider:
					return typeof(IHelperDescriptorProvider);
				case ServiceIdentification.FilterDescriptorProvider:
					return typeof(IFilterDescriptorProvider);
				case ServiceIdentification.ViewSourceLoader:
					return typeof(IViewSourceLoader);
				case ServiceIdentification.ResourceFactory:
					return typeof(IResourceFactory);
				case ServiceIdentification.ViewEngineManager:
					return typeof(IViewEngineManager);
				case ServiceIdentification.TransformFilterDescriptorProvider:
					return typeof(ITransformFilterDescriptorProvider);
				case ServiceIdentification.ValidatorRegistry:
					return typeof(IValidatorRegistry);
//				case ServiceIdentification.EmailSender:
//					return typeof(IEmailSender);
				case ServiceIdentification.ViewComponentFactory:
					return typeof(IViewComponentFactory);
				case ServiceIdentification.ScaffoldingSupport:
					return typeof(IScaffoldingSupport);
//				case ServiceIdentification.EmailTemplateService:
//					return typeof(IEmailTemplateService);
//				case ServiceIdentification.TransformationFilterFactory:
//					return typeof(ITransformFilterFactory);
//				case ServiceIdentification.AjaxProxyGenerator:
//					return typeof(IAjaxProxyGenerator);
//				case ServiceIdentification.ViewComponentDescriptorProvider:
//					return typeof(IViewComponentDescriptorProvider);
				default:
					throw new NotSupportedException("Id not supported " + id);
			}
		}

		private static void AssertImplementsService(Type service, Type impl)
		{
			if (!service.IsAssignableFrom(impl))
			{
				String message = String.Format("Initialization Exception: " +
											   "Service {0} does not implement or extend {1}", impl.FullName, service.FullName);
				throw new ConfigurationErrorsException(message);
			}
		}
	}
}