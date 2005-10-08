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

namespace Castle.MonoRail.TestSupport
{
	using System;
	using System.IO;
	using System.Net;
	using System.Text;
	using System.Web.Hosting;
	using System.Configuration;
	
	using NUnit.Framework;


	public abstract class AbstractMRTestCase
	{
		private static readonly String PhysicalWebDirConfigKey = "web.physical.dir";
		private static readonly String VirtualWebDirConfigKey = "web.virtual.dir";

		private WebAppHost host;
		private TestRequest request;
		private TestResponse response;
		private StringBuilder outputBuffer = new StringBuilder();

		#region Test Lifecycle 

		[TestFixtureSetUp]
		public virtual void FixtureInitialize()
		{
			String virDir = GetVirtualDir();
			String physicalDir = GetPhysicalDir();

			host = (WebAppHost) ApplicationHost.CreateApplicationHost( 
				typeof(WebAppHost), virDir, physicalDir );

			host.Configure(virDir, physicalDir);
		}

		[SetUp]
		public virtual void Initialize()
		{
			request = new TestRequest();
		}

		[TearDown]
		public virtual void Terminate()
		{
			outputBuffer.Length = 0;
		}

		[TestFixtureTearDown]
		public virtual void FixtureTerminate()
		{
			if (host != null) host.Dispose();
		}

		#endregion

		#region Actions

		public void DoGet(String path, params String[] queryStringParams)
		{
			if (queryStringParams.Length != 0) Request.QueryStringParams = queryStringParams;

			outputBuffer.Length = 0;

			Request.Url = path;

			StringWriter writer = new StringWriter(outputBuffer);

			response = host.Process( Request, writer );

			// Console.WriteLine( "Contents " + writer.GetStringBuilder().ToString() );
		}

		#endregion

		#region Properties

		public TestRequest Request
		{
			get { return request; }
		}

		public TestResponse Response
		{
			get { return response; }
		}

		#endregion

		#region Available Asserts

		protected void AssertSuccess()
		{
			Assert.IsNotNull(response, "No requests performed with DoGet or DoPost (?)");
			Assert.IsTrue(response.StatusCode < 400, "Status code different than > 400");
		}

		protected void AssertReplyEqualsTo(String expectedContents)
		{
			Assert.AreEqual( expectedContents, outputBuffer.ToString() );
		}

		protected void AssertReplyStartsWith(String contents)
		{
			String buffer = outputBuffer.ToString();

			Assert.IsTrue( buffer.StartsWith(contents), 
				"Reply string did not start with '{0}'. It was '{1}'", contents,
					buffer.Substring(0, Math.Min(contents.Length, buffer.Length) ) );
		}

		protected void AssertReplyEndsWith(String contents)
		{
			String buffer = outputBuffer.ToString();

			Assert.IsTrue( buffer.EndsWith(contents), 
				"Reply string did not end with '{0}'. It was '{1}'", contents,
					buffer.Substring(0, Math.Min(contents.Length, buffer.Length) ) );
		}

		protected void AssertReplyContains(String contents)
		{
			Assert.IsTrue( outputBuffer.ToString().IndexOf(contents) != -1, 
				"AssertReplyContains did not find the content '{0}'", contents );
		}

		protected void AssertReplyDoNotContain(String contents)
		{
			Assert.IsTrue( outputBuffer.ToString().IndexOf(contents) == -1, 
				"AssertReplyDoNotContain found the content '{0}'", contents );
		}

		protected void AssertRedirectedTo(String url)
		{
			Assert.AreEqual(302, Response.StatusCode, "Redirect status not used");
			AssertHasHeader("Location");
			Assert.AreEqual(url, Response.Headers["Location"]);
		}

		protected void AssertContentTypeEqualsTo(String expectedContentType)
		{
			AssertHasHeader("Content-Type");
			Assert.AreEqual(expectedContentType, Response.Headers["Content-Type"]);
		}

		protected void AssertContentTypeStartsWith(String expectedContentType)
		{
			AssertHasHeader("Content-Type");
			Assert.IsTrue( Response.Headers["Content-Type"].ToString().StartsWith(expectedContentType) );
		}

		protected void AssertContentTypeEndsWith(String expectedContentType)
		{
			AssertHasHeader("Content-Type");
			Assert.IsTrue( Response.Headers["Content-Type"].ToString().EndsWith(expectedContentType) );
		}

		protected void AssertHasHeader(String headerName)
		{
			Assert.IsTrue( Response.Headers[headerName] != null, 
				"Header '{0}' was not found", headerName );
		}

