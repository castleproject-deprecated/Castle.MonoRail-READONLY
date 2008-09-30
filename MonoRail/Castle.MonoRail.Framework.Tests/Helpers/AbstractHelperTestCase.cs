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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System.Collections;
	using System.Collections.Specialized;
	using System.Collections.Generic;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Test;
	using NUnit.Framework;

	[TestFixture]
	public class AbstractHelperTestCase
	{
		private DummyHelper helper;

		[SetUp]
		public void Init()
		{
			helper = new DummyHelper();
			helper.ServerUtility = new StubServerUtility();
		}

		[Test]
		public void BuildQueryString()
		{
			IDictionary parameters = new ListDictionary();
			parameters.Add("single", 1);
			parameters.Add("multiple", new int[] {2, 4, 99});
			parameters.Add("string", "test");

			string queryString = helper.BuildQueryString(parameters);

			Assert.AreEqual("single=1&amp;multiple=2&amp;multiple=4&amp;multiple=99&amp;string=test", queryString);
		}

		[Test]
		public void BuildQueryStringUsingNameValueCollection()
		{
			NameValueCollection parameters = new NameValueCollection();
			parameters.Add("single", "1");
			parameters.Add("multiple", "2");
			parameters.Add("multiple", "4");
			parameters.Add("multiple", "99");
			parameters.Add("string", "test");

			string queryString = helper.BuildQueryString(parameters);

			Assert.AreEqual("single=1&amp;multiple=2&amp;multiple=4&amp;multiple=99&amp;string=test", queryString);
		}

		[Test]
		public void JavascriptAsGenericSortedListTestOptionsTest()
		{
			IDictionary<string, string> options = new SortedList<string, string>();
			options.Add("key1","option1");
			options.Add("key2","option2");
			Assert.AreEqual("{key1:option1, key2:option2}",AbstractHelper.JavascriptOptions(options));
		}

		[Test]
		public void ScriptBlockGeneratesValidatableXHTML()
		{
			const string script = "var i = 1;";
			string scriptBlock = AbstractHelper.ScriptBlock(script);
			Assert.AreEqual("\r\n<script type=\"text/javascript\">/*<![CDATA[*/\r\n" + script + "/*]]>*/</script>\r\n", scriptBlock);
		}

		internal class DummyHelper : AbstractHelper
		{
			public new string BuildQueryString(IDictionary parameters)
			{
				return base.BuildQueryString(parameters);
			}

			public new string BuildQueryString(NameValueCollection parameters)
			{
				return base.BuildQueryString(parameters);
			}

			public override string HtmlEncode(string content)
			{
				return content;
			}

			public override string UrlEncode(string content)
			{
				return content;
			}
		}
	}
}
