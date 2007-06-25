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

namespace Castle.DynamicProxy
{
	using System;
	using System.Collections;
	using System.Runtime.Serialization;

	[Serializable]
	public class ProxyGenerationOptions : ISerializable
	{
		public static readonly ProxyGenerationOptions Default = new ProxyGenerationOptions();

		private IProxyGenerationHook hook;
		private IInterceptorSelector selector;
		private ArrayList mixins;
		private Type baseTypeForInterfaceProxy = typeof(object);
		private bool useSingleInterfaceProxy;
		private bool useSelector;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyGenerationOptions"/> class.
		/// </summary>
		/// <param name="hook">The hook.</param>
		public ProxyGenerationOptions(IProxyGenerationHook hook)
		{
			this.hook = hook;
			useSelector = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyGenerationOptions"/> class.
		/// </summary>
		public ProxyGenerationOptions() : this(new AllMethodsHook())
		{
		}

		private ProxyGenerationOptions (SerializationInfo info, StreamingContext context)
		{
			hook = (IProxyGenerationHook) info.GetValue ("hook", typeof (IProxyGenerationHook));
			selector = (IInterceptorSelector) info.GetValue ("selector", typeof (IInterceptorSelector));
			mixins = (ArrayList) info.GetValue ("mixins", typeof (ArrayList));
			baseTypeForInterfaceProxy = Type.GetType (info.GetString ("baseTypeForInterfaceProxy.AssemblyQualifiedName"));
			useSingleInterfaceProxy = info.GetBoolean ("useSingleInterfaceProxy");
			useSelector = info.GetBoolean ("useSelector");
		}

		public void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.AddValue ("hook", hook);
			info.AddValue ("selector", selector);
			info.AddValue ("mixins", mixins);
			info.AddValue ("baseTypeForInterfaceProxy.AssemblyQualifiedName", baseTypeForInterfaceProxy.AssemblyQualifiedName);
			info.AddValue ("useSingleInterfaceProxy", useSingleInterfaceProxy);
			info.AddValue ("useSelector", useSelector);
		}

		public IProxyGenerationHook Hook
		{
			get { return hook; }
			set { hook = value; }
		}

		public IInterceptorSelector Selector
		{
			get { return selector; }
			set { selector = value; }
		}

		public bool UseSelector
		{
			get { return useSelector; }
			set { useSelector = value; }
		}

		public bool UseSingleInterfaceProxy
		{
			get { return useSingleInterfaceProxy; }
			set { useSingleInterfaceProxy = value; }
		}

		public void AddMixinInstance(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			if (mixins == null)
			{
				mixins = new ArrayList();
			}

			mixins.Add(instance);
		}

		public object[] MixinsAsArray()
		{
			if (mixins == null) return new object[0];

			return mixins.ToArray();
		}

		public bool HasMixins
		{
			get { return mixins == null ? false : mixins.Count != 0; }
		}

		public Type BaseTypeForInterfaceProxy
		{
			get { return baseTypeForInterfaceProxy; }
			set { baseTypeForInterfaceProxy = value; }
		}

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			ProxyGenerationOptions proxyGenerationOptions = obj as ProxyGenerationOptions;
			if (proxyGenerationOptions == null) return false;
			if (!Equals(hook.GetType(), proxyGenerationOptions.hook.GetType())) return false;
			if (!Equals(selector, proxyGenerationOptions.selector)) return false;
			if (!Equals(mixins, proxyGenerationOptions.mixins)) return false;
			if (!Equals(baseTypeForInterfaceProxy, proxyGenerationOptions.baseTypeForInterfaceProxy)) return false;
			if (!Equals(useSelector, proxyGenerationOptions.useSelector)) return false;
			if (!Equals(useSingleInterfaceProxy, proxyGenerationOptions.useSingleInterfaceProxy)) return false;
			return true;
		}

		public override int GetHashCode()
		{
			int result = hook != null ? hook.GetType().GetHashCode() : 0;
			result = 29 * result + (selector != null ? selector.GetHashCode() : 0);
			result = 29 * result + (mixins != null ? mixins.GetHashCode() : 0);
			result = 29 * result + (baseTypeForInterfaceProxy != null ? baseTypeForInterfaceProxy.GetHashCode() : 0);
			result = 29 * result + useSelector.GetHashCode();
			result = 29 * result + useSingleInterfaceProxy.GetHashCode();
			return result;
		}
	}
}