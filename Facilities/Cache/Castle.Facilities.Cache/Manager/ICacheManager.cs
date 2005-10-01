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

using System.Collections;

namespace Castle.Facilities.Cache.Manager
{
	/// <summary>
	/// Description r�sum�e de ICacheManager.
	/// </summary>
	public interface ICacheManager
	{
		#region Properties
		/// <summary>
		/// Adds an item with the specified key and value into cached data.
		/// Gets a cached object with the specified key.
		/// </summary>
		/// <value>The cached object or <c>null</c></value>
		object this [object key] 
		{
			get;
			set;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Clears all elements from the cache.
		/// </summary>
		void Flush ();

		/// <summary>
		/// Configures the CacheController
		/// </summary>
		/// <param name="properties"></param>
		void Configure(IDictionary properties);
		#endregion
	}
}
