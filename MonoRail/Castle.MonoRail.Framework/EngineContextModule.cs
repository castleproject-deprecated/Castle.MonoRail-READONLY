// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Web;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Adapters;

	/// <summary>
	/// Provides the services used and shared by the framework. Also 
	/// is in charge of creating an implementation of <see cref="IRailsEngineContext"/>
	/// upon the start of a new request.
	/// </summary>
	public class EngineContextModule : IHttpModule
	{
		internal static readonly String RailsContextKey = "rails.context";
		private static readonly Object initLock = new Object();

		private static MonoRailServiceContainer container;

		private ILogger logger = NullLogger.Instance;

		/// <summary>
		/// Configures the framework, starts the services
		/// and application hooks.
		/// </summary>
		/// <param name="context"></param>
		public void Init(HttpApplication context)
		{
			if (context.Context.Error != null)
			{
				throw new Exception(
					"An exception happened on Global application or on a module that run before MonoRail's module. " + 
					"MonoRail will not be initialized and further requests are going to fail. " + 
					"Fix the cause of the error reported below.", context.Context.Error);
			}

			lock(initLock)
			{
				if (container == null)
				{
					container = new MonoRailServiceContainer();
					container.RegisterBaseService(typeof(IServerUtility), new ServerUtilityAdapter(context.Server));
					container.Start();

					ILoggerFactory loggerFactory = (ILoggerFactory) container.GetService(typeof(ILoggerFactory));

					if (loggerFactory != null)
					{
						logger = loggerFactory.Create(typeof(EngineContextModule));
					}
				}
			}

			context.BeginRequest += new EventHandler(OnStartMonoRailRequest);
			context.AuthorizeRequest += new EventHandler(CreateControllerAndRunStartRequestFilters);

			SubscribeToApplicationHooks(context);
		}

		/// <summary>
		/// Disposes of the resources (other than memory) used by the
		/// module that implements <see langword="IHttpModule."/>
		/// </summary>
		public void Dispose()
		{
		}

		#region MonoRail Request Lifecycle

		/// <summary>
		/// This method is invoked in response to BeginRequest event.
		/// It checks if the request should be treat by MonoRail (by reading the file extension)
		/// and if so, creates the <see cref="IRailsEngineContext"/> instance.
		/// </summary>
		/// <param name="sender">The HttpApplication instance</param>
		/// <param name="e">Event information</param>
		private void OnStartMonoRailRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication) sender;

			// Is this request a MonoRail request?
			if (!container.IsMonoRailRequest(app.Context.Request.FilePath))
			{
				return;
			}

			// Mark it so we dont have to check the file extension again
			MarkRequestAsMonoRailRequest(app.Context);

			// Creates the our context
			IRailsEngineContext context = CreateRailsEngineContext(app.Context);

			#region TestSupport related

			String isTest = context.Request.Headers["IsTestWorkerRequest"];
			
			if ("true" == isTest)
			{
				Castle.MonoRail.Framework.Internal.Test.TestContextHolder.SetContext(app.Context);
			}

			#endregion
		}

		/// <summary>
		/// Creates the controller, selects the target action 
		/// and run start request filters.
		/// </summary>
		/// <param name="sender">The HttpApplication instance</param>
		/// <param name="e">Event information</param>
		private void CreateControllerAndRunStartRequestFilters(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication) sender;
			if (!IsMonoRailRequest(app.Context)) return;

			IRailsEngineContext context = ObtainRailsEngineContext(app.Context);

			Controller controller = CreateController(context);

			IControllerLifecycleExecutor executor = CreateControllerExecutor(controller, context);

			UrlInfo info = context.UrlInfo;

			executor.InitializeController(info.Area, info.Controller, info.Action);

			if (!executor.SelectAction(info.Action, info.Controller))
			{
				// Could not even select the action, stop here

				return;
			}

			if (!executor.RunStartRequestFilters())
			{
				// Cancel request execution as
				// one of the filters returned false
				// We assume that the filter that stopped the request, also sent a redirect
				// or something similar

				context.UnderlyingContext.Response.End();

				executor.Dispose();
			}
		}

		#endregion

		private void MarkRequestAsMonoRailRequest(HttpContext context)
		{
			context.Items["is.mr.request"] = true;
		}

		private bool IsMonoRailRequest(HttpContext context)
		{
			return context.Items.Contains("is.mr.request");
		}

		/// <summary>
		/// Uses the url information and the controller factory
		/// to instantiate the proper controller.
		/// </summary>
		/// <param name="context">MonoRail's request context</param>
		/// <returns>A controller instance</returns>
		private Controller CreateController(IRailsEngineContext context)
		{
			UrlInfo info = context.UrlInfo;

			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Starting request process for '{0}'/'{1}.{2}' Extension '{3}' with url '{4}'",
					info.Area, info.Controller, info.Action, info.Extension, info.UrlRaw);
			}

			IControllerFactory controllerFactory = (IControllerFactory) context.GetService(typeof(IControllerFactory));

			Controller controller = controllerFactory.CreateController(info);

			return controller;
		}

		/// <summary>
		/// Creates the and initialize executor.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		private IControllerLifecycleExecutor CreateControllerExecutor(Controller controller, IRailsEngineContext context)
		{
			IControllerLifecycleExecutorFactory factory = 
				(IControllerLifecycleExecutorFactory) context.GetService(typeof(IControllerLifecycleExecutorFactory));

			IControllerLifecycleExecutor executor = factory.CreateExecutor(controller, context);
			
			context.Items[ControllerLifecycleExecutor.ExecutorEntry] = executor;

			return executor;
		}

		/// <summary>
		/// Registers to <c>HttpApplication</c> events
		/// </summary>
		/// <param name="context">The application instance</param>
		private void SubscribeToApplicationHooks(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(OnBeginRequest);
			context.EndRequest += new EventHandler(OnEndRequest);
			context.AcquireRequestState += new EventHandler(OnAcquireRequestState);
			context.ReleaseRequestState += new EventHandler(OnReleaseRequestState);
			context.PreRequestHandlerExecute += new EventHandler(OnPreRequestHandlerExecute);
			context.PostRequestHandlerExecute += new EventHandler(OnPostRequestHandlerExecute);
			context.AuthenticateRequest += new EventHandler(OnAuthenticateRequest);
			context.AuthorizeRequest += new EventHandler(OnAuthorizeRequest);
			context.Error += new EventHandler(OnError);
			context.ResolveRequestCache += new EventHandler(OnResolveRequestCache);
			context.UpdateRequestCache += new EventHandler(OnUpdateRequestCache);
		}

		#region Hooks dispatched to extensions

		private void OnBeginRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication) sender;
			if (!IsMonoRailRequest(app.Context)) return;
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			if (mrContext == null) throw new NullReferenceException("mrContext");

			if (container == null) throw new NullReferenceException("container");
			if (container.extensionManager == null) throw new NullReferenceException("container.extensionManager");

			container.extensionManager.RaiseContextCreated(mrContext);
		}

		private void OnAuthenticateRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if (!IsMonoRailRequest(app.Context)) return;
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaiseAuthenticateRequest(mrContext);
		}

		private void OnAuthorizeRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if (!IsMonoRailRequest(app.Context)) return;
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaiseAuthorizeRequest(mrContext);
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if (!IsMonoRailRequest(app.Context)) return;
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaiseContextDisposed(mrContext);
		}

		private void OnAcquireRequestState(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if (!IsMonoRailRequest(app.Context)) return;
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaiseAcquireRequestState(mrContext);
		}

		private void OnReleaseRequestState(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if (!IsMonoRailRequest(app.Context)) return;
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaiseReleaseRequestState(mrContext);
		}

		private void OnPreRequestHandlerExecute(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if (!IsMonoRailRequest(app.Context)) return;
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaisePreProcess(mrContext);
		}

		private void OnPostRequestHandlerExecute(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if (!IsMonoRailRequest(app.Context)) return;
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaisePostProcess(mrContext);
		}

		private void OnError(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if (!IsMonoRailRequest(app.Context)) return;
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			mrContext.LastException = mrContext.UnderlyingContext.Server.GetLastError();

			container.extensionManager.RaiseUnhandledError(mrContext);
		}

		private void OnResolveRequestCache(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if (!IsMonoRailRequest(app.Context)) return;
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaiseResolveRequestCache(mrContext);
		}

		private void OnUpdateRequestCache(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if (!IsMonoRailRequest(app.Context)) return;
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaiseUpdateRequestCache(mrContext);
		}

		#endregion

		private IRailsEngineContext CreateRailsEngineContext(HttpContext context)
		{
			IRailsEngineContext mrContext = ObtainRailsEngineContext(context);

			if (mrContext == null)
			{
				IUrlTokenizer urlTokenizer = (IUrlTokenizer) container.GetService(typeof(IUrlTokenizer));

				HttpRequest req = context.Request;

				UrlInfo urlInfo = urlTokenizer.TokenizeUrl(req.FilePath, req.Url, req.IsLocal, req.ApplicationPath);

				DefaultRailsEngineContext newContext = new DefaultRailsEngineContext(container, urlInfo, context);

				context.Items[RailsContextKey] = newContext;

				newContext.AddService(typeof(IRailsEngineContext), newContext);

				mrContext = newContext;
			}

			return mrContext;
		}

		private static IRailsEngineContext ObtainContextFromApplication(object sender)
		{
			HttpApplication app = (HttpApplication) sender;
			HttpContext context = app.Context;

			return ObtainRailsEngineContext(context);
		}

		internal static bool Initialized
		{
			get { return container != null; }
		}

		internal static IRailsEngineContext ObtainRailsEngineContext(HttpContext context)
		{
			return (IRailsEngineContext) context.Items[RailsContextKey];
		}
	}
}
