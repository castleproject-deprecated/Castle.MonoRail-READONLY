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

namespace Castle.MicroKernel.Registration
{
	using System;
	
	/// <summary>
	/// Describes how to register a group of related types.
	/// </summary>
	public class BasedOnDescriptor : IRegistration
	{
		private readonly Type basedOn;
		private readonly FromDescriptor from;
		private readonly ServiceDescriptor service;
		private Action<ComponentRegistration> configurer;
		private Predicate<Type> unlessFilter;
		private Predicate<Type> ifFilter;
		
		/// <summary>
		/// Initializes a new instance of the BasedOnDescriptor.
		/// </summary>
		internal BasedOnDescriptor(Type basedOn, FromDescriptor from)
		{
			this.basedOn = basedOn;
			this.from = from;
			service = new ServiceDescriptor(this);
		}

		/// <summary>
		/// Gets the type all types must be based on.
		/// </summary>
		internal Type InternalBasedOn
		{
			get { return basedOn; }
		}

		/// <summary>
		/// Assigns a conditional predication which must be satisfied.
		/// </summary>
		/// <param name="ifFilter">The predicate to satisfy.</param>
		/// <returns></returns>
		public BasedOnDescriptor If(Predicate<Type> ifFilter)
		{
			this.ifFilter = ifFilter;
			return this;
		}

		/// <summary>
		/// Assigns a conditional predication which must not be satisfied. 
		/// </summary>
		/// <param name="unlessFilter">The predicate not to satisify.</param>
		/// <returns></returns>
		public BasedOnDescriptor Unless( Predicate<Type> unlessFilter )
		{
			this.unlessFilter = unlessFilter;
			return this;
		}
		
		/// <summary>
		/// Gets the service descriptor.
		/// </summary>
		public ServiceDescriptor WithService
		{
			get { return service; }
		}
		
		/// <summary>
		/// Allows customized configurations of each matching type.
		/// </summary>
		/// <param name="configurer">The configuration action.</param>
		/// <returns></returns>
		public BasedOnDescriptor Configure(Action<ComponentRegistration> configurer)
		{
			this.configurer = configurer;
			return this;
		}

		/// <summary>
		/// Allows a type to be registered multiple times.
		/// </summary>
		public FromDescriptor AllowMultipleMatches()
		{
			return from.AllowMultipleMatches();
		}

		/// <summary>
		/// Returns the descriptor for accepting a new type.
		/// </summary>
		/// <typeparam name="T">The base type.</typeparam>
		/// <returns>The descriptor for the type.</returns>
		public BasedOnDescriptor BasedOn<T>()
		{
			return from.BasedOn<T>();
		}

		/// <summary>
		/// Returns the descriptor for accepting a new type.
		/// </summary>
		/// <param name="basedOn">The base type.</param>
		/// <returns>The descriptor for the type.</returns>
		public BasedOnDescriptor BasedOn(Type basedOn)
		{
			return from.BasedOn(basedOn);
		}

		internal bool TryRegister(Type type, IKernel kernel)
		{
			Type baseType;

			if (Accepts(type, out baseType))
			{
				Type serviceType = service.GetService(type, baseType);
				ComponentRegistration registration = Component.For(serviceType);
				registration.ImplementedBy(type);

				if (configurer != null)
				{
					configurer(registration);
				}

				if (String.IsNullOrEmpty(registration.Name))
				{
					registration.Named(type.FullName);
				}

				if (!kernel.HasComponent(registration.Name))
				{
					kernel.Register(registration);
				}

				return true;
			}

			return false;
		}

		private bool Accepts(Type type, out Type baseType)
		{
			baseType = basedOn;
			return type.IsClass && !type.IsAbstract 
				&& IsBasedOn(type, ref baseType)
				&& (ifFilter == null || ifFilter(type))
				&& (unlessFilter == null || !unlessFilter(type)
				);
		}

		private bool IsBasedOn(Type type, ref Type baseType)
		{
			if (basedOn.IsAssignableFrom(type))
			{
				return true;
			}
			else if (basedOn.IsGenericTypeDefinition)
			{
				if (basedOn.IsInterface)
				{
					return IsBasedOnGenericInterface(type, ref baseType);
				}
				return IsBasedOnGenericClass(type, ref baseType);
			}
			return false;

		}

		private bool IsBasedOnGenericInterface(Type type, ref Type baseType)
		{
			foreach (Type @interface in type.GetInterfaces())
			{
				if (@interface.IsGenericType &&
					@interface.GetGenericTypeDefinition() == basedOn)
				{
					if (@interface.ReflectedType == null &&
						@interface.ContainsGenericParameters)
					{
						baseType = @interface.GetGenericTypeDefinition();
					}
					else
					{
						baseType = @interface;
					}
					return true;
				}
			}
			return false;
		}

		private bool IsBasedOnGenericClass(Type type, ref Type baseType)
		{
			while (type != null)
			{
				if (type.IsGenericType &&
					type.GetGenericTypeDefinition() == basedOn)
				{
					baseType = type;
					return true;
				}

				type = type.BaseType;
			}
			return false;
		}

		#region IRegistration Members

		void IRegistration.Register(IKernel kernel)
		{
			((IRegistration)from).Register(kernel);
		}

		#endregion
	}
}
