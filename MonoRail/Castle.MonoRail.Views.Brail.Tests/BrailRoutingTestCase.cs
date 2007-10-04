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
namespace Castle.MonoRail.Views.Brail.Tests
{
	using Castle.MonoRail.Framework.Tests;
	using NUnit.Framework;

	[TestFixture]
	public class BrailRoutingTestCase : AbstractTestCase
	{
		[Test]
		public void BlogRoutingRule()
		{
			DoGet("blog/posts/2005/07/");
			string expected = "Blog: year=2005 month=7";

			AssertReplyEqualTo(expected);
		}

		[Test]
		public void NewsRoutingRule()
		{
			DoGet("news/2004/11/");
			string expected = "News: year=2004 month=11";
			AssertReplyEqualTo(expected);
		}
	}
}