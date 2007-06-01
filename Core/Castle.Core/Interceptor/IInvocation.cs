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

namespace Castle.Core.Interceptor
{
	using System;
	using System.Reflection;

	/// <summary>
	/// New interface that is going to be used by DynamicProxy 2
	/// </summary>
	public interface IInvocation
	{
		object Proxy { get; }

		object InvocationTarget { get; }

		Type TargetType { get; }

		object[] Arguments { get; }

		void SetArgumentValue(int index, object value);

		object GetArgumentValue(int index);

		MethodInfo Method { get; }

		/// <summary>
		/// For interface proxies, this will point to the
		/// <see cref="MethodInfo"/> on the target class
		/// </summary>
		MethodInfo MethodInvocationTarget { get; }

		object ReturnValue { get; set; }

		void Proceed();
	}
}