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

namespace Castle.Facilities.ActiveRecord.Tests.Model1
{
	using System;
	using System.Collections;

	using Castle.Facilities.ActiveRecord;


	[ActiveRecord]
	public class Order : ActiveRecordBase
	{
		private int _id;
		private String _description;
		private IList _items;

		public Order()
		{
		}

		[PrimaryKey]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public String Description
		{
			get { return _description; }
			set { _description = value; }
		}

		[HasMany( typeof(OrderItem), Key = "order_id" )]
		public IList Items
		{
			get { return _items; }
			set { _items = value; }
		}

		public static Order Find(int id)
		{
			return new Order();
		}
	}
}
