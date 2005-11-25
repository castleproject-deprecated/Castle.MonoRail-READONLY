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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Resources;

	/// <summary>
	/// Simple facade that provides the IResource interface to a
	/// ResourceSet instance.
	/// </summary>
	public class ResourceFacade : IResource
	{
		private readonly ResourceSet resourceSet;

		public ResourceFacade(ResourceSet resourceSet)
		{
			this.resourceSet = resourceSet;
		}

		public object this[String key]
		{
			get { return GetObject( key ); }
		}

		public String GetString(String key)
		{
			return key != null ? resourceSet.GetString(key, true) : null;
		}

		public object GetObject(String key)
		{
			return key != null ? resourceSet.GetObject(key, true) : null;
		}

		public void Dispose()
		{
			resourceSet.Dispose();
		}
	}
}
