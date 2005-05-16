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

namespace Castle.Facilities.AspectSharp.Tests.Interceptors
{
	using System;
	using System.Text;

	using AopAlliance.Intercept;

	/// <summary>
	/// Summary description for LoggerInterceptor.
	/// </summary>
	public class LoggerInterceptor : IMethodInterceptor
	{
		private static StringBuilder _builder = new StringBuilder();

		public object Invoke(IMethodInvocation invocation)
		{
			_builder.Append( String.Format("Enter {0}\r\n", invocation.Method.Name) );

			return invocation.Proceed();
		}

		public static StringBuilder Messages
		{
			get { return _builder; }
		}
	}
}
