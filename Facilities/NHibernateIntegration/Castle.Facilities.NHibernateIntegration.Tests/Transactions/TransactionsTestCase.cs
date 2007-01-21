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
	
	using NHibernate;
	using NUnit.Framework;

	[TestFixture]
	public class TransactionsTestCase : AbstractNHibernateTestCase
	{
		protected override void ConfigureContainer()
		{
			container.AddFacility("transactions", new TransactionFacility());

			container.AddComponent("root", typeof(RootService));
			container.AddComponent("myfirstdao", typeof(FirstDao));
			container.AddComponent("myseconddao", typeof(SecondDao));
		}

		[Test]
		public void TestTransaction()
		{
			ISessionManager sessionManager = (ISessionManager) container[typeof(ISessionManager)];

			RootService service = (RootService) container[typeof(RootService)];
			FirstDao dao = (FirstDao) container[typeof(FirstDao)];

			Blog blog = dao.Create("Blog1");

			try
			{
				service.DoBlogRefOperation(blog);
				
				// Expects a constraint exception on Commit

				Assert.Fail("Must fail");
			}
			catch(Exception)
			{
				// transaction exception expected
			}
		}

		[Test]
		// [Ignore("This doesn't work with the NH 1.2 transaction property, needs to be fixed")]
		public void TransactionNotHijackingTheSession()
		{
			ISessionManager sessionManager = (ISessionManager)
			                                 container[typeof(ISessionManager)];

			using(ISession session = sessionManager.OpenSession())
			{
				Assert.IsFalse(session.Transaction.IsActive);

				FirstDao service = (FirstDao) container["myfirstdao"];

				// This call is transactional
				Blog blog = service.Create();

				// TODO: Assert transaction was committed
				// Assert.IsTrue(session.Transaction.WasCommitted);

				RootService rootService = (RootService) container["root"];

				Array blogs = rootService.FindAll(typeof(Blog));
				Assert.AreEqual(1, blogs.Length);
			}
		}

		[Test]
		// [Ignore("This doesn't work with the NH 1.2 transaction property, needs to be fixed")]
		public void SessionBeingSharedByMultipleTransactionsInSequence()
		{
			ISessionManager sessionManager = (ISessionManager)
			                                 container[typeof(ISessionManager)];

			using(ISession session = sessionManager.OpenSession())
			{
				Assert.IsFalse(session.Transaction.IsActive);

				FirstDao service = (FirstDao) container["myfirstdao"];

				// This call is transactional
				service.Create();

				// TODO: Assert transaction was committed
				// Assert.IsTrue(session.Transaction.WasCommitted);

				// This call is transactional
				service.Create("ps2's blogs");

				// TODO: Assert transaction was committed
				// Assert.IsTrue(session.Transaction.WasCommitted);

				// This call is transactional
				service.Create("game cube's blogs");

				RootService rootService = (RootService) container["root"];

				Array blogs = rootService.FindAll(typeof(Blog));
				Assert.AreEqual(3, blogs.Length);
			}
		}

		[Test]
		// [Ignore("This doesn't work with the NH 1.2 transaction property, needs to be fixed")]
		public void NonTransactionalRoot()
		{
			ISessionManager sessionManager = (ISessionManager)
			                                 container[typeof(ISessionManager)];

			using(ISession session = sessionManager.OpenSession())
			{
				Assert.IsFalse(session.Transaction.IsActive);

				FirstDao first = (FirstDao) container["myfirstdao"];
				SecondDao second = (SecondDao) container["myseconddao"];

				// This call is transactional
				Blog blog = first.Create();

				// TODO: Assert transaction was committed
				// Assert.IsTrue(session.Transaction.WasCommitted);

				try
				{
					second.CreateWithException2(blog);
				}
				catch(Exception)
				{
					// Expected
				}

				// TODO: Assert transaction was rolledback
				// Assert.IsTrue(session.Transaction.WasRolledBack);

				RootService rootService = (RootService) container["root"];

				Array blogs = rootService.FindAll(typeof(Blog));
				Assert.AreEqual(1, blogs.Length);
				Array blogitems = rootService.FindAll(typeof(BlogItem));
				Assert.IsNull(blogitems);
			}
		}

		[Test]
		public void SimpleAndSucessfulSituationUsingRootTransactionBoundary()
		{
			RootService service = (RootService) container["root"];

			service.SuccessFullCall();

			Array blogs = service.FindAll(typeof(Blog));
			Array blogitems = service.FindAll(typeof(BlogItem));

			Assert.IsNotNull(blogs);
			Assert.IsNotNull(blogitems);
			Assert.AreEqual(1, blogs.Length);
			Assert.AreEqual(1, blogitems.Length);
		}

		[Test]
		public void CallWithException()
		{
			RootService service = (RootService) container["root"];

			try
			{
				service.CallWithException();
			}
			catch(Exception)
			{
			}

			// Ensure rollback happened

			Array blogs = service.FindAll(typeof(Blog));
			Array blogitems = service.FindAll(typeof(BlogItem));

			Assert.IsNull(blogs);
			Assert.IsNull(blogitems);
		}

		[Test]
		public void CallWithException2()
		{
			RootService service = (RootService) container["root"];

			try
			{
				service.CallWithException2();
			}
			catch(Exception)
			{
			}

			// Ensure rollback happened

			Array blogs = service.FindAll(typeof(Blog));
			Array blogitems = service.FindAll(typeof(BlogItem));

			Assert.IsNull(blogs);
			Assert.IsNull(blogitems);
		}
	}
}
