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

namespace Castle.MicroKernel.ModelBuilder.Inspectors
{
	using System;
	using System.Configuration;

	using Castle.Core;
	
	/// <summary>
	/// Inspects the component configuration and the type looking for a
	/// definition of lifestyle type. The configuration preceeds whatever
	/// is defined in the component.
	/// </summary>
	/// <remarks>
	/// This inspector is not guarantee to always set up an lifestyle type. 
	/// If nothing could be found it wont touch the model. In this case is up to
	/// the kernel to estabish a default lifestyle for components.
	/// </remarks>
	[Serializable]
	public class LifestyleModelInspector : IContributeComponentModelConstruction
	{
		/// <summary>
		/// Seaches for the lifestyle in the configuration and, if unsuccessful
		/// look for the lifestyle attribute in the implementation type.
		/// </summary>
		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (!ReadLifestyleFromConfiguration(model))
			{
				ReadLifestyleFromType(model);
			}
		}

		/// <summary>
		/// Reads the attribute "lifestyle" associated with the 
		/// component configuration and tries to convert to <see cref="LifestyleType"/>  
		/// enum type. 
		/// </summary>
		protected virtual bool ReadLifestyleFromConfiguration(ComponentModel model)
		{
			if (model.Configuration != null)
			{
				String lifestyle = model.Configuration.Attributes["lifestyle"];
				
				if (lifestyle != null)
				{
					try
					{
						LifestyleType type = (LifestyleType) 
							Enum.Parse(typeof(LifestyleType), lifestyle, true);

						model.LifestyleType = type;

					}
					catch(Exception)
					{
						String message = String.Format(
							"Could not convert the specified attribute value " + 
							"{0} to a valid LifestyleType enum type", lifestyle);

						throw new ConfigurationErrorsException(message);
					}

					if (model.LifestyleType == LifestyleType.Pooled)
					{
						ExtractPoolConfig(model);
					}
					else if(model.LifestyleType == LifestyleType.Custom)
					{
						ExtractCustomConfig(model);
					}


					return true;
				}
			}

			return false;
		}

		private static void ExtractPoolConfig(ComponentModel model)
		{
			String initial = model.Configuration.Attributes["initialPoolSize"];
			String maxSize = model.Configuration.Attributes["maxPoolSize"];
	
			if (initial != null)
			{
				model.ExtendedProperties[ExtendedPropertiesConstants.Pool_InitialPoolSize] = Convert.ToInt32(initial);
			}
			if (maxSize != null)
			{
				model.ExtendedProperties[ExtendedPropertiesConstants.Pool_MaxPoolSize] = Convert.ToInt32(maxSize);
			}
		}

		private static void ExtractCustomConfig(ComponentModel model)
		{
			String customLifestyleType = model.Configuration.Attributes["customLifestyleType"];

			if(customLifestyleType != null)
			{
				try
				{
					model.CustomLifestyle = Type.GetType(customLifestyleType, true, false);
				}
				catch(Exception)
				{
					String message = String.Format(
						"The Type {0} specified  in the customLifestyleType attribute could not be loaded.", customLifestyleType);

					throw new ConfigurationErrorsException(message);
				}
			}
			else
			{
				const string message = @"The attribute 'customLifestyleType' must be specified in conjunction with the 'lifestyle' attribute set to ""custom"".";

				throw new ConfigurationErrorsException(message);
			}
		}

		/// <summary>
		/// Check if the type expose one of the lifestyle attributes
		/// defined in Castle.Model namespace.
		/// </summary>
		protected virtual void ReadLifestyleFromType(ComponentModel model)
		{
			object[] attributes = model.Implementation.GetCustomAttributes( 
				typeof(LifestyleAttribute), true );

			if (attributes.Length != 0)
			{
				LifestyleAttribute attribute = (LifestyleAttribute)
					attributes[0];

				model.LifestyleType = attribute.Lifestyle;

				if (model.LifestyleType == LifestyleType.Custom)
				{
					CustomLifestyleAttribute custom = (CustomLifestyleAttribute)
						attribute;
					model.CustomLifestyle = custom.LifestyleHandlerType;
				}
				else if (model.LifestyleType == LifestyleType.Pooled)
				{
					PooledAttribute pooled = (PooledAttribute) attribute;
					model.ExtendedProperties[ExtendedPropertiesConstants.Pool_InitialPoolSize] = pooled.InitialPoolSize;
					model.ExtendedProperties[ExtendedPropertiesConstants.Pool_MaxPoolSize] = pooled.MaxPoolSize;
				}
			}
		}
	}
}
