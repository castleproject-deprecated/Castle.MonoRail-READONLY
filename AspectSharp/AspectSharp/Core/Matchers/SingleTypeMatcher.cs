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

namespace AspectSharp.Core.Matchers
{
	using System;

	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for SingleTypeMatcher.
	/// </summary>
	public class SingleTypeMatcher : IClassMatcher
	{
		private static readonly SingleTypeMatcher _instance = new SingleTypeMatcher();

		protected SingleTypeMatcher()
		{
		}

		public static SingleTypeMatcher Instance
		{
			get { return _instance; }
		}

		public bool Match(Type targetType, AspectDefinition aspect)
		{
			Type type = GetTypeToCompare(aspect);

			if ( type.IsAssignableFrom( targetType ) )
			{
				return true;
			}

			return false;
		}

		protected virtual Type GetTypeToCompare(AspectDefinition aspect)
		{
			return aspect.TargetType.SingleType.ResolvedType;
		}
	}
}
