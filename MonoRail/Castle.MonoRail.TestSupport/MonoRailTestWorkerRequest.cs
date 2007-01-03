// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System.Text;
	using System.Web.Hosting;
	using System.Collections;
	using System.Globalization;

	public class MonoRailTestWorkerRequest : SimpleWorkerRequest
	{
		private TextWriter output;
		private TestRequest requestData;
		private TestResponse response = new TestResponse();
		private String virtualAppPath;
		private String appPhysicalPath;
		private String filePath;
		private String[] knownRequestHeaders;
		private String[][] unknownRequestHeaders;
		private String queryString;
		private byte[] preloadedContent;
		private byte[] queryStringBytes;
		private StringBuilder buffer = new StringBuilder();

		public MonoRailTestWorkerRequest(TestRequest requestData,
		                                 String virtualAppPath,
		                                 String physicalAppPath,
		                                 TextWriter output) : base(virtualAppPath, String.Empty, output)
		{
			this.virtualAppPath = virtualAppPath;
			this.requestData = requestData;
			this.appPhysicalPath = physicalAppPath;
			this.output = output;
			this.filePath = requestData.Url;

			if (!appPhysicalPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				appPhysicalPath = appPhysicalPath + Path.DirectorySeparatorChar;
			}
		}

		protected internal void Prepare()
		{
			ProcessHeaders();

			ProcessQueryString();

			ProcessPostBody();
		}

		private void ProcessHeaders()
		{
			knownRequestHeaders = new string[RequestHeaderMaximum];

			IList unknownHeaders = new ArrayList();

			foreach(String name in requestData.Headers)
			{
				String value = requestData.Headers[name];

				int index = GetKnownRequestHeaderIndex(name);

				if (index >= 0)
				{
					knownRequestHeaders[index] = value;
				}
				else
				{
					unknownHeaders.Add(name);
					unknownHeaders.Add(value);
				}
			}

			int totalunknownHeaders = unknownHeaders.Count / 2;
			unknownRequestHeaders = new String[totalunknownHeaders][];

			int j = 0;

			for(int i = 0; i < totalunknownHeaders; i++)
			{
				unknownRequestHeaders[i] = new String[2];
				unknownRequestHeaders[i][0] = (String) unknownHeaders[j++];
				unknownRequestHeaders[i][1] = (String) unknownHeaders[j++];
			}
		}

		private void ProcessQueryString()
		{
			if (requestData.QueryStringParams != null)
			{
				buffer.Length = 0;
				
				foreach(String param in requestData.QueryStringParams)
				{
					buffer.AppendFormat("{0}&", param);
				}
				
				queryString = buffer.ToString();
				
				queryStringBytes = Encoding.ASCII.GetBytes(queryString);
			}
		}

		private void ProcessPostBody()
		{
			if (requestData.PostParams != null)
			{
				buffer.Length = 0;

				foreach(String param in requestData.PostParams)
				{
					buffer.AppendFormat("{0}&", param);
				}

				preloadedContent = Encoding.ASCII.GetBytes(buffer.ToString());
			}
		}

		public TestResponse Response
		{
			get { return response; }
		}

		private string GetPathInternal()
		{
			return virtualAppPath.Equals("/")
			       	? ("/" + filePath) : (virtualAppPath + "/" + filePath);
		}

		public override string GetAppPath()
		{
			return virtualAppPath;
		}

		public override string GetFilePathTranslated()
		{
			return appPhysicalPath + filePath.Replace('/', Path.DirectorySeparatorChar);
		}

		public override String MapPath(String path)
		{
			String mappedPath;

			if (path == null || path.Length == 0 || path.Equals("/"))
			{
				// asking for the site root
				if ("/".Equals(virtualAppPath))
				{
					// app at the site root
					mappedPath = appPhysicalPath;
				}
				else
				{
					// unknown site root - don't point to app root to avoid double config inclusion
					mappedPath = Environment.SystemDirectory;
				}
			}
			else
			{
				if (path.StartsWith("/") || path.StartsWith("\\"))
				{
					path = path.Substring(1);
				}

				mappedPath = new DirectoryInfo(Path.Combine(appPhysicalPath, path)).FullName;
			}

			mappedPath = mappedPath.Replace('/', Path.DirectorySeparatorChar);

			if (mappedPath.EndsWith(Path.DirectorySeparatorChar.ToString()) && !mappedPath.EndsWith(":\\"))
			{
				mappedPath = mappedPath.Substring(0, mappedPath.Length - 1);
			}

			return mappedPath;
		}

		public override String GetAppPathTranslated()
		{
			return appPhysicalPath;
		}

		public override byte[] GetPreloadedEntityBody()
		{
			return preloadedContent;
		}

		public override bool IsEntireEntityBodyIsPreloaded()
		{
			return true;
		}

		public override int ReadEntityBody(byte[] buffer, int size)
		{
			int bytestocopy = Math.Min(preloadedContent.Length, size);

			Buffer.BlockCopy(preloadedContent, 0, buffer, 0, bytestocopy);

			return bytestocopy;
		}

		public override String GetKnownRequestHeader(int index)
		{
			return knownRequestHeaders[index];
		}

		public override String GetUnknownRequestHeader(String name)
		{
			for(int i = 0; i < unknownRequestHeaders.Length; i++)
			{
				if (String.Compare(name, unknownRequestHeaders[i][0], true, CultureInfo.InvariantCulture) == 0)
				{
					return unknownRequestHeaders[i][1];
				}
			}

			return null;
		}

		public override String[][] GetUnknownRequestHeaders()
		{
			return unknownRequestHeaders;
		}

		public override string GetServerVariable(string name)
		{
			String value = requestData.ServerVariables[name];

			if (value == null)
			{
				switch(name)
				{
					case "SERVER_PROTOCOL":
						return requestData.Protocol;
					case "QUERY_STRING":
						return queryString;
				}
			}

			return value == null ? String.Empty : value;
		}

		public override void EndOfRequest()
		{
		}

		public override string GetFilePath()
		{
			return GetPathInternal();
		}

		public override string GetHttpVerbName()
		{
			return requestData.Verb;
		}

		public override string GetHttpVersion()
		{
			return requestData.Protocol;
		}

		public override void FlushResponse(bool finalFlush)
		{
			response.Complete();
		}

		public override string GetLocalAddress()
		{
			return requestData.LocalAddress;
		}

		public override int GetLocalPort()
		{
			return requestData.LocalPort;
		}

		public override String GetQueryString()
		{
			return queryString;
		}

		public override byte[] GetQueryStringRawBytes()
		{
			return queryStringBytes;
		}

		public override string GetRawUrl()
		{
			return String.Format("{0}{1}{2}",
			                     GetPathInternal(), queryString != null ? "?" : "", queryString);
		}

		public override string GetRemoteAddress()
		{
			return requestData.RemoteAddress;
		}

		public override int GetRemotePort()
		{
			return requestData.RemotePort;
		}

		public override string GetUriPath()
		{
			return GetPathInternal();
		}

		public override IntPtr GetUserToken()
		{
			return requestData.UserToken;
		}

		public override void SendKnownResponseHeader(int index, string value)
		{
			String key = GetKnownResponseHeaderName(index);

			SendUnknownResponseHeader(key, value);
		}

		public override void SendResponseFromFile(IntPtr handle, long offset, long length)
		{
			// TODO: SendResponseFromFile
		}

		public override void SendResponseFromFile(string filename, long offset, long length)
		{
			// TODO: SendResponseFromFile
		}

		public override void SendResponseFromMemory(byte[] data, int length)
		{
			output.Write(Encoding.Default.GetChars(data, 0, length));
		}

		public override void SendStatus(int statusCode, string statusDescription)
		{
			response.StatusCode = statusCode;
			response.StatusDescription = statusDescription;
		}

		public override void SendUnknownResponseHeader(string name, string value)
		{
			object existingValue = response.Headers[name];

			if (existingValue == null)
			{
				response.Headers[name] = value;
			}
			else
			{
				if (existingValue is IList)
				{
					(existingValue as IList).Add(value);
				}
				else
				{
					IList list = new ArrayList();

					list.Add(existingValue);
					list.Add(value);

					response.Headers[name] = list;
				}
			}
		}

		public override bool IsSecure()
		{
			return false;
		}

		public override string GetProtocol()
		{
			return requestData.Protocol;
		}

		public override void SendCalculatedContentLength(int contentLength)
		{
			response.Headers["Content-Length"] = contentLength.ToString();
		}

		public override bool HeadersSent()
		{
			return false;
		}

		public override bool IsClientConnected()
		{
			return true;
		}

		public override void CloseConnection()
		{
		}
	}
}
