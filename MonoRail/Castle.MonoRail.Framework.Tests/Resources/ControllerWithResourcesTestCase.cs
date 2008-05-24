﻿// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Resources
{
	using System.Collections;
	using Castle.MonoRail.Framework.Resources;
	using Descriptors;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Test;

	[TestFixture]
	public class ControllerWithResourcesTestCase
	{
		private MockRepository mockRepository = new MockRepository();
		private MockEngineContext engineContext;
		private ViewEngineManagerStub viewEngStub;
		private MockServices services;
		private IResourceFactory resourceFactoryMock;

		[SetUp]
		public void Init()
		{
			resourceFactoryMock = mockRepository.DynamicMock<IResourceFactory>();

			MockRequest request = new MockRequest();
			MockResponse response = new MockResponse();
			services = new MockServices();
			viewEngStub = new ViewEngineManagerStub();
			services.ViewEngineManager = viewEngStub;
			services.ResourceFactory = resourceFactoryMock;
			engineContext = new MockEngineContext(request, response, services, null);
		}

		[Test]
		public void CreatesResourcesSpecifiedThroughAttributes()
		{
			ControllerWithResource controller = new ControllerWithResource();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			using(mockRepository.Record())
			{
				Expect.Call(resourceFactoryMock.Create(
				            	new ResourceDescriptor(null, "key", "Castle.MonoRail.Framework.Tests.Resources.Language", "neutral",
				            	                       "Castle.MonoRail.Framework.Tests"),
				            	typeof(ControllerWithResourcesTestCase).Assembly)).Return(new DummyResource());
			}

			using(mockRepository.Playback())
			{
				controller.Process(engineContext, context);

				Assert.AreEqual(1, context.Resources.Count);
				Assert.IsNotNull(context.Resources["key"]);
			}
		}

		[Test, ExpectedException(typeof(MonoRailException), ExpectedMessage = "There is a duplicated entry on the resource dictionary. Resource entry name: key")]
		public void DuplicatedResourceEntriesHaveDecentErrorMessage()
		{
			ControllerWithResourceDuplicated controller = new ControllerWithResourceDuplicated();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			using(mockRepository.Record())
			{
				Expect.Call(resourceFactoryMock.Create(
				            	new ResourceDescriptor(null, "key", "Castle.MonoRail.Framework.Tests.Resources.Language", "neutral",
				            	                       "Castle.MonoRail.Framework.Tests"),
				            	typeof(ControllerWithResourcesTestCase).Assembly)).Return(new DummyResource());
			}

			using(mockRepository.Playback())
			{
				controller.Process(engineContext, context);
			}
		}

		#region Controllers

		[Resource("key", "Castle.MonoRail.Framework.Tests.Resources.Language", CultureName = "neutral", AssemblyName = "Castle.MonoRail.Framework.Tests")]
		private class ControllerWithResource : Controller
		{
			public void Index()
			{
			}
		}

		[Resource("key", "Castle.MonoRail.Framework.Tests.Resources.Language", CultureName = "neutral", AssemblyName = "Castle.MonoRail.Framework.Tests")]
		[Resource("key", "Castle.MonoRail.Framework.Tests.Resources.Language", CultureName = "neutral", AssemblyName = "Castle.MonoRail.Framework.Tests")]
		private class ControllerWithResourceDuplicated : Controller
		{
			public void Index()
			{
			}
		}

		#endregion

		private class DummyResource : IResource
		{
			public object this[string key]
			{
				get { throw new System.NotImplementedException(); }
			}

			public string GetString(string key)
			{
				throw new System.NotImplementedException();
			}

			public object GetObject(string key)
			{
				throw new System.NotImplementedException();
			}

			public IEnumerator GetEnumerator()
			{
				throw new System.NotImplementedException();
			}
		}
	}
}