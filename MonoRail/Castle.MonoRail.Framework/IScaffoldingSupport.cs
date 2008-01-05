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

namespace Castle.MonoRail.Framework
{
	/// <summary>
	/// Abstracts the implementation of scaffolding support for MonoRail.
	/// <para>
	/// Implementors should use information from the controller instance
	/// and add dynamic actions to it exposing operations to manipulate an entity.
	/// </para>
	/// </summary>
	public interface IScaffoldingSupport
	{
		/// <summary>
		/// Implementors should use this method to read information
		/// from the controller instance and add dynamic actions to the controller.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		void Process(IEngineContext context, IController controller, IControllerContext controllerContext);
	}
}
