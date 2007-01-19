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

namespace Castle.MonoRail.Views.Brail
{
	using System;
	using Boo.Lang;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Internal;

	public class BrailJSElementGenerator : JSElementGeneratorBase, IQuackFu
	{
		public object QuackGet(string name, object[] parameters)
		{
			base.InternalGet(name);
			return this;
		}

		public object QuackSet(string name, object[] parameters, object value)
		{
			InternalGet(name); //get the current element and then set it
			return InternalInvoke("set", parameters);
		}

		public object QuackInvoke(string name, params object[] args)
		{
			return base.InternalInvoke(name, args);
		}

		public BrailJSElementGenerator(IJSElementGenerator generator) : base(generator)
		{
		}
	}
}