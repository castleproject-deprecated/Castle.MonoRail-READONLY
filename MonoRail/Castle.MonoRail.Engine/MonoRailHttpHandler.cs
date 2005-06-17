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

namespace Castle.MonoRail.Engine
{
	using System;
	using System.Web;
	using System.Web.SessionState;

	using Castle.MonoRail.Engine.Adapters;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Extends the <see cref="ProcessEngine"/> and implements 
	/// <see cref="IHttpHandler"/> to dispatch the web
	/// requests. 
	/// </summary>
	public class MonoRailHttpHandler : ProcessEngine, IHttpHandler, IRequiresSessionState
	{
		private String _url;

		public MonoRailHttpHandler( String url, IViewEngine viewEngine, 
			IControllerFactory controllerFactory, IFilterFactory filterFactory, IResourceFactory resourceFactory)
			: base(controllerFactory, viewEngine, filterFactory, resourceFactory)
		{
			_url = url;
		}

		#region IHttpHandler Members

		public void ProcessRequest(HttpContext context)
		{
			Process(new RailsEngineContextAdapter(context, _url));
		}

		public bool IsReusable
		{
			get { return true; }
		}

		#endregion
	}
}
