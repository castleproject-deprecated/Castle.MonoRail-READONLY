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

namespace AuthenticationUsingFilters.Filters
{
	using System.Collections.Specialized;
	using AuthenticationUsingFilters.Model;
	
	using Castle.MonoRail.Framework;

	
	public class AuthenticationFilter : IFilter
	{
		public bool Perform(ExecuteWhen exec, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			// Read previous authenticated principal from session 
			// (could be from cookie although with more work)
			
			User user = (User) context.Session["user"];
			
			// Sets the principal as the current user
			context.CurrentUser = user;
			
			// Checks if it is OK
			if (context.CurrentUser == null || !context.CurrentUser.Identity.IsAuthenticated)
			{
				// Not authenticated, redirect to login
				NameValueCollection parameters = new NameValueCollection();
				parameters.Add("ReturnUrl", context.UrlInfo.UrlRaw);
				context.Response.Redirect("login", "index", parameters);
				
				// Prevent request from continue
				return false;
			}
			
			// Everything is ok
			return true;
		}
	}
}
