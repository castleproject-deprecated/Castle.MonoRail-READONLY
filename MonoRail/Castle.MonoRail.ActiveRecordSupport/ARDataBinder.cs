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

namespace Castle.MonoRail.ActiveRecordSupport
{
	using System;
	using System.Collections;
	using System.Reflection;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.Components.Binder;
	using Castle.MonoRail.Framework;
	
	using Iesi.Collections;

	/// <summary>
	/// Extends <see cref="DataBinder"/> class with some 
	/// ActiveRecord specific functionality.
	/// For example by specifying an "autoload" attribute to your form params
	/// this class will automatically load the database record before binding
	/// any properties values.
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ARDataBinder : DataBinder
	{
		protected internal static readonly object[] EmptyArg = new object[0];
		
		private bool autoLoad, validate, persistchanges;

		public ARDataBinder() : base()
		{
		}

//		public bool Validate
//		{
//			get { return validate; }
//			set { validate = value; }
//		}

		public bool PersistChanges
		{
			get { return persistchanges; }
			set { persistchanges = value; }
		}

		public bool AutoLoad
		{
			get { return autoLoad; }
			set { autoLoad = value; }
		}

		protected override object CreateInstance(Type instanceType, String paramPrefix, IBindingDataSourceNode node)
		{
			if (node == null)
			{
				throw new RailsException("Nothing found for the given prefix. Are you sure the form fields are using the prefix " + paramPrefix + "?");
			}

			if (IsContainerType(instanceType))
			{
				
			}

			object instance = null;

			bool shouldLoad = autoLoad;

			String autoloadOverride = node.GetMetaEntryValue("autoload");

			if (autoloadOverride != null)
			{
				shouldLoad = (autoloadOverride == "yes" || autoloadOverride == "true");
			}

			if (shouldLoad)
			{
				if (instanceType.IsArray)
				{
					throw new RailsException("ARDataBinder AutoLoad does not support arrays");
				}

				ActiveRecordModel model = ActiveRecordModel.GetModel(instanceType);

				PrimaryKeyModel pkModel;

				object id = ObtainPKValue(model, node, paramPrefix, out pkModel);

				instance = SupportingUtils.FindByPK(instanceType, id);
			}
			else
			{
				instance = base.CreateInstance(instanceType, paramPrefix, node);
			}

			return instance;
		}

		protected override bool ShouldRecreateInstance(object value, Type type, string prefix, IBindingDataSourceNode node)
		{
			ActiveRecordModel model = ActiveRecordModel.GetModel(type);

			if (!AutoLoad || model == null)
			{
				return base.ShouldRecreateInstance(value, type, prefix, node);
			}

			PrimaryKeyModel pkModel;

			object id = ObtainPKValue(model, node, prefix, out pkModel);

			object currentId = pkModel.Property.GetValue(value, null);

			return !Object.ReferenceEquals(id, currentId);
		}

		protected override bool ShouldIgnoreType(Type instanceType)
		{
			if (IsContainerType(instanceType))
			{
				return false;
			}

			return base.ShouldIgnoreType(instanceType);
		}

		protected override bool PerformCustomBinding(object instance, string prefix, IBindingDataSourceNode node)
		{
			Type type = instance.GetType();

			if (IsContainerType(type))
			{
				
			}

			return false;
		}

		private static object ObtainPKValue(ActiveRecordModel model, IBindingDataSourceNode node, String prefix, out PrimaryKeyModel pkModel)
		{
			if (model.Ids.Count != 1)
			{
				throw new RailsException("ARDataBinder does not support more than one primary key");
			}

			pkModel = model.Ids[0] as PrimaryKeyModel;

			String pkPropName = pkModel.Property.Name;
			String propValue = node.GetEntryValue(pkPropName);
	
			if (propValue == null)
			{
				throw new RailsException("ARDataBinder autoload failed as element {0} " + 
					"doesn't have a primary key {1} value", prefix, pkPropName);
			}

			return ConvertUtils.Convert(pkModel.Property.PropertyType, propValue);
		}

		protected override void AfterBinding(object instance, String paramPrefix, IBindingDataSourceNode node)
		{
			// Defensive programming
			if (instance == null) return;

			ActiveRecordModel model = ActiveRecordModel.GetModel(instance.GetType());

			if (model == null) return;

			if (validate)
			{
				ValidateInstances(instance);
			}

			if (persistchanges)
			{
				SaveManyMappings(instance, model, node);
				PersistInstances(instance);
			}
		}

		private void PersistInstances(object instances)
		{
			Type instanceType = instances.GetType();
			ActiveRecordBase[] records = null;

			if (instanceType.IsArray)
			{
				records = instances as ActiveRecordBase[];
			}
			else if (typeof(ActiveRecordBase).IsAssignableFrom(instanceType))
			{
				records = new ActiveRecordBase[] {(ActiveRecordBase) instances};
			}

			if (records != null)
			{
				foreach(ActiveRecordBase record in records)
				{
					record.Save();
				}
			}
		}

		private void ValidateInstances(object instances)
		{
			Type instanceType = instances.GetType();
			ActiveRecordValidationBase[] records = null;

			if (instanceType.IsArray)
			{
				records = instances as ActiveRecordValidationBase[];
			}
			else if (typeof(ActiveRecordValidationBase).IsAssignableFrom(instanceType))
			{
				records = new ActiveRecordValidationBase[] {(ActiveRecordValidationBase) instances};
			}

			if (records != null)
			{
				foreach(ActiveRecordValidationBase record in records)
				{
					if (!record.IsValid())
					{
						throw new RailsException("Error validating {0} {1}",
							record.GetType().Name, string.Join("\n", record.ValidationErrorMessages));
					}
				}
			}
		}

		protected void SaveManyMappings(object instance, ActiveRecordModel model, IBindingDataSourceNode node)
		{
			foreach(HasManyModel hasManyModel in model.HasMany)
			{
				if (hasManyModel.HasManyAtt.Inverse) continue;
				if (hasManyModel.HasManyAtt.RelationType != RelationType.Bag &&
					hasManyModel.HasManyAtt.RelationType != RelationType.Set) continue;

				ActiveRecordModel otherModel = ActiveRecordModel.GetModel(hasManyModel.HasManyAtt.MapType);

				PrimaryKeyModel keyModel = ARCommonUtils.ObtainPKProperty(otherModel);

				if (otherModel == null || keyModel == null)
				{
					continue; // Impossible to save
				}

				CreateMappedInstances(instance, hasManyModel.Property, keyModel, otherModel, node);
			}

			foreach(HasAndBelongsToManyModel hasManyModel in model.HasAndBelongsToMany)
			{
				if (hasManyModel.HasManyAtt.Inverse) continue;
				if (hasManyModel.HasManyAtt.RelationType != RelationType.Bag &&
					hasManyModel.HasManyAtt.RelationType != RelationType.Set) continue;

				ActiveRecordModel otherModel = ActiveRecordModel.GetModel(hasManyModel.HasManyAtt.MapType);

				PrimaryKeyModel keyModel = ARCommonUtils.ObtainPKProperty(otherModel);

				if (otherModel == null || keyModel == null)
				{
					continue; // Impossible to save
				}

				CreateMappedInstances(instance, hasManyModel.Property, keyModel, otherModel, node);
			}
		}

		private void CreateMappedInstances(object instance, PropertyInfo prop,
			PrimaryKeyModel keyModel, ActiveRecordModel otherModel, IBindingDataSourceNode node)
		{
			object container = InitializeRelationPropertyIfNull(instance, prop);

			String paramName = String.Format("{0}.{1}", prop.Name, keyModel.Property.Name);

			int[] ids = (int[]) ConvertUtils.Convert(typeof(int[]), paramName, node, null);

			if (ids != null)
			{
				foreach(int id in ids)
				{
					object item = Activator.CreateInstance(otherModel.Type);

					keyModel.Property.SetValue(item, id, EmptyArg);

					AddToContainer(container, item);
				}
			}
		}

		private static object InitializeRelationPropertyIfNull(object instance, PropertyInfo property)
		{
			object container = property.GetValue(instance, EmptyArg);

			if (container == null)
			{
				if (property.PropertyType == typeof(IList))
				{
					container = new ArrayList();
				}
				else if (property.PropertyType == typeof(ISet))
				{
					container = new HashedSet();
				}

				property.SetValue(instance, container, EmptyArg);
			}

			return container;
		}

		private void AddToContainer(object container, object item)
		{
			if (container is IList)
			{
				(container as IList).Add(item);
			}
			else if (container is ISet)
			{
				(container as ISet).Add(item);
			}
		}

		private bool IsContainerType(Type type)
		{
			return type == typeof(IList) || type == typeof(ISet);
		}
	}
}