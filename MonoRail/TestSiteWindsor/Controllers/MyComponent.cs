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

namespace TestSiteWindsor.Controllers
{
	using System;

	using Castle.MonoRail.Framework;

	public class MyComponent : ViewComponent
	{
		public MyComponent()
		{
		}
		
		/// <summary>
		/// Called by the framework once the component instance
		/// is initialized
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
		}

		/// <summary>
		/// Called by the framework so the component can 
		/// render its content
		/// </summary>
		public override void Render()
		{
			base.RenderText("my component");
		}
	}
}
