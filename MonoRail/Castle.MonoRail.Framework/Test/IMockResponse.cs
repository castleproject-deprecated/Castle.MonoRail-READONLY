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

namespace Castle.MonoRail.Framework.Test
{
	using System.IO;
	using System.Web;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Exposes methods on top of <see cref="IResponse"/>
	/// that are used by unit tests
	/// </summary>
	public interface IMockResponse : IResponse
	{
		/// <summary>
		/// Gets the urls the request was redirected to.
		/// </summary>
		/// <value>The redirected to.</value>
		string RedirectedTo { get; }

		/// <summary>
		/// Sets the output.
		/// </summary>
		/// <value>The output.</value>
		new TextWriter Output { set; }

		/// <summary>
		/// Sets the cache policy.
		/// </summary>
		/// <value>The cache policy.</value>
		new HttpCachePolicy CachePolicy { set; }

		/// <summary>
		/// Gets the output.
		/// </summary>
		/// <value>The output.</value>
		string OutputContent { get; }

//		/// <summary>
//		/// Gets the http headers.
//		/// </summary>
//		/// <value>The headers.</value>
//		NameValueCollection Headers { get; }
	}
}
