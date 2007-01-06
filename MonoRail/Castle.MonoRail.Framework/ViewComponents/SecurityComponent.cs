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

namespace Castle.MonoRail.Framework.ViewComponents
{
	using System;

	/// <summary>
	/// Only renders the body if the current user has the specified role
	/// <code>
	/// #blockcomponent(SecurityComponent with "role=IsAdmin")
	///		Content only available to admin
	/// #end
	/// </code>
	/// </summary>
	public class SecurityComponent : ViewComponent
	{
		private bool shouldRender;

		/// <summary>
		/// Called by the framework once the component instance
		/// is initialized
		/// </summary>
		public override void Initialize()
		{
			String role = (String) ComponentParams["role"];

			if (role == null) throw new RailsException("SecurityComponent: you must supply a role parameter");

			shouldRender = RailsContext.CurrentUser != null && RailsContext.CurrentUser.IsInRole(role);
		}

		/// <summary>
		/// Called by the framework so the component can
		/// render its content
		/// </summary>
		public override void Render()
		{
			if (shouldRender)
			{
				Context.RenderBody();
			}
		}
	}
}
