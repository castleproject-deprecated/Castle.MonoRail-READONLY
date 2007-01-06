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

namespace Castle.Facilities.Remoting.Tests
{
	using System;
	using System.Security.Policy;

	public class AppDomainFactory
	{
		public static AppDomain Create(String name)
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;

			AppDomainSetup setup = new AppDomainSetup();

			setup.ApplicationName = name;
			setup.ApplicationBase = currentDomain.SetupInformation.ApplicationBase;
			setup.PrivateBinPath = currentDomain.SetupInformation.PrivateBinPath;
			setup.ConfigurationFile = currentDomain.SetupInformation.ConfigurationFile;

			Evidence baseEvidence = currentDomain.Evidence;
			Evidence evidence = new Evidence(baseEvidence);

			return AppDomain.CreateDomain(name, evidence, setup);
		}
	}
}
