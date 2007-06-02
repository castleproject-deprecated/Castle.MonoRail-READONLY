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

namespace TestSiteNVelocity.Components
{
	using Castle.MonoRail.Framework;

	public class AutoParameterBinding : ViewComponent
	{
		private int id;
		private string name;
		private string url;

		[ViewComponentParam(Required = true)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[ViewComponentParam]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[ViewComponentParam]
		public string Url
		{
			get { return url; }
			set { url = value; }
		}

		public override void Render()
		{
			RenderText(string.Format("'{0}' '{1}' '{2}'", id, name, url));
		}
	}
}
