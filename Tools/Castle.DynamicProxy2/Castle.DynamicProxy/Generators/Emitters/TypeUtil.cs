// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;

	public abstract class TypeUtil
	{
		/// <summary>
		/// Returns list of all unique interfaces implemented given types, including their base interfaces.
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		public static ICollection<Type> GetAllInterfaces(params Type[] types)
		{
			if (types == null)
			{
				return Type.EmptyTypes;
			}

			var dummy = new object();
			// we should move this to HashSet once we no longer support .NET 2.0
			IDictionary<Type, object> interfaces = new Dictionary<Type, object>();
			foreach (var type in types)
			{
				if (type.IsInterface)
				{
					interfaces[type] = dummy;
				}
				foreach (var @interface in type.GetInterfaces())
				{
					interfaces[@interface] = dummy;
				}
			}
			return interfaces.Keys;
		}

		public static Type GetClosedParameterType(AbstractTypeEmitter type, Type parameter)
		{
			if (parameter.IsGenericTypeDefinition)
			{
				return parameter.GetGenericTypeDefinition().MakeGenericType(type.GetGenericArgumentsFor(parameter));
			}

			if (parameter.IsGenericType)
			{
				Type[] arguments = parameter.GetGenericArguments();
				if (CloseGenericParametersIfAny(type, arguments))
				{
					return parameter.GetGenericTypeDefinition().MakeGenericType(arguments);
				}
			}
			
			if (parameter.IsGenericParameter)
			{
				return type.GetGenericArgument(parameter.Name);
			}
			if (parameter.IsArray)
			{
				var elementType = GetClosedParameterType(type, parameter.GetElementType());
				return elementType.MakeArrayType();
			}
			
			return parameter;
		}

		public static MethodInfo FindImplementingMethod(MethodInfo interfaceMethod, Type implementingType)
		{
			Type interfaceType = interfaceMethod.DeclaringType;
			Debug.Assert(interfaceType.IsAssignableFrom(implementingType),
						 "interfaceMethod.DeclaringType.IsAssignableFrom(implementingType)");
			Debug.Assert(interfaceType.IsInterface, "interfaceType.IsInterface");
			InterfaceMapping map = implementingType.GetInterfaceMap(interfaceType);
			int index = Array.IndexOf(map.InterfaceMethods, interfaceMethod);
			if (index == -1)
			{
				// can this ever happen?
				return null;
			}
			return map.TargetMethods[index];
		}

		private static bool CloseGenericParametersIfAny(AbstractTypeEmitter emitter, Type[] arguments)
		{
			bool hasAnyGenericParameters = false;
			for (int i = 0; i < arguments.Length; i++)
			{
				var newType = GetClosedParameterType(emitter, arguments[i]);
				if (!ReferenceEquals(newType, arguments[i]))
				{
					arguments[i] = newType;
					hasAnyGenericParameters = true;
				}
			}
			return hasAnyGenericParameters;
		}
	}
}
