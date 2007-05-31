// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//		 http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


namespace Castle.DynamicProxy.Serialization
{
	using System;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Generators;

	/// <summary>
	/// Handles the deserialization of proxies.
	/// See here for more details:
	/// http://groups.google.com/group/castle-project-devel/msg/fb5ef9656d050ba5
	/// </summary>
	[Serializable]
	public class ProxyObjectReference : IObjectReference, ISerializable, IDeserializationCallback
	{
		private static ModuleScope _scope = new ModuleScope();

		private Type _baseType;
		private Type[] _interfaces;
		private IInterceptor[] _interceptors;
		private ProxySerializer.Indirection _data;
		private object _proxy;
		private ProxyGenerationOptions _options;

		/// <summary>
		/// Usefull for test cases
		/// </summary>
		public static void ResetScope()
		{
			_scope = new ModuleScope();
		}

		protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
		{
			_interceptors = (IInterceptor[]) info.GetValue("__interceptors", typeof(IInterceptor[]));
			_baseType = (Type) info.GetValue("__baseType", typeof(Type));
			_options = (ProxyGenerationOptions) info.GetValue("__generationOptions", typeof(ProxyGenerationOptions));

			String[] _interfaceNames = (String[]) info.GetValue("__interfaces", typeof(String[]));

			_interfaces = new Type[_interfaceNames.Length];

			for(int i = 0; i < _interfaceNames.Length; i++)
			{
				_interfaces[i] = Type.GetType(_interfaceNames[i]);
			}

			_proxy = RecreateProxy(info, context);
		}

		protected virtual object RecreateProxy(SerializationInfo info, StreamingContext context)
		{
			if (_baseType == typeof(object))
			{
				return RecreateInterfaceProxy(info, context);
			}
			else
			{
				return RecreateClassProxy(info, context);
			}
		}

		public object RecreateInterfaceProxy(SerializationInfo info, StreamingContext context)
		{
			object proxy = null;

			object target = info.GetValue("__target", typeof(object));
			InterfaceGeneratorType generatorType = (InterfaceGeneratorType) info.GetInt32("__interface_generator_type");
			string interfaceName = info.GetString("__theInterface");
			Type theInterface = Type.GetType(interfaceName, true, false);
			InterfaceProxyWithTargetGenerator generator;
			switch(generatorType)
			{
				case InterfaceGeneratorType.WithTarget:
					generator = new InterfaceProxyWithTargetGenerator(_scope, theInterface);
					break;
				case InterfaceGeneratorType.WithoutTarget:
					generator = new InterfaceProxyGeneratorWithoutTarget(_scope, theInterface);
					break;
				case InterfaceGeneratorType.WithTargetInterface:
					generator = new InterfaceProxyWithTargetInterfaceGenerator(_scope, theInterface);
					break;
				default:
					throw new InvalidOperationException(
						string.Format(
							"Got value {0} for the interface generator type, which is not known for the purpose of serialization.",
							generatorType));
			}

			Type proxy_type = generator.GenerateCode(target.GetType(), _interfaces, _options);

			proxy = Activator.CreateInstance(proxy_type, new object[] {_interceptors, target});

			return proxy;
		}

		public object RecreateClassProxy(SerializationInfo info, StreamingContext context)
		{
			bool delegateBaseSer = info.GetBoolean("__delegateToBase");

			object proxy = null;

			ClassProxyGenerator cpGen = new ClassProxyGenerator(_scope, _baseType);

			Type proxy_type = cpGen.GenerateCode(_interfaces, _options);


			if (delegateBaseSer)
			{
				proxy = Activator.CreateInstance(proxy_type, new object[] {info, context});
			}
			else
			{
				proxy = FormatterServices.GetSafeUninitializedObject(proxy_type);
				_data = (ProxySerializer.Indirection) info.GetValue("__data", typeof(ProxySerializer.Indirection));

				SetInterceptor(proxy, proxy_type);
			}

			return proxy;
		}

		private void SetInterceptor(object proxy, Type proxy_type)
		{
			FieldInfo interceptorField = proxy_type.GetField("__interceptors");

			if (interceptorField == null)
			{
				throw new SerializationException(
					"The SerializationInfo specifies an invalid proxy type, which has no __interceptors field.");
			}

			interceptorField.SetValue(proxy, _interceptors);
		}

		protected void InvokeCallback(object target)
		{
			if (target is IDeserializationCallback)
			{
				(target as IDeserializationCallback).OnDeserialization(this);
			}
		}

		public object GetRealObject(StreamingContext context)
		{
			return _proxy;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// There is no need to implement this method as 
			// this class would never be serialized.
		}

		// Class proxies must be populated in this method, since only at this point all members held by _data.IndirectedObject
		// have been fixed up.
		public void OnDeserialization(object sender)
		{
			if (_data != null)
			{
				object[] objectData = (object[]) _data.IndirectedObject;

				MemberInfo[] members = FormatterServices.GetSerializableMembers(_baseType);
				FormatterServices.PopulateObjectMembers(_proxy, members, objectData);
				_data = null;
			}
			InvokeCallback(_proxy);
		}
	}
}