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
	using System.Collections;

	using Castle.Rook.Compiler.Visitors;


	public class CompilationUnit : AbstractCodeNode, INameScopeAccessor
	{
		private IList statements = new ArrayList();
		private IList namespaces = new ArrayList();
		private INameScope namescope = new RootNameScope();

		public IList Statements
		{
			get { return statements; }
		}

		public IList Namespaces
		{
			get { return namespaces; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			visitor.VisitCompilationUnit(this);
			return true;
		}

		public INameScope Namescope
		{
			get { return namescope; }
		}
	}
}
