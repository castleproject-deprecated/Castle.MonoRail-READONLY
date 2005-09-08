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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Reflection;
	using System.Web;


	/// <summary>
	/// A DataBinder can be used to map properties from 
	/// a NameValueCollection to one or more instance types.
	/// </summary>
	public class DataBinder
	{
		private static readonly int DefaultNestedLevel = 3;

		private String root = null;
		private IRailsEngineContext context;
		
		public DataBinder(IRailsEngineContext context)
		{
			this.context = context;
		}

		public object BindObject(Type instanceType)
		{
			return BindObject(instanceType, String.Empty, context.Params, context.Request.Files, null, DefaultNestedLevel);
		}

		public object BindObject(Type instanceType, String paramPrefix, IList errorList, int nestedLevel)
		{
			return BindObject(instanceType, paramPrefix, context.Params, context.Request.Files, errorList, nestedLevel);
		}

		public object BindObject(Type instanceType, String paramPrefix, NameValueCollection paramList, 
			IDictionary files, IList errorList, int nestedLevel)
		{
			if (instanceType.IsAbstract || instanceType.IsInterface) return null;
			if (root == null) root = instanceType.Name;

			object instance = Activator.CreateInstance(instanceType);

			return BindObjectInstance(instance, paramPrefix, paramList, files, errorList, nestedLevel);
		}

		public object BindObjectInstance(object instance, String paramPrefix)
		{
			paramPrefix = NormalizeParamPrefix(paramPrefix);

			return InternalRecursiveBindObjectInstance(instance,  paramPrefix, context.Params, context.Request.Files, null, 0, DefaultNestedLevel);
		}

		public object BindObjectInstance(object instance, String paramPrefix, NameValueCollection paramList, 
			IDictionary files, IList errorList)
		{
			paramPrefix = NormalizeParamPrefix(paramPrefix);

			return InternalRecursiveBindObjectInstance(instance, paramPrefix, paramList, files, errorList,  0, DefaultNestedLevel);
		}

		public object BindObjectInstance(object instance, String paramPrefix, NameValueCollection paramList, 
			IDictionary files, IList errorList, int nestedLevel)
		{
			paramPrefix = NormalizeParamPrefix(paramPrefix);

			return InternalRecursiveBindObjectInstance(instance, paramPrefix, paramList, files, errorList,  0, nestedLevel);
		}

		private object InternalBindObject(Type instanceType, String paramPrefix, NameValueCollection paramList, 
			IDictionary files, IList errorList, int curNestedLevel, int maxNestedLevel)
		{
			if (instanceType.IsAbstract || instanceType.IsInterface) return null;
			if (root == null) root = instanceType.Name;

			object instance = Activator.CreateInstance(instanceType);

			return InternalRecursiveBindObjectInstance(instance, paramPrefix, 
				paramList, files, errorList, curNestedLevel, maxNestedLevel);
		}

		private object InternalRecursiveBindObjectInstance(object instance, String paramPrefix, 
			NameValueCollection paramList, IDictionary files, IList errorList, int curNestedLevel, int maxNestedLevel)
		{
			if (curNestedLevel > maxNestedLevel)
			{
				return instance;
			}

			BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			PropertyInfo[] props = instance.GetType().GetProperties(flags);

			foreach (PropertyInfo prop in props)
			{
				if (!prop.CanWrite)
				{
					continue;
				}

				Type propType = prop.PropertyType;
				
				bool simpleProperty = (!propType.IsPrimitive && 
					!propType.IsArray && propType != typeof(String) && 
					propType != typeof(Guid) && propType != typeof(DateTime) && 
					propType != typeof(HttpPostedFile) && !typeof(ICollection).IsAssignableFrom(propType));
				
				try
				{
					if (simpleProperty)
					{
						// if the property is an object, we look if it is already instanciated
						object value = prop.GetValue(instance, null);
						
						String propName = BuildParamName(paramPrefix, prop.Name);

						if (value == null) // if it's not there, we create it
						{
							value = InternalBindObject(prop.PropertyType, propName, paramList, files, 
								errorList, curNestedLevel + 1, maxNestedLevel);
							
							prop.SetValue(instance, value, null);
						}
						else // if the object already instanciated, then we use it 
						{
							InternalRecursiveBindObjectInstance(value, propName, paramList, files, 
								errorList, curNestedLevel + 1, maxNestedLevel);
						}
					}
					else
					{
						String paramName = BuildParamName(paramPrefix, prop.Name); 

						bool conversionSucceeded;

						String[] values = paramList.GetValues(paramName);
						
						object value = Convert(prop.PropertyType, values, paramName, files, context, out conversionSucceeded);
						
						// we don't want to set the value if the form param was missing
						// to avoid loosing existing values in the object instance
						if (conversionSucceeded && value != null)
						{
							prop.SetValue(instance, value, null);
						}
					}						
				}
				catch (Exception ex)
				{
					if (errorList != null)
					{
						errorList.Add(new DataBindError(root, BuildParamName(paramPrefix, prop.Name), ex));
					}
					else
					{
						throw;
					}
				}
			}

			return instance;
		}

		public static object Convert(Type desiredType, String value, String paramName, IDictionary files, IRailsEngineContext context)
		{
			return Convert(desiredType, new String[] { value }, paramName, files, context);
		}

		public static object Convert(Type desiredType, String[] values, String paramName, IDictionary files, IRailsEngineContext context)
		{
			bool conversionSucceeded; 
			return Convert(desiredType, values, paramName, files, context, out conversionSucceeded);
		}
		
		private static object Convert(Type desiredType, String[] values, String paramName, IDictionary files, IRailsEngineContext context, out bool conversionSucceeded)
		{
			String value = null;

			if (values != null && values.Length > 0) 
			{
				value = values[0];
				conversionSucceeded = true;
			}
			else
			{
				conversionSucceeded = false;
			}

			if (desiredType == typeof(String))
			{
				return value;
			}
			else if (desiredType == typeof(Guid))
			{
				if (value != null)
				{
					return new Guid(value.ToString());
				}
				else
				{
					return Guid.Empty; 
				}
			}
			else if (desiredType == typeof(UInt16))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToUInt16(value);
			}
			else if (desiredType == typeof(UInt32))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToUInt32(value);
			}
			else if (desiredType == typeof(UInt64))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToUInt64(value);
			}
			else if (desiredType == typeof(Int16))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToInt16(value);
			}
			else if (desiredType == typeof(Int32))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToInt32(value);
			}
			else if (desiredType == typeof(Int64))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToInt64(value);
			}
			else if (desiredType == typeof(Byte))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToByte(value);
			}
			else if (desiredType == typeof(SByte))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToSByte(value);
			}
			else if (desiredType == typeof(Single))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToSingle(value);
			}
			else if (desiredType == typeof(Double))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToDouble(value);
			}
			else if (desiredType == typeof(DateTime))
			{
				if (value == null)
				{
					String day = context.Params[paramName + "day"];
					String month = context.Params[paramName + "month"];
					String year = context.Params[paramName + "year"];

					if (day != null && month != null && year != null)
					{
						try
						{
							// we have found a composite date so we 
							// consider the convertion successful
							conversionSucceeded = true; 

							return new DateTime(
								System.Convert.ToInt32(year), 
								System.Convert.ToInt32(month), 
								System.Convert.ToInt32(day));
						}
						catch(Exception inner)
						{
							String message = String.Format("Invalid date (day {0} month {1} year {2}) for {3} ", 
								day, month, year, paramName);

							throw new ArgumentException(message, inner);
						}
					}
				}

				if (value == null || value == String.Empty)
				{
					return null;
				}
				else
				{
					return DateTime.Parse(value);
				}
			}
			else if (desiredType == typeof(Boolean))
			{
				if (value == null || String.Compare("false", value, true) == 0)
				{
					return false;
				}

				return value != null;
			}
			else if (desiredType == typeof(HttpPostedFile))
			{
				conversionSucceeded = true; // if we get some files we don't care about the values being null
				return files[paramName];
			}
			else if (desiredType.IsEnum)
			{
				if (value == String.Empty) return null;
				
				return Enum.Parse(desiredType, value, true);
			}
			else if (desiredType.IsArray)
			{
				return values != null ? ConvertToArray(desiredType, values, paramName, files, context) : null;
			}
			else if (context != null)
			{
				conversionSucceeded = false;

				String message = String.Format("Ignoring argument {0} with value {1} " + 
					" as we don't know how to convert from this value to its type", paramName, value);

				context.Trace(message);
			}

			return null;
		}

		private static object ConvertToArray(Type desiredType, String[] values, String paramName, IDictionary files, IRailsEngineContext context)
		{
			Type elemType	= desiredType.GetElementType();

			// Fix for mod_mono issue where array values are passed as a comma seperated String
			if(values.Length == 1 && (values[0].IndexOf(',') > -1))
			{
				values = values[0].Split(',');
			}

			Array newArray	= Array.CreateInstance(elemType, values.Length);
	
			for(int i=0; i < values.Length; i++)
			{
				newArray.SetValue(Convert(elemType, new String[] { values[i] }, paramName, files, context), i);
			}
	
			return newArray;
		}

		private static String NormalizeParamPrefix(String paramPrefix)
		{
			return (paramPrefix != null && paramPrefix != String.Empty) ? 
				paramPrefix.ToLower(CultureInfo.InvariantCulture) : String.Empty;
		}

		private static String BuildParamName(String prefix, String name)
		{
			if (prefix != String.Empty)
			{
				return String.Format("{0}.{1}", prefix, name);
			}
			else
			{
				return name;
			}
		}
	}
}
