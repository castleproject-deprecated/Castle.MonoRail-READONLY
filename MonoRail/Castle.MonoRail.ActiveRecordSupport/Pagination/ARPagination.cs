 // Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.ActiveRecordSupport.Pagination
{
	using System;
	using System.Collections;
	using System.Text.RegularExpressions;
	
	using Castle.ActiveRecord.Queries;
	using Castle.Components.Pagination;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;
	
	using NHibernate.Criterion;

	/// <summary>
	/// Pendent
	/// </summary>
	public class ARPaginationHelper : AbstractHelper
	{
		/// <summary>
		/// Paginates using an <see cref="ARPaginableSimpleQuery"/>
		/// </summary>
		public static IPaginatedPage CreatePagination(int pageSize, Type targetType, string hql, params object[] parameters)
		{
			IARPaginableDataSource criteria = new ARPaginableSimpleQuery(targetType, hql, parameters);
			return CreatePagination(pageSize, criteria);
		}

		/// <summary>
		/// Paginates using an <see cref="ARPaginableCriteria"/>
		/// </summary>
		public static IPaginatedPage CreatePagination(int pageSize, Type targetType, params Order[] orders)
		{
			return CreatePagination(pageSize, targetType, orders, null);
		}

		/// <summary>
		/// Paginates using an <see cref="ARPaginableCriteria"/>
		/// </summary>
		public static IPaginatedPage CreatePagination(int pageSize, Type targetType, Order[] orders, params ICriterion[] criterions)
		{
			IARPaginableDataSource criteria = new ARPaginableCriteria(targetType, orders, criterions);
			return CreatePagination(pageSize, criteria);
		}

		/// <summary>
		/// Paginates using an <see cref="ARPaginableCriteria"/>
		/// </summary>
		public static IPaginatedPage CreatePagination(int pageSize, Type targetType, DetachedCriteria detachedCriteria)
		{
			IARPaginableDataSource criteria = new ARPaginableCriteria(targetType, detachedCriteria);
			return CreatePagination(pageSize, criteria);
		}

		/// <summary>
		/// Paginates using the specified <see cref="IARPaginableDataSource"/>.
		/// </summary>
		public static IPaginatedPage CreatePagination(int pageSize, IARPaginableDataSource criteria)
		{
			return new ARPager(pageSize, criteria, ObtainCurrentPage());
		}

		private static int ObtainCurrentPage()
		{
			// Tight coupling that makes difficult to test. Review this!
			String page = MonoRailHttpHandlerFactory.CurrentEngineContext.Request.Params["page"];
			return page == null || Regex.IsMatch(page, "\\D")
				? 1 : Convert.ToInt32(page);
		}
	}

	/// <summary>
	/// Pendent
	/// </summary>
	public class ARPager : AbstractPage
	{
		private IEnumerable enumerable;

		public ARPager(int pageSize, IARPaginableDataSource source, int currentPage)
		{
			int count = source.ObtainCount();
			int startIndex = (pageSize * currentPage) - pageSize;
			int endIndex = Math.Min(startIndex + pageSize, count);

			enumerable = source.Paginate(pageSize, currentPage);

			CalculatePaginationInfo(startIndex, endIndex, count, pageSize, currentPage);
		}

		protected override IEnumerator GetEnumeratorImplementation()
		{
			return enumerable.GetEnumerator();
		}

		protected override object GetItemAtIndex(int itemIndex) {
			return Castle.Components.Pagination.PaginationSupport.GetItemAtIndex(this.enumerable, itemIndex);
		}
	}
}
