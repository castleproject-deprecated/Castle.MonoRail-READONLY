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

namespace Castle.MonoRail.WindsorExtension
{
	using System;
	using System.Web;

	using Castle.Windsor;

	using Castle.MonoRail.Framework;


	/// <summary>
	/// Uses the HttpContext and the <see cref="IContainerAccessor"/> 
	/// to access the container instance.
	/// </summary>
	public abstract class ContainerAccessorUtil
	{
		public static IWindsorContainer ObtainContainer()
		{
			IContainerAccessor containerAccessor = 
				HttpContext.Current.ApplicationInstance as IContainerAccessor;
	
			if (containerAccessor == null)
			{
				throw new RailsException("You must extend the HttpApplication in your web project " + 
					"and implement the IContainerAccessor to properly expose your container instance");
			}
	
			IWindsorContainer container = containerAccessor.Container;
	
			if (container == null)
			{
				throw new RailsException("The container seems to be unavailable in " + 
					"your HttpApplication subclass");
			}

			return container;
		}
	}
}
