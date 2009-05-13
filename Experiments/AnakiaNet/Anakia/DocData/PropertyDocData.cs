// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Anakia.DocData
{
	using System;

	public class PropertyDocData : CommonDocData
	{
		private readonly string name, id;
		private readonly string type;
		private readonly Visibility access;
		private readonly ParameterDocData[] parameters;

		public PropertyDocData(string name, string id, string type, Visibility access, ParameterDocData[] parameters)
		{
			this.name = name;
			this.id = id;
			this.type = type;
			this.access = access;
			this.parameters = parameters;
		}

		public string Name
		{
			get { return name; }
		}

		public string PropertyType
		{
			get { return type; }
		}

		public string Id
		{
			get { return id; }
		}

		public Visibility Access
		{
			get { return access; }
		}

		public ParameterDocData[] Parameters
		{
			get { return parameters; }
		}
	}
}
