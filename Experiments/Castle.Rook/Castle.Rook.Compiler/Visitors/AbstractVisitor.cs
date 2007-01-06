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

namespace Castle.Rook.Compiler.Visitors
{
	using System;
	using System.Collections;

	using Castle.Rook.Compiler.AST;


	public abstract class AbstractVisitor : IASTVisitor
	{
		public AbstractVisitor()
		{
		}

		public virtual bool VisitNode(IVisitableNode node)
		{
			if (node == null) return true;

			bool res = node.Accept(this);

			return res;
		}

		public virtual bool VisitNodes(IList nodes)
		{
//			String message = nodes.ToString();
//			System.Diagnostics.Debug.WriteLine("before " + message);

			for(int i=0; i < nodes.Count; i++)
			{
				VisitNode( (IVisitableNode) nodes[i] );
			}

//			message = nodes.ToString();
//			System.Diagnostics.Debug.WriteLine("after " + message);

			return true;
		}

		public virtual void VisitCompilationUnit(CompilationUnit compilationUnit)
		{
			VisitNodes(compilationUnit.SourceUnits);
		}

		public virtual bool VisitSourceUnit(SourceUnit unit)
		{
			VisitNodes(unit.Namespaces);
			VisitNodes(unit.Statements);

			return true;
		}

		public virtual bool VisitNamespace(NamespaceDeclaration ns)
		{
			if (VisitEnter(ns))
			{
				VisitNodes(ns.TypeDeclarations);

				return VisitLeave(ns);
			}

			return true;
		}

		public virtual bool VisitEnter(NamespaceDeclaration ns)
		{
			return true;
		}

		public virtual bool VisitLeave(NamespaceDeclaration ns)
		{
			return true;
		}

		public virtual bool VisitTypeDefinitionStatement(TypeDefinitionStatement typeDef)
		{
			if (VisitEnter(typeDef))
			{
//				VisitNodes(typeDef.Fields);
//				VisitNodes(typeDef.Constructors);
//				VisitNodes(typeDef.Methods);
				VisitNodes(typeDef.Statements);

				return VisitLeave(typeDef);
			}

			return false;
		}

		public virtual bool VisitEnter(TypeDefinitionStatement typeDef)
		{
			return true;
		}

		public virtual bool VisitLeave(TypeDefinitionStatement typeDef)
		{
			return true;
		}

		public virtual bool VisitMethodDefinitionStatement(MethodDefinitionStatement methodDef)
		{
			if (VisitEnter(methodDef))
			{
				VisitNodes(methodDef.Parameters);
				VisitNode(methodDef.ReturnType);
				VisitNodes(methodDef.Statements);

				return VisitLeave(methodDef);
			}

			return false;
		}

		public virtual bool VisitEnter(MethodDefinitionStatement methodDef)
		{
			return true;
		}

		public virtual bool VisitLeave(MethodDefinitionStatement methodDef)
		{
			return true;
		}

		//
		// References
		//

		public virtual bool VisitTypeReference(TypeReference reference)
		{
			return true;
		}

		public virtual bool VisitIdentifier(Identifier identifier)
		{
			VisitNode(identifier.TypeReference);

			return true;
		}

		public virtual bool VisitParameterIdentifier(ParameterIdentifier parameterIdentifier)
		{
			VisitNode(parameterIdentifier.InitExpression);
			VisitNode(parameterIdentifier.TypeReference);

			return true;
		}

		//
		// Statements
		//

		public virtual bool VisitIfStatement(IfStatement ifStatement)
		{
			VisitNode(ifStatement.Condition);
			VisitNodes(ifStatement.TrueStatements);
			VisitNodes(ifStatement.FalseStatements);
			return true;
		}

		public virtual bool VisitForStatement(ForStatement statement)
		{
			VisitNode(statement.EvalExp);
			VisitNodes(statement.Statements);

			return true;
		}

		public virtual bool VisitExpressionStatement(ExpressionStatement statement)
		{
			VisitNode(statement.Expression);

			return true;
		}

		public virtual bool VisitMultipleVariableDeclarationStatement(MultipleVariableDeclarationStatement varDecl)
		{
			VisitNodes(varDecl.Identifiers);
			
			// TODO: Decide if we shall visit the InitExpressions as this node is a kind of transient node

			return true;
		}

		public virtual bool VisitRepeatStatement(RepeatStatement statement)
		{
			VisitNode(statement.ConditionExp);
			VisitNodes(statement.Statements);

			return true;
		}

		public virtual bool VisitPostfixCondition(PostfixCondition postfixCondition)
		{
			VisitNode(postfixCondition.Condition);

			return true;
		}

