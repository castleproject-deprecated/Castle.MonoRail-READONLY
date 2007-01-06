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

namespace Castle.Facilities.Cache.Tests
{
	using System;

	/// <summary>
	/// Summary description for ServiceD.
	/// </summary>
	[Cache]
	public class ServiceD : IServiceD
	{
		#region IServiceD Members

		[Cache("Another.Cache")]
		public int MyMethodA(int a, int c)
		{
			int ret = a+c;

			Console.Write(ret.ToString() + Environment.TickCount.ToString());
			return (ret);
		}

		[Cache("A.Cache")]
		public string MyMethodB(string s)
		{
			string ret = "Hello "+s;

			Console.Write(ret + Environment.TickCount.ToString());
			return (ret);
		}

		#endregion
	}
}
