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

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using System;
	using Castle.MonoRail.Framework.Tests;
	using NUnit.Framework;

	using Castle.MonoRail.TestSupport;

	[TestFixture]
	public class SmartController2TestCase : AbstractTestCase
	{
		[Test]
		public void SimpleBindArray()
		{
			DoPost("smart2/SimpleBindArray.rails");
			AssertReplyEqualTo("incoming 0");

			DoGet("smart2/SimpleBindArray.rails");
			AssertReplyEqualTo("incoming 0");

			DoGet("smart2/SimpleBindArray.rails", "orders[0].Name=hammett", "orders[1].Name=hamilton");
			AssertReplyEqualTo("incoming 2");
		}
		
		[Test]
		public void SimpleBind()
		{
			DoPost("smart2/SimpleBind.rails");
			String expected = "incoming  0 0";
			AssertReplyEqualTo(expected);

			DoPost("smart2/SimpleBind.rails", "order.name=hammett", "order.itemcount=11", "order.price=20");
			expected = "incoming hammett 11 20";
			AssertReplyEqualTo(expected);

			// Double check
			DoPost("smart2/SimpleBind.rails", "order.name=hammett", "order.itemcount=11", "order.price=20");
			expected = "incoming hammett 11 20";
			AssertReplyEqualTo(expected);

			DoGet("smart2/SimpleBind.rails", "order.name=hammett", "order.itemcount=11", "order.price=20");
			expected = "incoming hammett 11 20";
			AssertReplyEqualTo(expected);

			DoGet("smart2/SimpleBind.rails", "order.name=hammett");
			expected = "incoming hammett 0 0";
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void ComplexBind()
		{
			DoGet( "smart2/ComplexBind.rails", "order.name=hammett", "order.itemcount=11", "order.price=20", "person.id=1", "person.contact.email=x", "person.contact.phone=y" );
			String expected = "incoming hammett 11 20 1 x y";

			AssertReplyEqualTo( expected );
		}

		[Test]
		public void ComplexBindExcludePrice()
		{
			DoGet( "smart2/ComplexBindExcludePrice.rails", "order.name=hammett", "order.itemcount=11", "order.price=20", "person.id=1", "person.contact.email=x", "person.contact.phone=y" );
			// This still includes zero in place of price due to 
			// the ToString() method of the domain object and price being of type float
			String expected = "incoming hammett 11 0 1 x y";

			AssertReplyEqualTo( expected );
		}

		[Test]
		public void ComplexBindExcludeName()
		{
			DoGet( "smart2/ComplexBindExcludeName.rails", "order.name=hammett", "order.itemcount=11", "order.price=20", "person.id=1", "person.contact.email=x", "person.contact.phone=y" );
			// This includes an extra space because of the ToString() 
			// method of the domain object and the custom String.Format it contains.
			String expected = "incoming  11 20 1 x y";

			AssertReplyEqualTo( expected );
		}

		[Test]
		public void ComplexBindWithPrefix()
		{
			DoGet("smart2/ComplexBindWithPrefix.rails", "order.name=hammett", "order.itemcount=11", "order.price=20", "person.id=1", "person.contact.email=x", "person.contact.phone=y");
			String expected = "incoming hammett 11 20 1 x y";

			AssertReplyEqualTo(expected);
		}

		[Test]
		public void FillingBehavior1()
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			
			DoGet("smart2/FillingBehavior.rails", "abc.name=someone", "abc.date1day=11", "abc.date1month=10", "abc.date1year=2005");
			
			String expected = "incoming someone " + new DateTime( 2005, 10, 11 ).ToShortDateString() + " " + 
				DateTime.Now.AddDays(1).ToShortDateString();

			AssertReplyEqualTo(expected);
		}

		[Test]
		public void FillingBehavior2()
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			
			DoGet("smart2/FillingBehavior.rails");
			
			String expected = "incoming hammett " + 
				DateTime.Now.ToShortDateString() + " " + DateTime.Now.AddDays(1).ToShortDateString();

			AssertReplyEqualTo(expected);
		}

		[Test]
		public void NullableAndDataBind()
		{
			DoGet("smart2/NullableConversion2.rails", "mov.name=hammett", "mov.amount=11");
			String expected = "incoming hammett 11";

			AssertReplyEqualTo(expected);
		}

		[Test]
		public void NullableAndDataBind2()
		{
			DoGet("smart2/NullableConversion2.rails", "mov.name=hammett");
			String expected = "incoming hammett ";

			AssertReplyEqualTo(expected);
		}

		[Test]
		public void ArrayBinding1()
		{
			DoGet("smart2/ArrayBinding.rails", "user.name=hammett", "user.roles=1", "user.roles=2", "user.permissions=10", "user.permissions=11");
			String expected = "User hammett 2 2 1 2 10 11";

			AssertReplyEqualTo(expected);
		}

		[Test]
		public void BugReportedOnForum1()
		{
			DoGet("smart2/CalculateUtilizationByDay.rails", "tp1.Hour=1", "tp1.Minute=2", "tp1.Second=3", "tp2.Hour=10", "tp2.Minute=20", "tp2.Second=30");
			String expected = " 1:2:3  10:20:30 ";

			AssertReplyEqualTo(expected);
		}
	}
}
