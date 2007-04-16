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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Web;

	/// <summary>
	/// 
	/// </summary>
	public interface IRequest
	{
		/// <summary>
		/// Gets the Http headers.
		/// </summary>
		/// <value>The Http headers.</value>
		NameValueCollection Headers { get; }

		/// <summary>
		/// Gets the <see cref="HttpPostedFile"/> per key.
		/// </summary>
		IDictionary Files { get; }

		NameValueCollection Params { get; }

		bool IsLocal { get; }

		String RawUrl { get; }

		Uri Uri { get; }

		String HttpMethod { get; }
		
		String FilePath { get; }

		byte[] BinaryRead(int count);

		String this [String key] { get; }

		String ReadCookie( String name );

		NameValueCollection QueryString { get; }

		NameValueCollection Form { get; }

		String[] UserLanguages { get; }

		/// <summary>
		/// Gets the IP host address of the remote client. 
		/// </summary>
		/// <value>The IP address of the remote client.</value>
		string UserHostAddress { get; }

		void ValidateInput();
	}
}
