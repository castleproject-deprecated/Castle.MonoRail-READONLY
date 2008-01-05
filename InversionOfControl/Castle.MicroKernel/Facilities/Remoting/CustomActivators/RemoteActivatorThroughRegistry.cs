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

namespace Castle.Facilities.Remoting
{
	using Castle.Core;
	
	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;


	/// <summary>
	/// Activates a client connecting to the remote server through the <see cref="RemotingRegistry"/>.
	/// </summary>
	public class RemoteActivatorThroughRegistry : DefaultComponentActivator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RemoteActivatorThroughRegistry"/> class.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="kernel">The kernel.</param>
		/// <param name="onCreation">The oncreation envent handler.</param>
		/// <param name="onDestruction">The ondestruction event handler.</param>
		public RemoteActivatorThroughRegistry(ComponentModel model, IKernel kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction) : base(model, kernel, onCreation, onDestruction)
		{
		}

		protected override object Instantiate(CreationContext context)
		{
			RemotingRegistry registry = (RemotingRegistry) 
				Model.ExtendedProperties["remoting.remoteregistry"];
			
			if (Model.Service.IsGenericType)
			{
				return registry.CreateRemoteInstance(Model.Service);
			}

			return registry.CreateRemoteInstance(Model.Name);
		}
	}
}
