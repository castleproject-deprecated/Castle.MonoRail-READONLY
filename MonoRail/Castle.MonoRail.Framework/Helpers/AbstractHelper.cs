// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Globalization;
	using System.Text;
	using System.Collections;

	/// <summary>
	/// Optional base class for helpers. 
	/// Extend from this class only if your helpers needs
	/// a reference to the controller which is using it or
	/// if you need to use one of the protected methods.
	/// </summary>
	public abstract class AbstractHelper : IControllerAware
	{
		private const string MonoRailVersion = "RC3_0001";

		#region Controller Reference

		/// <summary>
		/// Store's <see cref="Controller"/> for the current view.
		/// </summary>
		private Controller controller;
		
		/// <summary>
		/// Sets the controller.
		/// </summary>
		/// <param name="controller">Current view's <see cref="Controller"/>.</param>
		public virtual void SetController(Controller controller)
		{
			this.controller = controller;
		}

		/// <summary>
		/// Gets the controller.
		/// </summary>
		/// <value>The <see cref="Controller"/> used with the current view.</value>
		public Controller Controller
		{
			get { return controller; }
		}

		#endregion 

		/// <summary>
		/// Merges <paramref name="userOptions"/> with <paramref name="defaultOptions"/> placing results in
		/// <paramref name="userOptions"/>.
		/// </summary>
		/// <param name="userOptions">The user options.</param>
		/// <param name="defaultOptions">The default options.</param>
		/// <remarks>
		/// All <see cref="IDictionary.Values"/> and <see cref="IDictionary.Keys"/> in <paramref name="defaultOptions"/>
		/// are copied to <paramref name="userOptions"/>. Entries with the same <see cref="DictionaryEntry.Key"/> in
		/// <paramref name="defaultOptions"/> and <paramref name="userOptions"/> are skipped.
		/// </remarks>
		protected void MergeOptions(IDictionary userOptions, IDictionary defaultOptions)
		{
			foreach(DictionaryEntry entry in defaultOptions)
			{
				if (!userOptions.Contains(entry.Key))
				{
					userOptions[entry.Key] = entry.Value;
				}
			}
		}
		
		protected IRailsEngineContext CurrentContext
		{
			get { return controller.Context; }
		}

		#region Helper methods

		/// <summary>
		/// Renders the a script block with a <c>src</c> attribute
		/// pointing to the url. The url must not have an extension. 
		/// <para>
		/// For example, suppose you invoke it like:
		/// <code>
		/// RenderScriptBlockToSource("/my/url/to/my/scripts");
		/// </code>
		/// </para>
		/// <para>
		/// That will render
		/// <code><![CDATA[
		/// <script type="text/javascript" src="/my/url/to/my/scripts.rails?VERSIONID"></script>
		/// ]]>
		/// </code>
		/// As you see the file extension will be inferred
		/// </para>
		/// </summary>
		/// <param name="url">The url for the scripts (should start with a '/')</param>
		/// <returns>An empty script block</returns>
		protected string RenderScriptBlockToSource(string url)
		{
			return String.Format("<script type=\"text/javascript\" src=\"{0}.{1}?" + MonoRailVersion + "\"></script>",
				Controller.Context.ApplicationPath + url, Controller.Context.UrlInfo.Extension);
		}

		/// <summary>
		/// Generates HTML element attributes string from <paramref name="attributes"/>.
		/// <code>key1="value1" key2</code>
		/// </summary>
		/// <param name="attributes">The attributes for the element.</param>
		/// <returns><see cref="String"/> to use inside HTML element's tag.</returns>
		/// <remarks>
		/// <see cref="String.Empty"/> is returned if <paramref name="attributes"/> is <c>null</c> or empty.
		/// <para>
		/// If for some <see cref="DictionaryEntry.Key"/> <see cref="DictionaryEntry.Value"/> is <c>null</c> or
		/// <see cref="String.Empty"/> only attribute name is appended to the string.
		/// </para>
		/// </remarks>
		protected String GetAttributes(IDictionary attributes)
		{
			if (attributes == null || attributes.Count == 0) return String.Empty;

			StringBuilder contents = new StringBuilder();

			foreach (DictionaryEntry entry in attributes)
			{
				if (entry.Value == null || entry.Value.ToString() == String.Empty)
				{
					contents.Append(entry.Key);
				}
				else
				{
					contents.AppendFormat("{0}=\"{1}\"", entry.Key, entry.Value);
				}
				contents.Append(' ');
			}

			return contents.ToString();
		}

		/// <summary>
		/// Builds a query string.
		/// </summary>
		/// <remarks>
		/// Supports multi-value query strings, using any
		/// <see cref="IEnumerable"/> as a value.
		/// <example>
		///	<code>
		/// IDictionary dict = new Hashtable();
		/// dict.Add("id", 5);
		/// dict.Add("selectedItem", new int[] { 2, 4, 99 });
		/// string queryString = BuildQueryString(dict);
		/// // should result in: "id=5&amp;selectedItem=2&amp;selectedItem=4&amp;selectedItem=99&amp;"
		/// </code>
		/// </example>
		/// </remarks>
		/// <param name="parameters">The parameters</param>
		public String BuildQueryString(IDictionary parameters)
		{
			if (parameters == null) return String.Empty;

			Object[] singleValueEntry = new Object[1];
			StringBuilder sb = new StringBuilder();

			foreach(DictionaryEntry entry in parameters)
			{
				if (entry.Value == null) continue;

				IEnumerable values = singleValueEntry;
				if (!(entry.Value is String) && (entry.Value is IEnumerable))
					values = (IEnumerable) entry.Value;
				else
					singleValueEntry[0] = entry.Value;

				foreach(object value in values)
				{
					string encoded = UrlEncode(Convert.ToString(value, CultureInfo.CurrentCulture));

					sb.Append(entry.Key)
						.Append('=')
						.Append(encoded)
						.Append("&amp;");
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Concat two string in a query string format (<c>key=value&amp;key2=value2</c>) 
		/// building a third string with the result
		/// </summary>
		/// <param name="leftParams">key values</param>
		/// <param name="rightParams">key values</param>
		/// <returns>The concatenation result</returns>
		protected String ConcatQueryString(String leftParams, String rightParams)
		{
			if (leftParams == null || leftParams.Length == 0)
			{
				return rightParams;
			}
			if (rightParams == null || rightParams.Length == 0)
			{
				return leftParams;
			}

			if (leftParams.EndsWith("&") || leftParams.EndsWith("&amp;"))
			{
				leftParams = leftParams.Substring( 0, leftParams.Length - 1 );
			}

			return String.Format("{0}&amp;{1}", leftParams, rightParams);
		}

		/// <summary>
		/// HTML encodes a string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text string to HTML encode.</param>
		/// <returns>The HTML encoded text.</returns>
		public virtual String HtmlEncode(String content)
		{
			return controller.Context.Server.HtmlEncode(content);
		}

		/// <summary>
		/// URL encodes a string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text to URL encode.</param>
		/// <returns>The URL encoded text.</returns>
		public virtual String UrlEncode(String content)
		{
			return controller.Context.Server.UrlEncode(content);
		}

		/// <summary>
		/// URL encodes the path portion of a URL string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text to URL encode.</param>
		/// <returns>The URL encoded text.</returns>
		public String UrlPathEncode(String content)
		{
			return controller.Context.Server.UrlPathEncode(content);
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
			return controller.Context.Server.JavaScriptEscape(content);
		}

		/// <summary>
		/// Builds a JS associative array based on the specified dictionary instance.
		/// <para>
		/// For example: <c>{name: value, other: 'another'}</c>
		/// </para>
		/// </summary>
		/// <param name="jsOptions">The js options.</param>
		/// <returns>An associative array in javascript</returns>
		public static String JavascriptOptions(IDictionary jsOptions)
		{
			if (jsOptions == null || jsOptions.Count == 0)
			{
				return "{}";
			}

			StringBuilder sb = new StringBuilder(jsOptions.Count * 10);
			sb.Append("{");
			bool comma = false;

			foreach (DictionaryEntry entry in jsOptions)
			{
				if (!comma) comma = true; else sb.Append(", ");

				sb.Append(String.Format("{0}:{1}", entry.Key, entry.Value));
			}

			sb.Append("}");
			return sb.ToString();
		}

		/// <summary>
		/// Generates script block.
		/// <code>
		/// &lt;script type=\"text/javascript\"&gt;
		/// scriptContents
		/// &lt;/script&gt;
		/// </code>
		/// </summary>
		/// <param name="scriptContents">The script contents.</param>
		/// <returns><paramref name="scriptContents"/> placed inside <b>script</b> tags.</returns>
		public static String ScriptBlock(String scriptContents)
		{
			return "\r\n<script type=\"text/javascript\">\r\n" + scriptContents + "</script>\r\n";
		}

		/// <summary>
		/// Quotes the specified string with double quotes
		/// </summary>
		/// <param name="content">The content.</param>
		/// <returns>A quoted string</returns>
		public static string Quote(string content)
		{
			return "\"" + content + "\"";
		}

		/// <summary>
		/// Quotes the specified string with singdoublele quotes
		/// </summary>
		/// <param name="items">Items to quote</param>
		/// <returns>A quoted string</returns>
		public static string[] Quote(string[] items)
		{
			string[] quotedItems = new string[items.Length];

			int index = 0;

			foreach(string item in items)
			{
				quotedItems[index++] = Quote(item);
			}

			return quotedItems;
		}

		/// <summary>
		/// Quotes the specified string with double quotes
		/// </summary>
		/// <param name="content">The content.</param>
		/// <returns>A quoted string</returns>
		public static string SQuote(string content)
		{
			return "\'" + content + "\'";
		}

		#endregion 
	}
}
