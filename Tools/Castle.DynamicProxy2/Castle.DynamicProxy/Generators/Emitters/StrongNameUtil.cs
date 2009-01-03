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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;

	public static class StrongNameUtil
	{
		private static readonly IDictionary signedAssemblyCache = new Dictionary<Assembly, bool>();
		private static readonly object lockObject = new object();

		public static bool IsAssemblySigned(Assembly assembly)
		{
			lock (lockObject)
			{
				if (signedAssemblyCache.Contains(assembly) == false)
				{
					bool isSigned = ContainsPublicKey(assembly);
					signedAssemblyCache.Add(assembly, isSigned);
				}
				return (bool)signedAssemblyCache[assembly];
			}
		}

		private static bool ContainsPublicKey(Assembly assembly)
		{
#if SILVERLIGHT
			// Pulled from a comment on http://www.flawlesscode.com/post/2008/08/Mocking-and-IOC-in-Silverlight-2-Castle-Project-and-Moq-ports.aspx
			return !assembly.FullName.Contains("PublicKeyToken=null");
#else
			byte[] key = assembly.GetName().GetPublicKey();
			return key != null && key.Length != 0;
#endif
		}

		public static bool IsAnyTypeFromUnsignedAssembly(IEnumerable<Type> types)
		{
			foreach (Type t in types)
			{
				if (!IsAssemblySigned(t.Assembly))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsAnyTypeFromUnsignedAssembly(Type baseType, Type[] interfaces)
		{
			return !IsAssemblySigned(baseType.Assembly) || IsAnyTypeFromUnsignedAssembly(interfaces);
		}
	}
}
