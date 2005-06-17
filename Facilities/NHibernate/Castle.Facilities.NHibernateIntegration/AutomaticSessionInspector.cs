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

namespace Castle.Facilities.NHibernateIntegration
{
	using System;

	using Castle.Model;
	
	using Castle.MicroKernel;
	using Castle.MicroKernel.ModelBuilder;

	using Castle.Facilities.NHibernateExtension;


	public class AutomaticSessionInspector : IContributeComponentModelConstruction
	{
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (model.Implementation.IsDefined( 
				typeof(UsesAutomaticSessionCreationAttribute), true ))
			{
				model.Dependencies.Add( 
					new DependencyModel( DependencyType.Service, null, typeof(AutomaticSessionInterceptor), false ) );

				model.Interceptors.Add( 
					new InterceptorReference( typeof(AutomaticSessionInterceptor) ) );
			}
		}
	}
}
