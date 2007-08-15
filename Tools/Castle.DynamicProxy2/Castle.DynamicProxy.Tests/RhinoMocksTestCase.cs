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

using System.Runtime.CompilerServices;
[assembly :
	InternalsVisibleTo(
		"DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7"
		)]

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Runtime.InteropServices;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Tests.Interceptors;
	using NUnit.Framework;

	[TestFixture]
	public class RhinoMocksTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void GenericClassWithGenericMethod()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();
			IDoubleGeneric<int> proxy =
				(IDoubleGeneric<int>) generator.CreateInterfaceProxyWithTarget(typeof(IDoubleGeneric<int>),
				                                                               new DoubleGenericImpl<int>(), logger);
			proxy.Call<string>(1, "");
			Assert.AreEqual("Call", logger.Invocations[0]);
		}

		[Test]
		public void GenericClassWithGenericMethodWitoutTarget()
		{
			BasicInterfaceProxyWithoutTargetTestCase.ReturnThreeInterceptor interceptor =
				new BasicInterfaceProxyWithoutTargetTestCase.ReturnThreeInterceptor();
			IDoubleGeneric<int> proxy =
				(IDoubleGeneric<int>) generator.CreateInterfaceProxyWithoutTarget(typeof(IDoubleGeneric<int>),
				                                                                  interceptor);
			object o = proxy.Call<string>(1, "");
			Assert.AreEqual(3, o);
		}

		[Test]
		public void UsingEvents_Interface()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			IWithEvents proxy = (IWithEvents) generator.CreateInterfaceProxyWithTarget(typeof(IWithEvents),
			                                                                           new FakeWithEvents(),
			                                                                           logger);

			Assert.IsNotNull(proxy);

			proxy.Foo += null;
			proxy.Foo -= null;

			Assert.AreEqual(2, logger.Invocations.Count);
		}

		[Test]
		public void UsingEvents_Class()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();
			FakeWithEvents proxy = (FakeWithEvents) generator.CreateClassProxy(
			                                        	typeof(FakeWithEvents),
			                                        	ProxyGenerationOptions.Default,
			                                        	logger);

			Assert.IsNotNull(proxy);

			proxy.Foo += null;
			proxy.Foo -= null;

			Assert.AreEqual(2, logger.Invocations.Count);
		}

		[Test, Ignore("I dont think the effort to fix this edge case is worthwhile")]
		public void NeedingToCreateNewMethodTableSlot()
		{
			generator.CreateClassProxy(typeof(MultiClass), new Type[] {typeof(ISpecialMulti)});
		}

		[Test]
		public void ProxyingInterfaceWithGuid()
		{
			object o = generator.CreateInterfaceProxyWithoutTarget(typeof(IWithGuid), new StandardInterceptor());
			Assert.IsNotNull(o);
		}

		[Test]
		public void ProxyingInternalInterface()
		{
			object o = generator.CreateInterfaceProxyWithoutTarget(typeof(IFoo), new StandardInterceptor());
			Assert.IsNotNull(o);
		}

		[Test]
		public void CanProxyDataSet()
		{
			generator.CreateClassProxy(typeof(DataSet), new Type[0], new StandardInterceptor());
		}

		[Test]
		public void ProxyingComInteraces()
		{
			object o = generator
				.CreateInterfaceProxyWithoutTarget(typeof(IComServiceProvider), new StandardInterceptor());
			Assert.IsNotNull(o);
		}

		[Test]
		public void ProxyingGenericClassWithGenericClassConstraint()
		{
			object o = generator
				.CreateInterfaceProxyWithoutTarget(typeof(IFactory2), new StandardInterceptor());
			Assert.IsNotNull(o);
		}

		[Test]
		public void ProxyInternalMethod()
		{
			LogInvocationInterceptor logging = new LogInvocationInterceptor();
			WithInternalMethod o = (WithInternalMethod) generator.CreateClassProxy(typeof(WithInternalMethod),
			                                                                       new Type[0], logging);
			o.Foo();
			Assert.AreEqual("Foo ", logging.LogContents);
		}

		[Test]
		public void ProxyingProtectedInternalAbstractMethod()
		{
			LogInvocationInterceptor logging = new LogInvocationInterceptor();
			SomeClassWithProtectedInternalAbstractClass o =
				(SomeClassWithProtectedInternalAbstractClass)
				generator.CreateClassProxy(typeof(SomeClassWithProtectedInternalAbstractClass),
				                           new Type[0], logging);
			Assert.IsNotNull(o);
		}

		[Test]
		public void VirtualMethodCallsFromTheConstructor()
		{
			LogInvocationInterceptor logging = new LogInvocationInterceptor();
			MakeVirtualCallFromCtor o = (MakeVirtualCallFromCtor) generator.CreateClassProxy(typeof(MakeVirtualCallFromCtor),
			                                                                                 new Type[0], logging);
			Assert.AreEqual(1, logging.Invocations.Count);
			Assert.IsNotNull(o);
		}

		[Test]
		public void InternalClassWithInternalMethodAndProperty()
		{
			LogInvocationInterceptor logging = new LogInvocationInterceptor();
			InternalClassWithInternalMembers o =
				(InternalClassWithInternalMembers) generator.CreateClassProxy(typeof(InternalClassWithInternalMembers),
				                                                              new Type[0], logging);
			Assert.IsNotNull(o);
			o.TestMethod();
			Assert.AreEqual(1, logging.Invocations.Count);
			string t = o.TestProperty;
			Assert.AreEqual(2, logging.Invocations.Count);
		}

		public interface IWithEvents
		{
			event EventHandler Foo;
		}

		public class FakeWithEvents : IWithEvents
		{
			public virtual event EventHandler Foo;

			public void MethodToGetAroundFooNotUsedError()
			{
				Foo(this, EventArgs.Empty);
			}
		}

		public interface IMulti
		{
			void OriginalMethod1();
			void OriginalMethod2();
		}

		public class MultiClass : IMulti
		{
			// NON-virtual method
			public void OriginalMethod1()
			{
			}

			// VIRTUAL method
			public virtual void OriginalMethod2()
			{
			}
		}

		public interface ISpecialMulti : IMulti
		{
			void ExtraMethod();
		}

		[Guid("AFCF8240-61AF-4089-BD98-3CD4D719EDBA")]
		public interface IWithGuid
		{
		}
	}

	public class WithInternalMethod
	{
		internal virtual void Foo()
		{
		}
	}

	public interface IDoubleGeneric<One>
	{
		object Call<T>(One one, T two);
	}

	internal interface IFoo
	{
		int Bar();
	}

	public class DoubleGenericImpl<One> : IDoubleGeneric<One>
	{
		public object Call<T>(One one, T two)
		{
			return two;
		}
	}

	[ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"),
	 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IComServiceProvider
	{
		[return : MarshalAs(UnmanagedType.IUnknown)]
		object QueryService([In] ref Guid guidService, [In] ref Guid riid);
	}

	public interface IFactory2
	{
		T Create<T>() where T : List<T>;
	}

	public abstract class
		SomeClassWithProtectedInternalAbstractClass
	{
		protected internal abstract void Quack();
	}

	internal class InternalClassWithInternalMembers
	{
		internal virtual string TestMethod()
		{
			return "TestMethod";
		}

		internal virtual string TestProperty
		{
			get { return "TestProperty"; }
		}
	}

	public class MakeVirtualCallFromCtor
	{
		private string name;

		public MakeVirtualCallFromCtor()
		{
			Name = "Blah";
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}