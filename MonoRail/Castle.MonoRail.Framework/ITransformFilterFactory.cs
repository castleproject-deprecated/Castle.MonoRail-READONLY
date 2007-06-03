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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.IO;

	/// <summary>
	/// Depicts the contract used by the engine
	/// to obtain implementations of <see cref="ITransformFilter"/>.
	/// </summary>
	public interface ITransformFilterFactory
	{
		/// <summary>
		/// Creates the specified transform filter type.
		/// </summary>
		/// <param name="transformFilterType">Type of the transform filter.</param>
		/// <param name="baseStream">The base stream.</param>
		/// <returns></returns>
		ITransformFilter Create(Type transformFilterType, Stream baseStream);

		/// <summary>
		/// Releases the specified transform filter.
		/// </summary>
		/// <param name="transformFilter">The transform filter.</param>
		void Release(ITransformFilter transformFilter);
	}
}
