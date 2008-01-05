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

namespace Castle.MicroKernel.Handlers
{
	using System;
	using System.Collections;

	/// <summary>
	/// Might be implemented by a handler 
	/// so it can expose access to dependency information 
	/// which is used to construct meaningful error messages
	/// </summary>
	public interface IExposeDependencyInfo
	{
		/// <summary>
		/// Returns human readable list of dependencies 
		/// this handler is waiting for.
		/// <param name="dependenciesChecked">list of the dependecies that was already checked, used to avoid cycles.</param>
		/// </summary>
		String ObtainDependencyDetails(IList dependenciesChecked);
	}
}
