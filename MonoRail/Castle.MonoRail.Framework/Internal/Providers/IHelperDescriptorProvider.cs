// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	/// Defines the contract to an implementation 
	/// that wish to create <see cref="HelperDescriptor"/>.
	/// </summary>
	/// <remarks>
	/// The default implementation creates the descriptors
	/// based on <see cref="HelperAttribute"/> associated
	/// with the controller
	/// </remarks>
	public interface IHelperDescriptorProvider : IProvider
	{
		/// <summary>
		/// Implementors should collect the helper information
		/// and return descriptors instances, or an empty array if none 
		/// was found.
		/// </summary>
		/// <param name="controllerType">The controller type</param>
		/// <returns>An array of <see cref="HelperDescriptor"/></returns>
		HelperDescriptor[] CollectHelpers(Type controllerType);
	}
}
