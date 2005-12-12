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

	/// <summary>
	/// Utilities methods to inspect the controller Type
	/// and gathers its name and area.
	/// </summary>
	public abstract class ControllerInspectionUtil
	{
		public static ControllerDescriptor Inspect(Type controllerType)
		{
			if (controllerType.IsDefined(typeof(ControllerDetailsAttribute), true))
			{
				object[] attrs = controllerType.GetCustomAttributes( 
					typeof(ControllerDetailsAttribute), true );

				ControllerDetailsAttribute details = attrs[0] as ControllerDetailsAttribute;

				return new ControllerDescriptor( controllerType, 
					ObtainControllerName(details.Name, controllerType), 
						details.Area );
			}
			else
			{
				return new ControllerDescriptor( controllerType, 
					ObtainControllerName(null, controllerType), String.Empty );
			}
		}

		private static String ObtainControllerName(String name, Type controller)
		{
			if (name == null || name.Length == 0)
			{
				return Strip(controller.Name);
			}
			return name;
		}

		private static String Strip(String name)
		{
			if (name.EndsWith("Controller"))
			{
				return name.Substring(0, name.IndexOf("Controller"));
			}
			return name;
		}
	}
}
