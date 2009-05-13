// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Web;

	/// <summary>
	/// Defines the cache configuration for an action.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method|AttributeTargets.Class, AllowMultiple = false, Inherited = true), Serializable]
	public class CacheAttribute : Attribute, ICachePolicyConfigurer
	{
		private readonly HttpCacheability cacheability;
		private bool allowInHistory, slidingExpiration, validUntilExpires;
		private bool setEtagFromFileDependencies, setLastModifiedFromFileDependencies;
		private bool setNoServerCaching, setNoStore, setNoTransforms, omitVaryStar;
		private string cacheExtension;
		private string etag;
		private int duration;
		private string varyByCustom, varyByHeaders, varyByParams;
		private DateTime? lastModified;
		private TimeSpan? maxAge, proxyMaxAge;
		private HttpCacheRevalidation revalidation;

#if DOTNET2SP1 || DOTNET35
		private string varyByContentEncodings;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="CacheAttribute"/> class.
		/// </summary>
		/// <param name="cacheability">Sets the Cache-Control HTTP header. 
		/// The Cache-Control HTTP header controls how documents are to be cached on the network.</param>
		public CacheAttribute(HttpCacheability cacheability)
		{
			this.cacheability = cacheability;

			allowInHistory = true;
			validUntilExpires = true;
			revalidation = HttpCacheRevalidation.None;
		}

		/// <summary>
		/// From MSDN: Makes the response is available in the client browser 
		/// History cache, regardless of the HttpCacheability setting 
		/// made on the server, when the allow parameter is true.
		/// </summary>
		/// <remarks>
		/// When HttpCacheability is set to NoCache or ServerAndNoCache the Expires 
		/// HTTP header is by default set to -1; this tells the client not to 
		/// cache responses in the History folder, so that when you use the back/forward buttons 
		/// the client requests a new version of the response each time. You can override this 
		/// behavior by calling the SetAllowResponseInBrowserHistory method with the 
		/// allow parameter set to true.
		/// <para>
		/// If HttpCacheability is set to values other than NoCache or ServerAndNoCache, calling the SetAllowResponseInBrowserHistory method with either value for allow has no effect.
		/// </para>
		/// </remarks>
		public bool AllowInHistory
		{
			get { return allowInHistory; }
			set { allowInHistory = value; }
		}

		/// <summary>
		/// If true, sets the ETag HTTP header based on the time stamps of the handler's file dependencies.
		/// </summary>
		public bool SetEtagFromFileDependencies
		{
			get { return setEtagFromFileDependencies; }
			set { setEtagFromFileDependencies = value; }
		}

		/// <summary>
		/// If true, sets the Last-Modified HTTP header based on the time stamps of the handler's file dependencies.
		/// </summary>
		public bool SetLastModifiedFromFileDependencies
		{
			get { return setLastModifiedFromFileDependencies; }
			set { setLastModifiedFromFileDependencies = value; }
		}

		/// <summary>
		/// If true, stops all origin-server caching for the current response.
		/// </summary>
		public bool SetNoServerCaching
		{
			get { return setNoServerCaching; }
			set { setNoServerCaching = value; }
		}

		/// <summary>
		/// If true, sets the Cache-Control: no-store HTTP header.
		/// </summary>
		public bool SetNoStore
		{
			get { return setNoStore; }
			set { setNoStore = value; }
		}

		/// <summary>
		/// If true, sets the Cache-Control: no-transform HTTP header.
		/// </summary>
		public bool SetNoTransforms
		{
			get { return setNoTransforms; }
			set { setNoTransforms = value; }
		}

		/// <summary>
		/// If true, includes the vary:* header in the response when varying by parameters.
		/// </summary>
		public bool OmitVaryStar
		{
			get { return omitVaryStar; }
			set { omitVaryStar = value; }
		}

		/// <summary>
		/// From MSDN: Sets cache expiration to from absolute to sliding.
		/// </summary>
		/// <remarks>
		/// When cache expiration is set to sliding, the Cache-Control 
		/// HTTP header will be renewed with each response. This expiration mode 
		/// is identical to the IIS configuration option to add an expiration 
		/// header to all output set relative to the current time.
		/// <para>
		/// If you explicitly set sliding expiration to off (false), that setting 
		/// will be preserved and any attempts to enable sliding expiration will 
		/// silently fail. This method does not directly map to an HTTP header. 
		/// It is used by subsequent modules or worker requests to set origin-server cache policy.
		/// </para>
		/// </remarks>
		public bool SlidingExpiration
		{
			get { return slidingExpiration; }
			set { slidingExpiration = value; }
		}

		/// <summary>
		/// Specifies whether the ASP.NET cache should ignore HTTP Cache-Control 
		/// headers sent by the client that invalidate the cache.
		/// </summary>
		/// <remarks>
		/// This method is provided because some browsers, when refreshing a 
		/// page view, send HTTP cache invalidation headers to the Web server 
		/// and evict the page from the cache. When the validUntilExpires parameter 
		/// is true, ASP.NET ignores cache invalidation headers and the page 
		/// remains in the cache until it expires.
		/// </remarks>
		public bool ValidUntilExpires
		{
			get { return validUntilExpires; }
			set { validUntilExpires = value; }
		}

		/// <summary>
		/// Text to be appended to the Cache-Control HTTP header.
		/// </summary>
		public string CacheExtension
		{
			get { return cacheExtension; }
			set { cacheExtension = value; }
		}

		/// <summary>
		/// Sets the ETag HTTP header to the specified string.
		/// </summary>
		/// <remarks>
		/// The ETag header is a unique identifier for a specific version of 
		/// a document. It is used by clients to validate client-cached content to 
		/// avoid requesting it again. Once an ETag header is set, subsequent 
		/// attempts to set it fail and an exception is thrown.
		/// </remarks>
		public string ETag
		{
			get { return etag; }
			set { etag = value; }
		}

		/// <summary>
		/// Cache Duration (in seconds)
		/// </summary>
		public int Duration
		{
			get { return duration; }
			set { duration = value; }
		}

#if DOTNET2SP1 || DOTNET35
		/// <summary>
		/// Gets or sets the list of all Content-Encoding headers that will be used to vary the output cache.
		/// </summary>
		public string VaryByContentEncodings
		{
			get { return varyByContentEncodings; }
			set { varyByContentEncodings = value; }
		}
#endif

		/// <summary>
		/// Specifies a custom text string to vary cached output responses by.
		/// </summary>
		public string VaryByCustom
		{
			get { return varyByCustom; }
			set { varyByCustom = value; }
		}

		/// <summary>
		/// Gets or sets the list of all HTTP headers that will be used to vary cache output.
		/// </summary>
		/// <remarks>
		/// When a cached item has several vary headers, a separate version of 
		/// the requested document is available from the cache for each HTTP header type.
		/// </remarks>
		public string VaryByHeaders
		{
			get { return varyByHeaders; }
			set { varyByHeaders = value; }
		}

		/// <summary>
		/// Gets or sets the list of parameters received by an HTTP GET or HTTP POST that affect caching.
		/// </summary>
		/// <remarks>
		/// A separate version of the requested document is available from the cache 
		/// for each named parameter in the VaryByParams collection.
		/// </remarks>
		public string VaryByParams
		{
			get { return varyByParams; }
			set { varyByParams = value; }
		}

		/// <summary>
		/// Gets or sets the Last-Modified header HTTP header.
		/// </summary>
		public DateTime? LastModified
		{
			get { return lastModified; }
			set { lastModified = value; }
		}

		/// <summary>
		/// Gets or sets the Cache-Control: max-age HTTP header.
		/// </summary>
		public TimeSpan? MaxAge
		{
			get { return maxAge; }
			set { maxAge = value; }
		}

		/// <summary>
		/// Gets or sets the Cache-Control: s-maxage HTTP header.
		/// </summary>
		public TimeSpan? ProxyMaxAge
		{
			get { return proxyMaxAge; }
			set { proxyMaxAge = value; }
		}

		/// <summary>
		/// Gets or sets the Cache-Control must-revalidate or proxy-revalidate HTTP header.
		/// </summary>
		public HttpCacheRevalidation Revalidation
		{
			get { return revalidation; }
			set { revalidation = value; }
		}

		/// <summary>
		/// Configures ASP.Net's Cache policy based on properties set
		/// </summary>
		/// <param name="policy">cache policy to set</param>
		void ICachePolicyConfigurer.Configure(HttpCachePolicy policy)
		{			
			policy.SetAllowResponseInBrowserHistory(allowInHistory);
			policy.SetCacheability(cacheability);			
			policy.SetOmitVaryStar(omitVaryStar);			
			policy.SetRevalidation(revalidation);
			policy.SetSlidingExpiration(slidingExpiration);
			policy.SetValidUntilExpires(validUntilExpires);
			
			if (duration != 0)
			{
				policy.SetExpires(DateTime.Now.AddSeconds(duration));
			}

#if DOTNET2SP1 || DOTNET35
			if (varyByContentEncodings != null)
			{
				foreach (String header in varyByContentEncodings.Split(','))
				{
					policy.VaryByContentEncodings[header.Trim()] = true;
				}
			}
#endif

			if (varyByCustom != null)
			{
				policy.SetVaryByCustom(varyByCustom);
			}
			
			if (varyByHeaders != null)
			{
				foreach(String header in varyByHeaders.Split(','))
				{
					policy.VaryByHeaders[header.Trim()] = true;
				}
			}

			if (varyByParams != null)
			{
				foreach(String param in varyByParams.Split(','))
				{
					policy.VaryByParams[param.Trim()] = true;
				}
			}

			if (cacheExtension != null)
			{
				policy.AppendCacheExtension(cacheExtension);
			}

			if (setEtagFromFileDependencies)
			{
				policy.SetETagFromFileDependencies();
			}

			if (setLastModifiedFromFileDependencies)
			{
				policy.SetLastModifiedFromFileDependencies();
			}

			if (setNoServerCaching)
			{
				policy.SetNoServerCaching();
			}

			if (setNoStore)
			{
				policy.SetNoStore();
			}

			if (setNoTransforms)
			{
				policy.SetNoTransforms();
			}

			if (etag != null)
			{
				policy.SetETag(etag);
			}

			if (lastModified != null)
			{
				policy.SetLastModified(lastModified.Value);
			}

			if (maxAge != null)
			{
				policy.SetMaxAge(maxAge.Value);
			}

			if (proxyMaxAge != null)
			{
				policy.SetProxyMaxAge(proxyMaxAge.Value);
			}
		}
	}
}
