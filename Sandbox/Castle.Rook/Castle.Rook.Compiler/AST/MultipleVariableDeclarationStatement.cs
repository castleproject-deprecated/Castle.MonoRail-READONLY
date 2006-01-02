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

namespace Castle.Rook.Compiler.AST
{
	using System;
	using System.Collections;

	using Castle.Rook.Compiler.Visitors;


	public class MultipleVariableDeclarationStatement : AbstractStatement
	{
		private readonly AccessLevel accessLevel;
		private IList idents = new ArrayList();
		private ExpressionCollection initExps;

		public MultipleVariableDeclarationStatement(AccessLevel accessLevel)
		{
//			decls = new ExpressionCollection(this);
			initExps = new ExpressionCollection(this);
			this.accessLevel = accessLevel;
		}

		public void AddIdentifier(Identifier identifier)
		{
			idents.Add(identifier);
		}

		public void AddInitExp(IExpression exp)
		{
			initExps.Add(exp);
		}

		public IList Identifiers
		{
			get { return idents; }
		}

		public ExpressionCollection InitExpressions
		{
			get { return initExps; }
		}

		public AccessLevel AccessLevel
		{
			get { return accessLevel; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitMultipleVariableDeclarationStatement(this);
		}
	}
}
