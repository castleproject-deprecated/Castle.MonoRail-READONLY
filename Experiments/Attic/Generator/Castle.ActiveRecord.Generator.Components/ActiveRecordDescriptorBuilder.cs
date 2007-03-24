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

namespace Castle.ActiveRecord.Generator.Components
{
	using System;

	using Castle.ActiveRecord.Generator.Components.Database;


	public class ActiveRecordDescriptorBuilder : IActiveRecordDescriptorBuilder
	{
		private INamingService _namingService;
		private IPlainFieldInferenceService _plainFieldsService;
		private IRelationshipInferenceService _relationsService;

		public ActiveRecordDescriptorBuilder(INamingService namingService,
			IPlainFieldInferenceService plainFieldsService, 
			IRelationshipInferenceService relationsService)
		{
			_namingService = namingService;
			_plainFieldsService = plainFieldsService;
			_relationsService = relationsService;
		}

		#region IActiveRecordDescriptorBuilder Members

		public ActiveRecordDescriptor[] Build(BuildContext context)
		{
			while(context.HasPendents)
			{
				ActiveRecordDescriptor pendent = context.GetNextPendent();
				Build(pendent.Table, context);
			}
			
			ActiveRecordDescriptor[] array = new ActiveRecordDescriptor[context.NewlyCreatedDescriptors.Count];
			context.NewlyCreatedDescriptors.CopyTo(array, 0);
			return array;
		}

		public ActiveRecordDescriptor Build(TableDefinition tableDef, BuildContext context)
		{
			ActiveRecordDescriptor desc = ObtainDescriptor(tableDef);

			// ClassName

			desc.ClassName = _namingService.CreateClassName( tableDef.Name );

			// Plain fields

			desc.Properties.AddRange( _plainFieldsService.InferProperties(tableDef) );

			// Relations

			desc.PropertiesRelations.AddRange( _relationsService.InferRelations(desc, tableDef, context) );

			// Operations

			// TODO!

			context.RemovePendent(desc);

			Build(context);

			return desc;
		}

		#endregion

		private ActiveRecordDescriptor ObtainDescriptor(TableDefinition tableDef)
		{
			if (tableDef.RelatedDescriptor != null)
			{
				return tableDef.RelatedDescriptor;
			}
			else
			{
				return new ActiveRecordDescriptor(tableDef);
			}
		}
	}
}
