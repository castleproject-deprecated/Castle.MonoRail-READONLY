// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace TestSiteARSupport.Model
{
	using System;
	using Castle.ActiveRecord;
	using Castle.Components.Validator;

	[ActiveRecord("TSAS_Categories")]
	public class Category : ActiveRecordBase
	{
		private Guid id;
		private String name;

		public Category()
		{
		}

		public Category(String name)
		{
			this.name = name;
		}

		[PrimaryKey(PrimaryKeyType.Guid)]
		public Guid Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property, ValidateNonEmpty]
		public String Name
		{
			get { return name; }
			set { name = value; }
		}

		public static Category[] FindAll()
		{
			return (Category[]) FindAll(typeof (Category));
		}
	}
}