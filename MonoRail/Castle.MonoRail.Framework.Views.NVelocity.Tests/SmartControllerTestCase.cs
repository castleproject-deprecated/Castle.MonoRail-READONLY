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

	using Castle.MonoRail.TestSupport;

	[TestFixture]
	public class SmartControllerTestCase : AbstractMRTestCase
	{
		[Test]
		public void StringMethod()
		{
			DoGet("smart/stringmethod.rails", "name=hammett");
			String expected = "incoming hammett";

			AssertReplyEqualsTo(expected);

			DoGet("smart/stringmethod.rails", "NAME=hammett");
			expected = "incoming hammett";

			AssertReplyEqualsTo(expected);
		}

		[Test]
		public void Complex()
		{
			DoGet("smart/complex.rails", "strarg=hammett", "intarg=1", "strarray=a");
			String expected = "incoming hammett 1 a";

			AssertReplyEqualsTo(expected);

			DoGet("smart/complex.rails", "strarg=", "intarg=", "strarray=a,b,c");
			expected = "incoming  0 a,b,c";

			AssertReplyEqualsTo(expected);
		}

		[Test]
		public void SimpleBind()
		{
			DoGet("smart/SimpleBind.rails", "name=hammett", "itemcount=11", "price=20");
			String expected = "incoming hammett 11 20";

			AssertReplyEqualsTo(expected);

			DoGet("smart/SimpleBind.rails", "name=hammett");
			expected = "incoming hammett 0 0";

			AssertReplyEqualsTo(expected);
		}

		[Test]
		public void ComplexBind()
		{
			DoGet("smart/ComplexBind.rails", "name=hammett", "itemcount=11", "price=20", "id=1", "contact.email=x&contact.phone=y");
			String expected = "incoming hammett 11 20 1 x y";

			AssertReplyEqualsTo(expected);
		}

		[Test]
		public void ComplexBindWithPrefix()
		{
			DoGet("smart/ComplexBindWithPrefix.rails", "name=hammett", "itemcount=11", "price=20", "person.id=1", "person.contact.email=x", "person.contact.phone=y");
			String expected = "incoming hammett 11 20 1 x y";

			AssertReplyEqualsTo(expected);
		}

		[Test]
		//[Ignore("Crashes NUnit with '.', hexadecimal value 0x00, is an invalid character")]
		public void FillingBehavior1()
		{
			DoGet("smart/FillingBehavior.rails", "name=someone", "date1day=11", "date1month=10", "date1year=2005");
			String expected = "incoming someone " + new DateTime( 2005, 10, 11 ).ToShortDateString() + " " + 
				DateTime.Now.AddDays(1).ToShortDateString();

			AssertReplyEqualsTo(expected);
		}

		[Test]
		// [Ignore("Crashes NUnit with '.', hexadecimal value 0x00, is an invalid character")]
		public void FillingBehavior2()
		{
			DoGet("smart/FillingBehavior.rails");
			String expected = "incoming hammett " + 
				DateTime.Now.ToShortDateString() + " " + DateTime.Now.AddDays(1).ToShortDateString();

			AssertReplyEqualsTo(expected);
		}
	}
}
