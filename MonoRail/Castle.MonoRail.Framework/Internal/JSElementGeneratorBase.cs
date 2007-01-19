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

namespace Castle.MonoRail.Framework.Internal
{
	using Castle.MonoRail.Framework.Helpers;

	public abstract class JSElementGeneratorBase
	{
		protected readonly IJSElementGenerator generator;
		protected readonly IJSGenerator parentGenerator;

		/// <summary>
		/// Initializes a new instance of the <see cref="JSElementGeneratorBase"/> class.
		/// </summary>
		/// <param name="generator">The generator.</param>
		public JSElementGeneratorBase(IJSElementGenerator generator)
		{
			this.generator = generator;
			parentGenerator = generator.ParentGenerator;
		}

		protected void InternalGet(string propName)
		{
			PrototypeHelper.JSGenerator.ReplaceTailByPeriod(parentGenerator);
			PrototypeHelper.JSGenerator.Record(parentGenerator, propName);
		}

		protected object InternalInvoke(string method, object[] args)
		{
			if (method == "set")
			{
				PrototypeHelper.JSGenerator.RemoveTail(parentGenerator);

				PrototypeHelper.JSGenerator.Record(parentGenerator, " = " + args[0]);

				return null;
			}
			else
			{
				PrototypeHelper.JSGenerator.ReplaceTailByPeriod(parentGenerator);
				//TODO: This code is duplicated JSCollectionGeneratorBase line 65
				DynamicDispatchSupport dispInterface = generator as DynamicDispatchSupport;
				if (dispInterface == null)
				{
					throw new MonoRail.Framework.RailsException("JS Generators must inherit DynamicDispatchSupport");
				}
				if (dispInterface.IsGeneratorMethod(method))
				{
					dispInterface.Dispatch(method, args);
				}
				else
				{
					parentGenerator.Call(method, args);
				}

				return this;
			}
		}
	}
}