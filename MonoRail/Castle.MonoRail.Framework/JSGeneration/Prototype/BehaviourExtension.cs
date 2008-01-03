﻿// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.JSGeneration.Prototype
{
	/// <summary>
	/// Pendent
	/// </summary>
	public class BehaviourExtension
	{
		private readonly IJSCodeGenerator jsCodeGenerator;

		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptaculousExtension"/> class.
		/// </summary>
		/// <param name="jsCodeGenerator">The js code generator.</param>
		public BehaviourExtension(IJSCodeGenerator jsCodeGenerator)
		{
			this.jsCodeGenerator = jsCodeGenerator;
		}

		/// <summary>
		/// Re-apply Behaviour css' rules.
		/// </summary>
		/// <remarks>
		/// Only makes sense if you are using the Behaviour javascript library.
		/// </remarks>
		[DynamicOperation]
		public void ReApply()
		{
			jsCodeGenerator.Call("Behaviour.apply");
		}
	}
}
