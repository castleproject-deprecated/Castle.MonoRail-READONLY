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
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Web;

	/// <summary>
	/// Represents the response data and operations
	/// </summary>
	public interface IResponse
	{
		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		int StatusCode { get; set; }

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		string StatusDescription { get; set; }

		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		string ContentType { get; set; }

		/// <summary>
		/// Gets the caching policy (expiration time, privacy, 
		/// vary clauses) of a Web page.
		/// </summary>
		HttpCachePolicy CachePolicy  { get; }

		/// <summary>
		/// Sets the Cache-Control HTTP header to Public or Private.
		/// </summary>
		String CacheControlHeader { get; set; } 

		/// <summary>
		/// Gets or sets the HTTP character set of the output stream.
		/// </summary>
		String Charset { get; set; }

		/// <summary>
		/// Gets the output.
		/// </summary>
		/// <value>The output.</value>
		System.IO.TextWriter Output { get; }

		/// <summary>
		/// Gets the output stream.
		/// </summary>
		/// <value>The output stream.</value>
		System.IO.Stream OutputStream { get; }

		/// <summary>
		/// Appends the header.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		void AppendHeader(String name, String value);

//		/// <summary>
//		/// Writes the buffer to the browser
//		/// </summary>
//		/// <param name="buffer">The buffer.</param>
//		void BinaryWrite(byte[] buffer);
//
//		/// <summary>
//		/// Writes the stream to the browser
//		/// </summary>
//		/// <param name="stream">The stream.</param>
//		void BinaryWrite(System.IO.Stream stream);

		/// <summary>
		/// Clears the response (only works if buffered)
		/// </summary>
		void Clear();

		/// <summary>
		/// Clears the response content (only works if buffered).
		/// </summary>
		void ClearContent();

		/// <summary>
		/// Writes the specified string.
		/// </summary>
		/// <param name="s">The string.</param>
		void Write(String s);

		/// <summary>
		/// Writes the specified obj.
		/// </summary>
		/// <param name="obj">The obj.</param>
		void Write(object obj);

		/// <summary>
		/// Writes the specified char.
		/// </summary>
		/// <param name="ch">The char.</param>
		void Write(char ch);

		/// <summary>
		/// Writes the specified buffer.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="index">The index.</param>
		/// <param name="count">The count.</param>
		void Write(char[] buffer, int index, int count);

//		/// <summary>
//		/// Writes the file.
//		/// </summary>
//		/// <param name="fileName">Name of the file.</param>
//		void WriteFile(String fileName);

		/// <summary>
		/// Redirects the specified controller.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		void Redirect(object parameters);

		/// <summary>
		/// Redirects the specified controller.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		void Redirect(String controller, String action);

		/// <summary>
		/// Redirects the specified area.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		void Redirect(String area, String controller, String action);

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		void Redirect(string controller, string action, NameValueCollection parameters);

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		void Redirect(string area, string controller, string action, NameValueCollection parameters);

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		void Redirect(string controller, string action, IDictionary parameters);

		/// <summary>
		/// Redirects to another controller and action with the specified paramters.
		/// </summary>
		/// <param name="area">Area name</param>
		/// <param name="controller">Controller name</param>
		/// <param name="action">Action name</param>
		/// <param name="parameters">Key/value pairings</param>
		void Redirect(string area, string controller, string action, IDictionary parameters);

		/// <summary>
		/// Redirects the specified URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		void RedirectToUrl(String url);

		/// <summary>
		/// Redirects the specified URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="endProcess">if set to <c>true</c> [end process].</param>
		void RedirectToUrl(String url, bool endProcess);

		/// <summary>
		/// Gets a value indicating whether the response sent a redirect.
		/// </summary>
		/// <value><c>true</c> if was redirected; otherwise, <c>false</c>.</value>
		bool WasRedirected { get; }

		/// <summary>
		/// Gets a value indicating whether this instance is client connected.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is client connected; otherwise, <c>false</c>.
		/// </value>
		bool IsClientConnected { get; }

		/// <summary>
		/// Creates a cookie.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		void CreateCookie(String name, String value);

		/// <summary>
		/// Creates a cookie.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <param name="expiration">The expiration.</param>
		void CreateCookie(String name, String value, DateTime expiration);

		/// <summary>
		/// Creates a cookie.
		/// </summary>
		/// <param name="cookie">The cookie.</param>
		void CreateCookie(HttpCookie cookie);

		/// <summary>
		/// Removes a cookie.
		/// </summary>
		/// <param name="name">The name.</param>
		void RemoveCookie(string name);
	}
}
