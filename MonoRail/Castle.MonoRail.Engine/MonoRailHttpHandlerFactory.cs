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

	/// <summary>
	/// Coordinates the creation of new <see cref="MonoRailHttpHandler"/> 
	/// and uses the configuration to obtain the correct factories 
	/// instances.
	/// </summary>
	public class MonoRailHttpHandlerFactory : ProcessEngineFactory, IHttpHandlerFactory, IRequiresSessionState
	{
		public MonoRailHttpHandlerFactory()
		{
		}

		public virtual IHttpHandler GetHandler(HttpContext context, 
			String requestType, String url, String pathTranslated)
		{
			return new MonoRailHttpHandler(url, _viewEngine, _controllerFactory, 
				_filterFactory, _resourceFactory, _scaffoldingSupport, _viewCompFactory);
		}

		public virtual void ReleaseHandler(IHttpHandler handler)
		{
		}
	}
}
