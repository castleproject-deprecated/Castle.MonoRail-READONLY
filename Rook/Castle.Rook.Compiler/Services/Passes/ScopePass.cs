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

namespace Castle.Rook.Compiler.Services.Passes
{
	using System;

	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Visitors;


	public class ScopePass : ICompilerPass
	{
		public void ExecutePass(CompilationUnit unit)
		{
			Method2ScopeVisitor method2scope = new Method2ScopeVisitor();
			method2scope.VisitNode(unit);

			IdentifierNormalizationVisitor identVisitor = new IdentifierNormalizationVisitor();
			identVisitor.VisitNode(unit);
		}
	}

	public class IdentifierNormalizationVisitor : DepthFirstVisitor
	{
		
	}

	public class Method2ScopeVisitor : DepthFirstVisitor
	{
		public override bool VisitEnter(MethodDefinitionStatement methodDef)
		{
			return false;
		}

		public override bool VisitMethodDefinitionStatement(MethodDefinitionStatement methodDef)
		{
			methodDef.DefiningSymbolTable.Parent.AddMethod( methodDef );

			return base.VisitMethodDefinitionStatement(methodDef);
		}
	}
}
