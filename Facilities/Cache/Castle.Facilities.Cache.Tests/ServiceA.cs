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

namespace Castle.Facilities.Cache.Tests
{
	using System;

	/// <summary>
	/// Description r�sum�e de ServiceA.
	/// </summary>
	[Cache("FifoCacheManager")]
	public class ServiceA : IServiceA
	{
		#region IServiceA Members

		public string MyMethodNotcached(string a)
		{
			return "Hello "+a;
		}

		[Cache]
		public decimal MyMethod(int a , decimal c)
		{
			decimal ret = a+c;

			Console.Write(ret.ToString() + Environment.TickCount.ToString());
			return (ret);
		}

		#endregion

	}
}
