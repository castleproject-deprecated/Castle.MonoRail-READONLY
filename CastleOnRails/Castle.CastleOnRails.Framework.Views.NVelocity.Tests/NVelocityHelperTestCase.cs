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

namespace Castle.CastleOnRails.Framework.Views.NVelocity.Tests
{
	using System;
	using System.Net;
	using System.IO;

	using NUnit.Framework;

	using Castle.CastleOnRails.Engine.Tests;


	[TestFixture]
	public class NVelocityHelperTestCase : AbstractCassiniTestCase
	{
		[Test]
		public void InheritedHelpers()
		{
			HttpWebRequest myReq = (HttpWebRequest) 
				WebRequest.Create("http://localhost:8083/helper/inheritedhelpers.rails");

			HttpWebResponse response = (HttpWebResponse) myReq.GetResponse();

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("/helper/inheritedhelpers.rails", response.ResponseUri.PathAndQuery);
			Assert.IsTrue(response.ContentType.StartsWith("text/html"));
			AssertContents("Date formatted " + new DateTime(1979, 7, 16).ToShortDateString(), response);
		}

		protected override String ObtainPhysicalDir()
		{
			return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"..\TestSiteNVelocity" );
		}
	}
}
