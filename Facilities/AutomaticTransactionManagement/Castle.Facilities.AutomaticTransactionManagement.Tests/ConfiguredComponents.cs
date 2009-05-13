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

namespace Castle.Facilities.AutomaticTransactionManagement.Tests
{
	using System.Configuration;

	using Castle.MicroKernel.Facilities;
	using Castle.Windsor;

	using NUnit.Framework;

	[TestFixture]
	public class ConfiguredComponents
	{
		[Test, ExpectedException(typeof(FacilityException), ExpectedMessage = "The class Castle.Facilities.AutomaticTransactionManagement.Tests.TransactionalComp1 has configured transaction in a child node but has not specified istransaction=\"true\" on the component node.")]
		public void IsTransactionalMissing()
		{
			new WindsorContainer( ConfigHelper.ResolvePath("../IsTransactionalMissing.xml" ));
		}

		[Test]
		public void HasIsTransactionalButNothingIsConfigured()
		{
			WindsorContainer container = new WindsorContainer( ConfigHelper.ResolvePath("../HasIsTransactionalButNothingIsConfigured.xml" ));

			TransactionMetaInfoStore metaInfoStore = (TransactionMetaInfoStore) 
				container[typeof(TransactionMetaInfoStore)];

			TransactionMetaInfo meta = metaInfoStore.GetMetaFor(typeof(TransactionalComp1));
			Assert.IsNull(meta);
		}

		[Test]
		public void HasConfiguration()
		{
			WindsorContainer container = new WindsorContainer( ConfigHelper.ResolvePath("../HasConfiguration.xml") );

			TransactionMetaInfoStore metaInfoStore = (TransactionMetaInfoStore) 
				container[typeof(TransactionMetaInfoStore)];

			TransactionMetaInfo meta = metaInfoStore.GetMetaFor(typeof(TransactionalComp1));
			Assert.IsNotNull(meta);
			Assert.AreEqual(3, meta.Methods.Length);
		}

		[Test, ExpectedException(typeof(ConfigurationErrorsException))]
		public void HasInvalidMethod()
		{
			new WindsorContainer( ConfigHelper.ResolvePath("../HasInvalidMethod.xml") );
		}

		[Test]
		public void ValidConfigForInheritedMethods()
		{
			WindsorContainer container = new WindsorContainer( ConfigHelper.ResolvePath("../ValidConfigForInheritedMethods.xml") );

			TransactionMetaInfoStore metaInfoStore = (TransactionMetaInfoStore) 
				container[typeof(TransactionMetaInfoStore)];

			TransactionMetaInfo meta = metaInfoStore.GetMetaFor(typeof(TransactionalComp2));
			Assert.IsNotNull(meta);
			Assert.AreEqual(4, meta.Methods.Length);
		}

		[Test]
		public void ConfigForServiceWithInterface()
		{
			WindsorContainer container = new WindsorContainer( ConfigHelper.ResolvePath("../ConfigForServiceWithInterface.xml") );

			TransactionMetaInfoStore metaInfoStore = (TransactionMetaInfoStore) 
				container[typeof(TransactionMetaInfoStore)];

			TransactionMetaInfo meta = metaInfoStore.GetMetaFor(typeof(TransactionalComp3));
			Assert.IsNotNull(meta);
			Assert.AreEqual(2, meta.Methods.Length);
		}
	}
}
