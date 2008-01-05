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

namespace DiggExample
{
	using System;
	using System.Web;
	using DiggExample.Model;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Config;

	public class GlobalApplication : HttpApplication
	{
		public GlobalApplication()
		{
		}

		public void Application_OnStart()
		{
			ActiveRecordStarter.Initialize(ActiveRecordSectionHandler.Instance,
							   new Type[] { typeof(MyEntity) });

			ActiveRecordStarter.CreateSchema();
			
			using(new SessionScope())
			{
				for (int i = 0; i < 200; i++)
				{
					MyEntity e = new MyEntity();
					e.Name = "Entity " + (i + 1);
					e.Index = i;
					e.Create();
				}
			}
		}

		public void Application_OnEnd()
		{
			ActiveRecordStarter.DropSchema();
		}
	}
}
