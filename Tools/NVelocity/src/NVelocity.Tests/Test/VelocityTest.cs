namespace NVelocity.Test
{
	using System;
	using System.Collections;
	using System.IO;
	using NUnit.Framework;
	using NVelocity.App;

	/// <summary>
	/// Test Velocity processing
	/// </summary>
	[TestFixture]
	public class VelocityTest
	{
		[Test]
		public void Test_Evaluate()
		{
			VelocityContext c = new VelocityContext();
			c.Put("key", "value");
			c.Put("firstName", "Cort");
			c.Put("lastName", "Schaefer");
			Hashtable h = new Hashtable();
			h.Add("foo", "bar");
			c.Put("hashtable", h);

			AddressData address = new AddressData();
			address.Address1 = "9339 Grand Teton Drive";
			address.Address2 = "Office in the back";
			c.Put("address", address);

			ContactData contact = new ContactData();
			contact.Name = "Cort";
			contact.Address = address;
			c.Put("contact", contact);

			// test simple objects (no nesting)
			StringWriter sw = new StringWriter();
			bool ok = Velocity.Evaluate(c, sw, "", "$firstName is my first name, my last name is $lastName");
			Assert.IsTrue(ok, "Evalutation returned failure");
			String s = sw.ToString();
			Assert.AreEqual("Cort is my first name, my last name is Schaefer", s, "test simple objects (no nesting)");

			// test nested object
			sw = new StringWriter();
			String template = "These are the individual properties:\naddr1=9339 Grand Teton Drive\naddr2=Office in the back";
			ok = Velocity.Evaluate(c, sw, "", template);
			Assert.IsTrue(ok, "Evalutation returned failure");
			s = sw.ToString();
			Assert.IsFalse(String.Empty.Equals(s), "test nested object");

			// test hashtable
			sw = new StringWriter();
			template = "Hashtable lookup: foo=$hashtable.foo";
			ok = Velocity.Evaluate(c, sw, "", template);
			Assert.IsTrue(ok, "Evalutation returned failure");
			s = sw.ToString();
			Assert.AreEqual("Hashtable lookup: foo=bar", s, "Evaluation did not evaluate right");

			// test nested properties
			//    	    sw = new StringWriter();
			//	    template = "These are the nested properties:\naddr1=$contact.Address.Address1\naddr2=$contact.Address.Address2";
			//	    ok = Velocity.Evaluate(c, sw, "", template);
			//	    Assert("Evalutation returned failure", ok);
			//	    s = sw.ToString();
			//	    Assert("test nested properties", s.Equals("These are the nested properties:\naddr1=9339 Grand Teton Drive\naddr2=Office in the back"));

			// test key not found in context
			sw = new StringWriter();
			template = "$!NOT_IN_CONTEXT";
			ok = Velocity.Evaluate(c, sw, "", template);
			Assert.IsTrue(ok, "Evalutation returned failure");
			s = sw.ToString();
			Assert.AreEqual(String.Empty, s, "test key not found in context");

			// test nested properties where property not found
			//	    sw = new StringWriter();
			//	    template = "These are the non-existent nested properties:\naddr1=$contact.Address.Address1.Foo\naddr2=$contact.Bar.Address.Address2";
			//	    ok = Velocity.Evaluate(c, sw, "", template);
			//	    Assert("Evalutation returned failure", ok);
			//	    s = sw.ToString();
			//	    Assert("test nested properties where property not found", s.Equals("These are the non-existent nested properties:\naddr1=\naddr2="));
		}


		// inner classes to support tests --------------------------

		public class ContactData
		{
			private String name = String.Empty;
			private AddressData address = new AddressData();

			public String Name
			{
				get { return name; }
				set { name = value; }
			}

			public AddressData Address
			{
				get { return address; }
				set { address = value; }
			}
		}

		public class AddressData
		{
			private String address1 = String.Empty;
			private String address2 = String.Empty;

			public String Address1
			{
				get { return this.address1; }
				set { this.address1 = value; }
			}

			public String Address2
			{
				get { return this.address2; }
				set { this.address2 = value; }
			}
		}
	}
}