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
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using Castle.Core.Logging;

	/// <summary>
	/// Helper abstract class for <see cref="ITransaction"/> implementors. 
	/// </summary>
	public abstract class AbstractTransaction : MarshalByRefObject, ITransaction, IDisposable
	{
		private HybridDictionary context;
		private IList synchronizations;
		private TransactionStatus state = TransactionStatus.NoTransaction;
		private TransactionMode transactionMode;
		private IsolationMode isolationMode;
		private bool distributedTransaction;
		private string name;
		private ILogger logger = NullLogger.Instance;

		internal IList resources;

		public AbstractTransaction()
		{
			resources = new ArrayList();
			synchronizations = new ArrayList();
			context = new HybridDictionary(true);
		}

		public AbstractTransaction(TransactionMode transactionMode, IsolationMode isolationMode, bool distributedTransaction)
			: this(transactionMode, isolationMode, distributedTransaction, null)
		{
		}

		public AbstractTransaction(TransactionMode transactionMode, IsolationMode isolationMode, bool distributedTransaction,
		                           string name) : this()
		{
			this.transactionMode = transactionMode;
			this.isolationMode = isolationMode;
			this.distributedTransaction = distributedTransaction;

			if (String.IsNullOrEmpty(name))
			{
				this.name = ObtainName();
			}
			else
			{
				this.name = name;
			}
		}

		public ILogger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		#region MarshalByRefObject

		public override object InitializeLifetimeService()
		{
			return null;
		}

		#endregion

		#region ITransaction

		public virtual void Enlist(IResource resource)
		{
			logger.DebugFormat("Enlisting resource {0}", resource);

			if (resource == null) throw new ArgumentNullException("resource");

			// We can't add the resource more than once
			if (resources.Contains(resource)) return;

			if (Status == TransactionStatus.Active)
			{
				try
				{
					resource.Start();
				}
				catch(Exception ex)
				{
					state = TransactionStatus.Invalid;

					logger.Error("Enlisting resource failed", ex);

					throw;
				}
			}

			resources.Add(resource);

			logger.DebugFormat("Resource enlisted successfully {0}", resource);
		}

		public virtual void Begin()
		{
			logger.DebugFormat("Transaction '{0}' Begin", Name);

			AssertState(TransactionStatus.NoTransaction);
			state = TransactionStatus.Active;

			foreach(IResource resource in resources)
			{
				try
				{
					resource.Start();
				}
				catch(Exception ex)
				{
					state = TransactionStatus.Invalid;

					logger.Error("Failed to start transaction on resource.", ex);

					throw;
				}
			}
		}

		public virtual void Rollback()
		{
			logger.DebugFormat("Transaction '{0}' Rollback", Name);

			AssertState(TransactionStatus.Active);
			state = TransactionStatus.RolledBack;

			PerformSynchronizations(false);

			Exception error = null;
			ArrayList failedResources = new ArrayList(resources.Count);

			foreach(IResource resource in resources)
			{
				try
				{
					resource.Rollback();
				}
				catch(Exception ex)
				{
					state = TransactionStatus.Invalid;

					logger.Error("Failed to rollback transaction on resource.", ex);

					error = ex;

					failedResources.Add(resource);
				}
			}

			PerformSynchronizations(true);

			if (error != null)
			{
				throw new RollbackResourceException("Could not rollback transaction, one (or more) resources failed.", error,
				                                    (IResource) failedResources[failedResources.Count - 1],
				                                    (IResource[]) failedResources.ToArray((typeof(IResource))));
			}
		}

		public virtual void Commit()
		{
			logger.DebugFormat("Transaction '{0}' Commit", Name);

			AssertState(TransactionStatus.Active);
			state = TransactionStatus.Committed;

			PerformSynchronizations(false);

			Exception error = null;
			ArrayList failedResources = new ArrayList(resources.Count);

			foreach(IResource resource in resources)
			{
				try
				{
					resource.Commit();
				}
				catch(Exception ex)
				{
					state = TransactionStatus.Invalid;

					logger.Error("Failed to commit transaction on resource.", ex);

					error = ex;

					failedResources.Add(resource);
				}
			}

			PerformSynchronizations(true);

			if (error != null)
			{
				throw new CommitResourceException("Could not commit transaction, one (or more) of the resources failed", error,
				                                  (IResource) failedResources[failedResources.Count - 1],
				                                  (IResource[]) failedResources.ToArray((typeof(IResource))));
			}
		}

		public virtual void SetRollbackOnly()
		{
		}

		public TransactionStatus Status
		{
			get { return state; }
		}

		public virtual void RegisterSynchronization(ISynchronization synchronization)
		{
			logger.DebugFormat("Registering Synchronization {0}", synchronization);

			if (synchronization == null) throw new ArgumentNullException("synchronization");

			synchronizations.Add(synchronization);

			logger.DebugFormat("Synchronization registered successfully {0}", synchronization);
		}

		public virtual IDictionary Context
		{
			get { return context; }
		}

		public abstract bool IsChildTransaction { get; }

		public abstract bool IsRollbackOnlySet { get; }

		public TransactionMode TransactionMode
		{
			get { return transactionMode; }
		}

		public IsolationMode IsolationMode
		{
			get { return isolationMode; }
		}

		public bool DistributedTransaction
		{
			get { return distributedTransaction; }
		}

		public string Name
		{
			get { return name; }
		}

		#region ITransaction Members

		public IResource[] Resources
		{
			get { return (IResource[]) ((ArrayList) resources).ToArray(typeof(IResource)); }
		}

		#endregion

		#endregion

		#region IDisposable Members

		public virtual void Dispose()
		{
			resources.Clear();
			synchronizations.Clear();
		}

		#endregion

		#region Helper methods

		protected virtual void AssertState(TransactionStatus state)
		{
			if (this.state != state)
			{
				throw new TransactionException("Invalid transaction state to perform the requested action");
			}
		}

		protected string ObtainName()
		{
			return Convert.ToString(GetHashCode());
		}

		private void PerformSynchronizations(bool runAfterCompletion)
		{
			foreach(ISynchronization sync in synchronizations)
			{
				try
				{
					if (runAfterCompletion)
					{
						sync.AfterCompletion();
					}
					else
					{
						sync.BeforeCompletion();
					}
				}
				catch(Exception ex)
				{
					logger.Error("Synchronization failed", ex);

					// Exceptions should not be threw by syncs.
					// They will be swalled
				}
			}
		}

		#endregion
	}
}