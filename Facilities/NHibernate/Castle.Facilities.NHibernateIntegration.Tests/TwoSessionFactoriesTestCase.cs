// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.NHibernateIntegration.Tests
{
	using System;
	using System.Collections;

	using NUnit.Framework;

	using Castle.Facilities.AutomaticTransactionManagement;
	using Castle.Windsor;
	

	[TestFixture]
	public class TwoSessionFactoriesTestCase : AbstractNHibernateTestCase
	{
		[Test]
		public void BusinessLayerWithTransactions()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("transaction", new TransactionFacility());
			
			container.AddComponent("blogdao", typeof(BlogDao));
			container.AddComponent("orderdao", typeof(OrderDao));
			container.AddComponent("business", typeof(MyBusinessClass));
			container.AddComponent("business2", typeof(MyOtherBusinessClass));

			MyOtherBusinessClass service = (MyOtherBusinessClass) container[typeof(MyOtherBusinessClass)];
			Blog blog = service.Create(1);

			BlogDao dao = (BlogDao) container["blogdao"];
			IList blogs = dao.ObtainBlogs();
			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Count );

			OrderDao odao = (OrderDao) container["orderdao"];
			IList orders = odao.ObtainOrders();
			Assert.IsNotNull( orders );
			Assert.AreEqual( 1, orders.Count );
		}

		[Test]
		public void BusinessLayerWithTransactionsAndThread()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("transaction", new TransactionFacility());
			
			container.AddComponent("blogdao", typeof(BlogDao));
			container.AddComponent("orderdao", typeof(OrderDao));
			container.AddComponent("business", typeof(MyBusinessClass));
			container.AddComponent("business2", typeof(MyOtherBusinessClass));
		}

		[Test]
		public void Rollback()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("transaction", new TransactionFacility());

			container.AddComponent("blogdao", typeof(BlogDao));
			container.AddComponent("orderdao", typeof(OrderDao));
			container.AddComponent("business", typeof(MyBusinessClass));
			container.AddComponent("business2", typeof(MyOtherBusinessClass));

			MyOtherBusinessClass service = (MyOtherBusinessClass) container[typeof(MyOtherBusinessClass)];
			
			try
			{
				Blog blog = service.CreateWithError(1);
			}
			catch(Exception)
			{
				// Expected
			}

			BlogDao dao = (BlogDao) container["blogdao"];
			IList blogs = dao.ObtainBlogs();
			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Count );

			OrderDao odao = (OrderDao) container["orderdao"];
			IList orders = odao.ObtainOrders();
			Assert.IsNotNull( orders );
			Assert.AreEqual( 0, orders.Count );
		}
	}
}
