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

namespace Castle.Facilities.Synchronize.Tests
{
	using System;
	using System.Configuration;
	using System.Diagnostics;
	using System.Threading;
	using System.Windows.Forms;
	using Castle.Core;
	using Castle.Core.Configuration;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.Handlers;
	using Castle.MicroKernel.Proxy;
	using Castle.Windsor;
	using NUnit.Framework;

	[TestFixture]
	public class SynchronizeFacilityTest
	{
		private WindsorContainer container;
		private Exception uncaughtException;

		[SetUp]
		public void SetUp()
		{
			uncaughtException = null;
			container = new WindsorContainer();

			container.AddFacility("sync.facility", new SynchronizeFacility());
			container.AddComponent("dummy.form.class", typeof(DummyForm));
			container.AddComponent("dummy.form.service", typeof(IDummyForm), typeof(DummyForm));
			container.AddComponent("class.in.context", typeof(ClassUsingFormInContext));

			MutableConfiguration componentNode = new MutableConfiguration("component");
			componentNode.Attributes[Constants.SynchronizedAttrib] = "true";
			MutableConfiguration synchronizeNode = new MutableConfiguration("synchronize");
			synchronizeNode.Attributes["contextType"] = typeof(WindowsFormsSynchronizationContext).AssemblyQualifiedName;
			MutableConfiguration doWorkMethod = new MutableConfiguration("method");
			doWorkMethod.Attributes["name"] = "DoWork";
			doWorkMethod.Attributes["contextType"] = typeof(WindowsFormsSynchronizationContext).AssemblyQualifiedName;
			synchronizeNode.Children.Add(doWorkMethod);
			componentNode.Children.Add(synchronizeNode);

			container.AddComponent("generic.class.in.context", typeof(IClassUsingContext<>),
			                       typeof(ClassUsingContext<>));

			container.Kernel.ConfigurationStore.AddComponentConfiguration("class.needing.context", componentNode);
			container.AddComponent("class.needing.context", typeof(ClassUsingForm));
		}

		[Test]
		public void SynchronizeContextReference_DifferentInstanceSameKey_AreEqual()
		{
			SynchronizeContextReference sync1 = new SynchronizeContextReference("key1");
			SynchronizeContextReference sync2 = new SynchronizeContextReference("key1");

			Assert.AreEqual(sync1, sync2);
			Assert.AreEqual(sync1.GetHashCode(), sync2.GetHashCode());
		}

		[Test]
		public void SynchronizeContextReference_DifferentInstanceDifferentKey_AreNotEqual()
		{
			SynchronizeContextReference sync1 = new SynchronizeContextReference("key1");
			SynchronizeContextReference sync2 = new SynchronizeContextReference("key2");

			Assert.AreNotEqual(sync1, sync2);
			Assert.AreNotEqual(sync1.GetHashCode(), sync2.GetHashCode());
		}

		[Test]
		public void SynchronizeContextReference_DifferentInstanceSameType_AreEqual()
		{
			SynchronizeContextReference sync1 = new SynchronizeContextReference(typeof(string));
			SynchronizeContextReference sync2 = new SynchronizeContextReference(typeof(string));

			Assert.AreEqual(sync1, sync2);
			Assert.AreEqual(sync1.GetHashCode(), sync2.GetHashCode());
		}

		[Test]
		public void SynchronizeContextReference_DifferentInstanceDifferentType_AreNotEqual()
		{
			SynchronizeContextReference sync1 = new SynchronizeContextReference(typeof(string));
			SynchronizeContextReference sync2 = new SynchronizeContextReference(typeof(float));

			Assert.AreNotEqual(sync1, sync2);
			Assert.AreNotEqual(sync1.GetHashCode(), sync2.GetHashCode());
		}

		[Test]
		public void AddControl_DifferentThread_ThrowsException()
		{
			DummyForm form = new DummyForm();
			ExecuteInThread(delegate { form.AddControl(new Button()); });
			Assert.IsNotNull(uncaughtException, "Expected an exception");

			uncaughtException = null;

			ClassUsingFormInContext classInCtx = new ClassUsingFormInContext();
			ExecuteInThread(delegate { classInCtx.DoWork(form); });
			Assert.IsNotNull(uncaughtException, "Expected an exception");
		}

		[Test]
		public void AddControl_DifferentThreadUsingClass_WorksFine()
		{
			DummyForm form = container.Resolve<DummyForm>();
			ExecuteInThread(delegate { form.AddControl(new Button()); });
			Assert.IsNull(uncaughtException, "Expected no exception");
		}

