// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Internal
{
	using System;

	public class RescueDescriptor
	{
		private readonly string viewName;
		private readonly Type exceptionType;

		public RescueDescriptor(string viewName, Type exceptionType)
		{
			this.viewName = viewName;
			this.exceptionType = exceptionType;
		}

		public string ViewName
		{
			get { return viewName; }
		}

		public Type ExceptionType
		{
			get { return exceptionType; }
		}
	}
}
