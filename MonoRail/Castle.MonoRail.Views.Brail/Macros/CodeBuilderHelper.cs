// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.Brail
{
	using System.IO;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;

	public class CodeBuilderHelper
	{
		public static Expression CreateCallableFromMacroBody(BooCodeBuilder builder, MacroStatement macro)
		{
			// create closure for macro's body or null
			Expression macroBody = new NullLiteralExpression();
			if (macro.Block.Statements.Count > 0)
			{
				BlockExpression callableExpr = new BlockExpression();
				callableExpr.Body = macro.Block;
				callableExpr.Parameters.Add(
					new ParameterDeclaration("OutputStream",
					                         builder.CreateTypeReference(typeof(TextWriter))));

				macroBody = callableExpr;
			}
			return macroBody;
		}
	}
}