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

namespace Castle.ActiveRecord.Generator.Components.Database
{
	using System;
	using System.Collections;


	[Serializable]
	public class TableDefinition
	{
		private ArrayList _hasRelation = new ArrayList();
		private DatabaseDefinition _db;
		private String _name;
		private ActiveRecordDescriptor _relatedDescriptor;
		private ColumnDefinitionCollection _columns = new ColumnDefinitionCollection();

		public TableDefinition(String name, DatabaseDefinition db)
		{
			_name = name;
			_db = db;
		}

		public void AddManyRelation(TableDefinition def)
		{
			if (!_hasRelation.Contains(def))
			{
				_hasRelation.Add(def);
			}
		}

		public String Name
		{
			get { return _name; }
		}

		public ActiveRecordDescriptor RelatedDescriptor
		{
			get { return _relatedDescriptor; }
			set { _relatedDescriptor = value; }
		}

		public ColumnDefinition AddColumn( ColumnDefinition column )
		{
			_columns.Add( column );
			return column;
		}

		public ColumnDefinitionCollection Columns
		{
			get { return _columns; }
		}

		public IList TablesReferencedByHasRelation
		{
			get { return _hasRelation; }
		}

		public DatabaseDefinition DatabaseDefinition
		{
			get { return _db; }
			set { _db = value; }
		}

		public override String ToString()
		{
			return _name;
		}
	}
}
