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

namespace Castle.Rook.Compiler.AST
{
	using System;

	using Castle.Rook.Compiler.AST.Util;


	public class StatementCollection : LinkedListBase
	{
		private readonly IASTNode owner;

		public StatementCollection(IASTNode owner)
		{
			this.owner = owner;
		}

		protected override void PrepareNode(object value)
		{
			((IStatement) value).Parent = owner;
		}

		public int Add(IStatement node)
		{
			node.Parent = owner;
			return InnerList.Add(node);
		}

		public IStatement this [int index]
		{
			get { return InnerList[index] as IStatement; }
		}

		public void Replace(IASTNode originalNode, IASTNode replace)
		{
			if (!InnerList.Contains(originalNode))
			{
				throw new ArgumentException("Tried to replace inexistent node " + originalNode.ToString());
			}

			int index = InnerList.IndexOf(originalNode);
			
			InnerList.RemoveAt(index);

			if (replace != null)
			{
				PrepareNode(replace);
				InnerList.Insert(index, replace);
			}
		}
	}
}
