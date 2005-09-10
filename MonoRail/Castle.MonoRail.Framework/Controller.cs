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
	using System.Collections.Specialized;
	using System.IO;
	using System.Web;
	using System.Reflection;
	using System.Threading;

	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Implements the core functionality and exposes the
	/// common methods for concrete controllers.
	/// </summary>
	public abstract class Controller
	{
		#region Static and Instance Fields

		/// <summary>
		/// TODO: Document why this is necessary
		/// </summary>
		public static readonly String ControllerContextKey = "rails.controller";

		/// <summary>
		/// TODO: Document why this is necessary
		/// </summary>
		internal static readonly String OriginalViewKey = "rails.original_view";

		/// <summary>
		/// The reference to the <see cref="IViewEngine"/> instance
		/// </summary>
		private IViewEngine _viewEngine;

		/// <summary>
		/// Holds the request/context information
		/// </summary>
		private IRailsEngineContext _context;

		/// <summary>
		/// Holds information to pass to the view
		/// </summary>
		private IDictionary _bag;

		/// <summary>
		/// Holds the filters associated with the action
		/// </summary>
		private FilterDescriptor[] _filters;

		/// <summary>
		/// Reference to the <see cref="IFilterFactory"/> instance
		/// </summary>
		private IFilterFactory _filterFactory;

		/// <summary>
		/// Reference to the <see cref="IResourceFactory"/> instance
		/// </summary>
		private IResourceFactory _resourceFactory;

		/// <summary>
		/// The area name which was used to access this controller
		/// </summary>
		private String _areaName;

		/// <summary>
		/// The controller name which was used to access this controller
		/// </summary>
		private String _controllerName;

		/// <summary>
		/// The view name selected to be rendered after the execution 
		/// of the action
		/// </summary>
		private String _selectedViewName;

		/// <summary>
		/// The layout name that the view engine should use
		/// </summary>
		private String _layoutName;

		/// <summary>
		/// The original action requested
		/// </summary>
		private String _evaluatedAction;

		/// <summary>
		/// The helper instances collected
		/// </summary>
		private IDictionary _helpers = null;

		/// <summary>
		/// The resources associated with this controller
		/// </summary>
		private ResourceDictionary _resources = null;

		internal IDictionary _actions = new HybridDictionary(true);

		internal IDictionary _customActions = new HybridDictionary(true);

		private IScaffoldingSupport _scaffoldSupport;

		private IViewComponentFactory _viewComponentFactory;

		internal bool _directRenderInvoked;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a Controller
		/// </summary>
		public Controller()
		{
			_bag = new HybridDictionary();

			CollectActions();
		}

		#endregion

		#region Usefull Properties

		public ResourceDictionary Resources
		{
			get { return _resources; }
		}

		public IDictionary Helpers
		{
			get
			{
				if (_helpers == null) CreateAndInitializeHelpers();

				return _helpers;
			}
		}

		/// <summary>
		/// Gets the controller's name.
		/// </summary>
		public String Name
		{
			get { return _controllerName; }
		}

		/// <summary>
		/// Gets the controller's area name.
		/// </summary>
		public String AreaName
		{
			get { return _areaName; }
		}

		/// <summary>
		/// Gets or set the layout being used.
		/// </summary>
		public String LayoutName
		{
			get { return _layoutName; }
			set { _layoutName = value; }
		}

		/// <summary>
		/// Gets the name of the action being processed.
		/// </summary>
		public String Action
		{
			get { return _evaluatedAction; }
		}

		/// <summary>
		/// Gets the property bag, which is used
		/// to pass variables to the view.
		/// </summary>
		public IDictionary PropertyBag
		{
			get { return _bag; }
		}

		/// <summary>
		/// Gets the context of this web execution.
		/// </summary>
		public IRailsEngineContext Context
		{
			get { return _context; }
		}

		/// <summary>
		/// Gets the Session dictionary.
		/// </summary>
		protected IDictionary Session
		{
			get { return _context.Session; }
		}

		/// <summary>
		/// Gets a dictionary of volative items.
		/// Ideal for showing success and failures messages.
		/// </summary>
		protected IDictionary Flash
		{
			get { return _context.Flash; }
		}

		/// <summary>
		/// Gets the web context of ASP.NET API.
		/// </summary>
		protected HttpContext HttpContext
		{
			get { return _context.UnderlyingContext as HttpContext; }
		}

		/// <summary>
		/// Gets the request object.
		/// </summary>
		public IRequest Request
		{
			get { return Context.Request; }
		}

		/// <summary>
		/// Gets the response object.
		/// </summary>
		public IResponse Response
		{
			get { return Context.Response; }
		}

		/// <summary>
		/// Shortcut to Request.Params
		/// </summary>
		public NameValueCollection Params
		{
			get { return Request.Params; }
		}

		#endregion

		#region Usefull operations

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		public void RenderView(String name)
		{
			String basePath = _controllerName;

			if (_areaName != null)
			{
				basePath = Path.Combine(_areaName, _controllerName);
			}

			_selectedViewName = Path.Combine(basePath, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		public void RenderView(String name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();

			RenderView(name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		public void RenderView(String controller, String name)
		{
			_selectedViewName = Path.Combine(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		public void RenderView(String controller, String name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();

			RenderView(controller, name);
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public void RenderSharedView(String name)
		{
			_selectedViewName = name;
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public void RenderSharedView(String name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();
			
			RenderSharedView(name);
		}

		/// <summary>
		/// Cancels the view processing.
		/// </summary>
		public void CancelView()
		{
			_selectedViewName = null;
		}

		/// <summary>
		/// Cancels the layout processing.
		/// </summary>
		public void CancelLayout()
		{
			LayoutName = null;
		}

		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		/// <param name="contents"></param>
		public void RenderText(String contents)
		{
			CancelView();

			Response.Write(contents);
		}

		/// <summary>
		/// Sends raw contents to be rendered directly by the view engine.
		/// It's up to the view engine just to apply the layout and nothing else.
		/// </summary>
		/// <param name="contents">Contents to be rendered.</param>
		public void DirectRender(String contents)
		{
			CancelView();

			if (_directRenderInvoked)
			{
				throw new ControllerException("DirectRender should be called only once.");
			}

			_directRenderInvoked = true;

			_viewEngine.ProcessContents(_context, this, contents);
		}

		/// <summary>
		/// Returns true if the specified template exists.
		/// </summary>
		/// <param name="templateName"></param>
		public bool HasTemplate(String templateName)
		{
			return _viewEngine.HasTemplate(templateName);
		}

		/// <summary>
		/// Redirects to the specified URL.
		/// </summary>
		/// <param name="url">Target URL</param>
		public void Redirect(String url)
		{
			CancelView();

			_context.Response.Redirect(url);
		}

		/// <summary>
		/// Redirects to another controller and action.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		public void Redirect(String controller, String action)
		{
			CancelView();

			_context.Response.Redirect(controller, action);
		}

		/// <summary>
		/// Redirects to another controller and action.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		public void Redirect(String area, String controller, String action)
		{
			CancelView();

			_context.Response.Redirect(area, controller, action);
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(String controller, String action, NameValueCollection parameters)
		{
			CancelView();

			String querystring = String.Empty;

			HttpServerUtility serverUtility = HttpContext.Server;

			foreach (String key in parameters.Keys)
			{
				querystring += String.Format("{0}{1}={2}", (querystring.Length == 0 ? String.Empty : "&"),
				                             serverUtility.HtmlEncode(key),
				                             serverUtility.HtmlEncode(parameters[key]));
			}

			String url = UrlInfo.CreateAbsoluteRailsUrl(
				Context.ApplicationPath, controller, action, Context.UrlInfo.Extension);

			_context.Response.Redirect(String.Format("{0}?{1}", url, querystring));
		}

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		public void Redirect(String area, String controller, String action, NameValueCollection parameters)
		{
			CancelView();

			_context.Params.Add(parameters);

			_context.Response.Redirect(area, controller, action);
		}

		#endregion

		#region Core members

		public IDictionary CustomActions
		{
			get { return _customActions; }
		}

		protected internal virtual void CollectActions()
		{
			MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);

			foreach (MethodInfo m in methods)
			{
				_actions[m.Name] = m;
			}

			ScreenCommonPublicMethods(_actions);
		}

		protected void ScreenCommonPublicMethods(IDictionary actions)
		{
			actions.Remove("ToString");
			actions.Remove("GetHashCode");
			actions.Remove("RenderView");
			actions.Remove("RenderSharedView");
			actions.Remove("CancelView");
			actions.Remove("RenderText");
			actions.Remove("DirectRender");
			actions.Remove("Redirect");
			actions.Remove("Process");
			actions.Remove("Send");
			actions.Remove("PreSendView");
			actions.Remove("PostSendView");
		}

		/// <summary>
		/// Method invoked by the engine to start 
		/// the controller process. 
		/// </summary>
		public void Process(IRailsEngineContext context, IFilterFactory filterFactory, IResourceFactory resourceFactory,
		                    String areaName, String controllerName, String actionName, IViewEngine viewEngine,
		                    IScaffoldingSupport scaffoldSupport, IViewComponentFactory viewComponentFactory)
		{
			_areaName = areaName;
			_controllerName = controllerName;
			_viewEngine = viewEngine;
			_context = context;
			_filterFactory = filterFactory;
			_resourceFactory = resourceFactory;
			_scaffoldSupport = scaffoldSupport;
			_viewComponentFactory = viewComponentFactory;

			if (GetType().IsDefined(typeof(FilterAttribute), true))
			{
				_filters = CollectFilterDescriptor();
			}

			if (GetType().IsDefined(typeof(LayoutAttribute), true))
			{
				LayoutName = ObtainDefaultLayoutName();
			}

			if (GetType().IsDefined(typeof(ScaffoldingAttribute), false))
			{
				if (_scaffoldSupport == null)
				{
					String message = "You must enable scaffolding support on the " +
						"configuration file, or, to use the standard ActiveRecord support " +
						"copy the necessary assemblies to the bin folder.";

					throw new RailsException(message);
				}

				_scaffoldSupport.Process(this);
			}

			if (GetType().IsDefined(typeof(DynamicActionProviderAttribute), true))
			{
				ActionProviderUtil.RegisterActions(this);
			}

			Initialize();

			InternalSend(actionName);
		}

		/// <summary>
		/// Used to process the method associated
		/// with the action name specified.
		/// </summary>
		/// <param name="action"></param>
		public void Send(String action)
		{
			InternalSend(action);
		}

		/// <summary>
		/// Performs the specified action, which means:
		/// <br/>
		/// 1. Define the default view name<br/>
		/// 2. Run the before filters<br/>
		/// 3. Select the method related to the action name and invoke it<br/>
		/// 4. On error, execute the rescues if available<br/>
		/// 5. Run the after filters<br/>
		/// 6. Invoke the view engine<br/>
		/// </summary>
		/// <param name="action">Action name</param>
		protected virtual void InternalSend(String action)
		{
			// Record the action
			_evaluatedAction = action;

			// Record the default view for this area/controller/action
			RenderView(action);

			// If we have an HttpContext available, store the original view name
			if (HttpContext.Current != null)
			{
				if (!HttpContext.Current.Items.Contains(OriginalViewKey))
				{
					HttpContext.Current.Items[OriginalViewKey] = _selectedViewName;
				}
			}

			// Look for the target method
			MethodInfo method = SelectMethod(action, _actions, _context.Request);

			// If we couldn't find a method for this action, look for a dynamic action
			IDynamicAction dynAction = null;
			if (method == null)
			{
				dynAction = CustomActions[action] as IDynamicAction;

				if (dynAction == null)
				{
					method = FindOutDefaultMethod();

					if (method == null)
					{
						throw new ControllerException(String.Format("No action for '{0}' found", action));
					}
				}
			}

			HybridDictionary filtersToSkip = new HybridDictionary();

			bool skipFilters = ShouldSkip(method, filtersToSkip);
			bool hasError = false;

			try
			{
				// If we are supposed to run the filters...
				if (!skipFilters)
				{
					// ...run them. If they fail...
					if (!ProcessFilters(ExecuteEnum.Before, filtersToSkip))
					{
						// Record that they failed.
						hasError = true;
					}
				}

				// If we haven't failed anywhere yet...
				if (!hasError)
				{
					CreateResources(method);

					// Execute the method / dynamic action
					if (method != null)
					{
						InvokeMethod(method);
					}
					else
					{
						dynAction.Execute(this);
					}
				}
			}
			catch (ThreadAbortException)
			{
				hasError = true;
			}
			catch (Exception ex)
			{
				hasError = true;

				// Try and perform the rescue
				if (!PerformRescue(method, GetType(), ex))
				{
					//If the rescue fails, let the exception bubble
					throw;
				}
			}
			finally
			{
				// Run the filters if required
				if (!skipFilters)
				{
					ProcessFilters(ExecuteEnum.After, filtersToSkip);
				}

				DisposeFilter();
			}

			// If we haven't failed anywhere yet...
			if (!hasError)
			{
				// Render the actual view then cleanup
				ProcessView();

				ReleaseResources();
			}
		}

		/// <summary>
		/// The following lines were added to handle _default processing
		/// if present look for and load _default action method
		/// <seealso cref="DefaultActionAttribute"/>
		/// </summary>
		private MethodInfo FindOutDefaultMethod()
		{
			object[] attributes = this.GetType().GetCustomAttributes( typeof(DefaultActionAttribute), true );
	
			if (attributes.Length != 0)
			{
				DefaultActionAttribute attr = (DefaultActionAttribute)attributes[0];
				return SelectMethod(attr.DefaultAction, _actions, _context.Request);
			}

			return null;
		}

		protected virtual void CreateAndInitializeHelpers()
		{
			_helpers = new HybridDictionary();

			Attribute[] helpers = Attribute.GetCustomAttributes(this.GetType(), typeof(HelperAttribute));

			foreach (HelperAttribute helper in helpers)
			{
				object helperInstance = Activator.CreateInstance(helper.HelperType);

				IControllerAware aware = helperInstance as IControllerAware;

				if (aware != null)
				{
					aware.SetController(this);
				}

				_helpers.Add(helper.HelperType.Name, helperInstance);
			}

			AbstractHelper[] builtInHelpers =
				new AbstractHelper[]
					{
						new AjaxHelper(), new AjaxHelper2(),
						new EffectsFatHelper(), new Effects2Helper(),
						new DateFormatHelper(), new HtmlHelper(),
						new ValidationHelper(), new DictHelper()
					};

			foreach (AbstractHelper helper in builtInHelpers)
			{
				helper.SetController(this);

				string helperName = helper.GetType().Name;

				if (!_helpers.Contains(helperName))
				{
					_helpers.Add(helperName, helper);
				}
			}
		}

		#endregion

		#region Action Invocation

		protected virtual MethodInfo SelectMethod(String action, IDictionary actions, IRequest request)
		{
			return actions[action] as MethodInfo;
		}

		private void InvokeMethod(MethodInfo method)
		{
			InvokeMethod(method, _context.Request);
		}

		protected virtual void InvokeMethod(MethodInfo method, IRequest request)
		{
			method.Invoke(this, new object[0]);
		}

		#endregion

		#region Resources

		protected virtual void CreateResources(MethodInfo method)
		{
			_resources = new ResourceDictionary();

			Assembly typeAssembly = this.GetType().Assembly;

			Attribute[] resources = Attribute.GetCustomAttributes(this.GetType(), typeof(AbstractResourceAttribute));

			foreach (AbstractResourceAttribute resource in resources)
			{
				_resources.Add(resource.Name, _resourceFactory.Create(resource, typeAssembly));
			}

			if (method == null) return;

			resources = Attribute.GetCustomAttributes(method, typeof(AbstractResourceAttribute));

			foreach (AbstractResourceAttribute resource in resources)
			{
				_resources[resource.Name] = _resourceFactory.Create(resource, typeAssembly);
			}
		}

		protected virtual void ReleaseResources()
		{
			foreach (IResource resource in _resources.Values)
			{
				_resourceFactory.Release(resource);
			}
		}

		#endregion

		#region Filters

		protected internal bool ShouldSkip(MethodInfo method, IDictionary filtersToSkip)
		{
			if (_filters == null)
			{
				// No filters, so skip 
				return true;
			}

			if (!method.IsDefined(typeof(SkipFilterAttribute), true))
			{
				// Nothing against filters declared for this action
				return false;
			}

			object[] skipInfo = method.GetCustomAttributes(typeof(SkipFilterAttribute), true);

			foreach (SkipFilterAttribute skipfilter in skipInfo)
			{
				// If the user declared a [SkipFilterAttribute] then skip all filters
				if (skipfilter.BlanketSkip) return true;

				filtersToSkip[skipfilter.FilterType] = String.Empty;
			}

			return false;
		}

		protected internal FilterDescriptor[] CollectFilterDescriptor()
		{
			object[] attrs = GetType().GetCustomAttributes(typeof(FilterAttribute), true);
			FilterDescriptor[] desc = new FilterDescriptor[attrs.Length];

			for (int i = 0; i < attrs.Length; i++)
			{
				desc[i] = new FilterDescriptor(attrs[i] as FilterAttribute);
			}

			return desc;
		}

		private bool ProcessFilters(ExecuteEnum when, IDictionary filtersToSkip)
		{
			foreach (FilterDescriptor desc in _filters)
			{
				if (filtersToSkip.Contains(desc.FilterType)) continue;

				if ((desc.When & when) != 0)
				{
					if (!ProcessFilter(when, desc))
					{
						return false;
					}
				}
			}

			return true;
		}

		private bool ProcessFilter(ExecuteEnum when, FilterDescriptor desc)
		{
			if (desc.FilterInstance == null)
			{
				desc.FilterInstance = _filterFactory.Create(desc.FilterType);

				if (typeof(IFilterAttributeAware).IsInstanceOfType(desc.FilterInstance))
					(desc.FilterInstance as IFilterAttributeAware).FilterAttribute = desc.Attribute;
			}

			return desc.FilterInstance.Perform(when, _context, this);
		}

		private void DisposeFilter()
		{
			if (_filters == null) return;

			foreach (FilterDescriptor desc in _filters)
			{
				if (desc.FilterInstance != null)
				{
					_filterFactory.Release(desc.FilterInstance);
				}
			}
		}

		#endregion

		#region Views and Layout

		protected virtual String ObtainDefaultLayoutName()
		{
			object[] attrs = GetType().GetCustomAttributes(typeof(LayoutAttribute), true);

			if (attrs.Length == 1)
			{
				LayoutAttribute layoutDef = (LayoutAttribute) attrs[0];
				return layoutDef.LayoutName;
			}

			return null;
		}

		private void ProcessView()
		{
			if (_selectedViewName != null)
			{
				_viewEngine.Process(_context, this, _selectedViewName);
			}
		}

		#endregion

		#region Rescue

		protected virtual bool PerformRescue(MethodInfo method, Type controllerType, Exception ex)
		{
			if (ex is TargetInvocationException)
			{
				_context.LastException = ex.InnerException;
			}
			else
			{
				_context.LastException = ex;
			}

			if (method != null && method.IsDefined(typeof(SkipRescueAttribute), true))
			{
				return false;
			}

			RescueAttribute att = null;

			if (method != null && method.IsDefined(typeof(RescueAttribute), true))
			{
				att = method.GetCustomAttributes(
					typeof(RescueAttribute), true)[0] as RescueAttribute;
			}
			else if (controllerType.IsDefined(typeof(RescueAttribute), true))
			{
				att = controllerType.GetCustomAttributes(
					typeof(RescueAttribute), true)[0] as RescueAttribute;
			}

			if (att != null)
			{
				try
				{
					_selectedViewName = String.Format("rescues\\{0}", att.ViewName);
					ProcessView();
					return true;
				}
				catch (Exception e)
				{
					// In this situation, the rescue view could not be found
					// So we're back to the default error exibition
					Console.WriteLine(e);
				}
			}

			return false;
		}

		#endregion

		#region Lifecycle (overridables)

		protected virtual void Initialize()
		{
			
		}

		/// <summary>
		/// Invoked by the view engine to perform
		/// any logic before the view is sent to the client.
		/// </summary>
		/// <param name="view"></param>
		public virtual void PreSendView(object view)
		{
			if (view is IControllerAware)
			{
				(view as IControllerAware).SetController(this);
			}

			if (_context.UnderlyingContext != null)
			{
				((HttpContext) _context.UnderlyingContext).Items[ControllerContextKey] = this;
			}
		}

		/// <summary>
		/// Invoked by the view engine to perform
		/// any logic after the view had been sent to the client.
		/// </summary>
		/// <param name="view"></param>
		public virtual void PostSendView(object view)
		{
		}

		#endregion
	}
}
