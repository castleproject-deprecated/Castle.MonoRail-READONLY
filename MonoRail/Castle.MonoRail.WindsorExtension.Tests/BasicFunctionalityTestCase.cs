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

namespace Castle.MonoRail.WindsorExtension.Tests
{
	using Castle.MonoRail.Framework.Tests;
	using NUnit.Framework;

	[TestFixture]
	public class WindsorExtensionBasicFunctionalityTestCase : AbstractTestCase
	{
		[Test]
		public void SimpleControllerAction()
		{
			DoGet("home/index.rails");

			AssertSuccess();

			AssertReplyEqualTo("My View contents for Home\\Index");
		}

		[Test]
		public void UsingComponent()
		{
			DoGet("home/componenttest.rails");

			AssertSuccess();

			AssertReplyEqualTo("my component");
		}

		[Test]
		public void UsingComponentFromLayout()
		{
			DoGet("home/ComponentOnLayout.rails");

			AssertSuccess();

			AssertReplyEqualTo("my component\r\n\r\nInner content");
		}

		[Test]
		public void UsingBuiltinComponent()
		{
			DoGet("home/builtincomponenttest.rails");

			AssertSuccess();

			AssertReplyEqualTo("stuff\r\n");
		}
	}
}