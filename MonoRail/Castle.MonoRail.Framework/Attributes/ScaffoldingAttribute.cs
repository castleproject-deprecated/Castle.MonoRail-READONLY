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

	/// <summary>
	/// Associates scaffolding support with a controller.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true), Serializable]
	public class ScaffoldingAttribute : Attribute
	{
		private readonly Type model;

		/// <summary>
		/// Initializes a new instance of the <see cref="ScaffoldingAttribute"/> class.
		/// </summary>
		/// <param name="model">The model/entity that should be implemented</param>
		public ScaffoldingAttribute(Type model)
		{
			this.model = model;
		}

		/// <summary>
		/// Gets the model/entity type
		/// </summary>
		/// <value>The model/entity type.</value>
		public Type Model
		{
			get { return model; }
		}
	}
}
