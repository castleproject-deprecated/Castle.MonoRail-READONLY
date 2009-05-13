// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests
{
	using System.IO;
	using System.Reflection;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Tests.Classes;
	using NUnit.Framework;

	[TestFixture]
	public class ClassWithAttributesTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void EnsureProxyHasAttributesOnClassAndMethods()
		{
			AttributedClass instance = (AttributedClass)
			                           generator.CreateClassProxy(typeof (AttributedClass), new StandardInterceptor());

			object[] attributes = instance.GetType().GetCustomAttributes(typeof (NonInheritableAttribute), false);
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOfType(typeof (NonInheritableAttribute), attributes[0]);

			attributes = instance.GetType().GetMethod("Do1").GetCustomAttributes(typeof (NonInheritableAttribute), false);
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOfType(typeof (NonInheritableAttribute), attributes[0]);
		}

		[Test]
		public void EnsureProxyHasAttributesOnClassAndMethods_ComplexAttributes()
		{
			AttributedClass2 instance = (AttributedClass2)
			                            generator.CreateClassProxy(typeof (AttributedClass2), new StandardInterceptor());

			object[] attributes = instance.GetType().GetCustomAttributes(typeof (ComplexNonInheritableAttribute), false);
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOfType(typeof (ComplexNonInheritableAttribute), attributes[0]);
			ComplexNonInheritableAttribute att = (ComplexNonInheritableAttribute) attributes[0];
			// (1, 2, true, "class", FileAccess.Write)
			Assert.AreEqual(1, att.Id);
			Assert.AreEqual(2, att.Num);
			Assert.AreEqual(true, att.IsSomething);
			Assert.AreEqual("class", att.Name);
			Assert.AreEqual(FileAccess.Write, att.Access);

			attributes = instance.GetType().GetMethod("Do1").GetCustomAttributes(typeof (ComplexNonInheritableAttribute), false);
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOfType(typeof (ComplexNonInheritableAttribute), attributes[0]);
			att = (ComplexNonInheritableAttribute) attributes[0];
			// (2, 3, "Do1", Access = FileAccess.ReadWrite)
			Assert.AreEqual(2, att.Id);
			Assert.AreEqual(3, att.Num);
			Assert.AreEqual(false, att.IsSomething);
			Assert.AreEqual("Do1", att.Name);
			Assert.AreEqual(FileAccess.ReadWrite, att.Access);

			attributes = instance.GetType().GetMethod("Do2").GetCustomAttributes(typeof (ComplexNonInheritableAttribute), false);
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOfType(typeof (ComplexNonInheritableAttribute), attributes[0]);
			att = (ComplexNonInheritableAttribute) attributes[0];
			// (3, 4, "Do2", IsSomething=true)
			Assert.AreEqual(3, att.Id);
			Assert.AreEqual(4, att.Num);
			Assert.AreEqual(true, att.IsSomething);
			Assert.AreEqual("Do2", att.Name);
		}

		[Test]
		public void EnsureProxyHasAttributesOnProperties()
		{
			AttributedClass proxy = generator.CreateClassProxy<AttributedClass>();
			PropertyInfo nameProperty = proxy.GetType().GetProperty("Name");
			Assert.IsTrue(nameProperty.IsDefined(typeof(NonInheritableAttribute), false));
		}
	}
}