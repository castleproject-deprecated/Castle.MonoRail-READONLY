﻿// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Rescues
{
	using System;
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class ActionLevelRescuesTestCase
	{
		private MockEngineContext engineContext;
		private ViewEngineManagerStub viewEngStub;
		private MockServices services;
		private MockResponse response;

		[SetUp]
		public void Init()
		{
			MockRequest request = new MockRequest();
			response = new MockResponse();
			services = new MockServices();
			viewEngStub = new ViewEngineManagerStub();
			services.ViewEngineManager = viewEngStub;
			engineContext = new MockEngineContext(request, response, services, null);
		}

		[Test]
		public void ActionRescueHasPrecedenceOverControllerRescue()
		{
			ControllerWithRescue controller = new ControllerWithRescue();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "home", "index", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			controller.Process(engineContext, context);

			Assert.AreEqual(500, response.StatusCode);
			Assert.AreEqual("Error processing action", response.StatusDescription);
			Assert.AreEqual("rescues\\specificerror", viewEngStub.TemplateRendered);
		}

		#region Controllers

		[Rescue("generalerror")]
		class ControllerWithRescue : Controller
		{
			[Rescue("specificerror")]
			public void Index()
			{
				throw new InvalidOperationException();
			}
		}

		#endregion
	}
}
