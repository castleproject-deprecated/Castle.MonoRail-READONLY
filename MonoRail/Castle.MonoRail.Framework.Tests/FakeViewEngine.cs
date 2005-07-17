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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Summary description for FakeViewEngine.
	/// </summary>
	public class FakeViewEngine : ViewEngineBase
	{
		private Hashtable _paths = 
			new Hashtable( 
				CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default );

		public FakeViewEngine()
		{
		}

		#region IViewEngine Members

		public override void Init()
		{
		}

		public override bool HasTemplate(String templateName)
		{
			throw new NotImplementedException();
		}

		public override void Process(IRailsEngineContext context, Controller controller, String viewName)
		{
			String path = PathFrom(viewName);
			String view = ViewFrom(viewName);

			ViewCollection viewColl = ObtainViewCollection(path);

			String contents = viewColl[view];

			if (contents == null)
			{
				contents = "view not found!";
			}

			context.Response.Write(contents);

			AdjustContentType(context);
		}

		public override void ProcessContents(IRailsEngineContext context, Controller controller, String contents)
		{
			throw new NotImplementedException();
		}

		#endregion

		public void AddView( String path, String name, String contents )
		{
			ViewCollection viewColl = ObtainViewCollection(path);
			viewColl.Add(name, contents);
		}

		protected ViewCollection ObtainViewCollection(string path)
		{
			ViewCollection coll = _paths[path] as ViewCollection;

			if (coll == null)
			{
				coll = new ViewCollection();
				_paths[path] = coll;
			}

			return coll;
		}

		protected string ViewFrom(string name)
		{
			int index = name.LastIndexOf('/');
			if (index == -1) index = name.LastIndexOf('\\');

			return name.Substring(index + 1);
		}

		protected string PathFrom(string name)
		{
			int index = name.LastIndexOf('/');
			if (index == -1) index = name.LastIndexOf('\\');

			return name.Substring(0, index);
		}
	}

	public class FakeViewEngineWithLayoutSupport : FakeViewEngine
	{
		public override void Process(IRailsEngineContext context, Controller controller, String viewName)
		{
			String layoutContents = null;

			if (controller.LayoutName != null)
			{
				ViewCollection viewLayoutsColl = ObtainViewCollection("layouts");
				layoutContents = viewLayoutsColl[controller.LayoutName];
			}

			String path = PathFrom(viewName);
			String view = ViewFrom(viewName);

			ViewCollection viewColl = ObtainViewCollection(path);

			String contents = viewColl[view];

			if (contents == null)
			{
				contents = "view not found!";
			}

			if (layoutContents != null)
			{
				contents = String.Format(layoutContents, contents);
			}

			context.Response.Write(contents);
		}
	}

	public class ViewCollection : NameObjectCollectionBase
	{
		public void Add(String name, String contents)
		{
			BaseAdd(name, contents);
		}

		public String this [String name]
		{
			get { return BaseGet(name) as String; }
		}
	}
}
