// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Registration
{
	using Castle.Core;
	using Castle.MicroKernel.ComponentActivator;

	public class ComponentInstanceDescriptior<S> : ComponentDescriptor<S>
	{
		private readonly object instance;

		public ComponentInstanceDescriptior(object instance)
		{
			this.instance = instance;
		}

		protected internal override void ApplyToModel(IKernel kernel, ComponentModel model)
		{
			model.CustomComponentActivator = typeof(ExternalInstanceActivator);
			model.ExtendedProperties["instance"] = instance;

			// The constructor collection is filled because the instance gets inspected 
			// by the kernel when added. The kernel doesn't need to resolve any 
			// dependencies since we pass an instance.
			model.Constructors.Clear();
		}
	}
}