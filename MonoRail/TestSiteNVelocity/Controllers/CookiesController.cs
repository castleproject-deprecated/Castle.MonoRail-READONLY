// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace TestSiteNVelocity.Controllers
{
	using System;

	using Castle.MonoRail.Framework;


	public class CookiesController : Controller
	{
		public void Index()
		{
			
		}

		public void AddCookie()
		{
			Context.Response.CreateCookie("cookiename", "value");
			Context.Response.CreateCookie("cookiename2", "value2");
			RenderView("Index");
		}

		public void AddCookieRedirect()
		{
			Context.Response.CreateCookie("cookiename", "value");
			Redirect("cookies", "index");
		}

		public void AddCookieExpiration()
		{
			DateTime twoWeeks = DateTime.Now.Add(new TimeSpan(14, 0, 0, 0));
			Context.Response.CreateCookie("cookiename2", "value", twoWeeks);
			RenderView("Index");
		}

		public void AddCookieExpirationRedirect()
		{
			DateTime twoWeeks = DateTime.Now.Add(new TimeSpan(14, 0, 0, 0));
			Context.Response.CreateCookie("cookiename2", "value", twoWeeks);
			Redirect("cookies", "index");
		}
	}
}
