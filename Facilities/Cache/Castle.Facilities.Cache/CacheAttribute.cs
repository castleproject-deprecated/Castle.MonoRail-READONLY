// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.Cache
{
	using System;

	/// <summary>
	/// Indicates the cache support for a method.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=false)]
	public class CacheAttribute : System.Attribute
	{
		string _cacheManagerId = string.Empty;

		/// <summary>
		/// Gets the cache manager id.
		/// </summary>
		/// <value>The cache manager id.</value>
		public string CacheManagerId
		{
			get { return _cacheManagerId; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CacheAttribute"/> class.
		/// </summary>
		public CacheAttribute()
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="CacheAttribute"/> class.
		/// </summary>
		/// <param name="cacheManagerId">The cache manager id.</param>
		public CacheAttribute(string cacheManagerId)
		{
			_cacheManagerId = cacheManagerId;
		}
	}
}
