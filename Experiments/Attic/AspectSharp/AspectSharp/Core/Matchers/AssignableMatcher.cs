using AspectSharp.Lang.AST;
// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace AspectSharp.Core.Matchers
{
	using System;

	/// <summary>
	/// Summary description for AssignableMatcher.
	/// </summary>
	public class AssignableMatcher : SingleTypeMatcher
	{
		private static readonly AssignableMatcher _instance = new AssignableMatcher();

		protected AssignableMatcher()
		{
		}

		public new static AssignableMatcher Instance
		{
			get { return _instance; }
		}

		protected override Type GetTypeToCompare(AspectDefinition aspect)
		{
			return aspect.TargetType.AssignType.ResolvedType;
		}
	}
}
