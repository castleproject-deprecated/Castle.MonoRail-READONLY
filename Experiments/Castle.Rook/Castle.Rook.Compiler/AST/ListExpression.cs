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
	using System.Collections;

	using Castle.Rook.Compiler.Visitors;


	public class ListExpression : AbstractExpression
	{
		private ExpressionCollection items;

		public ListExpression()
		{
			items = new ExpressionCollection(this);
		}

		public void Add(IExpression item)
		{
			items.Add(item);
		}

		public ExpressionCollection Items
		{
			get { return items; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitListExpression(this);
		}

		public override IExpression Accept(IExpressionAttrVisitor visitor)
		{
			return visitor.VisitListExpression(this);
		}
	}
}
