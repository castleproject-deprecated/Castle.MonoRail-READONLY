// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Invocation
{
	using System;
	using System.Reflection;


	public class InterfaceInvocation : SameClassInvocation
	{
		private MethodInfo _methodInvocationTarget;

		public InterfaceInvocation(ICallable callable, object proxy, MethodInfo method) : 
			base(callable, proxy, method)
		{
		}

//		public override MethodInfo MethodInvocationTarget
//		{
//			get
//			{
//				if (_methodInvocationTarget == null)
//				{
//					ParameterInfo[] paramsInfo = Method.GetParameters();
//					Type[] parameters = new Type[paramsInfo.Length];
//
//					int index = 0;
//
//					foreach(ParameterInfo paramInfo in paramsInfo)
//					{
//						parameters[index++] = paramInfo.ParameterType;
//					}
//
//					_methodInvocationTarget = InvocationTarget.GetType().GetMethod(
//						Method.Name, parameters);
//				}
//
//				return _methodInvocationTarget;
//			}
//		}
	}
}