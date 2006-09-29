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

namespace Anakia
{
	using System;

	public class FileToCopy
	{
		private readonly string sourceFile;
		private readonly string targetFile;

		public FileToCopy(string sourceFile, string targetFile)
		{
			this.sourceFile = sourceFile;
			this.targetFile = targetFile;
		}

		public string SourceFile
		{
			get { return sourceFile; }
		}

		public string TargetFile
		{
			get { return targetFile; }
		}
	}
}
