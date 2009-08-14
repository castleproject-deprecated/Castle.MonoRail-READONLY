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

namespace PetStore.Model
{
	using System.Linq;
	using NHibernate;

	public class Storage<T> where T:class, IAggregateRoot
	{
		private static IDao<T> dao = new GenericDao<T>();

		public static void RegisterDao(IDao<T> replacement)
		{
			dao = replacement;
		}

		public static void Create(T t)
		{
			dao.Create(t);
		}

		public static void Update(T t)
		{
			dao.Update(t);
		}

		public static void Save(T t)
		{
			dao.Save(t);
		}

		public static void Delete(T t)
		{
			dao.Delete(t);
		}

		public static T Find(object id)
		{
			return dao.Find(id);
		}

		public static IQueryable<T> Linq()
		{
			return dao.Linq();
		}

		public static ICriteria GetCriteria()
		{
			return dao.GetCriteria();
		}

		public static ICriteria GetCriteria(string alias)
		{
			return dao.GetCriteria(alias);
		}

		public static IQuery GetQuery(string query)
		{
			return dao.GetQuery(query);
		}

		public static IQuery GetNamedQuery(string name)
		{
			return dao.GetNamedQuery(name);
		}
	}
}