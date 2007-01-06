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

namespace TestSiteWindsor
{
	using System.Web;

	using Castle.Core.Resource;
	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;

	public class MyHttpApplication : HttpApplication, IContainerAccessor
	{
		private static WindsorContainer container;

		public void Application_OnStart() 
		{
			container = new WindsorContainer(new XmlInterpreter(new ConfigResource()));

			// container.AddFacility( "rails", new RailsFacility() );

			// AddControllers(container);
		}

//		private void AddControllers(WindsorContainer container)
//		{
//			container.AddComponent( "home", typeof(HomeController) );
//			container.AddComponent( "mycomp", typeof(MyComponent) );
//		}

		public void Application_OnEnd() 
		{
			container.Dispose();
		}

		#region IContainerAccessor implementation

		public IWindsorContainer Container
		{
			get { return container; }
		}

		#endregion
	}
}
