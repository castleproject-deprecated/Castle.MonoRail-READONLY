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

namespace Castle.Windsor.Tests
{
	using System;
	using Castle.MicroKernel.Exceptions;
	using Castle.MicroKernel.Handlers;
	using Castle.Windsor.Tests.Components;
	using NUnit.Framework;

	[TestFixture]
	public class CircularDependencyTests
	{
		[Test]
		public void ThrowsACircularDependencyException()
		{
			IWindsorContainer container = new WindsorContainer();
			container.AddComponent("controller", typeof(IController), typeof(Controller));
			container.AddComponent("view", typeof(IView), typeof(View));
			try
			{
				container.Resolve("controller");

				throw new Exception(
					@"A cycle was detected when trying to resolve a dependency. The dependency graph that resulted in a cycle is:
 - Service dependency 'view' type 'Castle.Windsor.Tests.Components.IView' for Void .ctor(Castle.Windsor.Tests.Components.IView) in type Castle.Windsor.Tests.Components.Controller
 - Service dependency 'Controller' type 'Castle.Windsor.Tests.Components.IController' for Castle.Windsor.Tests.Components.IController Controller in type Castle.Windsor.Tests.Components.View
 + Service dependency 'view' type 'Castle.Windsor.Tests.Components.IView' for Void .ctor(Castle.Windsor.Tests.Components.IView) in Castle.Windsor.Tests.Components.Controller
"
					);
			}
			catch(CircularDependecyException)
			{
			}
		}

		[Test]
		[
			ExpectedException(typeof(HandlerException),
				@"Can't create component 'compA' as it has dependencies to be satisfied. 
compA is waiting for the following dependencies: 

Services: 
- Castle.Windsor.Tests.Components.CompB which was registered but is also waiting for dependencies. 

compB is waiting for the following dependencies: 

Services: 
- Castle.Windsor.Tests.Components.CompC which was registered but is also waiting for dependencies. 

compC is waiting for the following dependencies: 

Services: 
- Castle.Windsor.Tests.Components.CompD which was registered but is also waiting for dependencies. 

compD is waiting for the following dependencies: 

Services: 
- Castle.Windsor.Tests.Components.CompA which was registered but is also waiting for dependencies. 
"
				)]
		public void ThrowsACircularDependencyException2()
		{
			IWindsorContainer container = new WindsorContainer();
			container.AddComponent("compA", typeof(CompA));
			container.AddComponent("compB", typeof(CompB));
			container.AddComponent("compC", typeof(CompC));
			container.AddComponent("compD", typeof(CompD));

			container.Resolve("compA");
		}

		[Test]
		public void ShouldNotGetCircularDepencyExceptionWhenResolvingTypeOnItselfWithDifferentModels()
		{
			WindsorContainer container = new WindsorContainer(ConfigHelper.ResolveConfigPath("IOC-51.xml"));
			object o = container["path.fileFinder"];
			Assert.IsNotNull(o);
		}
	}

	namespace IOC51
	{
		using System.Reflection;

		public interface IPathProvider
		{
			string Path { get; }
		}

		public class AssemblyPath : IPathProvider
		{
			public string Path
			{
				get
				{
					Uri uriPath = new Uri(Assembly.GetExecutingAssembly().GetName(false).CodeBase);
					return uriPath.LocalPath;
				}
			}
		}

		public class RelativeFilePath : IPathProvider
		{
			public RelativeFilePath(IPathProvider basePathProvider, string extensionsPath)
			{
				_path = System.IO.Path.Combine(basePathProvider.Path + "\\", extensionsPath);
			}

			public string Path
			{
				get { return _path; }
			}

			private string _path;
		}
	}
}