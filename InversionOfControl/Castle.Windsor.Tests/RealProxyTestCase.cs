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

namespace Castle.Windsor.Tests
{
	using System;
	using System.Runtime.Remoting;
	using NUnit.Framework;

	using Castle.Windsor.Proxy;
	using Castle.Windsor.Tests.Components;

	[TestFixture]
	public class RealProxyTestCase
	{
		private IWindsorContainer _container;

		[SetUp]
		public void Init()
		{
			_container = new WindsorContainer( new RealProxyProxyFactory() );

			_container.AddFacility( "1", new MyInterceptorGreedyFacility() );
		}

		[TearDown]
		public void Terminate()
		{
			_container.Dispose();
		}

		[Test]
		public void InterfaceProxy()
		{
			_container.AddComponent( "interceptor", typeof(ResultModifierInterceptor) );
			_container.AddComponent( "key", typeof(ICalcService), typeof(MarshalCalculatorService)  );

			ICalcService service = (ICalcService) _container.Resolve("key");

			Assert.IsNotNull(service);
			Assert.IsTrue(RemotingServices.IsTransparentProxy(service));
			Assert.AreEqual( 5, service.Sum(2,2) );
		}

		[Test]
		public void ConcreteClassProxy()
		{
			_container.AddComponent( "interceptor", typeof(ResultModifierInterceptor) );
			_container.AddComponent( "key", typeof(MarshalCalculatorService), typeof(MarshalCalculatorService)  );

			MarshalCalculatorService service = (MarshalCalculatorService) 
				_container.Resolve("key");

			Assert.IsNotNull(service);
			Assert.IsTrue(RemotingServices.IsTransparentProxy(service));
			Assert.AreEqual( 5, service.Sum(2,2) );
		}

		[Test]
		public void InterfaceProxyWithLifecycle()
		{
			_container.AddComponent( "interceptor", typeof(ResultModifierInterceptor) );
			_container.AddComponent( "key", 
				typeof(ICalcService), typeof(CalculatorServiceWithLifecycle)  );

			ICalcService service = (ICalcService) _container.Resolve("key");

			Assert.IsNotNull(service);
			Assert.IsTrue(RemotingServices.IsTransparentProxy(service));
			Assert.IsTrue( service.Initialized );
			Assert.AreEqual( 5, service.Sum(2,2) );

			Assert.IsFalse( service.Disposed );

			_container.Release( service );
		}

		[Test]
		public void ClassProxyWithAttributes()
		{
			_container = new WindsorContainer(); // So we wont use the facilities

			_container.AddComponent( "interceptor", typeof(ResultModifierInterceptor) );
			_container.AddComponent( "key", typeof(CalculatorServiceWithAttributes)  );

			CalculatorServiceWithAttributes service = 
				(CalculatorServiceWithAttributes) _container.Resolve("key");

			Assert.IsNotNull(service);
			Assert.IsTrue(RemotingServices.IsTransparentProxy(service));
			Assert.AreEqual( 5, service.Sum(2,2) );
		}
	}
}
