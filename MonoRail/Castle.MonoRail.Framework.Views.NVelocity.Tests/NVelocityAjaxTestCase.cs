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

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using System;
	using System.IO;

	using NUnit.Framework;

	using Castle.MonoRail.Engine.Tests;


	[TestFixture]
	public class NVelocityAjaxTestCase : AbstractNVelocityTestCase
	{
		[Test]
		public void JsFunctions()
		{
			string url = "/ajax/JsFunctions.rails";
			string expected = "<script type=\"text/javascript\" src=\"/MonoRail/Files/AjaxScripts.rails\"></script>";

			Execute(url, expected);
		}

		[Test]
		public void LinkToFunction()
		{
			string url = "/ajax/LinkToFunction.rails";
			string expected = "<a href=\"#\" onclick=\"alert('Ok'); return false;\" ><img src='myimg.gid'></a>";

			Execute(url, expected);
		}

		[Test]
		public void LinkToRemote()
		{
			string url = "/ajax/LinkToRemote.rails";
			string expected = "<a href=\"#\" onclick=\"new " + 
				"Ajax.Request('/controller/action.rails', {asynchronous:true}); " + 
				"return false;\" ><img src='myimg.gid'></a>";

			Execute(url, expected);
		}

		[Test]
		public void BuildFormRemoteTag()
		{
			string url = "/ajax/BuildFormRemoteTag.rails";
			string expected = "<form  onsubmit=\"new Ajax.Request('url', " +
				"{asynchronous:true, parameters:Form.serialize(this)}); " + 
				"return false;\" enctype=\"multipart/form-data\">";

			Execute(url, expected);
		}

		[Test]
		public void ObserveField()
		{
			string url = "/ajax/ObserveField.rails";
			string expected = "<script type=\"text/javascript\">new Form.Element.Observer('myfieldid', 2, " + 
				"function(element, value) { new Ajax.Updater('elementToBeUpdated', '/url', " + 
				"{asynchronous:true, parameters:newcontent}) })</script>";

			Execute(url, expected);
		}

		[Test]
		public void ObserveForm()
		{
			string url = "/ajax/ObserveForm.rails";
			string expected = "<script type=\"text/javascript\">new Form.Observer('myfieldid', 2, " + 
				"function(element, value) { new Ajax.Updater('elementToBeUpdated', '/url', " + 
				"{asynchronous:true, parameters:newcontent}) })</script>";

			Execute(url, expected);
		}
	}
}
