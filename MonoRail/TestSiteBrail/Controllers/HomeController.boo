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
namespace Castle.MonoRail.Views.Brail.TestSite.Controllers

import System
import Castle.MonoRail.Framework

class HomeController(Controller):
	
	def Index():
		pass
		
	def Welcome():
		RenderView("heyhello")
	
	def RedirectAction():
		Redirect("home", "index")
	
	def RedirectForOtherArea():
		Redirect("subrea","home","index")
	
	def Bag():
		PropertyBag.Add("CustomerName", "hammett")
		PropertyBag.Add( "List", ("1","2","3"))
		
	def Bag2():
		PropertyBag.Add("CustomerName", "hammett")
		PropertyBag.Add( "List", ("1","2","3"))
	
	def HelloFromCommon():
		pass
		
	def PreProcessor():
		PropertyBag.Add("Title", "Ayende")
		
	def ShowList():
		dic = { "Ayende": "Rahien", "Foo": "Bar" }
		PropertyBag.Add("dic", dic)
		
	def ShowEmptyList():
		PropertyBag.Add("dic",{})
		RenderView("ShowList")
		
	def Nullables():
		PropertyBag.Add("doesExists","foo")
		
	def Empty():
		pass
		
	def NullableProperties():
		PropertyBag.Add( "List", (Foo("Bar"),Foo(null),Foo("Baz")))
		
		
class Foo:
	
	[Property(Foo)]
	foo as string
	
	def constructor(foo as string):
		self.foo = foo