// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;

	public class ClassEmitter : AbstractTypeEmitter
	{
		private static IDictionary signedAssemblyCache = new Hashtable();

		public ClassEmitter(ModuleScope modulescope, String name, Type baseType, Type[] interfaces, bool serializable)
		{
			TypeAttributes flags = 
				TypeAttributes.Public|TypeAttributes.Class|TypeAttributes.Serializable;

			if (serializable)
			{
				flags |= TypeAttributes.Serializable;
			}

            bool isAssemblySigned = IsAssemblySigned(baseType);
            
			typebuilder = modulescope.ObtainDynamicModule(isAssemblySigned).DefineType(
				name, flags, baseType, interfaces);
		}

		private bool IsAssemblySigned(Type baseType)
		{
			lock(signedAssemblyCache)
			{
				if (signedAssemblyCache.Contains(baseType.Assembly) == false)
				{
					byte[] key = baseType.Assembly.GetName().GetPublicKey();
					bool isSigned = key != null  && key.Length != 0;
					signedAssemblyCache.Add(baseType.Assembly, isSigned);
				}
				return (bool) signedAssemblyCache[baseType.Assembly];
			}
		}
	}
}
