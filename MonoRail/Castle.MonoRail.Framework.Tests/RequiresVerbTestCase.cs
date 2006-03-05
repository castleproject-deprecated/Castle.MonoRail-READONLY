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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using System.Net;

	using NUnit.Framework;

	using Castle.MonoRail.TestSupport;


	[TestFixture]
    public class RequiresVerbTestCase : AbstractMRTestCase
	{
		[Test]
        public void AccessRequiresPostVerbByPost()
		{
            DoPost("home/PostOnlyMethod.rails");
            AssertSuccess();
		}

        [Test]
        public void AccessRequiresPostVerbByGet()
        {
            DoGet("home/PostOnlyMethod.rails");
            AssertReplyContains("Access to the action [postonlymethod] on controller [home] is not allowed by the http verb [GET].");
        }

		[Test]
        public void AccessRequiresGetVerbByPost()
		{
            DoPost("home/GetOnlyMethod.rails");
            AssertReplyContains("Access to the action [getonlymethod] on controller [home] is not allowed by the http verb [POST].");
		}

        [Test]
        public void AccessRequiresGetVerbByGet()
        {
            DoGet("home/GetOnlyMethod.rails");
            AssertSuccess();
        }

	}
}
