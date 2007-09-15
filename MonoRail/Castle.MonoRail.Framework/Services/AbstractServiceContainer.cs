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
	using System.Collections.Specialized;
	using System.ComponentModel.Design;

	/// <summary>
	/// Basic implementation of <see cref="IServiceContainer"/>
	/// </summary>
	public abstract class AbstractServiceContainer : MarshalByRefObject, IServiceContainer
	{
		private IServiceContainer parent;
		private IDictionary type2Service;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractServiceContainer"/> class.
		/// </summary>
		public AbstractServiceContainer()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractServiceContainer"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public AbstractServiceContainer(IServiceContainer parent)
		{
			this.parent = parent;
		}

		#region IServiceContainer

		/// <summary>
		/// Adds the specified service to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="serviceInstance">An instance of the service type to add. This object must implement or inherit from the type indicated by the serviceType parameter.</param>
		public void AddService(Type serviceType, object serviceInstance)
		{
			AddService(serviceType, serviceInstance, false);
		}

		/// <summary>
		/// Adds the specified service to the service container, and optionally promotes the service to any parent service containers.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="serviceInstance">An instance of the service type to add. This object must implement or inherit from the type indicated by the serviceType parameter.</param>
		/// <param name="promote">true to promote this request to any parent service containers; otherwise, false.</param>
		public void AddService(Type serviceType, object serviceInstance, bool promote)
		{
			if (promote)
			{
				IServiceContainer parentContainer = ParentContainer;

				if (parentContainer != null)
				{
					parentContainer.AddService(serviceType, serviceInstance, promote);
					return;
				}
			}

			if (type2Service == null)
			{
				type2Service = new HybridDictionary();
			}

			type2Service[serviceType] = serviceInstance;		
		}

		/// <summary>
		/// Adds the specified service to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="callback">A callback object that is used to create the service. This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
		public void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Adds the specified service to the service container, and optionally promotes the service to parent service containers.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="callback">A callback object that is used to create the service. This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
		/// <param name="promote">true to promote this request to any parent service containers; otherwise, false.</param>
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes the specified service type from the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to remove.</param>
		public void RemoveService(Type serviceType)
		{
			RemoveService(serviceType, false);
		}

		/// <summary>
		/// Removes the specified service type from the service container, and optionally promotes the service to parent service containers.
		/// </summary>
		/// <param name="serviceType">The type of service to remove.</param>
		/// <param name="promote">true to promote this request to any parent service containers; otherwise, false.</param>
		public void RemoveService(Type serviceType, bool promote)
		{
			if (promote)
			{
				IServiceContainer parentContainer = ParentContainer;

				if (parentContainer != null)
				{
					parentContainer.RemoveService(serviceType, promote);
					return;
				}
			}

			if (type2Service != null)
			{
				type2Service.Remove(serviceType);
			}
		}

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		/// <param name="serviceType">An object that specifies the type of service object to get.</param>
		/// <returns>
		/// A service object of type serviceType.-or- null if there is no service object of type serviceType.
		/// </returns>
		public object GetService(Type serviceType)
		{
			object service = null;

			if (serviceType == typeof(IServiceContainer))
			{
				return this;
			}
			
			if (type2Service != null)
			{
				service = type2Service[serviceType];
			}

			if (service == null && parent != null)
			{
				service = parent.GetService(serviceType);
			}

			return service;
		}

		#endregion

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetService<T>()
		{
			return (T) GetService(typeof(T));
		}

		/// <summary>
		/// Gets or sets the parent container.
		/// </summary>
		/// <value>The parent.</value>
		public IServiceContainer Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		/// <summary>
		/// Gets the parent container.
		/// </summary>
		/// <value>The parent container.</value>
		private IServiceContainer ParentContainer
		{
			get
			{
				IServiceContainer container = null;
				
				if (parent != null)
				{
					container = (IServiceContainer) parent.GetService(typeof(IServiceContainer));
				}
				
				return container;
			}
		}
	}
}
