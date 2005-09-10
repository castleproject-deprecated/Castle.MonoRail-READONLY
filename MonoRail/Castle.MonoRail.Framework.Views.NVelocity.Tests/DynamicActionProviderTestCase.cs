// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using System;

	using NUnit.Framework;


	[TestFixture]
	public class DynamicActionProviderTestCase : AbstractNVelocityTestCase
	{
		[Test]
		public void DynamicActionUsingView()
		{
			string url = "/dyn/save.rails";
			string expected = "Hello from save dynamic action";

			Execute(url, expected);
		}

		[Test]
		public void DynamicActionUsingRenderText()
		{
			string url = "/dyn/index.rails";
			string expected = "xx";//"Rendered from the view: hello!";

			Execute(url, expected);
		}
	}
}
