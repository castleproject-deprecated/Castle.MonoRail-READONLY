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

namespace Castle.MonoRail.Engine
{
	using System;
	using System.Web;
	using System.Web.SessionState;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Views.Aspx;

	using Castle.MonoRail.Engine.Configuration;

	/// <summary>
	/// Coordinates the creation of new <see cref="MonoRailHttpHandler"/> 
	/// and uses the configuration to obtain the correct factories 
	/// instances.
	/// </summary>
	public class MonoRailHttpHandlerFactory : IHttpHandlerFactory, IRequiresSessionState
	{
		private MonoRailConfiguration _config;
		private IViewEngine _viewEngine;
		private IFilterFactory _filterFactory;
		private IResourceFactory _resourceFactory;
		private IControllerFactory _controllerFactory;
		private IScaffoldingSupport _scaffoldingSupport;
		private IViewComponentFactory _viewCompFactory;

		public MonoRailHttpHandlerFactory()
		{
			ObtainConfiguration();
			InitializeControllerFactory();
			InitializeViewComponentFactory();
			InitializeFilterFactory();
			InitializeResourceFactory();
			InitializeViewEngine();
			InitializeScaffoldingSupport();

			ConnectViewComponentFactoryToViewEngine();
		}

		#region IHttpHandlerFactory

		public virtual IHttpHandler GetHandler(HttpContext context, 
			String requestType, String url, String pathTranslated)
		{
			return new MonoRailHttpHandler(url, _viewEngine, _controllerFactory, 
				_filterFactory, _resourceFactory, _scaffoldingSupport, _viewCompFactory);
		}

		public virtual void ReleaseHandler(IHttpHandler handler)
		{
		}

		#endregion

		protected virtual void ObtainConfiguration()
		{
			_config = MonoRailConfiguration.GetConfig();
		}

		protected virtual void InitializeViewEngine()
		{
			if (_config.CustomViewEngineType != null)
			{
				_viewEngine = (IViewEngine) Activator.CreateInstance(_config.CustomViewEngineType);
			}
			else
			{
				// If nothing was specified, we use the default view engine 
				// based on webforms
				_viewEngine = new AspNetViewEngine();
			}

			_viewEngine.ViewRootDir = _config.ViewsPhysicalPath;
			_viewEngine.XhtmlRendering = _config.ViewsXhtmlRendering;

			_viewEngine.Init();
		}

		protected virtual void InitializeFilterFactory()
		{
			if (_config.CustomFilterFactoryType != null)
			{
				_filterFactory = (IFilterFactory) Activator.CreateInstance(_config.CustomFilterFactoryType);
			}
			else
			{
				_filterFactory = new DefaultFilterFactory();
			}
		}

		protected virtual void InitializeViewComponentFactory()
		{
			if (_config.CustomViewComponentFactoryType != null)
			{
				_viewCompFactory = (IViewComponentFactory) Activator.CreateInstance(_config.CustomViewComponentFactoryType);
			}
			else
			{
				DefaultViewComponentFactory compFactory = new DefaultViewComponentFactory();

				foreach(String assemblyName in _config.ComponentsAssemblies)
				{
					compFactory.Inspect(assemblyName);
				}

				_viewCompFactory = compFactory;
			}
		}

		protected virtual void InitializeResourceFactory()
		{
			if (_config.CustomResourceFactoryType != null)
			{
				_resourceFactory = (IResourceFactory) Activator.CreateInstance(_config.CustomResourceFactoryType);
			}
			else
			{
				_resourceFactory = new DefaultResourceFactory();
			}
		}
		
		protected virtual void InitializeScaffoldingSupport()
		{
			if (_config.ScaffoldingType != null)
			{
				_scaffoldingSupport = (IScaffoldingSupport) Activator.CreateInstance(_config.ScaffoldingType);
			}
		}

		protected virtual void InitializeControllerFactory()
		{
			if (_config.CustomControllerFactoryType != null)
			{
				_controllerFactory = (IControllerFactory) 
					Activator.CreateInstance(_config.CustomControllerFactoryType);
			}
			else
			{
				DefaultControllerFactory factory = new DefaultControllerFactory();

				foreach(String assemblyName in _config.ControllerAssemblies)
				{
					factory.Inspect(assemblyName);
				}

				_controllerFactory = factory;
			}
		}
		
		private void ConnectViewComponentFactoryToViewEngine()
		{
			_viewCompFactory.ViewEngine = _viewEngine;
			_viewEngine.ViewComponentFactory = _viewCompFactory;
		}
	}
}
