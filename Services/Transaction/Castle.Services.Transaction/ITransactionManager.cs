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

namespace Castle.Services.Transaction
{
	public delegate void TransactionCreationInfoDelegate(ITransaction transaction, TransactionMode transactionMode, IsolationMode isolationMode, bool distributedTransaction);

	public delegate void TransactionDelegate(ITransaction transaction);
	
	public delegate void TransactionErrorDelegate(ITransaction transaction, TransactionException transactionError);

	/// <summary>
	/// Manages the creation and disposal of <see cref="ITransaction"/> instances.
	/// </summary>
	public interface ITransactionManager
	{
		/// <summary>
		/// Raised when a top level transaction was created
		/// </summary>
		event TransactionCreationInfoDelegate TransactionCreated;

		/// <summary>
		/// Raised when a child transaction was created
		/// </summary>
		event TransactionCreationInfoDelegate ChildTransactionCreated;

		/// <summary>
		/// Raised when the transaction was committed successfully
		/// </summary>
		event TransactionDelegate TransactionCommitted;

		/// <summary>
		/// Raised when the transaction was rolledback successfully
		/// </summary>
		event TransactionDelegate TransactionRolledback;

		/// <summary>
		/// Raised when the transaction was disposed
		/// </summary>
		event TransactionDelegate TransactionDisposed;
		
		/// <summary>
		/// Raised when the transaction has failed on commit/rollback
		/// </summary>
		event TransactionErrorDelegate TransactionFailed;

		/// <summary>
		/// Creates a transaction.
		/// </summary>
		/// <param name="transactionMode">The transaction mode.</param>
		/// <param name="isolationMode">The isolation mode.</param>
		/// <returns></returns>
		ITransaction CreateTransaction(TransactionMode transactionMode, IsolationMode isolationMode);

		/// <summary>
		/// Creates a transaction.
		/// </summary>
		/// <param name="transactionMode">The transaction mode.</param>
		/// <param name="isolationMode">The isolation mode.</param>
		/// <param name="distributedTransaction">if set to <c>true</c>, the TM will create a distributed transaction.</param>
		/// <returns></returns>
		ITransaction CreateTransaction(TransactionMode transactionMode, IsolationMode isolationMode, bool distributedTransaction);

		/// <summary>
		/// Returns the current <see cref="ITransaction"/>. 
		/// The transaction manager will probably need to 
		/// hold the created transaction in the thread or in 
		/// some sort of context.
		/// </summary>
		ITransaction CurrentTransaction { get; }

		/// <summary>
		/// Should guarantee the correct disposal of transaction
		/// resources.
		/// </summary>
		/// <param name="transaction"></param>
		void Dispose(ITransaction transaction);
	}
}