		protected void AssertPropertyBagContains(String entryKey)
		{
			Assert.IsNotNull(response.PropertyBag, "PropertyBag could not be used. Are you using a testing enable version of MonoRail Engine and Framework?"); 
			Assert.IsTrue(response.PropertyBag.Contains(entryKey), "Entry {0} was not on PropertyBag", entryKey);
		}

		protected void AssertPropertyBagEntryEquals(String entryKey, object expectedValue)
		{
			AssertPropertyBagContains(entryKey);
			Assert.AreEqual(expectedValue, response.PropertyBag[entryKey], "PropertyBag entry differs from the expected");
		}

		protected void AssertFlashContains(String entryKey)
		{
			Assert.IsNotNull(response.Flash, "Flash could not be used. Are you using a testing enable version of MonoRail Engine and Framework?"); 
			Assert.IsTrue(response.Flash.Contains(entryKey), "Entry {0} was not on Flash", entryKey);
		}

		protected void AssertFlashEntryEquals(String entryKey, object expectedValue)
		{
			AssertFlashContains(entryKey);
			Assert.AreEqual(expectedValue, response.Flash[entryKey], "Flash entry differs from the expected");
		}

		protected void AssertSessionContains(String entryKey)
		{
			Assert.IsNotNull(response.Session, "Session could not be used. Are you using a testing enable version of MonoRail Engine and Framework?"); 
			Assert.IsTrue(response.Session.Contains(entryKey), "Entry {0} was not on Session", entryKey);
		}

		protected void AssertSessionEntryEqualsTo(String entryKey, object expectedValue)
		{
			AssertSessionContains(entryKey);
			Assert.AreEqual(expectedValue, response.Session[entryKey], "Session entry differs from the expected");
		}

		protected void AssertHasCookie(String cookieName)
		{
			CookieCollection cookies = Response.Cookies.GetCookies( new Uri("http://localhost") );

			foreach(Cookie cookie in cookies)
			{
				if (cookie.Name.Equals(cookieName)) return;
			}

			Assert.Fail( "Cookie '{0}' was not found", cookieName );
		}

		protected void AssertCookieValueEqualsTo(String cookieName, String expectedValue)
		{
			AssertHasCookie(cookieName);

			CookieCollection cookies = Response.Cookies.GetCookies( new Uri("http://localhost") );

			foreach(Cookie cookie in cookies)
			{
				if (cookie.Name.Equals(cookieName))
				{
					Assert.AreEqual(expectedValue, cookie.Value);
					break;
				}
			}
		}

		protected void AssertCookieExpirationEqualsTo(String cookieName, DateTime expectedExpiration)
		{
			AssertHasCookie(cookieName);

			CookieCollection cookies = Response.Cookies.GetCookies( new Uri("http://localhost") );

			foreach(Cookie cookie in cookies)
			{
				if (cookie.Name.Equals(cookieName))
				{
					Assert.AreEqual(expectedExpiration.Day, cookie.Expires.Day);
					Assert.AreEqual(expectedExpiration.Month, cookie.Expires.Month);
					Assert.AreEqual(expectedExpiration.Year, cookie.Expires.Year);
					Assert.AreEqual(expectedExpiration.Hour, cookie.Expires.Hour);
					Assert.AreEqual(expectedExpiration.Minute, cookie.Expires.Minute);
					Assert.AreEqual(expectedExpiration.Second, cookie.Expires.Second);
					break;
				}
			}
		}

		#endregion

		#region Overridables

		protected virtual string GetPhysicalDir()
		{
			String dir = ConfigurationSettings.AppSettings[PhysicalWebDirConfigKey];

			if (dir == null)
			{
				String message = String.Format("Could not find a configuration key " + 
					"defining the web application physical directory. You must create " + 
					"a key ('{0}') on your configuration file or override the method " + 
					"AbstractMRTestCase.GetPhysicalDir", PhysicalWebDirConfigKey);

				throw new ConfigurationException(message);
			}

			if (!Path.IsPathRooted(dir))
			{
				DirectoryInfo dinfo = new DirectoryInfo( Path.Combine( AppDomain.CurrentDomain.SetupInformation.ApplicationBase, dir ) );

				dir = dinfo.FullName;
			}

			return dir;
		}

		protected virtual string GetVirtualDir()
		{
			String dir = ConfigurationSettings.AppSettings[VirtualWebDirConfigKey];

			if (dir == null)
			{
				dir = "/";
			}

			return dir;
		}

		#endregion
	}
}
