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

	using Castle.Rook.Compiler.Visitors;

	public enum UnaryOp
	{
		Plus,
		Minus,
		Not,
		BitwiseNot
	}

	public class UnaryExpression : AbstractExpression
	{
		private readonly UnaryOp op;
		private readonly IExpression inner;

		public UnaryExpression(IExpression inner, UnaryOp op)
		{
			this.op = op;
			this.inner = inner;
		}

		public UnaryOp Operation
		{
			get { return op; }
		}

		public IExpression Inner
		{
			get { return inner; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitUnaryExpression(this);
		}

		public override IExpression Accept(IExpressionAttrVisitor visitor)
		{
			return visitor.VisitUnaryExpression(this);
		}
	}
}
