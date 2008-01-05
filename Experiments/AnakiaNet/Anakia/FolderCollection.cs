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

namespace Anakia
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	public class FolderCollection : NameObjectCollectionBase, IEnumerable
	{
		private readonly Folder owner;

		public FolderCollection(Folder owner) : base(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default)
		{
			this.owner = owner;
		}

		public void Add(Folder folder)
		{
			BaseAdd(folder.Name, folder);
			
			folder.Parent = owner;
		}
		
		public Folder this[String folderName]
		{
			get { return (Folder) BaseGet(folderName); }
			set { Add(value); }
		}

		public new IEnumerator GetEnumerator()
		{
			return BaseGetAllValues().GetEnumerator();
		}
	}
}
