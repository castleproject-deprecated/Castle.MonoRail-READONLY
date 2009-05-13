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

namespace Castle.Windsor.Tests
{
	using System.Runtime.Remoting;
	using Castle.Core.Interceptor;
	using Castle.Windsor.Tests.Components;
	using NUnit.Framework;

	[TestFixture]
	public class SmartProxyTestCase
	{
		private IWindsorContainer _container;

		public SmartProxyTestCase()
		{
		}

		[SetUp]
		public void Init()
		{
			_container = new WindsorContainer();

			_container.AddFacility("1", new MyInterceptorGreedyFacility());
			_container.AddFacility("2", new MyInterceptorGreedyFacility());
			_container.AddFacility("3", new MyInterceptorGreedyFacility());
		}

		[TearDown]
		public void Terminate()
		{
			_container.Dispose();
		}

		[Test]
		public void InterfaceInheritance()
		{
			_container.AddComponent("interceptor", typeof(StandardInterceptor));
			_container.AddComponent("key", typeof(ICameraService), typeof(CameraService));

			ICameraService service = (ICameraService) _container.Resolve("key");

			Assert.IsNotNull(service);
		}

		[Test]
		public void InterfaceProxy()
		{
			_container.AddComponent("interceptor", typeof(ResultModifierInterceptor));
			_container.AddComponent("key",
			                        typeof(ICalcService), typeof(CalculatorService));

			ICalcService service = (ICalcService) _container.Resolve("key");

			Assert.IsNotNull(service);
			Assert.IsFalse(RemotingServices.IsTransparentProxy(service));
			Assert.AreEqual(7, service.Sum(2, 2));
		}

		[Test]
		public void ConcreteClassProxy()
		{
			_container.AddComponent("interceptor", typeof(ResultModifierInterceptor));
			_container.AddComponent("key", typeof(CalculatorService));

			CalculatorService service = (CalculatorService) _container.Resolve("key");

			Assert.IsNotNull(service);
			Assert.IsFalse(RemotingServices.IsTransparentProxy(service));
			Assert.AreEqual(7, service.Sum(2, 2));
		}
	}
}