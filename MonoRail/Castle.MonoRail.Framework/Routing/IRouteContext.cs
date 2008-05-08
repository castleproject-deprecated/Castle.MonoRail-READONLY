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

namespace Castle.MonoRail.Framework
{
	using System.Web;
	using System.Collections;

	/// <summary>
	/// Context used for matching routes.
	/// </summary>
	public interface IRouteContext
	{
		/// <summary>
		/// The ApplicationPath
		/// </summary>
		string ApplicationPath { get; }

		/// <summary>
		/// The Http Request
		/// </summary>
		IRequest Request { get; }

		/// <summary>
		/// Gets the response.
		/// </summary>
		/// <value>The response.</value>
		HttpResponse Response { get;}

		/// <summary>
		/// Gets the context items.
		/// </summary>
		/// <value>The context items.</value>
		IDictionary ContextItems { get; }
	}
}
