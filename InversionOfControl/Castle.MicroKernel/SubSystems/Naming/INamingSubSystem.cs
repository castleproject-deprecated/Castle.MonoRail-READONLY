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

namespace Castle.MicroKernel
{
	using System;

	/// <summary>
	/// 
	/// </summary>
	public interface INamingSubSystem : ISubSystem
	{
		void Register(String key, IHandler handler);

		void UnRegister(String key);

		void UnRegister(Type service);

		bool Contains(String key);

		bool Contains(Type service);

		int ComponentCount { get; }

		IHandler GetHandler(String key);

		IHandler[] GetHandlers(String query);

		IHandler GetHandler(Type service);

		IHandler[] GetHandlers(Type service);

		IHandler[] GetHandlers();

		IHandler[] GetAssignableHandlers(Type service);

		IHandler this[Type service] { set; }

		IHandler this[String key] { set; }
	}
}