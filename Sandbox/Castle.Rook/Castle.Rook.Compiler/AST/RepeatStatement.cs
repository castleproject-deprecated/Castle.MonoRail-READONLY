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


	public enum RepeatType
	{
		Until,
		While
	}

	public class RepeatStatement : AbstractStatement
	{
		private StatementCollection statements;
		private RepeatType type;
		private IExpression conditionExp;

		public RepeatStatement(RepeatType type, IExpression conditionExp)
		{
			statements = new StatementCollection(this);
			this.type = type;
			this.conditionExp = conditionExp;
		}

		public RepeatType Type
		{
			get { return type; }
		}

		public StatementCollection Statements
		{
			get { return statements; }
		}

		public IExpression ConditionExp
		{
			get { return conditionExp; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitRepeatStatement(this);
		}
	}
}
