// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Rook.Compiler.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.Rook.Compiler.AST;

	[TestFixture]
	public class SourceFileTestCase : AbstractContainerTestCase
	{
		[Test]
		public void EmptySourceFile()
		{
			container.ParserService.Parse("  ");

			AssertNoErrorOrWarnings();

			container.ParserService.Parse("\r\n\r\n");

			AssertNoErrorOrWarnings();
		}

		[Test]
		public void CommentsAtTop()
		{
			container.ParserService.Parse("# this is my first source file");

			AssertNoErrorOrWarnings();

			container.ParserService.Parse("# this is my first source file\r\n");

			AssertNoErrorOrWarnings();
		}

		[Test]
		public void NamespaceDecl()
		{
			container.ParserService.Parse("namespace My.First.Namespace \r\n\r\nend\r\n");

			AssertNoErrorOrWarnings();
		}

		[Test]
		public void QualifiedRefs1()
		{
			SourceUnit unit = 
				container.ParserService.Parse("System::Console.WriteLine(\"something\") \r\n\r\n");

			IStatement stmt = unit.Statements[0];

			AssertNoErrorOrWarnings();

			container.ParserService.Parse("puts(\"something\") \r\n\r\n");

			AssertNoErrorOrWarnings();
		}
	}
}
