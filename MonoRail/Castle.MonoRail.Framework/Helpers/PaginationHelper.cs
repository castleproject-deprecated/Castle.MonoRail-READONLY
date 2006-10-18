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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
#if DOTNET2
	using System.Collections.Generic;
#endif

	/// <summary>
	/// Used as callback handler to obtain the items 
	/// to be displayed. 
	/// </summary>
	public delegate IList DataObtentionDelegate();

	/// <summary>
	/// This helper allows you to easily paginate through a data source 
	/// (anything that implements <see cref="IList"/>)
	/// </summary>
	public class PaginationHelper : AbstractHelper
	{
		/// <summary>
		/// Creates a link to navigate to a specific page
		/// </summary>
		/// <param name="page"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public String CreatePageLink(int page, String text)
		{
			return CreatePageLink(page, text, null, null);
		}

		public String CreatePageLink(int page, String text, IDictionary htmlAttributes)
		{
			return CreatePageLink(page, text, htmlAttributes, null);
		}

		public String CreatePageLink(int page, String text, IDictionary htmlAttributes, IDictionary queryStringParams)
		{
			String filePath = CurrentContext.Request.FilePath;

			if (queryStringParams == null)
			{
				queryStringParams = new Hashtable();
			}
			else if (queryStringParams.IsReadOnly || queryStringParams.IsFixedSize)
			{
				queryStringParams = new Hashtable(queryStringParams);
			}

			queryStringParams["page"] = page.ToString();

			return String.Format("<a href=\"{0}?{1}\" {2}>{3}</a>", 
				filePath, BuildQueryString(queryStringParams), GetAttributes(htmlAttributes), text);
		}

		/// <summary>
		/// Creates a <see cref="Page"/> which is a sliced view of
		/// the data source
		/// </summary>
		/// <param name="datasource">Data source to be used as target of the pagination</param>
		/// <param name="pageSize">Page size</param>
		/// <returns>A <see cref="Page"/> instance</returns>
		public static IPaginatedPage CreatePagination(IList datasource, int pageSize)
		{
			String currentPage = CurrentContext.Request.Params["page"];

			int curPage = 1;

			if (currentPage != null)
			{
				curPage = Int32.Parse(currentPage);
			}

			return CreatePagination(datasource, pageSize, curPage);
		}

		/// <summary>
		/// Creates a <see cref="Page"/> which is a sliced view of
		/// the data source
		/// </summary>
		/// <param name="datasource">Data source to be used as target of the pagination</param>
		/// <param name="pageSize">Page size</param>
		/// <param name="currentPage">current page index (1 based)</param>
		/// <returns>A <see cref="Page"/> instance</returns>
		public static IPaginatedPage CreatePagination(IList datasource, int pageSize, int currentPage)
		{
			if (currentPage <= 0) currentPage = 1;

			return new Page(datasource, currentPage, pageSize);
		}

#if DOTNET2

		/// <summary>
		/// Creates a <see cref="Page"/> which is a sliced view of
		/// the data source
		/// </summary>
		/// <param name="datasource">Data source to be used as target of the pagination</param>
		/// <param name="pageSize">Page size</param>
		/// <returns>A <see cref="Page"/> instance</returns>
		public static IPaginatedPage CreatePagination<T>(IList<T> datasource, int pageSize)
		{
			String currentPage = CurrentContext.Request.Params["page"];

			int curPage = 1;

			if (currentPage != null)
			{
				curPage = Int32.Parse(currentPage);
			}

			return CreatePagination(datasource, pageSize, curPage);
		}

		/// <summary>
		/// Creates a <see cref="Page"/> which is a sliced view of
		/// the data source
		/// </summary>
		/// <param name="datasource">Data source to be used as target of the pagination</param>
		/// <param name="pageSize">Page size</param>
		/// <param name="currentPage">current page index (1 based)</param>
		/// <returns>A <see cref="Page"/> instance</returns>
		public static IPaginatedPage CreatePagination<T>(IList<T> datasource, int pageSize, int currentPage)
		{
			if (currentPage <= 0) currentPage = 1;

			return new GenericPage<T>(datasource, currentPage, pageSize);
		}

#endif
		
		/// <summary>
		/// Creates a <see cref="Page"/> which is a sliced view of
		/// the data source. This method first looks for the datasource 
		/// in the <see cref="System.Web.Caching.Cache"/> and if not found, 
		/// it invokes the <c>dataObtentionCallback</c> and caches the result
		/// using the specifed <c>cacheKey</c>
		/// </summary>
		/// <param name="cacheKey">Cache key used to query/store the datasource</param>
		/// <param name="pageSize">Page size</param>
		/// <param name="dataObtentionCallback">Callback to be used to populate the cache</param>
		/// <returns>A <see cref="Page"/> instance</returns>
		public static IPaginatedPage CreateCachedPagination(String cacheKey, int pageSize, DataObtentionDelegate dataObtentionCallback)
		{
			IList datasource = (IList) CurrentContext.Cache[cacheKey];

			if (datasource == null)
			{
				datasource = dataObtentionCallback();

				CurrentContext.Cache.Insert(
					cacheKey, datasource, null, 
					DateTime.MaxValue, TimeSpan.FromSeconds(10));
			}

			return CreatePagination(datasource, pageSize);
		}
	}

	/// <summary>
	/// Represents the sliced data and offers
	/// a few read only properties to create a pagination bar.
	/// </summary>
	public class Page : AbstractPage
	{
		private readonly IList slice = new ArrayList();

		public Page(IList list, int curPage, int pageSize)
		{
			// Calculate slice indexes
			int startIndex = (pageSize * curPage) - pageSize;
			int endIndex =  Math.Min(startIndex + pageSize, list.Count);

			CreateSlicedCollection(startIndex, endIndex, list);

			CalculatePaginationInfo(startIndex, endIndex, list.Count, pageSize, curPage);
		}

		private void CreateSlicedCollection(int startIndex, int endIndex, IList list)
		{
			for(int index = startIndex; index < endIndex; index++)
			{
				slice.Add( list[index] );
			}
		}

		public override IEnumerator GetEnumerator()
		{
			return slice.GetEnumerator();
		}
	}

