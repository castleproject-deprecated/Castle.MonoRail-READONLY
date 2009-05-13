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

namespace Extending2
{
	using System;

	using Castle.Core.Interceptor;

	/// <summary>
	/// Summary description for TransactionInterceptor.
	/// </summary>
	public class TransactionInterceptor : IInterceptor
	{
		private ITransactionManager _transactionManager;
		private TransactionConfigHolder _transactionConfHolder;

		public TransactionInterceptor(ITransactionManager transactionManager, 
			TransactionConfigHolder transactionConfHolder)
		{
			_transactionManager = transactionManager;
			_transactionConfHolder = transactionConfHolder;
		}

		public void Intercept(IInvocation invocation)
		{
			if (_transactionManager.CurrentTransaction != null)
			{
				// No support for nested transactions
				// is necessary
				invocation.Proceed();
				return;
			}

			TransactionConfig config = 
				_transactionConfHolder.GetConfig( 
					invocation.Method.DeclaringType );

			if (config != null && config.IsMethodTransactional( invocation.Method ))
			{
				ITransaction transaction = 
					_transactionManager.CreateTransaction();

				try
				{
					invocation.Proceed();
					
					transaction.Commit();
				}
				catch(Exception ex)
				{
					transaction.Rollback();

					throw ex;
				}
				finally
				{
					_transactionManager.Release(transaction);
				}
			}
			else
			{
				invocation.Proceed();
			}
		}
	}
}
