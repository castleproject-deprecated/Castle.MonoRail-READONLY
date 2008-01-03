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

namespace Castle.MonoRail.Framework
{
	/// <summary>
	/// Delegate to create dynamic actions without the need for a separated class.
	/// </summary>
	public delegate object ActionDelegate(IEngineContext engineContext, IController controller, IControllerContext controllerContext);

	/// <summary>
	/// Represents a dynamic action that forwards the
	/// call to an <see cref="ActionDelegate"/>
	/// </summary>
	public class DelegateDynamicAction : IDynamicAction
	{
		private readonly ActionDelegate actionDelegate;

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateDynamicAction"/> class.
		/// </summary>
		/// <param name="actionDelegate">The action delegate.</param>
		public DelegateDynamicAction(ActionDelegate actionDelegate)
		{
			this.actionDelegate = actionDelegate;
		}

		/// <summary>
		/// Implementors should perform the action
		/// upon this invocation
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			return actionDelegate(engineContext, controller, controllerContext);
		}
	}
}
