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

namespace Castle.Facilities.NHibernateIntegration.Tests
{
	using System;
	using System.Collections;

	public class BlogItem
	{
		private string id;
		private string text;
		private string title;
		private Blog blog;
		private DateTime dateTime;

		public Blog ParentBlog
		{
			get { return blog; }
			set { blog = value; }
		}

		public DateTime ItemDate
		{
			get { return dateTime; }
			set { dateTime = value; }
		}

		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		public string Title
		{
			get { return title; }
			set { title = value; }
		}
	}
}