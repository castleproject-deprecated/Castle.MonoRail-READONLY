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

namespace Castle.MicroKernel.Lifestyle.Pool
{
	using System;

	/// <summary>
	/// Pool implementation contract. 
	/// </summary>
	public interface IPool : IDisposable
	{
		/// <summary>
		/// Implementors should return a component instance.
		/// </summary>
		/// <returns></returns>
		object Request(CreationContext context);

		/// <summary>
		/// Implementors should release the instance or put it
		/// on the pool
		/// </summary>
		/// <param name="instance"></param>
		void Release(object instance);
	}
}
