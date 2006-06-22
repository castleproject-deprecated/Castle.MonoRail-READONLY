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

namespace Castle.Facilities.Remoting.Tests
{
	using System;
	using System.Runtime.Remoting;

	using Castle.Windsor;

	using Castle.Facilities.Remoting.TestComponents;

	using NUnit.Framework;

	[TestFixture, Serializable]
	public class RemoteComponentTestCase : AbstractRemoteTestCase
	{
		protected override String GetServerConfigFile()
		{
			return "../Castle.Facilities.Remoting.Tests/server_kernelcomponent.xml";
		}

		[Test]
		public void ClientContainerConsumingRemoteComponent()
		{
			clientDomain.DoCallBack(new CrossAppDomainDelegate(ClientContainerConsumingRemoteComponentCallback));
		}

		[Test, Ignore("Fixing")]
		public void ServerRestarted()
		{
			clientDomain.DoCallBack(new CrossAppDomainDelegate(ClientContainerInvokingRemoteComponent));

			serverContainer.Dispose();
			AppDomain.Unload(serverDomain);

			serverDomain = AppDomainFactory.Create("server");
			serverContainer = CreateRemoteContainer(serverDomain, GetServerConfigFile() );

			clientDomain.DoCallBack(new CrossAppDomainDelegate(ClientContainerInvokingRemoteComponent));
		}

		[Test, Ignore("Fixing")]
		public void ClientDisposal()
		{
			IWindsorContainer clientContainer = GetClientContainer();

			clientContainer.Dispose();
		}

		public void ClientContainerConsumingRemoteComponentCallback()
		{
			IWindsorContainer clientContainer = CreateRemoteContainer(clientDomain, 
				"../Castle.Facilities.Remoting.Tests/client_kernelcomponent.xml");

			ICalcService service = (ICalcService) clientContainer[ typeof(ICalcService) ];

			Assert.IsTrue( RemotingServices.IsTransparentProxy(service) );
			Assert.IsTrue( RemotingServices.IsObjectOutOfAppDomain(service) );

			Assert.AreEqual(10, service.Sum(7,3));
		}

		public void ClientContainerInvokingRemoteComponent()
		{
			IWindsorContainer clientContainer = GetClientContainer();

			ICalcService service = (ICalcService) clientContainer[ typeof(ICalcService) ];

			Assert.IsTrue( RemotingServices.IsTransparentProxy(service) );
			Assert.IsTrue( RemotingServices.IsObjectOutOfAppDomain(service) );

			Assert.AreEqual(10, service.Sum(7,3));
		}

		private IWindsorContainer GetClientContainer()
		{
			return GetContainer(clientDomain, 
			                             "../Castle.Facilities.Remoting.Tests/client_kernelcomponent.xml");
		}
	}
}
