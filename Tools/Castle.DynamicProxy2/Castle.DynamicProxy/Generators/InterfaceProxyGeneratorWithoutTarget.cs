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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Reflection;
	using Castle.DynamicProxy.Generators.Emitters;

	[CLSCompliant(false)]
	public class InterfaceProxyGeneratorWithoutTarget : InterfaceProxyWithTargetGenerator
	{
		public InterfaceProxyGeneratorWithoutTarget(ModuleScope scope, Type theInterface) : base(scope, theInterface)
		{
		}

		protected override void CreateInvocationForMethod(ClassEmitter emitter, MethodInfo method, Type proxyTargetType)
		{
			method2methodOnTarget[method] = method;
			
			method2Invocation[method] = BuildInvocationNestedType(emitter, targetType,
			                                                      proxyTargetType,
			                                                      method, null,
			                                                      ConstructorVersion.WithTargetMethod);
		}

		protected override InterfaceGeneratorType GeneratorType
		{
			get
			{
				return InterfaceGeneratorType.WithoutTarget;
			}
		}
	}
}
