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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Text;

	/// <summary>
	/// Code shared by Helpers/Controllers/Others
	/// </summary>
	public static class CommonUtils
	{
		/// <summary>
		/// Obtains the entry.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		/// <param name="key">The key.</param>
		/// <returns>The generated form element</returns>
		public static string ObtainEntry(IDictionary attributes, string key)
		{
			if (attributes != null && attributes.Contains(key))
			{
				return (String)attributes[key];
			}

			return null;
		}

		/// <summary>
		/// Obtains the entry.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>the entry value or the default value</returns>
		public static string ObtainEntry(IDictionary attributes, string key, string defaultValue)
		{
			string value = ObtainEntry(attributes, key);

			return value ?? defaultValue;
		}

		/// <summary>
		/// Obtains the entry and remove it if found.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>the entry value or the default value</returns>
		public static string ObtainEntryAndRemove(IDictionary attributes, string key, string defaultValue)
		{
			string value = ObtainEntryAndRemove(attributes, key);

			return value ?? defaultValue;
		}

		/// <summary>
		/// Obtains the entry and remove it if found.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		/// <param name="key">The key.</param>
		/// <returns>the entry value or null</returns>
		public static string ObtainEntryAndRemove(IDictionary attributes, string key)
		{
			string value = null;

			if (attributes != null && attributes.Contains(key))
			{
				value = (String)attributes[key];

				attributes.Remove(key);
			}

			return value;
		}

		/// <summary>
		/// Obtains the entry and remove it if found.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		/// <param name="key">The key.</param>
		/// <returns>the entry value or null</returns>
		public static object ObtainObjectEntryAndRemove(IDictionary attributes, string key)
		{
			object value = null;

			if (attributes != null && attributes.Contains(key))
			{
				value = attributes[key];

				attributes.Remove(key);
			}

			return value;
		}

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
		public static void MergeOptions(IDictionary userOptions, IDictionary defaultOptions)
		{
			foreach(DictionaryEntry entry in defaultOptions)
			{
				if (!userOptions.Contains(entry.Key))
				{
					userOptions[entry.Key] = entry.Value;
				}
			}
		}

		/// <summary>
		/// Builds a query string.
		/// </summary>
		/// <remarks>
		/// Supports multi-value query strings, using any
		/// <see cref="IEnumerable"/> as a value.
		/// </remarks>
		/// <param name="parameters">The parameters</param>
		/// <param name="serverUtil">The server utility instance</param>
		/// <param name="encodeAmp">if <c>true</c>, the separation of entries will be encoded.</param>
		public static string BuildQueryString(IServerUtility serverUtil, NameValueCollection parameters, bool encodeAmp)
		{
			if (parameters == null || parameters.Count == 0) return string.Empty;
			if (serverUtil == null) throw new ArgumentNullException("serverUtil");

			StringBuilder sb = new StringBuilder();

			bool useSeparator = false;

			foreach (string key in parameters.Keys)
			{
				if (key == null) continue;

				foreach (string value in parameters.GetValues(key))
				{
					if (useSeparator)
					{
						if (encodeAmp)
						{
							sb.Append("&amp;");
						}
						else
						{
							sb.Append("&");
						}
					}
					else
					{
						useSeparator = true;
					}

					sb.Append(serverUtil.UrlEncode(key))
						.Append('=')
						.Append(serverUtil.UrlEncode(value));
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Builds a query string.
		/// </summary>
		/// <remarks>
		/// Supports multi-value query strings, using any
		/// <see cref="IEnumerable"/> as a value.
		/// </remarks>
		/// <param name="parameters">The parameters</param>
		public static string BuildQueryString(NameValueCollection parameters)
		{
			if (parameters == null || parameters.Count == 0) return string.Empty;

			StringBuilder sb = new StringBuilder();

			bool useSeparator = false;

			foreach(string key in parameters.Keys)
			{
				if (key == null) continue;

				foreach(string value in parameters.GetValues(key))
				{
					if (useSeparator)
					{
						sb.Append("&");
					}
					else
					{
						useSeparator = true;
					}

					sb.Append(key)
						.Append('=')
						.Append(value);
				}
			}

			return sb.ToString();
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
		/// string querystring = BuildQueryString(dict);
		/// // should result in: "id=5&amp;selectedItem=2&amp;selectedItem=4&amp;selectedItem=99&amp;"
		/// </code>
		/// </example>
		/// </remarks>
		/// <param name="parameters">The parameters</param>
		/// <param name="serverUtil">The server utility instance</param>
		/// <param name="encodeAmp">if <c>true</c>, the separation of entries will be encoded.</param>
		public static string BuildQueryString(IServerUtility serverUtil, IDictionary parameters, bool encodeAmp)
		{
			if (parameters == null || parameters.Count == 0) return string.Empty;
			if (serverUtil == null) throw new ArgumentNullException("serverUtil");

			Object[] singleValueEntry = new Object[1];
			StringBuilder sb = new StringBuilder();

			bool useSeparator = false;

			foreach(DictionaryEntry entry in parameters)
			{
				if (entry.Value == null) continue;

				IEnumerable values = singleValueEntry;

				if (!(entry.Value is String) && (entry.Value is IEnumerable))
				{
					values = (IEnumerable) entry.Value;
				}
				else
				{
					singleValueEntry[0] = entry.Value;
				}

				foreach(object value in values)
				{
					if (useSeparator)
					{
						if (encodeAmp)
						{
							sb.Append("&amp;");
						}
						else
						{
							sb.Append("&");
						}
					}
					else
					{
						useSeparator = true;
					}

					string encoded = serverUtil.UrlEncode(Convert.ToString(value, CultureInfo.CurrentCulture));

					sb.Append(serverUtil.UrlEncode(entry.Key.ToString())).Append('=').Append(encoded);
				}
			}

			return sb.ToString();
		}
	}
}