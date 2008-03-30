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

namespace Castle.Facilities.WcfIntegration.Demo
{
	using System.ServiceModel;
#if DOTNET35
	using System.ServiceModel.Web;
#endif

	[ServiceContract()]
	public interface IAmUsingWindsor
	{
		[OperationContract]
		int GetValueFromWindsorConfig();

		[OperationContract]
#if DOTNET35
		[WebGet]
#endif
		int MultiplyValueFromWindsorConfig(int multiplier);
	}

	public class UsingWindsor : IAmUsingWindsor
	{
		private readonly int number;

		public UsingWindsor(int number)
		{
			this.number = number;
		}

		#region IAmUsingWindsor Members

		public int GetValueFromWindsorConfig()
		{
			return number;
		}

		public int MultiplyValueFromWindsorConfig(int multiplier)
		{
			return number * multiplier;
		}

		#endregion
	}

	public class UsingWindsorWithoutConfig : UsingWindsor
	{
		public UsingWindsorWithoutConfig(int number)
			: base(number)
		{
		}
	}
}