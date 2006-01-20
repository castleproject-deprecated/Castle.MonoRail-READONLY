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

namespace TestScaffolding.Model
{
	using System;

	using Castle.ActiveRecord;

	[ActiveRecord("Categories")]
	public class Category : ActiveRecordBase
	{
		private int _id;
		private String _name;

		public Category()
		{
		}

		public Category(String name)
		{
			_name = name;
		}

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Property, ValidateNotEmptyAttribute]
		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}
