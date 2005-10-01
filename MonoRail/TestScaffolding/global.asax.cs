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

namespace TestScaffolding 
{
	using System;
	using System.Configuration;
	
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;

	using TestScaffolding.Model;

	public class MyHttpApplication : System.Web.HttpApplication
	{
		private System.ComponentModel.IContainer components = null;

		public MyHttpApplication()
		{
			InitializeComponent();
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{
			IConfigurationSource source = 
				ConfigurationSettings.GetConfig("activerecord") as IConfigurationSource;

			ActiveRecordStarter.Initialize( source, 
			  typeof(Blog), typeof(Person), typeof(Customer), typeof(Account), 
			  typeof(AccountPermission), typeof(ProductLicense) );
		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_Error(Object sender, EventArgs e)
		{

		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{

		}
			
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.components = new System.ComponentModel.Container();
		}
		#endregion
	}
}

