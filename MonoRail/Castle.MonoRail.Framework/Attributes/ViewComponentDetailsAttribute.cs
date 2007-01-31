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

namespace Castle.MonoRail.Framework
{
	using System;

	[AttributeUsage(AttributeTargets.Class), Serializable]
	public class ViewComponentDetailsAttribute : Attribute
	{
		private String name;

		/// <param name="name">The specified ViewComponent Name</param>
		public ViewComponentDetailsAttribute(String name)
		{
			this.name = name;
		}

		/// <summary>
		/// The component's name
		/// </summary>
		public String Name
		{
			get { return name; }
		}
	}
}
