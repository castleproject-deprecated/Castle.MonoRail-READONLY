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

using NVelocity;

namespace Castle.MonoRail.Framework.Views.NVelocity.JSGeneration
{
	using System;
	using Castle.MonoRail.Framework.JSGeneration;
	using Castle.MonoRail.Framework.JSGeneration.DynamicDispatching;

	/// <summary>
	/// 
	/// </summary>
	public class JSGeneratorDuck : JSGeneratorDispatcherBase, IDuck
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JSGeneratorDuck"/> class.
		/// </summary>
		/// <param name="codeGen">The code gen.</param>
		/// <param name="generator">The generator.</param>
		/// <param name="extensions">The extensions.</param>
		/// <param name="elementExtensions">The element extensions.</param>
		public JSGeneratorDuck(IJSCodeGenerator codeGen, IJSGenerator generator, object[] extensions, object[] elementExtensions) :
			base(codeGen, generator, extensions, elementExtensions)
		{
		}

		#region IDuck

		/// <summary>
		/// Defines the behavior when a property is read
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <returns>value back to the template</returns>
		public object GetInvoke(string propName)
		{
			return Invoke(propName, new object[0]);
		}

		/// <summary>
		/// Defines the behavior when a property is written
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <param name="value">The value to assign.</param>
		public void SetInvoke(string propName, object value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Invokes the specified method.
		/// </summary>
		/// <param name="method">The method name.</param>
		/// <param name="args">The method arguments.</param>
		/// <returns>value back to the template</returns>
		public object Invoke(string method, params object[] args)
		{
			return InternalInvoke(method, args);
		}

		#endregion

		protected override object CreateNullGenerator()
		{
			return null;
		}

		/// <summary>
		/// Creates a JS element generator.
		/// </summary>
		/// <param name="codeGen">The code gen.</param>
		/// <param name="elementGenerator">The element generator.</param>
		/// <param name="elementExtensions">The element extensions.</param>
		/// <returns></returns>
		protected override object CreateJSElementGeneratorProxy(IJSCodeGenerator codeGen, IJSElementGenerator elementGenerator, object[] elementExtensions)
		{
			return new JSElementGeneratorDuck(codeGen, elementGenerator, elementExtensions);
		}

		public override string ToString()
		{
			return CodeGen.ToString();
		}
	}
}