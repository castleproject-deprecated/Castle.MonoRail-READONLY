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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Helper used to create <see cref="IDictionary"/> instances
	/// </summary>
	public class DictHelper : AbstractHelper
	{
		/// <summary>
		/// Creates an <see cref="IDictionary"/> with entries
		/// infered from the arguments. 
		/// <code>
		/// CreateDict( "style=display: none;", "selected" )
		/// </code>
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public IDictionary CreateDict(params String[] args)
		{
			return Create(args);
		}

		/// <summary>
		/// Creates a dictionary from specified arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public static IDictionary Create(params String[] args)
		{
			IDictionary dict = new HybridDictionary(true);

			foreach(String arg in args)
			{
				String[] parts = arg.Split('=');

				if (parts.Length == 1)
				{
					dict[arg] = "";
				}
				else if (parts.Length == 2)
				{
					dict[ parts[0] ] = parts[1];
				}
				else
				{
					dict[ parts[0] ] = String.Join("=", parts, 1, parts.Length - 1);
				}
			}

			return dict;
		}

		/// <summary>
		/// Creates a dictionary fros a name value collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <returns></returns>
		public IDictionary FromNameValueCollection(NameValueCollection collection)
		{
			IDictionary dict = new HybridDictionary(true);

			foreach(string key in collection.AllKeys)
			{
				if (key == null) continue;
				
				dict[key] = collection[key];
			}
			
			return dict;
		}
	}
}
