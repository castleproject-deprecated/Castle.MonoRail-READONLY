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

namespace Castle.Facilities.ActiveRecordGenerator.Model
{
	using System;
	using System.Collections;


	public class ActiveRecordDescriptor
	{
		private bool _generate = true;
		private String _className;
		private IList _properties = new ArrayList();

		public ActiveRecordDescriptor(String className)
		{
			_className = className;
		}

		public String ClassName
		{
			get { return _className; }
			set { _className = value; }
		}

		public void AddProperty( ActiveRecordPropertyDescriptor propertyDescriptor )
		{
			_properties.Add(propertyDescriptor);
		}

		public IList Properties
		{
			get { return _properties; }
		}

		public bool Generate
		{
			get { return _generate; }
			set { _generate = value; }
		}
	}

	public class ActiveRecordPropertyDescriptor
	{
		private bool _generate = true;
		private String _columnName;
		private String _columnTypeName = "VARCHAR";
		private String _propertyName;
		private String _propertyFieldName;
		private Type _propertyType;

		public ActiveRecordPropertyDescriptor(string columnName, string columnTypeName, string propertyName, string propertyFieldName, Type propertyType)
		{
			_columnName = columnName;
			_columnTypeName = columnTypeName;
			_propertyName = propertyName;
			_propertyFieldName = propertyFieldName;
			_propertyType = propertyType;
		}

		public string ColumnName
		{
			get { return _columnName; }
			set { _columnName = value; }
		}

		public string ColumnTypeName
		{
			get { return _columnTypeName; }
			set { _columnTypeName = value; }
		}

		public string PropertyName
		{
			get { return _propertyName; }
			set { _propertyName = value; }
		}

		public string PropertyFieldName
		{
			get { return _propertyFieldName; }
			set { _propertyFieldName = value; }
		}

		public Type PropertyType
		{
			get { return _propertyType; }
			set { _propertyType = value; }
		}

		public bool Generate
		{
			get { return _generate; }
			set { _generate = value; }
		}
	}
}