#if DOTNET2

	/// <summary>
	/// Represents the sliced data and offers
	/// a few read only properties to create a pagination bar.
	/// </summary>
	public class GenericPage<T> : AbstractPage
	{
		private readonly int sliceStart, sliceEnd;
		private readonly IList<T> sourceList;

		public GenericPage(IList<T> list, int curPage, int pageSize)
		{
			// Calculate slice indexes
			int startIndex = this.sliceStart = (pageSize * curPage) - pageSize;
			int endIndex = this.sliceEnd = Math.Min(startIndex + pageSize, list.Count);

			this.sourceList = list;

			CalculatePaginationInfo(startIndex, endIndex, list.Count, pageSize, curPage);
		}

		public override IEnumerator GetEnumerator()
		{
			for (int i = sliceStart; i < sliceEnd; i++)
			{
				yield return this.sourceList[i];
			}
		}
	}

#endif
	
	public abstract class AbstractPage : IPaginatedPage
	{
		private int firstItem, lastItem, totalItems;
		private int previousIndex, nextIndex, lastIndex, curIndex;
		private bool hasPrev, hasNext, hasFirst, hasLast;

		protected void CalculatePaginationInfo(int startIndex, int endIndex, int count, int pageSize, int curPage)
		{
			firstItem = count != 0 ? startIndex + 1 : 0;
			lastItem = endIndex;
			totalItems = count;
	
			hasPrev = startIndex != 0;
			hasNext = count == -1 || (startIndex + pageSize) < count;
			hasFirst = curPage != 1;
			hasLast = count > curPage * pageSize;
	
			curIndex = curPage;
			previousIndex = curPage - 1;
			nextIndex = curPage + 1;
			lastIndex = count == -1 ? -1 : count / pageSize;
	
			if (count != -1 && count / (float) pageSize > lastIndex)
			{
				lastIndex++;
			}
		}

		public int FirstIndex
		{
			get { return 1; }
		}

		public int CurrentIndex
		{
			get { return curIndex; }
		}

		public int LastIndex
		{
			get { return lastIndex; }
		}

		public int PreviousIndex
		{
			get { return previousIndex; }
		}

		public int NextIndex
		{
			get { return nextIndex; }
		}

		public bool HasPrevious
		{
			get { return hasPrev; }
		}

		public bool HasNext
		{
			get { return hasNext; }
		}

		public bool HasFirst
		{
			get { return hasFirst; }
		}

		public bool HasLast
		{
			get { return hasLast; }
		}

		public int FirstItem
		{
			get { return firstItem; }
		}

		public int LastItem
		{
			get { return lastItem; }
		}

		public int TotalItems
		{
			get { return totalItems; }
		}

		public abstract IEnumerator GetEnumerator();
	}
}
