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

namespace Castle.MonoRail.Framework.Internal
{
	using System;

	/// <summary>
	/// Represents the information about a <see cref="Controller"/>.
	/// </summary>
	public class ControllerDescriptor
	{
		private Type controllerType;
		private String name;
		private String area;

		public ControllerDescriptor(Type controllerType, String name, String area)
		{
			this.controllerType = controllerType;
			this.name = name;
			this.area = area;
		}

		public Type ControllerType
		{
			get { return controllerType; }
		}

		public String Name
		{
			get { return name; }
		}

		public String Area
		{
			get { return area; }
		}
	}
}
