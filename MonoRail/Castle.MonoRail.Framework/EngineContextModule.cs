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
	using System.ComponentModel.Design;
	using System.Web;

	using Castle.Components.Common.EmailSender;
	using Castle.Components.Common.EmailSender.SmtpEmailSender;

	using Castle.MonoRail.Framework.Adapters;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Views.Aspx;

	/// <summary>
	/// Pendent
	/// </summary>
	public class EngineContextModule : AbstractServiceContainer, IHttpModule 
	{
		internal static readonly String RailsContextKey = "rails.context";
		
		private static bool initialized;
		
		private ExtensionManager extensionManager;

		/// <summary>Keeps extensions. Just prevents them from being collected.</summary>
		private ArrayList extensions = new ArrayList();
		private MonoRailConfiguration monoRailConfiguration;
		private IViewEngine viewEngine;
		private IFilterFactory filterFactory;
		private IResourceFactory resourceFactory;
		private IControllerFactory controllerFactory;
		private IScaffoldingSupport scaffoldingSupport;
		private IViewComponentFactory viewCompFactory;
		private IEmailSender emailSender;
		private ControllerDescriptorBuilder controllerDescriptorBuilder;

		public void Init(HttpApplication context)
		{
			InitServices();
			InitApplicationHooks(context);

			initialized = true;
		}

		public void Dispose()
		{
			
		}

		private void InitServices()
		{
			monoRailConfiguration = ObtainConfiguration();

			InitializeDescriptorBuilder();

			InitializeExtensions();
			InitializeControllerFactory();
			InitializeViewComponentFactory();
			InitializeFilterFactory();
			InitializeResourceFactory();
			InitializeViewEngine();
			InitializeScaffoldingSupport();
			InitializeEmailSender();

			ConnectViewComponentFactoryToViewEngine();

			RegisterServices(this);
		}

		private void InitApplicationHooks(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(OnBeginRequest);
			context.EndRequest += new EventHandler(OnEndRequest);
			context.AcquireRequestState += new EventHandler(OnAcquireRequestState);
			context.ReleaseRequestState += new EventHandler(OnReleaseRequestState);
			context.PreRequestHandlerExecute += new EventHandler(OnPreRequestHandlerExecute);
			context.PostRequestHandlerExecute += new EventHandler(OnPostRequestHandlerExecute);
			context.Error += new EventHandler(OnError);
		}

		private MonoRailConfiguration ObtainConfiguration()
		{
			return MonoRailConfiguration.GetConfig();
		}

		private void InitializeDescriptorBuilder()
		{
			controllerDescriptorBuilder = new ControllerDescriptorBuilder();
		}

		protected virtual void InitializeExtensions()
		{
			extensionManager = new ExtensionManager();

			foreach(Type extensionType in monoRailConfiguration.Extensions)
			{
				IMonoRailExtension extension = 
					Activator.CreateInstance( extensionType ) as IMonoRailExtension;

				extension.Init(extensionManager, monoRailConfiguration);

				extensions.Add(extension);
			}
		}

		protected virtual void InitializeViewEngine()
		{
			if (monoRailConfiguration.CustomViewEngineType != null)
			{
				viewEngine = (IViewEngine) 
					Activator.CreateInstance(monoRailConfiguration.CustomViewEngineType);
			}
			else
			{
				// If nothing was specified, we use the default view engine 
				// based on webforms
				viewEngine = new AspNetViewEngine();
			}

			viewEngine.ViewRootDir = monoRailConfiguration.ViewsPhysicalPath;
			viewEngine.XhtmlRendering = monoRailConfiguration.ViewsXhtmlRendering;

			viewEngine.Init();
		}

		protected virtual void InitializeFilterFactory()
		{
			if (monoRailConfiguration.CustomFilterFactoryType != null)
			{
				filterFactory = (IFilterFactory) Activator.CreateInstance(monoRailConfiguration.CustomFilterFactoryType);
			}
			else
			{
				filterFactory = new DefaultFilterFactory();
			}
		}

		protected virtual void InitializeViewComponentFactory()
		{
			if (monoRailConfiguration.CustomViewComponentFactoryType != null)
			{
				viewCompFactory = (IViewComponentFactory) 
					Activator.CreateInstance(monoRailConfiguration.CustomViewComponentFactoryType);
			}
			else
			{
				DefaultViewComponentFactory compFactory = new DefaultViewComponentFactory();

				foreach(String assemblyName in monoRailConfiguration.ComponentsAssemblies)
				{
					compFactory.Inspect(assemblyName);
				}

				viewCompFactory = compFactory;
			}
		}

		protected virtual void InitializeResourceFactory()
		{
			if (monoRailConfiguration.CustomResourceFactoryType != null)
			{
				resourceFactory = (IResourceFactory) 
					Activator.CreateInstance(monoRailConfiguration.CustomResourceFactoryType);
			}
			else
			{
				resourceFactory = new DefaultResourceFactory();
			}
		}
		
		protected virtual void InitializeScaffoldingSupport()
		{
			if (monoRailConfiguration.ScaffoldingType != null)
			{
				scaffoldingSupport = (IScaffoldingSupport) 
					Activator.CreateInstance(monoRailConfiguration.ScaffoldingType);
			}
		}

		protected virtual void InitializeControllerFactory()
		{
			if (monoRailConfiguration.CustomControllerFactoryType != null)
			{
				controllerFactory = (IControllerFactory) 
					Activator.CreateInstance(monoRailConfiguration.CustomControllerFactoryType);
			}
			else
			{
				DefaultControllerFactory factory = new DefaultControllerFactory();

				foreach(String assemblyName in monoRailConfiguration.ControllerAssemblies)
				{
					factory.Inspect(assemblyName);
				}

				controllerFactory = factory;
			}
		}

		protected void InitializeEmailSender()
		{
			// TODO: allow user to customize this

			emailSender = new SmtpSender(monoRailConfiguration.SmtpConfig.Host);

			ISupportInitialize initializer = emailSender as ISupportInitialize;

			if (initializer != null)
			{
				initializer.BeginInit();
				initializer.EndInit();
			}
		}
		
		private void ConnectViewComponentFactoryToViewEngine()
		{
			viewCompFactory.ViewEngine = viewEngine;
			viewEngine.ViewComponentFactory = viewCompFactory;
		}

		#region Hooks dispatched to extensions

		private void OnBeginRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication) sender;
			HttpContext context = app.Context;

			IRailsEngineContext mrContext = CreateRailsEngineContext(context);

			extensionManager.RaiseContextCreated(mrContext);
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			extensionManager.RaiseContextDisposed(mrContext);
		}

		private void OnAcquireRequestState(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			extensionManager.RaiseAcquireRequestState(mrContext);
		}

		private void OnReleaseRequestState(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			extensionManager.RaiseReleaseRequestState(mrContext);
		}

		private void OnPreRequestHandlerExecute(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			extensionManager.RaisePreProcess(mrContext);
		}

		private void OnPostRequestHandlerExecute(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			extensionManager.RaisePostProcess(mrContext);
		}

		private void OnError(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			extensionManager.RaiseUnhandledError(mrContext);
		}

		private static IRailsEngineContext ObtainContextFromApplication(object sender)
		{
			HttpApplication app = (HttpApplication) sender;
			HttpContext context = app.Context;

			return ObtainRailsEngineContext(context);
		}

		#endregion

		private IRailsEngineContext CreateRailsEngineContext(HttpContext context)
		{
			IRailsEngineContext mrContext = ObtainRailsEngineContext(context);

			if (mrContext == null)
			{
				mrContext = new DefaultRailsEngineContext(this, context);

				context.Items[RailsContextKey] = mrContext;
			}

			return mrContext;
		}

		protected virtual void RegisterServices(IServiceContainer context)
		{
			context.AddService(typeof(ExtensionManager), extensionManager);
			context.AddService(typeof(IControllerFactory), controllerFactory);
			context.AddService(typeof(IViewEngine), viewEngine);
			context.AddService(typeof(IFilterFactory), filterFactory);
			context.AddService(typeof(IResourceFactory), resourceFactory);
			context.AddService(typeof(IScaffoldingSupport), scaffoldingSupport);
			context.AddService(typeof(IViewComponentFactory), viewCompFactory);
			context.AddService(typeof(ControllerDescriptorBuilder), controllerDescriptorBuilder);
			context.AddService(typeof(IEmailSender), emailSender);
			context.AddService(typeof(MonoRailConfiguration), monoRailConfiguration);
		}

		internal static bool Initialized
		{
			get { return initialized; }
		}

		internal static IRailsEngineContext ObtainRailsEngineContext(HttpContext context)
		{
			return (IRailsEngineContext) context.Items[RailsContextKey];
		}
	}
}
