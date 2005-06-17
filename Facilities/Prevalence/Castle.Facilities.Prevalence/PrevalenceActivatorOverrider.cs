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

namespace Castle.Facilities.Prevalence
{
	using System;

	using Bamboo.Prevalence;

	using Castle.MicroKernel;
	using Castle.MicroKernel.ModelBuilder;

	/// <summary>
	/// This inspector registers custom activators for PrevalenceEngines and
	/// PrevalenceSystem.
	/// </summary>
	public class PrevalenceActivatorOverriderModelInspector : IContributeComponentModelConstruction
	{
		public PrevalenceActivatorOverriderModelInspector()
		{
		}

		public void ProcessModel(IKernel kernel, Castle.Model.ComponentModel model)
		{
			if (model.Implementation == typeof(PrevalenceEngine))
			{
				model.CustomComponentActivator = typeof(PrevalenceEngineComponentActivator);
			}
			else
			{
				object value = model.ExtendedProperties[ PrevalenceFacility.EngineIdPropertyKey ];

				if (value != null)
				{
					model.CustomComponentActivator = typeof(PrevalenceSystemComponentActivator);
				}
			}
		}
	}
}
