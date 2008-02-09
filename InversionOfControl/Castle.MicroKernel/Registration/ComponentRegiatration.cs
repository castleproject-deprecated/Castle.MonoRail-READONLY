// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Registration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Castle.Core;
	using Castle.Core.Configuration;
	using MicroKernel;

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="S">The service type</typeparam>
	public class ComponentRegistration<S> : IComponentRegistration
	{
		private String name;
		private bool overwrite;
		private readonly Type serviceType;
		private Type classType;
		private readonly List<ComponentDescriptor<S>> descriptors;
		private ComponentModel componentModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentRegistration{S}"/> class.
		/// </summary>
		public ComponentRegistration()
			: this(typeof(S))
		{
		}

		protected ComponentRegistration(Type serviceType)
		{
			overwrite = false;
			this.serviceType = serviceType;
			descriptors = new List<ComponentDescriptor<S>>();
		}

		internal bool Overwrite
		{
			get { return overwrite; }	
		}

		/// <summary>
		/// With the overwrite.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<S> OverWrite()
		{
			overwrite = true;
			return this;
		}

		/// <summary>
		/// With the name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Named(String name)
		{
			if (this.name != null)
			{
				String message = String.Format("This component has " +
					"already been assigned name '{0}'", this.name);

				throw new ComponentRegistrationException(message);					
			}

			this.name = name;
			return this;
		}

		public ComponentRegistration<S> ImplementedBy<C>()
		{
			return ImplementedBy(typeof(C));
		}

		public ComponentRegistration<S> ImplementedBy(Type type)
		{
			if (classType != null)
			{
				String message = String.Format("This component has " +
					"already been assigned implementation {0}", classType.FullName);
				throw new ComponentRegistrationException(message);					
			}

			classType = type;
			return this;
		}

		/// <summary>
		/// With the instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public ComponentRegistration<S> Instance(S instance)
		{
			return AddDescriptor(new ComponentInstanceDescriptior<S>(instance));
		}

		/// <summary>
		/// Gets the proxy.
		/// </summary>
		/// <value>The proxy.</value>
		public Proxy.ProxyGroup<S> Proxy
		{
			get { return new Proxy.ProxyGroup<S>(this); }
		}

		/// <summary>
		/// Gets the with lifestyle.
		/// </summary>
		/// <value>The with lifestyle.</value>
		public Lifestyle.LifestyleGroup<S> LifeStyle
		{
			get { return new Lifestyle.LifestyleGroup<S>(this); }
		}

		/// <summary>
		/// With the activator.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<S> Activator<A>() where A : IComponentActivator
		{
			return AddAttributeDescriptor("componentActivatorType", typeof(A).AssemblyQualifiedName);
		}

		/// <summary>
		/// With the extended properties.
		/// </summary>
		/// <param name="properties">The properties.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ExtendedProperties(params Property[] properties)
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor<S>(properties));
		}

		/// <summary>
		/// With the extended properties.
		/// </summary>
		/// <param name="anonymous">The properties.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ExtendedProperties(object anonymous)
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor<S>(anonymous));
		}

		/// <summary>
		/// With the custom dependencies.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<S> CustomDependencies(params Property[] dependencies)
		{
			return AddDescriptor(new CustomDependencyDescriptor<S>(dependencies));	
		}

		/// <summary>
		/// With the custom dependencies.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<S> CustomDependencies(IDictionary dependencies)
		{
			return AddDescriptor(new CustomDependencyDescriptor<S>(dependencies));	
		}

		/// <summary>
		/// With the custom dependencies.
		/// </summary>
		/// <param name="anonymous">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<S> CustomDependencies(object anonymous)
		{
			return AddDescriptor(new CustomDependencyDescriptor<S>(anonymous));
		}

		/// <summary>
		/// With the service overrides.
		/// </summary>
		/// <param name="overrides">The overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ServiceOverrides(params ServiceOverride[] overrides)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<S>(overrides));
		}

		/// <summary>
		/// With the service overrides.
		/// </summary>
		/// <param name="overrides">The overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ServiceOverrides(IDictionary overrides)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<S>(overrides));
		}

		/// <summary>
		/// With the service overrides.
		/// </summary>
		/// <param name="anonymous">The overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<S> ServiceOverrides(object anonymous)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<S>(anonymous));
		}

		/// <summary>
		/// With the interceptors.
		/// </summary>
		/// <param name="interceptors">The interceptors.</param>
		/// <returns></returns>
		public Interceptor.InterceptorGroup<S> Interceptors(
				params InterceptorReference[] interceptors)
		{
			return new Interceptor.InterceptorGroup<S>(this, interceptors);
		}

		/// <summary>
		/// Ases the startable.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<S> Startable()
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor<S>(
			                     	Property.ForKey("startable").Eq(true)));
		}

		/// <summary>
		/// Registers this component with the <see cref="IKernel"/>.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		void IComponentRegistration.Register(IKernel kernel)
		{
			if (componentModel == null)
			{
				InitializeDefaults();
				componentModel = BuildComponentModel(kernel);
				kernel.AddCustomComponent(componentModel);
			}
		}

		/// <summary>
		/// Builds the component model.
		/// </summary>
		/// <returns></returns>
		private ComponentModel BuildComponentModel(IKernel kernel)
		{
			IConfiguration configuration = EnsureComponentConfiguration(kernel);
			foreach(ComponentDescriptor<S> descriptor in descriptors)
			{
				descriptor.ApplyToConfiguration(kernel, configuration);
			}

			ComponentModel model = kernel.ComponentModelBuilder.BuildModel(
				name, serviceType, classType, null);
			foreach(ComponentDescriptor<S> descriptor in descriptors)
			{
				descriptor.ApplyToModel(kernel, model);
			}

			return model;
		}

		/// <summary>
		/// Adds the attribute descriptor.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public ComponentRegistration<S> AddAttributeDescriptor(string name, string value)
		{
			AddDescriptor(new AttributeDescriptor<S>(name, value));
			return this;
		}

		/// <summary>
		/// Adds the descriptor.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public ComponentRegistration<S> AddDescriptor(ComponentDescriptor<S> descriptor)
		{
			descriptor.Registration = this;
			descriptors.Add(descriptor);
			return this;
		}

		internal void AddParameter(IKernel kernel, ComponentModel model, String name, String value)
		{
			IConfiguration configuration = EnsureComponentConfiguration(kernel);
			IConfiguration parameters = configuration.Children["parameters"];
			if (parameters == null)
			{
				parameters = new MutableConfiguration("component");
				configuration.Children.Add(parameters);
			}

			MutableConfiguration reference = new MutableConfiguration(name, value);
			parameters.Children.Add(reference);
			model.Parameters.Add(name, value);
		}

		private void InitializeDefaults()
		{
			if (classType == null)
			{
				classType = serviceType;	
			}

			if (String.IsNullOrEmpty(name))
			{
				name = classType.FullName;
			}
		}

		private IConfiguration EnsureComponentConfiguration(IKernel kernel)
		{
			IConfiguration configuration = kernel.ConfigurationStore.GetComponentConfiguration(name);
			if (configuration == null)
			{
				configuration = new MutableConfiguration("component");
				kernel.ConfigurationStore.AddComponentConfiguration(name, configuration);
			}
			return configuration;
		}
	}

	public class ComponentRegistration : ComponentRegistration<object>
	{
		public ComponentRegistration(Type serviceType)
			: base( serviceType )
		{
		}
	}
}
