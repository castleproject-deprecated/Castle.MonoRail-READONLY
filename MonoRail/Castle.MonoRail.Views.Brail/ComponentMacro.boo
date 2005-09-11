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
namespace Castle.MonoRail.Views.Brail

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Ast.Visitors
import Castle.MonoRail.Framework

class ComponentMacro(AbstractAstMacro):

	override def Expand(macro as MacroStatement):
		if macro.Arguments.Count == 0:
			raise RailsException("component must be called with a component name")
		componentName = StringLiteralExpression(macro.Arguments[0].ToString())
		dictionary as Expression
		# get the hash table or an empty one.
		if macro.Arguments.Count == 2:
			dictionary = macro.Arguments[1]
		else:
			dictionary = MethodInvocationExpression( Target: AstUtil.CreateReferenceExpression("System.Collections.Hashtable") )
		
		block = Block()
		
		# create closure for macro's body or null
		macroBody as Expression = NullLiteralExpression()
		if macro.Block.Statements.Count > 0:
			macroBody = CallableBlockExpression(Body: macro.Block)
			
		initContext =  MethodInvocationExpression( 
				Target: AstUtil.CreateReferenceExpression("Castle.MonoRail.Views.Brail.BrailViewComponentContext") )
		initContext.Arguments.Extend( (macroBody, componentName, AstUtil.CreateReferenceExpression("OutputStream") , dictionary) )
		
		# compilerContext = BrailViewComponentContext(macroBodyClosure, "componentName", OutputStream, dictionary)
		block.Add(BinaryExpression(Operator: BinaryOperatorType.Assign,
			Left: ReferenceExpression("componentContext"),
			Right: initContext ))
		
		# AddProperties( compilerContext.ContextVars )
		mie = MethodInvocationExpression( Target: AstUtil.CreateReferenceExpression("AddProperties") )
		mie.Arguments.Add( AstUtil.CreateReferenceExpression("componentContext.ContextVars") )
		block.Add( mie )
		
		# component = viewEngine.ViewComponentFactory.Create("componentName")
		createComponent = MethodInvocationExpression( 
				Target: AstUtil.CreateReferenceExpression("viewEngine.ViewComponentFactory.Create"))
		createComponent.Arguments.Add( componentName )
		block.Add(BinaryExpression(Operator: BinaryOperatorType.Assign,
			Left: ReferenceExpression("component"),
			Right: createComponent ) )
		
		# component.Init(context, componentContext)
		initComponent = MethodInvocationExpression( 
			Target: AstUtil.CreateReferenceExpression("component.Init"))
		initComponent.Arguments.Extend( (AstUtil.CreateReferenceExpression("context"), ReferenceExpression("componentContext")) )
		
		block.Add(initComponent )
		
		# component.Render()
		block.Add(MethodInvocationExpression( 
			Target: AstUtil.CreateReferenceExpression("component.Render") ) )
		
		# if component.ViewToRender is not null:
		#	OutputSubView("/"+component.ViewToRender)
		renderView = Block() 
		outputSubView = MethodInvocationExpression( 
			Target: AstUtil.CreateReferenceExpression("OutputSubView"))
		outputSubView.Arguments.Add(BinaryExpression(Operator: BinaryOperatorType.Addition,
			Left: StringLiteralExpression('/'),
			Right: AstUtil.CreateReferenceExpression("componentContext.ViewToRender") ) )
		
		renderView.Add( outputSubView )
		
		block.Add( IfStatement(Condition: AstUtil.CreateReferenceExpression("componentContext.ViewToRender"),
			TrueBlock: renderView) )
		
		return block
