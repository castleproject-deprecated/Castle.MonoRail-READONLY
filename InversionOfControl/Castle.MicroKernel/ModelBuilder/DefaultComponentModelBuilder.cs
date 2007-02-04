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

namespace Castle.MicroKernel.ModelBuilder
{
	using System;
	using System.Collections;

	using Castle.Core;
	using Castle.MicroKernel.ModelBuilder.Inspectors;

	/// <summary>
	/// Summary description for DefaultComponentModelBuilder.
	/// </summary>
	[Serializable]
	public class DefaultComponentModelBuilder : IComponentModelBuilder
	{
		private readonly IKernel kernel;
		private readonly ArrayList contributors;

		public DefaultComponentModelBuilder(IKernel kernel)
		{
			this.kernel = kernel;
			this.contributors = new ArrayList();

			InitializeContributors();
		}

		public ComponentModel BuildModel(String key, Type service, Type classType, IDictionary extendedProperties)
		{
			ComponentModel model = new ComponentModel(key, service, classType);
			
			if (extendedProperties != null)
			{
				model.ExtendedProperties = extendedProperties;
			}

			foreach(IContributeComponentModelConstruction contributor in contributors)
			{
				contributor.ProcessModel( kernel, model );
			}
			
			return model;
		}

		public IContributeComponentModelConstruction[] Contributors
		{
			get
			{
				return (IContributeComponentModelConstruction[])
					contributors.ToArray(typeof(IContributeComponentModelConstruction));
			}
		}

		public void AddContributor(IContributeComponentModelConstruction contributor)
		{
			contributors.Add(contributor);
		}

		public void RemoveContributor(IContributeComponentModelConstruction contributor)
		{
			contributors.Remove(contributor);
		}

		protected virtual void InitializeContributors()
		{
#if DOTNET2
			AddContributor(new GenericInspector());
#endif
			AddContributor(new ConfigurationModelInspector());
			AddContributor(new LifestyleModelInspector());
			AddContributor(new ConstructorDependenciesModelInspector());
			AddContributor(new PropertiesDependenciesModelInspector());
			AddContributor(new LifecycleModelInspector());
			AddContributor(new ConfigurationParametersInspector());
			AddContributor(new InterceptorInspector());
			AddContributor(new ComponentActivatorInspector());
		}
	}
}
