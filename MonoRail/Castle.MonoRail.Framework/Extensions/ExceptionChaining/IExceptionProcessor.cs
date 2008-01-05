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

namespace Castle.MonoRail.Framework.Extensions.ExceptionChaining
{
	using System;

	/// <summary>
	/// Provides an interface to the ExceptionChaingingExtension 
	/// for manual triggering
	/// </summary>
	public interface IExceptionProcessor
	{
		/// <summary>
		/// Initiates the ExceptionChainingExtension manualy
		/// </summary>
		/// <param name="exception">The exception to process</param>
		/// <param name="engineContext">The engine context.</param>
		void ProcessException(Exception exception, IEngineContext engineContext);
	}
}