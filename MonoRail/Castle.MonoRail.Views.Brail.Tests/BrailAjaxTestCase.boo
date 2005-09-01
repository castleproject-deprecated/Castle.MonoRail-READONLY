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
namespace Castle.MonoRail.Views.Brail.Tests

import System
import System.IO
import NUnit.Framework
import Castle.MonoRail.Engine.Tests

[TestFixture]
class BrailAjaxTestCase(AbstractCassiniTestCase):
	
	[Test]
	def JsFunctions():
		url = "/ajax/JsFunctions.rails"
		expected = "<script type=\"text/javascript\" src=\"/MonoRail/Files/AjaxScripts.rails\"></script>"
		Execute(url, expected)
		
	[Test]
	def LinkToFunction():
		url = "/ajax/LinkToFunction.rails"
		expected = "<a href=\"#\" onclick=\"alert('Ok'); return false;\" ><img src='myimg.gid'></a>"
		Execute(url, expected)
		
	[Test]
	def LinkToRemote():
		url = "/ajax/LinkToRemote.rails";
		expected = "<a href=\"#\" onclick=\"new Ajax.Request('/controller/action.rails', {asynchronous:true}); return false;\" ><img src='myimg.gid'></a>"
		Execute(url, expected)
		
	[Test]
	def BuildFormRemoteTag():
		url = "/ajax/BuildFormRemoteTag.rails"
		expected = "<form  onsubmit=\"new Ajax.Request('url', {asynchronous:true, parameters:Form.serialize(this)}); return false;\" enctype=\"multipart/form-data\">"
		Execute(url, expected);
		
	[Test]
	def ObserveField():
		url = "/ajax/ObserveField.rails"
		expected = "<script type=\"text/javascript\">new Form.Element.Observer('myfieldid', 2, function(element, value) { new Ajax.Updater('elementToBeUpdated', '/url', {asynchronous:true, parameters:newcontent}) })</script>"
		Execute(url, expected)
	
	
	[Test]
	def ObserveForm():
		url = "/ajax/ObserveForm.rails"
		expected = "<script type=\"text/javascript\">new Form.Observer('myfieldid', 2, function(element, value) { new Ajax.Updater('elementToBeUpdated', '/url', {asynchronous:true, parameters:newcontent}) })</script>"
		Execute(url, expected);
	
	override def ObtainPhysicalDir():
		return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"""..\TestSiteBrail""")
	
	
		
