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

namespace Castle.Rook.Compiler.Services.Passes
{
	using System;
	using System.Collections;

	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Visitors;

	/// <summary>
	/// 
	/// </summary>
	public class DeclarationBinding : BreadthFirstVisitor, ICompilerPass
	{
		private readonly IIdentifierNameService identifierService;
		private readonly IErrorReport errorReport;

		public DeclarationBinding(IIdentifierNameService identifierService, IErrorReport errorReport)
		{
			this.identifierService = identifierService;
			this.errorReport = errorReport;
		}

		public void ExecutePass(CompilationUnit unit)
		{
			VisitNode(unit);
		}

		public override bool VisitMultipleVariableDeclarationStatement(MultipleVariableDeclarationStatement varDecl)
		{
			IList stmts = CreateSimpleExpressions(varDecl);

			ReplaceVarDeclBySingleDecls(varDecl, stmts);

			EnsureTypeDeclarationsBelongsToThisScope(varDecl, stmts);

			return base.VisitMultipleVariableDeclarationStatement(varDecl);
		}

		private void EnsureTypeDeclarationsBelongsToThisScope(MultipleVariableDeclarationStatement varDecl, IList stmts)
		{
			INameScope namescope = (varDecl.Parent as INameScopeAccessor).Namescope;
	
			foreach(SingleVariableDeclarationStatement typeDecl in stmts)
			{
				Identifier ident = typeDecl.Identifier;

				if (namescope.IsDefined(ident.Name))
				{
					errorReport.Error( "TODOFILENAME", typeDecl.Position, 
					                   "Sorry but '{0}' is already defined.", ident.Name );
				}
				else
				{
					// TODO: If its a instance or static, and we're 
					// in an inner block, we need to move these statments
					// to the type/class level statements list

					if (ident.Type == IdentifierType.Local)
					{
						namescope.AddVariable( ident );
					}
					else if (namescope.NameScopeType == NameScopeType.Global || 
						namescope.NameScopeType == NameScopeType.Type ||
						namescope.NameScopeType == NameScopeType.Namespace)
					{
						namescope.AddVariable( ident );
					}
					else
					{
						IASTNode parent = varDecl.Parent;
						
						while(parent != null && 
							parent.NodeType != NodeType.TypeDefinition && 
							parent.NodeType != NodeType.NamespaceDefinition && 
							parent.NodeType != NodeType.Global)
						{
							parent = parent.Parent;
						}

						if (parent != null)
						{
							INameScopeAccessor accessor  = parent as INameScopeAccessor;
							IStatementContainer typeOrGlobalStmtsContainer = parent as IStatementContainer;
							
							System.Diagnostics.Debug.Assert( accessor != null );
							System.Diagnostics.Debug.Assert( typeOrGlobalStmtsContainer != null );

							if (accessor.Namescope.IsDefined(ident.Name))
							{
								errorReport.Error( "TODOFILENAME", typeDecl.Position, 
								                   "Sorry but '{0}' is already defined.", ident.Name );
							}
							else
							{
								accessor.Namescope.AddVariable(ident);

								// We can replace the declaration on the method 
								// body with an assignment if and only if this type decl has
								// an init expression, so CreateAssignmentFromTypeDecl can return null
								AssignmentExpression assignExp = CreateAssignmentFromTypeDecl(typeDecl);
								ExpressionStatement assingExpStmt = new ExpressionStatement(assignExp);

								// Clear the InitExp as it might be invalid aside from this context
								typeDecl.InitExp = null;
							
								// Replace the declaration with an assignment
								(varDecl.Parent as IStatementContainer).Statements.Replace(typeDecl, assingExpStmt);

								// Add the member/field declaration to the parent
								typeOrGlobalStmtsContainer.Statements.Add( typeDecl );
							}
						}
						else
						{
							errorReport.Error( "TODOFILENAME", typeDecl.Position, 
							                   "The instance of static declaration '{0}' could not be mapped to the parent type", ident.Name );
						}
					}
				}
			}
		}

		private IList CreateSimpleExpressions(MultipleVariableDeclarationStatement varDecl)
		{
			IList newStmts = new ArrayList();

			int index = 0;
			ExpressionCollection initExps = varDecl.InitExpressions; 
	
			foreach(Identifier ident in varDecl.Identifiers)
			{
				// Here we are converting expression from 
				// x:int, y:long = 1, 2L
				// to an AST representation equivalent to
				// x:int = 1; y:long = 2L

				SingleVariableDeclarationStatement svStmt = new SingleVariableDeclarationStatement(ident);

				if (index < initExps.Count)
				{
					svStmt.InitExp = initExps[index];
					EnsureNoPostFixStatement(svStmt.InitExp);
				}
				
				index++;

				newStmts.Add(svStmt);
			}
	
			// We don't need them anymore
			initExps.Clear();

			return newStmts;
		}

		private void ReplaceVarDeclBySingleDecls(MultipleVariableDeclarationStatement varDecl, IList stmts)
		{
			int index;

			// Replace the VariableDeclarationStatement node by a 
			// (possible) sequence of SingleVariableDeclarationStatement
	
			IStatementContainer stmtContainer = varDecl.Parent as IStatementContainer;
	
			index = stmtContainer.Statements.IndexOf(varDecl);
			stmtContainer.Statements.RemoveAt(index);
	
			foreach(SingleVariableDeclarationStatement svDecl in stmts)
			{
				stmtContainer.Statements.Insert( index++, svDecl );
			}
		}

		private void EnsureNoPostFixStatement(IExpression initExpression)
		{
			if (initExpression.PostFixStatement != null)
			{
				errorReport.Error( "TODOFILENAME", initExpression.Position, 
				                   "Sorry but a variable initializer can not be conditional or " + 
				                   	"has a while/until statement attached.");
			}
		}

		private AssignmentExpression CreateAssignmentFromTypeDecl(SingleVariableDeclarationStatement decl)
		{
			if (decl.InitExp == null) return null;

			return new AssignmentExpression( new VariableReferenceExpression(decl.Identifier), decl.InitExp );
		} 

		public override bool VisitAssignmentExpression(AssignmentExpression assignExp)
		{
			return base.VisitAssignmentExpression(assignExp);
		}

//		public override bool VisitVariableReferenceExpression(VariableReferenceExpression variableReferenceExpression)
//		{
//			if (variableReferenceExpression.Type == VariableReferenceType.LocalOrArgument)
//			{
//				if (!namescope.IsDefined(variableReferenceExpression.Name))
//				{
//					errorReport.Error( "TODOFILENAME", variableReferenceExpression.Position, 
//						"'{0}' is undefined. You can defined it through a formal declaration - '{0}':sometype - or just an " + 
//						"assignment ('{0} = something').", variableReferenceExpression.Name );
//				}
//			}
//			else 
//			{
//				if (!namescope.IsDefinedInParent(variableReferenceExpression.Name))
//				{
//					errorReport.Error( "TODOFILENAME", variableReferenceExpression.Position, 
//						"'{0}' is undefined. You can defined it through a formal declaration - '{0}':sometype, an assignment " + 
//						"('{0} = something'), or using the attr family.", variableReferenceExpression.Name );
//				}
//			}
//
//			return base.VisitVariableReferenceExpression(variableReferenceExpression);
//		}

		public override bool VisitMemberAccessExpression(MemberAccessExpression accessExpression)
		{
			return base.VisitMemberAccessExpression(accessExpression);
		}
	}
}
