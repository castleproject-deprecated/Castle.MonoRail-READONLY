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

namespace Castle.Facilities.NHibernateIntegration
{
	using System;

	/// <summary>
	/// Summary description for IGenericDao.
	/// </summary>
	/// <remarks>
	/// Contributed by Steve Degosserie &lt;steve.degosserie@vn.netika.com&gt;
	/// </remarks>
	public interface IGenericDao
	{
		Array FindAll(Type type);

		Array FindAll(Type type, int firstRow, int maxRows);
		
		object FindById(Type type, object id);
		
		object Create(object instance);
		
		void Update(object instance);
		
		void Delete(object instance);
		
		void DeleteAll(Type type);
		
		void Save(object instance);
	}
}
