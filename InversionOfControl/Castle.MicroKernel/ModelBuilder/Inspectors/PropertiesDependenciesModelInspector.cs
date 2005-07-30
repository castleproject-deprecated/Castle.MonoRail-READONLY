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

namespace Castle.MicroKernel.ModelBuilder.Inspectors
{
	using System;
	using System.Reflection;

	using Castle.Model;
	using Castle.MicroKernel.SubSystems.Conversion;

	/// <summary>
	/// This implementation of <see cref="IContributeComponentModelConstruction"/>
	/// collects all potential writable puplic properties exposed by the component 
	/// implementation and populates the model with them.
	/// The Kernel might be able to set some of these properties when the component 
	/// is requested.
	/// </summary>
	[Serializable]
	public class PropertiesDependenciesModelInspector : IContributeComponentModelConstruction
	{
		[NonSerialized]
		private ITypeConverter converter;

		public PropertiesDependenciesModelInspector()
		{
		}

		/// <summary>
		/// Adds the properties as optional dependencies of this component.
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="model"></param>
		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (converter == null)
			{
				converter = (ITypeConverter) 
					kernel.GetSubSystem( SubSystemConstants.ConversionManagerKey );
			}

			InspectProperties(model);
		}

		protected virtual void InspectProperties(ComponentModel model)
		{
			Type targetType = model.Implementation;
	
			PropertyInfo[] properties = targetType.GetProperties( 
				BindingFlags.Public|BindingFlags.Instance );
	
			foreach(PropertyInfo property in properties)
			{
				if (!property.CanWrite)
				{
					continue;
				}

				DependencyModel dependency = null;

				Type propertyType = property.PropertyType;

				// All these dependencies are simple guesses
				// So we make them optional (the 'true' parameter below)

				if ( converter.CanHandleType(propertyType) )
				{
					dependency = new DependencyModel(DependencyType.Parameter, property.Name, propertyType, true);
				}
				else if (propertyType.IsInterface || propertyType.IsClass)
				{
					dependency = new DependencyModel(DependencyType.Service, property.Name, propertyType, true);
				}
				else
				{
					// What is it?!
					// Awkward type, probably.

					continue;
				}

				model.Properties.Add( new PropertySet(property, dependency) );
			}
		}
	}
}