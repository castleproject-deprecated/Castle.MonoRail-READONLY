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

namespace Castle.Facilities.NHibernateIntegration.Tests.Transactions
{
	using System;
	using Castle.Facilities.AutomaticTransactionManagement;
	using NUnit.Framework;

	[TestFixture]
	public class DistributedTransactionsTestCase : AbstractNHibernateTestCase
	{
		protected override void ConfigureContainer()
		{
			container.AddFacility("transactions", new TransactionFacility());

			container.AddComponent("root", typeof(RootService2));
			container.AddComponent("myfirstdao", typeof(FirstDao2));
			container.AddComponent("myseconddao", typeof(SecondDao2));
			container.AddComponent("myorderdao", typeof(OrderDao2));
		}

		[Test]
		public void SuccessfulSituationWithTwoDatabases()
		{
			RootService2 service = (RootService2) container["root"];
			OrderDao2 orderDao = (OrderDao2) container["myorderdao"];

			service.DoTwoDBOperation_Create(false);

			Array blogs = service.FindAll(typeof(Blog));
			Array blogitems = service.FindAll(typeof(BlogItem));
			Array orders = orderDao.FindAll(typeof(Order));

			Assert.IsNotNull(blogs);
			Assert.IsNotNull(blogitems);
			Assert.IsNotNull(orders);
			Assert.AreEqual(1, blogs.Length);
			Assert.AreEqual(1, blogitems.Length);
			Assert.AreEqual(1, orders.Length);
		}

		[Test]
		public void ExceptionOnEndWithTwoDatabases()
		{
			RootService2 service = (RootService2)container["root"];
			OrderDao2 orderDao = (OrderDao2)container["myorderdao"];

			try
			{
				service.DoTwoDBOperation_Create(true);
			}
			catch(Exception)
			{
				// Expected
			}

			Array blogs = service.FindAll(typeof(Blog));
			Array blogitems = service.FindAll(typeof(BlogItem));
			Array orders = orderDao.FindAll(typeof(Order));

			Assert.IsNotNull(blogs);
			Assert.IsNotNull(blogitems);
			Assert.IsNotNull(orders);
			Assert.AreEqual(0, blogs.Length);
			Assert.AreEqual(0, blogitems.Length);
			Assert.AreEqual(0, orders.Length);
		}
	}
}