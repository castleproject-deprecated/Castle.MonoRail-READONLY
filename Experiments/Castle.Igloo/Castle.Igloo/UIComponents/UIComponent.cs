#region Apache Notice
/*****************************************************************************
 * 
 * Castle.Igloo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Igloo.Navigation;
using Castle.MicroKernel;
using Castle.Igloo.Attributes;
using Castle.Igloo.Scopes;
using Castle.Igloo.Controllers;
using Castle.Igloo.Util;

namespace Castle.Igloo.UIComponents
{
    /// <summary>
    /// A UI component that keeps all infos to do injection. 
    /// </summary>
    public sealed class UIComponent
    {
        /// <summary>
        /// Binding token
        /// </summary>
        private BindingFlags BINDING_FLAGS_SET
            = BindingFlags.Public
            | BindingFlags.SetProperty
            | BindingFlags.Instance
            | BindingFlags.SetField
            ;
        
        public const string COMPONENT_SUFFIX = ".uicomponent";
        public const string VIEW_SUFFIX = "view.";
        public const string UICOMPONENT_TO_REFRESH = "_UICOMPONENT_TO_REFRESH_";

        private string _name = string.Empty;
        private IKernel _kernel = null;
        private Type _componentType = null;
        private IDictionary<InjectAttribute, PropertyInfo> _inMembers = new Dictionary<InjectAttribute, PropertyInfo>();
        private IDictionary<OutjectAttribute, PropertyInfo> _outMembers = new Dictionary<OutjectAttribute, PropertyInfo>();
        private IList<PropertyInfo> _inControllers = new List<PropertyInfo>();
        private PropertyInfo _inNavigationState = null;
        
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
        }


        /// <summary>
        /// Gets the in members (properties or fields).
        /// </summary>
        /// <value>The in properties.</value>
        public IDictionary<InjectAttribute, PropertyInfo> InMembers
        {
            get { return _inMembers; }
        }

        /// <summary>
        /// Gets the out (properties or fields).
        /// </summary>
        /// <value>The out properties.</value>
        public IDictionary<OutjectAttribute, PropertyInfo> OutMembers
        {
            get { return _outMembers; }
        }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        public Type ComponentType
        {
            get { return _componentType; }
        }

        /// <summary>
        /// Gets a value indicating whether [needs injection].
        /// </summary>
        /// <value><c>true</c> if [needs injection]; otherwise, <c>false</c>.</value>
        public bool NeedsInjection
        {
            get { return _inMembers.Count > 0; }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UIComponent"/> class.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <param name="kernel">The kernel</param>
        public UIComponent(Type componentType, IKernel kernel)
        {
            AssertUtils.ArgumentNotNull(componentType, "componentType");
            AssertUtils.ArgumentNotNull(kernel, "kernel");

            _kernel = kernel; 
            _name = componentType.FullName + COMPONENT_SUFFIX; ;
            _componentType = componentType;
            
            InitMembers();
        }

        
        /// <summary>
        /// Inject context variable values into [Inject] attributes
        /// of a component instance.
        /// </summary>
        /// <param name="instance">A UI component instance.</param>
        public void Inject(Object instance)
        {
            InjectMembers(instance, true);
            InjectIOCComponnet(instance);
        }


        /// <summary>
        /// Outject context variable values from [Outject] attributes
        /// of a component instance.
        /// </summary>
        /// <param name="instance">A NTie component instance.</param>
        public void Outject(Object instance)
        {
            OutjectMembers(instance);
            // TO DO Outject field
        }


        /// <summary>
        /// Injects the members.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="trackInjectedMembers">
        /// if set to <c>true</c> tracks injected members so they can be refresh them on back call from controller method.
        /// </param>
        /// <remarks>TO DO Inject fields</remarks>
        public void InjectMembers(Object instance, bool trackInjectedMembers)
        {
            if (NeedsInjection)
            {
                TraceUtil.Log("Injecting dependencies of : " + _name);

                IScopeRegistry scopeRegistry = _kernel[typeof(IScopeRegistry)] as IScopeRegistry;

                if (trackInjectedMembers)
                {
                    // Tracks the injected members so bijection interceptor
                    // can refresh them on back call from controller method.
                   //UIComponent, object
                     IScope requestScope = scopeRegistry[ScopeType.Request];
                     IDictionary<UIComponent, object> uiComponentToRefresh = (IDictionary<UIComponent, object>)requestScope[UIComponent.UICOMPONENT_TO_REFRESH];
                     if (uiComponentToRefresh==null)
                     {
                         uiComponentToRefresh = new Dictionary<UIComponent, object>();
                         requestScope[UICOMPONENT_TO_REFRESH] = uiComponentToRefresh;
                     }
                     uiComponentToRefresh.Add(this, instance);
                }

                foreach (KeyValuePair<InjectAttribute, PropertyInfo> kvp in InMembers)
                {                   
                    PropertyInfo propertyInfo = kvp.Value;
                    object instanceToInject = scopeRegistry.GetFromScopes(kvp.Key);

                    if (instanceToInject == null)
                    {
                        if (kvp.Key.Create)
                        {
                            instanceToInject = Activator.CreateInstance(propertyInfo.PropertyType);

                            IScope scope = scopeRegistry[kvp.Key.Scope];
                            scope[kvp.Key.Name] = instanceToInject;
                        }
                        else
                        {
                            // do log / use  kvp.Key.Required
                        }
                    }
                    propertyInfo.SetValue(instance, instanceToInject, null);
                }
            }
        }

        private void InjectIOCComponnet(Object instance)
        {
            // Inject Navigation State
            object navigationState = _kernel[_inNavigationState.PropertyType];
            _inNavigationState.SetValue(instance, navigationState, null);

            // Inject controllers
            foreach(PropertyInfo propertyInfo in _inControllers)
            {
                object controller = _kernel[propertyInfo.PropertyType];
                propertyInfo.SetValue(instance, controller, null);
            }       
        }
        

        private void OutjectMembers(Object instance)
        {
            // TO DO
        }
        
        private void InitMembers()
        {
            if (AttributeUtil.HasScopeAttribute(_componentType))
            {
                _name = VIEW_SUFFIX + _name;
                RetrieveInjectedProperties();
            }
        }
        
        /// <summary>
        /// Retrieves the injected user context scoped object and
        /// injected IController components on a view component
        /// </summary>
        /// <remarks>
        /// Today, only check injected properties memnbers
        /// TO DO also check injected fields 
        /// </remarks>
        private void RetrieveInjectedProperties()
        {
            PropertyInfo[] properties = _componentType.GetProperties(BINDING_FLAGS_SET);
            
            for (int i = 0; i < properties.Length; i++)
            {
                if ( !typeof(IController).IsAssignableFrom(properties[i].PropertyType))
                {
                    InjectAttribute injectAttribute = AttributeUtil.GetInjectAttribute(properties[i]);
                    if (injectAttribute != null)
                    {
                        if (injectAttribute.Name.Length == 0)
                        {
                            injectAttribute.Name = properties[i].Name;
                        }
                        _inMembers.Add(injectAttribute, properties[i]);
                    }
                }
            }
            
            // Retrieves IOC setter injection
            for (int i = 0; i < properties.Length; i++)
            {
                if ( (typeof(IController).IsAssignableFrom(properties[i].PropertyType)) )
                {
                    _inControllers.Add(properties[i]);
                }
                if ((typeof(NavigationState).IsAssignableFrom(properties[i].PropertyType)))
                {
                    _inNavigationState = properties[i];
                }  
            }
        }
    }
}
