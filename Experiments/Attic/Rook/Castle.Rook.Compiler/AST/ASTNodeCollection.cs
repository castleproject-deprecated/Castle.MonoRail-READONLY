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

namespace Castle.Rook.Compiler.AST
{
	using System;

	using Castle.Core.Internal;


	public class ASTNodeCollection : LinkedList
	{
		private readonly IASTNode owner;

		public ASTNodeCollection(IASTNode owner)
		{
			this.owner = owner;
		}

		public override void AddFirst(object value)
		{
			base.AddFirst(value);

			(value as IASTNode).Parent = owner;
		}

		public override int Add(object value)
		{
			int index = base.Add(value);

			(value as IASTNode).Parent = owner;

			return index;
		}

		public override bool Replace(object old, object value)
		{
			(old as IASTNode).Parent = null;

			if (value != null)
			{
				(value as IASTNode).Parent = owner;
			}

			return base.Replace(old, value);
		}

		public override void Insert(int index, object value)
		{
			(value as IASTNode).Parent = owner;

			base.Insert(index, value);
		}

		public override void Remove(object value)
		{
			(value as IASTNode).Parent = null;

			base.Remove(value);
		}
	}
}
