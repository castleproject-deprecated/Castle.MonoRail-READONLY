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

namespace PetStore.Web
{
	using System;
	using System.Web;

	using Castle.ActiveRecord;
	
	using Castle.Model.Resource;

	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;


	public class GlobalApplication : HttpApplication, IContainerAccessor
	{
		private static WindsorContainer container;

		public GlobalApplication()
		{
			this.BeginRequest += new EventHandler(OnBeginRequest);
			this.EndRequest += new EventHandler(OnEndRequest);
		}

		public void Application_OnStart()
		{
			container = new WindsorContainer( new XmlInterpreter(new ConfigResource()) );
		}

		public void Application_OnEnd() 
		{
			container.Dispose();
		}

		public void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		public IWindsorContainer Container
		{
			get { return container; }
		}

		public void OnBeginRequest(object sender, EventArgs e)
		{
			HttpContext.Current.Items.Add( "nh.sessionscope", new SessionScope() );
		}

		public void OnEndRequest(object sender, EventArgs e)
		{
			SessionScope scope = (SessionScope) HttpContext.Current.Items["nh.sessionscope"];

			try
			{
				if (scope != null) scope.Dispose();
			}
			catch(Exception ex)
			{
				HttpContext.Current.Trace.Warn( "Error", "Problems with the session:" + ex.Message, ex );
			}
		}
	}
}
