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

	using Castle.Model;

	using Castle.MicroKernel;

	using Castle.Model.Configuration;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Internal.Graph;
	using Castle.MonoRail.Framework.Controllers;


	/// <summary>
	/// Facility responsible for registering the controllers in
	/// the tree.
	/// </summary>
	public class RailsFacility : IFacility
	{
		private ControllerTree _tree;

		public RailsFacility()
		{
		}

		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			kernel.AddComponent( "rails.controllertree", typeof(ControllerTree) );

			_tree = (ControllerTree) kernel["rails.controllertree"];

			kernel.ComponentModelCreated += new ComponentModelDelegate(OnComponentModelCreated);

			AddBuitInControllers(kernel);
		}

		public void Terminate()
		{
		}

		protected virtual void AddBuitInControllers(IKernel kernel)
		{
			kernel.AddComponent("files", typeof(FilesController), typeof(FilesController));
		}

		private void OnComponentModelCreated(ComponentModel model)
		{
			bool isController = typeof(Controller).IsAssignableFrom(model.Implementation);

			if ( !isController && !typeof(ViewComponent).IsAssignableFrom(model.Implementation) )
			{
				return;
			}

			// Ensure it's transient
			model.LifestyleType = LifestyleType.Transient;

			if (isController)
			{
				ControllerDescriptor descriptor = ControllerInspectionUtil.Inspect(model.Implementation);
			
				_tree.AddController( descriptor.Area, descriptor.Name, model.Name );
			}
		}
	}
}
