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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using System.Web;
	using System.Web.Caching;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Security.Principal;

	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Internal.Test;

	/// <summary>
	/// Summary description for RailsEngineContextImpl.
	/// </summary>
	public class RailsEngineContextImpl : IRailsEngineContext
	{
		private String _url;
		private String _requestType;
		private Exception _lastException;
		private HttpContext _context;
		private TestRequest _request = new TestRequest();
		private MockResponse _response = new MockResponse();
		private ITrace _trace = new MockTrace();
		private ServerUtilityImpl _server = new ServerUtilityImpl();
		private Hashtable _session = new Hashtable();
		private Hashtable _flashItems = new Hashtable();
		private IPrincipal _user;

		public RailsEngineContextImpl(String url) : this(url, "GET")
		{
		}

		public RailsEngineContextImpl(String url, String requestType)
		{
			_url = url;
			_requestType = requestType;

			// TODO: Review this
			_context = null;
		}

		public void AddRequestParam(String name, String value)
		{
			_request._params.Add(name, value);
		}

		public object Output
		{
			get { return _response._contents.ToString(); }
		}

		public Exception LastException
		{
			get { return _lastException; }
			set { _lastException = value; }
		}

		public String RequestType
		{
			get { return _requestType; }
		}

		public String Url
		{
			get { return _url; }
		}

		public String UrlReferrer
		{
			get { return null; }
		}

		public HttpContext UnderlyingContext
		{
			get { return _context; }
		}

		public NameValueCollection Params
		{
			get { return _request._params; }
		}

		public IDictionary Session
		{
			get { return _session; }
		}

		public IRequest Request
		{
			get { return _request; }
		}

		public IResponse Response
		{
			get { return _response; }
		}
		
		public ITrace Trace
		{
			get { return _trace; }
		}

		public Cache Cache
		{
			get { throw new NotImplementedException(); }
		}

		public IDictionary Flash
		{
			get { return _flashItems; }
		}

		public void Transfer(String path, bool preserveForm)
		{
			throw new NotImplementedException();
		}

		public IPrincipal CurrentUser
		{
			get { return _user; }
			set { _user = value; }
		}

		public string ApplicationPath
		{
			get { return AppDomain.CurrentDomain.BaseDirectory; }
		}

		public UrlInfo UrlInfo
		{
			get { throw new NotImplementedException(); }
		}

		public IServerUtility Server
		{
			get { return _server; }
		}
	}
}