		public virtual bool VisitReturnStatement(ReturnStatement statement)
		{
			return true;
		}

		public virtual bool VisitRequireStatement(RequireStatement statement)
		{
			return true;
		}

		//
		// Expressions
		//

		public virtual bool VisitAssignmentExpression(AssignmentExpression assignExp)
		{
			VisitNode(assignExp.Target);
			VisitNode(assignExp.Value);
			VisitExpression(assignExp);

			return true;
		}

		public virtual bool VisitAugAssignmentExpression(AugAssignmentExpression auAssignExp)
		{
			VisitNode(auAssignExp.Target);
			VisitNode(auAssignExp.Value);
			VisitExpression(auAssignExp);

			return true;
		}

		public virtual bool VisitYieldExpression(YieldExpression yieldExpression)
		{
			VisitExpression(yieldExpression);
			return true;
		}

		public virtual bool VisitVariableReferenceExpression(VariableReferenceExpression variableReferenceExpression)
		{
			VisitNode(variableReferenceExpression.Identifier);
			VisitExpression(variableReferenceExpression);
			return true;
		}

		public virtual bool VisitUnaryExpression(UnaryExpression unaryExpression)
		{
			VisitNode(unaryExpression.Inner);
			VisitExpression(unaryExpression);
			return true;
		}

		public virtual bool VisitSingleVariableDeclarationStatement(SingleVariableDeclarationStatement varDecl)
		{
			VisitNode(varDecl.InitExp);
			VisitNode(varDecl.DependencyExpression);
			VisitNode(varDecl.Identifier);

			return true;
		}

		public virtual bool VisitRetryExpression(RetryExpression expression)
		{
			VisitExpression(expression);

			return true;
		}

		public virtual bool VisitNextExpression(NextExpression expression)
		{
			VisitExpression(expression);

			return true;
		}

		public virtual bool VisitRedoExpression(RedoExpression expression)
		{
			VisitExpression(expression);

			return true;
		}

		public virtual bool VisitRangeExpression(RangeExpression expression)
		{
			VisitNode(expression.LeftHandSide);
			VisitNode(expression.RightHandSide);
			VisitExpression(expression);

			return true;
		}

		public virtual bool VisitRaiseExpression(RaiseExpression expression)
		{
			VisitNode(expression.Inner);
			VisitExpression(expression);

			return true;
		}

		public virtual bool VisitMethodInvocationExpression(MethodInvocationExpression invocationExpression)
		{
			VisitNode(invocationExpression.Target);
			VisitNodes(invocationExpression.Arguments);
			VisitExpression(invocationExpression);

			return true;
		}

		public virtual bool VisitMemberAccessExpression(MemberAccessExpression accessExpression)
		{
			VisitNode(accessExpression.Target);
			VisitExpression(accessExpression);

			return true;
		}

		public virtual bool VisitLiteralReferenceExpression(LiteralReferenceExpression expression)
		{
			VisitExpression(expression);

			return true;
		}

		public virtual bool VisitListExpression(ListExpression expression)
		{
			VisitNodes(expression.Items);
			VisitExpression(expression);

			return true;
		}

		public virtual bool VisitLambdaExpression(LambdaExpression expression)
		{
			VisitNode(expression.Block);
			VisitExpression(expression);

			return true;
		}

		public virtual bool VisitDictExpression(DictExpression expression)
		{
			// TODO: Expose keys and values expressions

			VisitExpression(expression);

			return true;
		}

		public virtual bool VisitCompoundExpression(CompoundExpression expression)
		{
			VisitNodes(expression.Statements);
			VisitExpression(expression);

			return true;
		}

		public virtual bool VisitBreakExpression(BreakExpression breakExpression)
		{
			VisitExpression(breakExpression);

			return true;
		}

		public virtual bool VisitBlockExpression(BlockExpression expression)
		{
			VisitNodes(expression.Parameters);
			VisitNodes(expression.Statements);
			VisitExpression(expression);

			return true;
		}

		public virtual bool VisitBinaryExpression(BinaryExpression expression)
		{
			VisitNode(expression.LeftHandSide);
			VisitNode(expression.RightHandSide);
			VisitExpression(expression);

			return true;
		}

		public bool VisitBaseReferenceExpression(BaseReferenceExpression expression)
		{
			return true;
		}

		public bool VisitSelfReferenceExpression(SelfReferenceExpression expression)
		{
			return true;
		}

		public bool VisitNullCheckExpression(NullCheckExpression expression)
		{
			VisitNode(expression.Expression);
			VisitExpression(expression);

			return true;
		}

		private void VisitExpression(IExpression exp)
		{
			VisitNode(exp.PostFixStatement);
		}
	}
}
