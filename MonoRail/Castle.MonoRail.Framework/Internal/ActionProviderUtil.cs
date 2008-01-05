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

namespace Castle.MonoRail.Framework.Internal
{
	using System;

	/// <summary>
	/// Util class that deals with action providers
	/// </summary>
	public abstract class ActionProviderUtil
	{
		/// <summary>
		/// Registers the actions on the controller.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		public static void RegisterActions(IEngineContext engineContext, IController controller, IControllerContext context)
		{
			foreach(Type providerType in context.ControllerDescriptor.ActionProviders)
			{
				IDynamicActionProvider provider = (IDynamicActionProvider) Activator.CreateInstance(providerType);

				provider.IncludeActions(engineContext, controller, context);
			}
		}
	}
}