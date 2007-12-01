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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Threading;

	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Constructs and caches all collected information
	/// about a <see cref="Controller"/> and its actions.
	/// <seealso cref="ControllerMetaDescriptor"/>
	/// </summary>
	public class DefaultControllerDescriptorProvider : IControllerDescriptorProvider
	{
		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;

		/// <summary>
		/// Used to lock the cache
		/// </summary>
		private ReaderWriterLock locker = new ReaderWriterLock();
		private Hashtable descriptorRepository = new Hashtable();
		private IHelperDescriptorProvider helperDescriptorProvider;
		private IFilterDescriptorProvider filterDescriptorProvider;
		private ILayoutDescriptorProvider layoutDescriptorProvider;
		private IRescueDescriptorProvider rescueDescriptorProvider;
		private IResourceDescriptorProvider resourceDescriptorProvider;
		private ITransformFilterDescriptorProvider transformFilterDescriptorProvider;

		#region IServiceEnabledComponent implementation

		/// <summary>
		/// Services the specified service provider.
		/// </summary>
		/// <param name="serviceProvider">The service provider.</param>
		public void Service(IServiceProvider serviceProvider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory) serviceProvider.GetService(typeof(ILoggerFactory));
			
			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(DefaultControllerDescriptorProvider));
			}

			helperDescriptorProvider = (IHelperDescriptorProvider) 
				serviceProvider.GetService(typeof(IHelperDescriptorProvider));

			filterDescriptorProvider = (IFilterDescriptorProvider)
				serviceProvider.GetService(typeof(IFilterDescriptorProvider));

			layoutDescriptorProvider = (ILayoutDescriptorProvider)
				serviceProvider.GetService(typeof(ILayoutDescriptorProvider));

			rescueDescriptorProvider = (IRescueDescriptorProvider)
				serviceProvider.GetService(typeof(IRescueDescriptorProvider));
		
			resourceDescriptorProvider = (IResourceDescriptorProvider)
				serviceProvider.GetService(typeof(IResourceDescriptorProvider));
			
			transformFilterDescriptorProvider = (ITransformFilterDescriptorProvider)
				serviceProvider.GetService(typeof(ITransformFilterDescriptorProvider));
		}

		#endregion

		/// <summary>
		/// Constructs and populates a <see cref="ControllerMetaDescriptor"/>.
		/// </summary>
		/// <remarks>
		/// This implementation is also responsible for caching 
		/// constructed meta descriptors.
		/// </remarks>
		public ControllerMetaDescriptor BuildDescriptor(Controller controller)
		{
			Type controllerType = controller.GetType();
			
			return BuildDescriptor(controllerType);
		}

		/// <summary>
		/// Constructs and populates a <see cref="ControllerMetaDescriptor"/>.
		/// </summary>
		/// <remarks>
		/// This implementation is also responsible for caching 
		/// constructed meta descriptors.
		/// </remarks>
		public ControllerMetaDescriptor BuildDescriptor(Type controllerType)
		{
			ControllerMetaDescriptor desc;

			locker.AcquireReaderLock(-1);

			desc = (ControllerMetaDescriptor) descriptorRepository[controllerType];

			if (desc != null)
			{
				locker.ReleaseReaderLock();
				return desc;
			}

			try
			{
				locker.UpgradeToWriterLock(-1);

				// We need to recheck after getting the writer lock
				desc = (ControllerMetaDescriptor) descriptorRepository[controllerType];

				if (desc != null)
				{
					return desc;
				}
				
				desc = InternalBuildDescriptor(controllerType);

				descriptorRepository[controllerType] = desc;
			}
			finally
			{
				locker.ReleaseWriterLock();
			}

			return desc;
		}

		/// <summary>
		/// Builds the <see cref="ControllerMetaDescriptor"/> for the specified controller type
		/// </summary>
		/// <param name="controllerType">Type of the controller.</param>
		/// <returns></returns>
		private ControllerMetaDescriptor InternalBuildDescriptor(Type controllerType)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Building controller descriptor for {0}", controllerType);
			}

			ControllerMetaDescriptor descriptor = new ControllerMetaDescriptor();

			CollectClassLevelAttributes(controllerType, descriptor);
			
			CollectActions(controllerType, descriptor);

			CollectActionLevelAttributes(descriptor);
			
			return descriptor;
		}

		#region Action data

		/// <summary>
		/// Collects the actions.
		/// </summary>
		/// <param name="controllerType">Type of the controller.</param>
		/// <param name="desc">The desc.</param>
		private void CollectActions(Type controllerType, ControllerMetaDescriptor desc)
		{
			// HACK: GetRealControllerType is a workaround for DYNPROXY-14 bug
			// see: http://support.castleproject.org/browse/DYNPROXY-14
			controllerType = GetRealControllerType(controllerType);

			MethodInfo[] methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

			foreach(MethodInfo method in methods)
			{
				Type declaringType = method.DeclaringType;

				if (declaringType == typeof(Object) || 
					declaringType == typeof(Controller) || 
					declaringType == typeof(SmartDispatcherController))
				{
					continue;
				}

				if (desc.Actions.Contains(method.Name))
				{
					ArrayList list = desc.Actions[method.Name] as ArrayList;

					if (list == null)
					{
						list = new ArrayList();
						list.Add(desc.Actions[method.Name]);

						desc.Actions[method.Name] = list;
					}

					list.Add(method);
				}
				else
				{
					desc.Actions[method.Name] = method;
				}
			}
		}

		/// <summary>
		/// Collects the action level attributes.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		private void CollectActionLevelAttributes(ControllerMetaDescriptor descriptor)
		{
			foreach(object action in descriptor.Actions.Values)
			{
				if (action is IList)
				{
					foreach(MethodInfo overloadedAction in (action as IList))
					{
						CollectActionAttributes(overloadedAction, descriptor);
					}

					continue;
				}

				CollectActionAttributes(action as MethodInfo, descriptor);
			}
		}

		/// <summary>
		/// Collects the action attributes.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="descriptor">The descriptor.</param>
		private void CollectActionAttributes(MethodInfo method, ControllerMetaDescriptor descriptor)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Collection attributes for action {0}", method.Name);
			}

			ActionMetaDescriptor actionDescriptor = descriptor.GetAction(method);

			CollectResources(actionDescriptor, method);
			CollectSkipFilter(actionDescriptor, method);
			CollectRescues(actionDescriptor, method);
			CollectAccessibleThrough(actionDescriptor, method);
			CollectSkipRescue(actionDescriptor, method);
			CollectLayout(actionDescriptor, method);
			CollectCacheConfigures(actionDescriptor, method);
			CollectTransformFilter(actionDescriptor, method);
			
			if (method.IsDefined(typeof(AjaxActionAttribute), true))
			{
				descriptor.AjaxActions.Add(method);
			}

			if (method.IsDefined(typeof(DefaultActionAttribute), true))
			{
				if (descriptor.DefaultAction != null)
				{
					throw new MonoRailException("Cannot resolve a default action for {0}, DefaultActionAttribute was declared more than once.", method.DeclaringType.FullName);
				}
				descriptor.DefaultAction = new DefaultActionAttribute(method.Name);
			}
		}

		/// <summary>
		/// Collects the skip rescue.
		/// </summary>
		/// <param name="actionDescriptor">The action descriptor.</param>
		/// <param name="method">The method.</param>
		private void CollectSkipRescue(ActionMetaDescriptor actionDescriptor, MethodInfo method)
		{
			object[] attributes = method.GetCustomAttributes(typeof(SkipRescueAttribute), true);
			
			if (attributes.Length != 0)
			{
				actionDescriptor.SkipRescue = (SkipRescueAttribute) attributes[0];
			}
		}

		/// <summary>
		/// Collects the accessible through.
		/// </summary>
		/// <param name="actionDescriptor">The action descriptor.</param>
		/// <param name="method">The method.</param>
		private void CollectAccessibleThrough(ActionMetaDescriptor actionDescriptor, MethodInfo method)
		{
			object[] attributes = method.GetCustomAttributes(typeof(AccessibleThroughAttribute), true);
			
			if (attributes.Length != 0)
			{
				actionDescriptor.AccessibleThrough = (AccessibleThroughAttribute) attributes[0];
			}
		}

		/// <summary>
		/// Collects the skip filter.
		/// </summary>
		/// <param name="actionDescriptor">The action descriptor.</param>
		/// <param name="method">The method.</param>
		private void CollectSkipFilter(ActionMetaDescriptor actionDescriptor, MethodInfo method)
		{
			object[] attributes = method.GetCustomAttributes(typeof(SkipFilterAttribute), true);
			
			foreach(SkipFilterAttribute attr in attributes)
			{
				actionDescriptor.SkipFilters.Add(attr);
			}
		}

		/// <summary>
		/// Collects the resources.
		/// </summary>
		/// <param name="desc">The desc.</param>
		/// <param name="memberInfo">The member info.</param>
		private void CollectResources(BaseMetaDescriptor desc, MemberInfo memberInfo)
		{
			desc.Resources = resourceDescriptorProvider.CollectResources(memberInfo);
		}

		/// <summary>
		/// Collects the transform filter.
		/// </summary>
		/// <param name="actionDescriptor">The action descriptor.</param>
		/// <param name="method">The method.</param>
		private void CollectTransformFilter(ActionMetaDescriptor actionDescriptor, MethodInfo method)
		{
			actionDescriptor.TransformFilters = transformFilterDescriptorProvider.CollectFilters((method));
			Array.Sort(actionDescriptor.TransformFilters, TransformFilterDescriptorComparer.Instance);
		}
		
		/// <summary>
		/// Gets the real controller type, instead of the proxy type.
		/// </summary>
		/// <remarks>
		/// Workaround for DYNPROXY-14 bug. See: http://support.castleproject.org/browse/DYNPROXY-14
		/// </remarks>
		private Type GetRealControllerType(Type controllerType)
		{
			Type prev = controllerType;

			// try to get the first non-proxy type
			while(controllerType.Assembly.FullName.StartsWith("DynamicProxyGenAssembly2") || 
				  controllerType.Assembly.FullName.StartsWith("DynamicAssemblyProxyGen"))
			{
				controllerType = controllerType.BaseType;

				if (controllerType == typeof(SmartDispatcherController) || 
				    controllerType == typeof(Controller))
				{
					// oops, it's a pure-proxy controller. just let it go.
					controllerType = prev;
					break;
				}
			}

			return controllerType;
		}

		#endregion

		#region Controller data

		/// <summary>
		/// Collects the class level attributes.
		/// </summary>
		/// <param name="controllerType">Type of the controller.</param>
		/// <param name="descriptor">The descriptor.</param>
		private void CollectClassLevelAttributes(Type controllerType, ControllerMetaDescriptor descriptor)
		{
			CollectHelpers(descriptor, controllerType);
			CollectResources(descriptor, controllerType);
			CollectFilters(descriptor, controllerType);
			CollectLayout(descriptor, controllerType);
			CollectRescues(descriptor, controllerType);
			CollectDefaultAction(descriptor, controllerType);
			CollectScaffolding(descriptor, controllerType);
			CollectDynamicAction(descriptor, controllerType);
		}

		/// <summary>
		/// Collects the default action.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="controllerType">Type of the controller.</param>
		private void CollectDefaultAction(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			object[] attributes = controllerType.GetCustomAttributes(typeof(DefaultActionAttribute), true);

			if (attributes.Length != 0)
			{
				descriptor.DefaultAction = (DefaultActionAttribute) attributes[0];
			}
		}

		/// <summary>
		/// Collects the scaffolding.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="controllerType">Type of the controller.</param>
		private void CollectScaffolding(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			object[] attributes = controllerType.GetCustomAttributes(typeof(ScaffoldingAttribute), false);

			if (attributes.Length != 0)
			{
				foreach(ScaffoldingAttribute scaffolding in attributes)
				{
					descriptor.Scaffoldings.Add(scaffolding);
				}
			}
		}

		/// <summary>
		/// Collects the dynamic action.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="controllerType">Type of the controller.</param>
		private void CollectDynamicAction(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			object[] attributes = controllerType.GetCustomAttributes(typeof(DynamicActionProviderAttribute), true);

			if (attributes.Length != 0)
			{
				foreach(DynamicActionProviderAttribute attr in attributes)
				{
					descriptor.ActionProviders.Add(attr.ProviderType);
				}
			}
		}

		/// <summary>
		/// Collects the helpers.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="controllerType">Type of the controller.</param>
		private void CollectHelpers(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			descriptor.Helpers = helperDescriptorProvider.CollectHelpers(controllerType);
		}

		/// <summary>
		/// Collects the filters.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="controllerType">Type of the controller.</param>
		private void CollectFilters(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			descriptor.Filters = filterDescriptorProvider.CollectFilters(controllerType);

			Array.Sort(descriptor.Filters, FilterDescriptorComparer.Instance);
		}

		/// <summary>
		/// Collects the layout.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="memberInfo">The member info.</param>
		private void CollectLayout(BaseMetaDescriptor descriptor, MemberInfo memberInfo)
		{
			descriptor.Layout = layoutDescriptorProvider.CollectLayout(memberInfo);
		}

		/// <summary>
		/// Collects the rescues.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="memberInfo">The member info.</param>
		private void CollectRescues(BaseMetaDescriptor descriptor, MethodInfo memberInfo)
		{
			descriptor.Rescues = rescueDescriptorProvider.CollectRescues(memberInfo);
		}

		/// <summary>
		/// Collects the rescues.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="type">The type.</param>
		private void CollectRescues(BaseMetaDescriptor descriptor, Type type)
		{
			descriptor.Rescues = rescueDescriptorProvider.CollectRescues(type);
		}

		/// <summary>
		/// Collects the cache configures.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="memberInfo">The member info.</param>
		private void CollectCacheConfigures(ActionMetaDescriptor descriptor, MemberInfo memberInfo)
		{
			object[] configurers = memberInfo.GetCustomAttributes(typeof(ICachePolicyConfigurer), true);

			if (configurers.Length != 0)
			{
				foreach(ICachePolicyConfigurer cacheConfigurer in configurers)
				{
					descriptor.CacheConfigurers.Add(cacheConfigurer);
				}
			}
		}

		#endregion

		/// <summary>
		/// This <see cref="IComparer"/> implementation
		/// is used to sort the filters based on their Execution Order.
		/// </summary>
		class FilterDescriptorComparer : IComparer
		{
			private static readonly FilterDescriptorComparer instance = new FilterDescriptorComparer();

			/// <summary>
			/// Initializes a new instance of the <see cref="FilterDescriptorComparer"/> class.
			/// </summary>
			private FilterDescriptorComparer()
			{
			}

			/// <summary>
			/// Gets the instance.
			/// </summary>
			/// <value>The instance.</value>
			public static FilterDescriptorComparer Instance
			{
				get { return instance; }
			}

			/// <summary>
			/// Compares the specified left.
			/// </summary>
			/// <param name="left">The left.</param>
			/// <param name="right">The right.</param>
			/// <returns></returns>
			public int Compare(object left, object right)
			{
				return ((FilterDescriptor) left).ExecutionOrder - ((FilterDescriptor) right).ExecutionOrder;
			}
		}

		/// <summary>
		/// This <see cref="IComparer"/> implementation
		/// is used to sort the transformfilters based on their Execution Order.
		/// </summary>
		class TransformFilterDescriptorComparer : IComparer
		{
			private static readonly TransformFilterDescriptorComparer instance = new TransformFilterDescriptorComparer();

			/// <summary>
			/// Initializes a new instance of the <see cref="TransformFilterDescriptorComparer"/> class.
			/// </summary>
			private TransformFilterDescriptorComparer()
			{
			}

			/// <summary>
			/// Gets the instance.
			/// </summary>
			/// <value>The instance.</value>
			public static TransformFilterDescriptorComparer Instance
			{
				get { return instance; }
			}

			/// <summary>
			/// Compares the specified left.
			/// </summary>
			/// <param name="left">The left.</param>
			/// <param name="right">The right.</param>
			/// <returns></returns>
			public int Compare(object left, object right)
			{
				return ((TransformFilterDescriptor)right).ExecutionOrder - ((TransformFilterDescriptor)left).ExecutionOrder;
			}
		}
	}
}
