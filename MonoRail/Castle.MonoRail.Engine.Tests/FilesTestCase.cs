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

namespace Castle.MonoRail.Engine.Tests
{
	using System;

	using NUnit.Framework;

	[TestFixture]
	public class FilesTestCase : AbstractCassiniTestCase
	{
		public FilesTestCase()
		{
		}

		[Test]
		public void Ajax()
		{
			string expected = "\r\n/*  Prototype: an object-oriented Javascript library, version 1.2.0";
			string url = "/MonoRail/Files/AjaxScripts.rails";

			Execute(url, expected);
		}

		[Test]
		public void Fade()
		{
			string expected = "\r\n// @name      The Fade Anything Technique";
			string url = "/MonoRail/Files/EffectsFatScripts.rails";

			Execute(url, expected);
		}
	}
}
