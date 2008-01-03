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

namespace Castle.MonoRail.Framework.Test
{
	using System;

	/// <summary>
	/// Exposes methods on top of <see cref="IRequest"/>
	/// that can be useful to write unit tests
	/// </summary>
	public interface IMockRequest : IRequest
	{
		/// <summary>
		/// Sets the accept header.
		/// </summary>
		/// <value>The accept header.</value>
		new string AcceptHeader { set; }

		/// <summary>
		/// Sets the path info.
		/// </summary>
		/// <value>The path info.</value>
		new string PathInfo { set; }

		/// <summary>
		/// Sets a value indicating whether this 
		/// requeest is from a local address.
		/// </summary>
		/// <value><c>true</c> if this instance is local; otherwise, <c>false</c>.</value>
		new bool IsLocal { set; }

		/// <summary>
		/// Sets the raw URL.
		/// </summary>
		/// <value>The raw URL.</value>
		new string RawUrl { set; }

		/// <summary>
		/// Sets the URI.
		/// </summary>
		/// <value>The URI.</value>
		new Uri Uri { set; }

		/// <summary>
		/// Sets the HTTP method.
		/// </summary>
		/// <value>The HTTP method.</value>
		new string HttpMethod { set; }

		/// <summary>
		/// Sets the file path.
		/// </summary>
		/// <value>The file path.</value>
		new string FilePath { set; }

		/// <summary>
		/// Sets the user languages.
		/// </summary>
		/// <value>The user languages.</value>
		new string[] UserLanguages { set; }

		/// <summary>
		/// Sets the IP host address of the remote client. 
		/// </summary>
		/// <value>The IP address of the remote client.</value>
		new string UserHostAddress { set; }
	}
}
