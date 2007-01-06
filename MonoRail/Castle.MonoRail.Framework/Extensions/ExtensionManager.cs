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
	using System.ComponentModel;
	using System.ComponentModel.Design;

	public delegate void ExtensionHandler(IRailsEngineContext context);

	public class ExtensionManager : MarshalByRefObject
	{
		private static readonly object ContextCreatedEvent = new object();
		private static readonly object ContextDisposedEvent = new object();
		private static readonly object ActionExceptionEvent = new object();
		private static readonly object UnhandledExceptionEvent = new object();
		private static readonly object AcquireSessionStateEvent = new object();
		private static readonly object ReleaseSessionStateEvent = new object();
		private static readonly object PreProcessEvent = new object();
		private static readonly object PostProcessEvent = new object();
		private static readonly object AuthorizeRequestEvent = new object();
		private static readonly object AuthenticateRequestEvent = new object();
		private static readonly object ResolveRequestCacheEvent = new object();
		private static readonly object UpdateRequestCacheEvent = new object();

		private EventHandlerList events;

		private IServiceContainer serviceContainer;

		public ExtensionManager(IServiceContainer serviceContainer)
		{
			events = new EventHandlerList();
			this.serviceContainer = serviceContainer;
		}

		public IServiceContainer ServiceContainer
		{
			get { return serviceContainer; }
		}

		public event ExtensionHandler ContextCreated
		{
			add { events.AddHandler(ContextCreatedEvent, value); }
			remove { events.RemoveHandler(ContextCreatedEvent, value); }
		}

		public event ExtensionHandler ContextDisposed		
		{
			add { events.AddHandler(ContextDisposedEvent, value); }
			remove { events.RemoveHandler(ContextDisposedEvent, value); }
		}

		public event ExtensionHandler ActionException		
		{
			add { events.AddHandler(ActionExceptionEvent, value); }
			remove { events.RemoveHandler(ActionExceptionEvent, value); }
		}

		public event ExtensionHandler UnhandledException
		{
			add { events.AddHandler(UnhandledExceptionEvent, value); }
			remove { events.RemoveHandler(UnhandledExceptionEvent, value); }
		}

		public event ExtensionHandler AcquireSessionState
		{
			add { events.AddHandler(AcquireSessionStateEvent, value); }
			remove { events.RemoveHandler(AcquireSessionStateEvent, value); }
		}

		public event ExtensionHandler ReleaseSessionState
		{
			add { events.AddHandler(ReleaseSessionStateEvent, value); }
			remove { events.RemoveHandler(ReleaseSessionStateEvent, value); }
		}

		public event ExtensionHandler PreProcess
		{
			add { events.AddHandler(PreProcessEvent, value); }
			remove { events.RemoveHandler(PreProcessEvent, value); }
		}

		public event ExtensionHandler PostProcess
		{
			add { events.AddHandler(PostProcessEvent, value); }
			remove { events.RemoveHandler(PostProcessEvent, value); }
		}

		public event ExtensionHandler AuthenticateRequest
		{
			add { events.AddHandler(AuthenticateRequestEvent, value); }
			remove { events.RemoveHandler(AuthenticateRequestEvent, value); }
		}

		public event ExtensionHandler AuthorizeRequest
		{
			add { events.AddHandler(AuthorizeRequestEvent, value); }
			remove { events.RemoveHandler(AuthorizeRequestEvent, value); }
		}

		public event ExtensionHandler ResolveRequestCache
		{
			add { events.AddHandler(ResolveRequestCacheEvent, value); }
			remove { events.RemoveHandler(ResolveRequestCacheEvent, value); }
		}

		public event ExtensionHandler UpdateRequestCache
		{
			add { events.AddHandler(UpdateRequestCacheEvent, value); }
			remove { events.RemoveHandler(UpdateRequestCacheEvent, value); }
		}

		internal void RaiseContextCreated(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[ContextCreatedEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseContextDisposed(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[ContextDisposedEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaisePostProcess(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[PostProcessEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaisePreProcess(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[PreProcessEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseReleaseRequestState(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[ReleaseSessionStateEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseAcquireRequestState(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[AcquireSessionStateEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseUnhandledError(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[UnhandledExceptionEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseActionError(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[ActionExceptionEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseAuthenticateRequest(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[AuthenticateRequestEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseAuthorizeRequest(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[AuthorizeRequestEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseResolveRequestCache(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[UpdateRequestCacheEvent];
			if (eventDelegate != null) eventDelegate(context);
		}
		
		internal void RaiseUpdateRequestCache(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[UpdateRequestCacheEvent];
			if (eventDelegate != null) eventDelegate(context);
		}
	}
}
