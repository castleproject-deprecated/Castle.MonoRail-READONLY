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
	using System.Collections;
	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Tests.GenClasses;
	using Castle.DynamicProxy.Tests.GenInterfaces;
	using Castle.DynamicProxy.Tests.Interceptors;
	using NUnit.Framework;


	[TestFixture]
	public class GenericInterfaceProxyTestCase : BasePEVerifyTestCase
	{
		private LogInvocationInterceptor logger;

		[SetUp]
		public override void Init()
		{
			base.Init();

			logger = new LogInvocationInterceptor();
		}

		[Test]
		public void ProxyWithGenericArgument()
		{
			GenInterface<int> proxy =
				generator.CreateInterfaceProxyWithTarget<GenInterface<int>>(
					new GenInterfaceImpl<int>(), logger);

			Assert.IsNotNull(proxy);

			Assert.AreEqual(1, proxy.DoSomething(1));

			Assert.AreEqual("DoSomething ", logger.LogContents);
		}

		[Test]
		public void ProxyWithGenericArgumentAndGenericMethod()
		{
			GenInterfaceWithGenMethods<int> proxy =
				generator.CreateInterfaceProxyWithTarget<GenInterfaceWithGenMethods<int>>(
					new GenInterfaceWithGenMethodsImpl<int>(), logger);

			Assert.IsNotNull(proxy);

			proxy.DoSomething<long>(10L, 1);

			Assert.AreEqual("DoSomething ", logger.LogContents);
		}

		[Test]
		public void ProxyWithGenericArgumentAndGenericMethodAndGenericReturn()
		{
			GenInterfaceWithGenMethodsAndGenReturn<int> proxy =
				generator.CreateInterfaceProxyWithTarget<GenInterfaceWithGenMethodsAndGenReturn<int>>(
					new GenInterfaceWithGenMethodsAndGenReturnImpl<int>(), logger);

			Assert.IsNotNull(proxy);

			Assert.AreEqual(10L, proxy.DoSomething<long>(10L, 1));

			Assert.AreEqual("DoSomething ", logger.LogContents);
		}

		[Test]
		public void ProxyWithGenInterfaceWithGenericArrays()
		{
			IGenInterfaceWithGenArray<int> proxy =
				generator.CreateInterfaceProxyWithTarget<IGenInterfaceWithGenArray<int>>(
					new GenInterfaceWithGenArray<int>(), logger);

			Assert.IsNotNull(proxy);

			int[] items = new int[] {1, 2, 3};
			proxy.CopyTo(items);
			items = proxy.CreateItems();
			Assert.IsNotNull(items);
			Assert.AreEqual(3, items.Length);

			Assert.AreEqual("CopyTo CreateItems ", logger.LogContents);
		}

		[Test]
		public void ProxyWithGenInterfaceWithBase()
		{
			IGenInterfaceHierarchySpecialization<int> proxy =
				generator.CreateInterfaceProxyWithTarget<IGenInterfaceHierarchySpecialization<int>>(
					new GenInterfaceHierarchy<int>(), logger);

			Assert.IsNotNull(proxy);

			proxy.Add();
			proxy.Add(1);
			Assert.IsNotNull(proxy.FetchAll());

			Assert.AreEqual("Add Add FetchAll ", logger.LogContents);
		}

		[Test]
		[ExpectedException(typeof(GeneratorException),
			"DynamicProxy cannot create an interface (with target) proxy for 'InterfaceWithExplicitImpl`1' as the target 'GenExplicitImplementation`1' has an explicit implementation of one of the methods exposed by the interface. The runtime prevents use from invoking the private method on the target. Method Castle.DynamicProxy.Tests.GenInterfaces.InterfaceWithExplicitImpl<T>.GetEnum1"
			)]
		public void ProxyWithGenExplicitImplementation()
		{
			generator.CreateInterfaceProxyWithTarget<InterfaceWithExplicitImpl<int>>(
				new GenExplicitImplementation<int>(), logger);
		}

		[Test]
		public void TwoGenericsInterfaceWithoutTarget()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(GenInterface<object>),
			                                            new Type[] {typeof(InterfaceWithExplicitImpl<int>)},
			                                            new LogInvocationInterceptor());
		}

		[Test, Ignore("[MD]: Error: Method has a duplicate, token=0x06000006. [token:0x06000005]")]
		public void NonGenInterfaceWithParentGenClassImplementingGenInterface()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(IUserRepository),
			                                            new Type[] {typeof(InterfaceWithExplicitImpl<int>)},
			                                            new LogInvocationInterceptor());
		}

		[Test, Ignore("[MD]: Error: Method has a duplicate, token=0x06000006. [token:0x06000005]")]
		public void WithoutTarget()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(InterfaceWithExplicitImpl<int>), new LogInvocationInterceptor());
		}

		[Test]
		public void MethodInfoClosedInGenIfcGenMethodRefTypeNoTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			GenInterfaceWithGenMethods<ArrayList> proxy =
				generator.CreateInterfaceProxyWithoutTarget<GenInterfaceWithGenMethods<ArrayList>>(interceptor);

			proxy.DoSomething(1, null);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void), typeof(int),
			                                           typeof(ArrayList));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomething(new Hashtable(), new ArrayList());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void),
			                                           typeof(Hashtable), typeof(ArrayList));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		public void MethodInfoClosedInGenIfGenMethodValueTypeNoTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			GenInterfaceWithGenMethods<int> proxy =
				generator.CreateInterfaceProxyWithoutTarget<GenInterfaceWithGenMethods<int>>(interceptor);

			proxy.DoSomething(1, 1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void), typeof(int),
			                                           typeof(int));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomething(new Hashtable(), 1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void),
			                                           typeof(Hashtable), typeof(int));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		public void MethodInfoClosedInGenIfcNongenMethodRefTypeNoTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			IGenInterfaceHierarchyBase<ArrayList> proxy =
				generator.CreateInterfaceProxyWithoutTarget<IGenInterfaceHierarchyBase<ArrayList>>(interceptor);

			proxy.Get();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(ArrayList));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.Add(null);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void),
			                                           typeof(ArrayList));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test, Ignore("[MD]: Error: Method has a duplicate, token=0x06000009. [token:0x06000005]")]
		public void MethodInfoClosedInGenIfcNongenMethodValueTypeNoTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			IGenInterfaceHierarchyBase<int> proxy =
				generator.CreateInterfaceProxyWithoutTarget<IGenInterfaceHierarchyBase<int>>(interceptor);

			proxy.Get();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.Add(0);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void), typeof(int));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		public void MethodInfoClosedInNongenIfcGenMethodNoTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			OnlyGenMethodsInterface proxy = generator.CreateInterfaceProxyWithoutTarget<OnlyGenMethodsInterface>(interceptor);

			proxy.DoSomething(1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int), typeof(int));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomething(new Hashtable());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(Hashtable),
			                                           typeof(Hashtable));
			Assert.AreEqual(interceptor.Invocation.GetConcreteMethod(),
			                interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		public void MethodInfoClosedInGenIfcGenMethodRefTypeWithTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			GenInterfaceWithGenMethods<ArrayList> target = new GenInterfaceWithGenMethodsImpl<ArrayList>();
			GenInterfaceWithGenMethods<ArrayList> proxy =
				generator.CreateInterfaceProxyWithTarget<GenInterfaceWithGenMethods<ArrayList>>(target, interceptor);

			proxy.DoSomething(1, null);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void), typeof(int),
			                                           typeof(ArrayList));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(void),
			                                           typeof(int), typeof(ArrayList));
			Assert.AreNotEqual(interceptor.Invocation.GetConcreteMethod(),
			                   interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomething(new Hashtable(), new ArrayList());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void),
			                                           typeof(Hashtable), typeof(ArrayList));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(void),
			                                           typeof(Hashtable), typeof(ArrayList));
			Assert.AreNotEqual(interceptor.Invocation.GetConcreteMethod(),
			                   interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		public void MethodInfoClosedInGenIfcGenMethodValueTypeWithTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			GenInterfaceWithGenMethods<int> target = new GenInterfaceWithGenMethodsImpl<int>();
			GenInterfaceWithGenMethods<int> proxy =
				generator.CreateInterfaceProxyWithTarget<GenInterfaceWithGenMethods<int>>(target, interceptor);

			proxy.DoSomething(1, 1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void), typeof(int),
			                                           typeof(int));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(void),
			                                           typeof(int), typeof(int));
			Assert.AreNotEqual(interceptor.Invocation.GetConcreteMethod(),
			                   interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomething(new Hashtable(), 1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void),
			                                           typeof(Hashtable), typeof(int));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(void),
			                                           typeof(Hashtable), typeof(int));
			Assert.AreNotEqual(interceptor.Invocation.GetConcreteMethod(),
			                   interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		public void MethodInfoClosedInGenIfcNongenMethodRefTypeWithTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			IGenInterfaceHierarchyBase<ArrayList> target = new GenInterfaceHierarchy<ArrayList>();
			IGenInterfaceHierarchyBase<ArrayList> proxy =
				generator.CreateInterfaceProxyWithTarget<IGenInterfaceHierarchyBase<ArrayList>>(target, interceptor);

			proxy.Add(null);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void),
			                                           typeof(ArrayList));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(void),
			                                           typeof(ArrayList));
			Assert.AreNotEqual(interceptor.Invocation.GetConcreteMethod(),
			                   interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.Get();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(ArrayList));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(),
			                                           typeof(ArrayList));
			Assert.AreNotEqual(interceptor.Invocation.GetConcreteMethod(),
			                   interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test, Ignore("[MD]: Error: Method has a duplicate, token=0x06000009. [token:0x06000005]")]
		public void MethodInfoClosedInGenIfcNongenMethodValueTypeWithTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			IGenInterfaceHierarchyBase<int> target = new GenInterfaceHierarchy<int>();
			IGenInterfaceHierarchyBase<int> proxy =
				generator.CreateInterfaceProxyWithTarget<IGenInterfaceHierarchyBase<int>>(target, interceptor);

			proxy.Add(0);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(void), typeof(int));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(void),
			                                           typeof(int));
			Assert.AreNotEqual(interceptor.Invocation.GetConcreteMethod(),
			                   interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.Get();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(int));
			Assert.AreNotEqual(interceptor.Invocation.GetConcreteMethod(),
			                   interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		public void MethodInfoClosedInNongenIfcGenMethodWithTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			OnlyGenMethodsInterface target = new OnlyGenMethodsInterfaceImpl();
			OnlyGenMethodsInterface proxy =
				generator.CreateInterfaceProxyWithTarget<OnlyGenMethodsInterface>(target, interceptor);

			proxy.DoSomething(1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int), typeof(int));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(), typeof(int),
			                                           typeof(int));
			Assert.AreNotEqual(interceptor.Invocation.GetConcreteMethod(),
			                   interceptor.Invocation.GetConcreteMethodInvocationTarget());

			proxy.DoSomething(new Hashtable());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(Hashtable),
			                                           typeof(Hashtable));
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethodInvocationTarget(),
			                                           typeof(Hashtable), typeof(Hashtable));
			Assert.AreNotEqual(interceptor.Invocation.GetConcreteMethod(),
			                   interceptor.Invocation.GetConcreteMethodInvocationTarget());
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ThrowsWhenProxyingGenericTypeDefNoTarget()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			object o = generator.CreateInterfaceProxyWithoutTarget(typeof(IGenInterfaceHierarchyBase<>), interceptor);
		}

		[Test]
		public void UsingGenericConstraintOnGenericMethod()
		{
			SkipCallingMethodInterceptor interceptor = new SkipCallingMethodInterceptor();
			IHaveGenericMethod test =
				(IHaveGenericMethod) generator.CreateInterfaceProxyWithoutTarget(typeof(IHaveGenericMethod), interceptor);
			test.Method2<string>("");
		}
	}

	public interface IHaveGenericMethod
	{
		void Method2<T2>(T2 t2) where T2 : class;
	}
}