		[Test]
		public void AddControl_DifferentThreadUsingService_WorksFine()
		{
			IDummyForm form = container.Resolve<IDummyForm>();
			ExecuteInThread(delegate { form.AddControl(new Button()); });
			Assert.IsNull(uncaughtException, "Expected no exception");
		}

		[Test]
		public void AddControl_DifferentThreadInContext_WorksFine()
		{
			DummyForm form = new DummyForm();
			ClassUsingFormInContext client = container.Resolve<ClassUsingFormInContext>();
			ExecuteInThread(delegate { client.DoWork(form); });
			Assert.IsNull(uncaughtException, "Expected no exception");
		}

		[Test]
		public void AddControl_DifferentThreadInContextUsingConfiguration_WorksFine()
		{
			DummyForm form = new DummyForm();
			ClassUsingForm client = container.Resolve<ClassUsingForm>();
			ExecuteInThread(delegate { client.DoWork(form); });
			Assert.IsNull(uncaughtException, "Expected no exception");
		}

		[Test]
		public void DoWorkGeneric_DifferentThreadInContext_WorksFine()
		{
			DummyForm form = new DummyForm();
			IClassUsingContext<DummyForm> client = container.Resolve<IClassUsingContext<DummyForm>>();
			ExecuteInThread(delegate { client.DoWork(form); });
			Assert.IsNull(uncaughtException, "Expected no exception");
		}

		[Test, ExpectedException(typeof(FacilityException))]
		public void AddContextComponent_WithoutVirtualMethod_ThrowsFacilityException()
		{
			container.AddComponent("class.in.context.bad", typeof(ClassInContextWithoutVirtualMethod));
		}

		[Test, ExpectedException(typeof(HandlerException))]
		public void ResolveContextComponent_WithMissingDependency_ThrowsHandlerException()
		{
			container.AddComponent("class.in.context.bad", typeof(ClassInContextWithMissingDependency));
			container.Resolve("class.in.context.bad");
		}

		[Test]
		public void RegisterFacility_WithControlProxyHook_WorksFine()
		{
			WindsorContainer container2 = new WindsorContainer();

			MutableConfiguration facNode = new MutableConfiguration("facility");
			facNode.Attributes["id"] = "sync.facility";
			facNode.Attributes[Constants.ControlProxyHookAttrib] = typeof(DummyProxyHook).AssemblyQualifiedName;
			container2.Kernel.ConfigurationStore.AddFacilityConfiguration("sync.facility", facNode);
			container2.AddFacility("sync.facility", new SynchronizeFacility());

			container2.AddComponent("dummy.form.class", typeof(DummyForm));
			ComponentModel model = container2.Kernel.GetHandler("dummy.form.class").ComponentModel;
			ProxyOptions options = ProxyUtil.ObtainProxyOptions(model, false);
			Assert.IsNotNull(options, "Proxy options should not be null");
			Assert.IsTrue(options.Hook is DummyProxyHook, "Proxy hook should be a DummyProxyHook");
		}

#if DOTNET2
		[Test, ExpectedException(typeof(ConfigurationErrorsException))]
#else
		[Test, ExpectedException(typeof(ConfigurationException))]
#endif
		public void RegisterFacility_WithBadControlProxyHook_ThrowsConfigurationException()
		{
			WindsorContainer container2 = new WindsorContainer();

			MutableConfiguration facNode = new MutableConfiguration("facility");
			facNode.Attributes["id"] = "sync.facility";
			facNode.Attributes[Constants.ControlProxyHookAttrib] = typeof(string).AssemblyQualifiedName;
			container2.Kernel.ConfigurationStore.AddFacilityConfiguration("sync.facility", facNode);
			container2.AddFacility("sync.facility", new SynchronizeFacility());
		}

		private void ExecuteInThread(ThreadStart run)
		{
			Thread thread = new Thread((ThreadStart) delegate
			                                         	{
			                                         		try
			                                         		{
			                                         			run();
			                                         		}
			                                         		catch(Exception e)
			                                         		{
			                                         			uncaughtException = e;
			                                         		}

			                                         		Application.DoEvents();
			                                         		Application.Exit();
			                                         	});

			Form form = new Form();
            //According to MSDN: referencing the 'Handle' property will force the handle to be created.
            if (form.Handle == IntPtr.Zero)
                throw new InvalidOperationException("DummyForm have a null window handle");

			form.BeginInvoke((MethodInvoker) delegate { thread.Start(); });

			Application.Run();
		}
	}
}
