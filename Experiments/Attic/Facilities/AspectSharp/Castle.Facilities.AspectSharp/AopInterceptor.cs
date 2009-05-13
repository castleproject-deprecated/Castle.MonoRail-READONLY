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

using AspectSharp;
using AspectSharp.Core;
using AspectSharp.Lang.AST;

namespace Castle.Facilities.AspectSharp
{
	using System;
	using Castle.Core;
	using Castle.Core.Interceptor;
	using Castle.MicroKernel;

	/// <summary>
	/// Summary description for AopInterceptor.
	/// </summary>
	[Transient]
	public class AopInterceptor : IInterceptor, IOnBehalfAware
	{
		private IKernel _kernel;
		private AspectEngine _engine;
		private IInvocationDispatcher _dispatcher;

		public AopInterceptor(AspectEngine engine, IKernel kernel)
		{
			_engine = engine;
			_kernel = kernel;
		}

		public void SetInterceptedComponentModel(ComponentModel target)
		{
			AspectDefinition aspectDef = (AspectDefinition) 
				target.ExtendedProperties["aop.aspect"];

			System.Diagnostics.Debug.Assert( aspectDef != null );

			_dispatcher = new ContainerInvocationDispatcher(aspectDef, _kernel);
			_dispatcher.Init(_engine);
		}

		public void Intercept(IInvocation invocation)
		{
			_dispatcher.Intercept( invocation);
		}
	}
}
