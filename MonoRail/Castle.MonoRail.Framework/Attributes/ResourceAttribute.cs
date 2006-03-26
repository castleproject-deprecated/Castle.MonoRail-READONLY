// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Declares that for the specified class or method, the given resource file should be 
	/// loaded and set available in the PropertyBag with the specified name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=true, Inherited=true), Serializable]
	public class ResourceAttribute : Attribute, IResourcesAttribute
	{
		private ResourceItem[] resources;
		
		/// <summary>
		/// Constructs a resource attribute, with the specified name, based
		/// on the resource in a satellite assembly.
		/// </summary>
		/// <param name="name">Name the resource will be available as in the PropertyBag</param>
		/// <param name="resourceName">Fully qualified name of the resource in the sattelite assembly</param>
		public ResourceAttribute( String name, String resourceName )
		{
			ResourceItem res;
			
			if (resourceName.IndexOf(',') > 0)
			{
				String[] pair = resourceName.Split(',');
				res = new ResourceItem(name, pair[0].Trim());
				res.AssemblyName = pair[1].Trim();
			}
			else
				res = new ResourceItem(name, resourceName);

			this.resources = new ResourceItem[] { res };
		}

		public ResourceItem[] GetResources()
		{
			return resources;
		}

		public String Name
		{
			get { return resources[0].Key; }
		}

		public String ResourceName
		{
			get { return resources[0].ResourceName; }
		}

		public String CultureName
		{
			get { return resources[0].CultureName; }
			set { resources[0].CultureName = value; }
		}

		public String AssemblyName
		{
			get { return resources[0].AssemblyName; }
			set { resources[0].AssemblyName = value; }
		}

		public Type ResourceType
		{
			get { return resources[0].ResourceType; }
			set { resources[0].ResourceType = value; }
		}
	}
}
