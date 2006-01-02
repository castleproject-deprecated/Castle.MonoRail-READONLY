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

namespace TestScaffolding.Model
{
	using System;

	using Castle.ActiveRecord;


	[ActiveRecord("Persons", DiscriminatorColumn="type", DiscriminatorValue="person")]
	public class Person : ActiveRecordValidationBase
	{
		private int id;
		private String name;
		private String email;
		private int age;
		private DateTime dob;
		private ContactInfo info;

		public Person()
		{
			dob = DateTime.Now;
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Nested]
		public ContactInfo Contact
		{
			get { return info; }
			set { info = value; }
		}

		[Property, ValidateNotEmpty]
		public String Name
		{
			get { return name; }
			set { name = value; }
		}

		[Property, ValidateNotEmpty, ValidateEmail]
		public String Email
		{
			get { return email; }
			set { email = value; }
		}

		[Property]
		public int Age
		{
			get { return age; }
			set { age = value; }
		}

		[Property]
		public DateTime Dob
		{
			get { return dob; }
			set { dob = value; }
		}
	}
}
