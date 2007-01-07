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

#if (DOTNET2 && NET)

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Collections;

	public abstract class AbstractBindingScope : IBindingScope
	{
		#region IBindingScope

		object IBindingScope.ResolveSymbol(string symbol)
		{
			return ResolveSymbol(symbol);
		}

		void IBindingScope.AddActionArguments(BindingContext context,
		                                      IDictionary resolvedActionArgs)
		{
			AddActionArguments(context, resolvedActionArgs);
		}

		#endregion

		protected virtual object ResolveSymbol(string symbol)
		{
			return null;
		}

		protected virtual void AddActionArguments(BindingContext context,
		                                          IDictionary resolvedActionArgs)
		{
		}
	}
}

#endif
