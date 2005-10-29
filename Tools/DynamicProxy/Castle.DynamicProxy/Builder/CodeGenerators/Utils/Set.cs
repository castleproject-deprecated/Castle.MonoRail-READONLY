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

namespace Castle.DynamicProxy.Builder.CodeGenerators
{
	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for Set.
	/// </summary>
	internal class Set : DictionaryBase
	{
		public void AddArray( object[] items )
		{
			foreach( object item in items )
			{
				Add( item );
			}
		}

		public void Add( object item )
		{
			if (!Dictionary.Contains( item ))
			{
				Dictionary.Add( item, String.Empty );
			}
		}

		public void Remove( object item )
		{
			Dictionary.Remove( item );
		}

		public Array ToArray( Type elementType )
		{
			Array array = Array.CreateInstance( elementType, Dictionary.Keys.Count );
			
			int index = 0;
			foreach( object item in Dictionary.Keys )
			{
				array.SetValue(item, index++);
			}

			return array;
		}
	}
}
