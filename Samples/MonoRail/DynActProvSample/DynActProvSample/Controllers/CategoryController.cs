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

namespace DynActProvSample.Controllers
{
	using System;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using Castle.MonoRail.ActiveRecordSupport;
	using Castle.MonoRail.Framework;
	using DynActProvSample.Models;

	[Layout("default"), Rescue("generalerror")]
	public class CategoryController : ARSmartDispatcherController
	{
		public void List()
		{
			PropertyBag["items"] = Category.FindAll();
		}
		
		public void New()
		{
		}
		
		[AccessibleThrough(Verb.Post)]
		public void Create([ARDataBind("category", AutoLoadBehavior.OnlyNested)] Category category)
		{
			try
			{
				category.Create();

				RedirectToAction("list");
			}
			catch(Exception ex)
			{
				Flash["errormessage"] = ex.Message;
				Flash["category"] = category;

				RedirectToAction("new");
			}
		}
		
		public void Edit(int id)
		{
			if (!Flash.Contains("category"))
			{
				PropertyBag["category"] = Category.Find(id);
			}
		}

		[AccessibleThrough(Verb.Post)]
		public void Update([ARDataBind("category", AutoLoadBehavior.Always)] Category category)
		{
			try
			{
				category.Create();

				RedirectToAction("list");
			}
			catch (Exception ex)
			{
				Flash["errormessage"] = ex.Message;
				Flash["category"] = category;

				RedirectToAction("edit", "id" + category.Id);
			}
		}
		
		public void ConfirmDelete([ARFetch("id", false, true)] Category category)
		{
			PropertyBag["category"] = category;
		}
		
		[AccessibleThrough(Verb.Post)]
		public void Delete([ARFetch("category.id", false, true)] Category category)
		{
			try
			{
				category.Delete();

				RedirectToAction("list");
			}
			catch(Exception ex)
			{
				Flash["errormessage"] = ex.Message;

				RedirectToAction("confirmdelete", "id" + category.Id);
			}
		}
	}

	
	[Crud(typeof(Category))]
	[DynamicActionProvider(typeof(CrudActionProvider))]
	[Layout("default"), Rescue("generalerror")]
	public class Category2Controller : ARSmartDispatcherController
	{
		
	}
	
}
