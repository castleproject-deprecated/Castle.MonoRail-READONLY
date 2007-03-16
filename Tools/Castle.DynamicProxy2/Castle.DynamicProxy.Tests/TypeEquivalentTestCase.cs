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

namespace Castle.DynamicProxy.Tests
{

	using System;
#if DOTNET2
	using System.Collections.Generic;
#endif
	using Castle.DynamicProxy.Generators;
	using NUnit.Framework;

	[TestFixture]
	public class TypeEquivalentTestCase
	{
		[Test]
		public void SimpleCases()
		{
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(typeof(string), typeof(string)));
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(typeof(int), typeof(int)));
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(typeof(long), typeof(long)));
			
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(typeof(string), typeof(int)));
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(typeof(int), typeof(string)));
		}

#if DOTNET2
		[Test]
		public void GenericTypeParameter()
		{
			Type[] genericArgs = typeof(Nested<,>).GetGenericArguments();

			Type T = genericArgs[0];
			Type Z = genericArgs[1];

			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(T, T));
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(T, Z));
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(Z, T));
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(typeof(int), genericArgs[0]));
		}

		[Test]
		public void GenericTypesWithGenericParameter()
		{
			Type[] genericArgs = typeof(Nested<,>).GetGenericArguments();
			
			Type T = genericArgs[0];
			Type Z = genericArgs[1];
			
			Type listOfT = typeof(List<>).MakeGenericType(T);
			Type listOfZ = typeof(List<>).MakeGenericType(Z);

			Type listOfString = typeof(List<>).MakeGenericType(typeof(String));
			Type listOfInt = typeof(List<>).MakeGenericType(typeof(int));

			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfString, listOfString));
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfInt, listOfInt));
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfT, listOfT));
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfZ, listOfZ));

			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfString, listOfInt));
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfT, listOfZ));
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfZ, listOfT));
		}
#endif

		[Test]
		public void ArrayTypes()
		{
			Array arrayOfInt1 = Array.CreateInstance(typeof(int), 1);
			Array arrayOfInt2 = Array.CreateInstance(typeof(int), 2);
			Array arrayOfStr = Array.CreateInstance(typeof(string), 2);

			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(arrayOfInt1.GetType(), arrayOfInt1.GetType()));
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(arrayOfInt1.GetType(), arrayOfInt2.GetType()));

			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(arrayOfStr.GetType(), arrayOfInt1.GetType()));
		}

#if DOTNET2
		[Test]
		public void GenericArrayTypes()
		{
			Type[] genericArgs = typeof(Nested<,>).GetGenericArguments();

			Type T = genericArgs[0];
			Type Z = genericArgs[1];

			Type arrayOfT = T.MakeArrayType();
			Type arrayOfTDiff = T.MakeArrayType();
			Type arrayOfT2 = T.MakeArrayType(2);
			Type arrayOfZ = Z.MakeArrayType();

			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(arrayOfT, arrayOfT));
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(arrayOfT, arrayOfTDiff));
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(arrayOfZ, arrayOfZ));

			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(arrayOfZ, arrayOfT));
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(arrayOfT, arrayOfT2));
		}

		public class Nested<T, Z>
		{
			
		}
	}
#endif
}
