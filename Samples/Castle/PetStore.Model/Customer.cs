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

namespace PetStore.Model
{
	using System;

	using Castle.ActiveRecord;


	[ActiveRecord(DiscriminatorValue="customer")]
	public class Customer : User
	{
		private String address;
		private String city;
		private String country;
		private String zipcode;

		[Property]
		public string Address
		{
			get { return address; }
			set { address = value; }
		}

		[Property]
		public string City
		{
			get { return city; }
			set { city = value; }
		}

		[Property]
		public string Country
		{
			get { return country; }
			set { country = value; }
		}

		[Property]
		public string Zipcode
		{
			get { return zipcode; }
			set { zipcode = value; }
		}
	}
}
