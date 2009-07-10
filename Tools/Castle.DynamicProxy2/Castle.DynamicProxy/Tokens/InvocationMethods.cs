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

namespace Castle.DynamicProxy.Tokens
{
	using System;
	using System.Reflection;
	using Core.Interceptor;

	/// <summary>
	/// Holds <see cref="MethodInfo"/> objects representing methods of <see cref="AbstractInvocation"/> class.
	/// </summary>
	public static class InvocationMethods
	{
		public static readonly MethodInfo GetArguments =
			typeof(AbstractInvocation).GetMethod("get_Arguments");

		public static readonly MethodInfo GetArgumentValue =
			typeof(AbstractInvocation).GetMethod("GetArgumentValue");

		public static readonly MethodInfo GetReturnValue =
			typeof(AbstractInvocation).GetMethod("get_ReturnValue");

		public static readonly MethodInfo SetArgumentValue =
			typeof(AbstractInvocation).GetMethod("SetArgumentValue");

		public static readonly MethodInfo SetGenericMethodArguments =
			typeof(AbstractInvocation).GetMethod("SetGenericMethodArguments", new Type[] { typeof(Type[]) });

		public static readonly MethodInfo SetReturnValue =
			typeof(AbstractInvocation).GetMethod("set_ReturnValue");

		public static readonly ConstructorInfo ConstructorWithTargetMethod =
			typeof(AbstractInvocation).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
			new Type[] { typeof(object), typeof(object), typeof(IInterceptor[]), typeof(Type),
				typeof(MethodInfo), typeof(MethodInfo), typeof(object[])
			},
			null);

		public static readonly ConstructorInfo ConstructorWithTargetMethodWithSelector =
			typeof(AbstractInvocation).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
			new Type[] { typeof(object), typeof(object), typeof(IInterceptor[]), typeof(Type),
				typeof(MethodInfo), typeof(MethodInfo), typeof(object[]), typeof(IInterceptorSelector),
				typeof(IInterceptor[]).MakeByRefType()
			},
			null);

		public static readonly MethodInfo Proceed =
			typeof(AbstractInvocation).GetMethod("Proceed", BindingFlags.Instance | BindingFlags.Public);

		public static readonly MethodInfo EnsureValidTarget =
			typeof (AbstractInvocation).GetMethod("EnsureValidTarget", BindingFlags.Instance | BindingFlags.NonPublic);
	}
}
