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

namespace Castle.Services.Transaction
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Remoting.Messaging;

	[Serializable]
	public class Activity : MarshalByRefObject, ILogicalThreadAffinative
	{
		private Guid id;
		private Stack<ITransaction> transactionStack = new Stack<ITransaction>(2);

		/// <summary>
		/// Initializes a new instance of the <see cref="Activity"/> class.
		/// </summary>
		public Activity()
		{
			id = Guid.NewGuid();
		}

		public ITransaction CurrentTransaction
		{
			get
			{
				if (transactionStack.Count == 0)
				{
					return null;
				}

				return transactionStack.Peek();
			}
		}

		public void Push(ITransaction transaction)
		{
			transactionStack.Push(transaction);
		}

		public ITransaction Pop()
		{
			return transactionStack.Pop();
		}

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			Activity activity = obj as Activity;
			if (activity == null) return false;
			return Equals(id, activity.id);
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}
	}
}
