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

namespace Castle.MonoRail.Framework.Adapters
{
	using System;
	using System.Web;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Adapts the ASP.Net HttpServerUtility to MonoRail's interface for the same service.
	/// </summary>
	public class ServerUtilityAdapter : IServerUtility
	{
		private readonly HttpServerUtility server;

		/// <summary>
		/// Initializes a new instance of the <see cref="ServerUtilityAdapter"/> class.
		/// </summary>
		/// <param name="server">The server.</param>
		public ServerUtilityAdapter(HttpServerUtility server)
		{
			this.server = server;
		}

		/// <summary>
		/// HTML encodes a string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text string to HTML encode.</param>
		/// <returns>The HTML encoded text.</returns>
		public String HtmlEncode(String content)
		{
			return server.HtmlEncode(content);
		}

		/// <summary>
		/// Escapes JavaScript with Url encoding and returns the encoded string.  
		/// </summary>
		/// <remarks>
		/// Converts quotes, single quotes and CR/LFs to their representation as an escape character.
		/// </remarks>
		/// <param name="content">The text to URL encode and escape JavaScript within.</param>
		/// <returns>The URL encoded and JavaScript escaped text.</returns>
		public String JavaScriptEscape(String content)
		{
			// TODO: Replace by a regular expression, which should be much more efficient

			return content.Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n").Replace("'","\\'");
		}

		/// <summary>
		/// URL encodes a string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text to URL encode.</param>
		/// <returns>The URL encoded text.</returns>
		public String UrlEncode(String content)
		{
			return server.UrlEncode(content);
		}

		/// <summary>
		/// URL decodes a string and returns the decoded string.  
		/// </summary>
		/// <param name="content">The text to URL decode.</param>
		/// <returns>The URL decoded text.</returns>
		public String UrlDecode(String content)
		{
			return server.UrlDecode(content);
		}

		/// <summary>
		/// URL encodes the path portion of a URL string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text to URL encode.</param>
		/// <returns>The URL encoded text.</returns>
		public String UrlPathEncode(String content)
		{
			return server.UrlPathEncode(content);
		}

		/// <summary>
		/// Returns the physical path for the 
		/// specified virtual path.
		/// </summary>
		/// <param name="virtualPath">The virtual path.</param>
		/// <returns>The mapped path</returns>
		public String MapPath(String virtualPath)
		{
			return server.MapPath(virtualPath);
		}
	}
}
