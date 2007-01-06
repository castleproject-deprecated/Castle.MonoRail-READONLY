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

namespace Castle.Rook.AST
{
	using System;


	public class FieldDeclarationStatement : Statement
	{
		private IdentifierReferenceExpression target;
		private Expression value;
		private AccessLevel scopeAccessLevel;
		private Identifier typeName;

		public FieldDeclarationStatement(AccessLevel scopeAccessLevel, IdentifierReferenceExpression var, Identifier typeName, Expression value)
		{
			this.scopeAccessLevel = scopeAccessLevel;
			this.target = var;
			this.value = value;
			this.typeName = typeName;
		}

		public IdentifierReferenceExpression Target
		{
			get { return target; }
		}

		public Expression Value
		{
			get { return this.value; }
		}

		public AccessLevel ScopeAccessLevel
		{
			get { return scopeAccessLevel; }
		}

		public Identifier TypeName
		{
			get { return typeName; }
		}
	}
}
