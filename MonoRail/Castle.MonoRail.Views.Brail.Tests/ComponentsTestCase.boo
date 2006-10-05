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
import Castle.MonoRail.Framework
import Castle.MonoRail.Views.Brail
import Castle.MonoRail.TestSupport
import Castle.MonoRail.Framework.Tests

[TestFixture]
class ComponentsTestCase(AbstractMRTestCase):

	[Test]
	def SimpleInlineViewComponent():
		expected = "static 1\r\nHello from SimpleInlineViewComponent\r\nstatic 2"
		DoGet("usingcomponents/index1.rails")
		AssertReplyEqualTo(expected)
		
	
	[Test]
	def InlineComponentUsingRender():
		expected = 'static 1\r\nThis is a view used by a component static 2'
		DoGet('usingcomponents/index2.rails')
		AssertReplyEqualTo(expected)
		
	
	[Test()]
	def InlineComponentNotOverridingRender():
		expected = 'static 1\r\ndefault component view picked up automatically static 2'
		DoGet('usingcomponents/index3.rails')
		AssertReplyEqualTo(expected)
	
	[Test]
	def InlineComponentWithParam1():
		DoGet('usingcomponents/index4.rails')
		AssertReplyEqualTo('Done')


	[Test]
	def CanGetParamsFromTheComponentInTheView():
		DoGet('usingcomponents/template.rails')
		AssertReplyEqualTo('123')

	[Test]
	def BlockComp1():
		DoGet('usingcomponents/index5.rails')
		AssertReplyEqualTo('  item 0\r\n  item 1\r\n  item 2\r\n')
	
	[Test]
	def BlockWithinForEach():
		DoGet('usingcomponents/index8.rails')
		AssertReplyEqualTo('\r\ninner content 1\r\n\r\ninner content 2\r\n')
	
	[Test]
	def SeveralComponentsInvocation():
		for i in range(10):
			expected = 'static 1\r\nContent 1\r\nstatic 2\r\nContent 2\r\nstatic 3\r\nContent 3\r\nstatic 4\r\nContent 4\r\nstatic 5\r\nContent 5\r\n'
			DoGet('usingcomponents/index9.rails')
			AssertReplyEqualTo(expected)

	[Test]
	def UsingCaptureFor():
		DoGet('usingcomponents/captureFor.rails')
		AssertReplyEqualTo("\r\n1234 Foo, Bar")
	
