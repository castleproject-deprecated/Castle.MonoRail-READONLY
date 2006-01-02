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

	using Castle.Rook.Compiler.Visitors;


	public class SourceUnit : AbstractCodeNode, IStatementContainer
	{
		private readonly CompilationUnit unit;
		private StatementCollection statements;
		private NamespaceCollection namespaces;

		public SourceUnit(CompilationUnit unit, ISymbolTable parentScope) : base(NodeType.SourceUnit)
		{
			this.unit = unit;

			nameScope = new SymbolTable(ScopeType.SourceUnit, parentScope);

			statements = new StatementCollection(this);
			namespaces = new NamespaceCollection(this);
		}

		public StatementCollection Statements
		{
			get { return statements; }
		}

		public NamespaceCollection Namespaces
		{
			get { return namespaces; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitSourceUnit(this);
		}

		public CompilationUnit CompilationUnit
		{
			get { return unit; }
		}
	}
}
