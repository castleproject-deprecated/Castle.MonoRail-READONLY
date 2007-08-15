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

namespace Castle.MicroKernel.Util
{
	using System;
	using System.Collections;

	/// <summary>
	/// Compares if the reference of two objects are equals.
	/// </summary>
	[Serializable]
	public class ReferenceComparer : IComparer
	{
		public ReferenceComparer()
		{
		}

		public int Compare(object x, object y)
		{
			return object.ReferenceEquals(x, y) ? 0 : 1;
		}
	}

	[Serializable]
	public class ReferenceEqualityComparer : IEqualityComparer
	{
		public new bool Equals(object x, object y)
		{
			return ReferenceEquals(x, y);
		}

		public int GetHashCode(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			return obj.GetHashCode();
		}
	}
}
