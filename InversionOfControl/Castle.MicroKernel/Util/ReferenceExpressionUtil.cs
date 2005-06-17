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

namespace Castle.MicroKernel.Util
{
	using System;

	/// <summary>
	/// Summary description for ReferenceExpressionUtil.
	/// </summary>
	internal abstract class ReferenceExpressionUtil
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsReference(String value)
		{
			if (value == null || value.Length <= 3 || 
				!value.StartsWith("${") || !value.EndsWith("}"))
			{
				return false;
			}

			return true;
		}

		public static String ExtractComponentKey(String value)
		{
			if (IsReference(value))
			{
				return value.Substring( 2, value.Length - 3 );
			}

			return null;
		}
	}
}
