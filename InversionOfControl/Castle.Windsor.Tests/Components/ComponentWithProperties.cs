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

namespace Castle.Windsor.Tests.Components
{
	using System;

	public class ComponentWithProperties
	{
		private String prop1;
		private int prop2;

		public string Prop1
		{
			get { return prop1; }
			set { prop1 = value; }
		}

		public int Prop2
		{
			get { return prop2; }
			set { prop2 = value; }
		}
	}

	public class ExtendedComponentWithProperties : ComponentWithProperties
	{
		private int prop3;

		public int Prop3
		{
			get { return prop3; }
			set { prop3 = value; }
		}
	}
}