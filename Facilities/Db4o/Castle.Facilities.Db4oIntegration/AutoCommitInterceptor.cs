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

namespace Castle.Facilities.Db4oIntegration
{
	using System;

	using com.db4o;

	using Castle.MicroKernel;
	using Castle.Core.Interceptor;
	using Castle.Services.Transaction;
	
	public class AutoCommitInterceptor : IMethodInterceptor
	{
		private static readonly String ContextKey = "db40.transaction.context";

		private readonly IKernel _kernel;
		private readonly ObjectContainer _objContainer;

		[CLSCompliant(false)]
		public AutoCommitInterceptor(IKernel kernel, ObjectContainer objContainer)
		{
			_objContainer = objContainer;
			_kernel = kernel;
		}

		public object Intercept(IMethodInvocation invocation, params object[] args)
		{
			EnlistObjectContainerIfHasTransactionActive();

			return invocation.Proceed(args);
		}

		private void EnlistObjectContainerIfHasTransactionActive()
		{
			if (!_kernel.HasComponent(typeof(ITransactionManager))) return;

			ITransactionManager manager = (ITransactionManager) _kernel[ typeof(ITransactionManager) ];

			ITransaction transaction = manager.CurrentTransaction;

			if (transaction != null)
			{
				if (!transaction.Context.Contains(ContextKey))
				{
					transaction.Context[ContextKey] = true;
					transaction.Enlist(new ResourceObjectContainerAdapter(_objContainer));
				}
			}

			_kernel.ReleaseComponent(manager);
		}
	}
}
