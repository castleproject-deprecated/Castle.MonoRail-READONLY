// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.Remoting.Tests
{
	using System;
	using System.Runtime.Remoting;

	using Castle.Windsor;
	using Castle.Facilities.Remoting.TestComponents;

	using NUnit.Framework;


	[TestFixture, Serializable]
	public class RemoteSingletonTestCase : AbstractRemoteTestCase
	{
		protected override String GetServerConfigFile()
		{
			return BuildConfigPath("server_simple_scenario.xml");
		}

		[Test]
		public void CommonAppConsumingRemoteComponents()
		{
			clientDomain.DoCallBack(new CrossAppDomainDelegate(CommonAppConsumingRemoteComponentsCallback));
		}

		public void CommonAppConsumingRemoteComponentsCallback()
		{
			ICalcService service = (ICalcService) 
				Activator.GetObject( typeof(ICalcService), "tcp://localhost:2133/calcservice.rem" );

			Assert.IsTrue( RemotingServices.IsTransparentProxy( service ) );
			Assert.IsTrue( RemotingServices.IsObjectOutOfAppDomain(service) );

			Assert.AreEqual(10, service.Sum(7,3));
		}

		[Test]
		public void ClientContainerConsumingRemoteComponent()
		{
			clientDomain.DoCallBack(new CrossAppDomainDelegate(ClientContainerConsumingRemoteComponentCallback));
		}

		public void ClientContainerConsumingRemoteComponentCallback()
		{
			IWindsorContainer clientContainer = CreateRemoteContainer(clientDomain, BuildConfigPath("client_simple_scenario.xml"));

			ICalcService service = (ICalcService) clientContainer[ typeof(ICalcService) ];

			Assert.IsTrue( RemotingServices.IsTransparentProxy(service) );
			Assert.IsTrue( RemotingServices.IsObjectOutOfAppDomain(service) );

			Assert.AreEqual(10, service.Sum(7,3));
		}

		[Test]
		public void WiringRemoteComponent()
		{
			clientDomain.DoCallBack(new CrossAppDomainDelegate(WiringRemoteComponentCallback));
		}

		public void WiringRemoteComponentCallback()
		{
			IWindsorContainer clientContainer = CreateRemoteContainer(clientDomain, BuildConfigPath("client_simple_scenario.xml"));

			clientContainer.AddComponent("comp", typeof(ConsumerComp));

			ConsumerComp service = (ConsumerComp) clientContainer[ typeof(ConsumerComp) ];

			Assert.IsNotNull( service.Calcservice );
			Assert.IsTrue( RemotingServices.IsTransparentProxy(service.Calcservice) );
			Assert.IsTrue( RemotingServices.IsObjectOutOfAppDomain(service.Calcservice) );
		}
	}
}
