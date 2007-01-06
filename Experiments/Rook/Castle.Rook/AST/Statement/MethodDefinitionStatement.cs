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
	using System.Collections;


	public class MethodDefinitionStatement : Statement
	{
		private Identifier returnType;
		private String name;
		private String boundTo;
		private IList parameters = new ArrayList();
		private IList statements = new ArrayList();
		private AccessLevel scopeAccessLevel;

		public MethodDefinitionStatement(AccessLevel scopeAccessLevel, String[] nameParts)
		{
			this.scopeAccessLevel = scopeAccessLevel;

			if (nameParts[1] == null)
			{
				name = nameParts[0];
			}
			else
			{
				boundTo = nameParts[0]; // self, interface and so on
				name = nameParts[1];
			}
		}

		public IList Parameters
		{
			get { return parameters; }
		}

		public IList Statements
		{
			get { return statements; }
		}

		public Identifier ReturnType
		{
			get { return returnType; }
			set { returnType = value; }
		}

		public string Name
		{
			get { return name; }
		}

		public string BoundTo
		{
			get { return boundTo; }
		}

		public AccessLevel ScopeAccessLevel
		{
			get { return scopeAccessLevel; }
		}
	}
}
