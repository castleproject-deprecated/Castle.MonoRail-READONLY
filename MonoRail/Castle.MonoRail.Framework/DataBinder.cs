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
		private IRailsEngineContext context;

		private String root = null;
		private String parent = String.Empty;
		
		public DataBinder( IRailsEngineContext context )
		{
			this.context = context;
		}

		public object BindObject( Type instanceType )
		{
			return BindObject( instanceType, String.Empty, context.Params, context.Request.Files, null );
		}

		public object BindObject( Type instanceType, String paramPrefix, IList errorList )
		{
			return BindObject( instanceType, paramPrefix, context.Params, context.Request.Files, errorList );
		}

		public object BindObject( Type instanceType, String paramPrefix, NameValueCollection paramList, IDictionary files, IList errorList )
		{
			if ( root == null ) root = instanceType.Name;
			if ( instanceType.IsAbstract || instanceType.IsInterface ) return null;

			object instance = Activator.CreateInstance( instanceType );

			return BindObjectInstance(instance, paramPrefix, paramList, files, errorList);			
		}

		public object BindObjectInstance( object instance, String paramPrefix)
		{
			return BindObjectInstance(instance,  paramPrefix, context.Params, context.Request.Files, null );
		}

		public object BindObjectInstance( object instance, String paramPrefix, NameValueCollection paramList, IDictionary files, IList errorList )
		{
			paramPrefix = (paramPrefix != null && paramPrefix != String.Empty) ?  paramPrefix.ToLower( CultureInfo.InvariantCulture ) + "." : String.Empty;

			PropertyInfo[] props = instance.GetType().GetProperties( BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );

			foreach ( PropertyInfo prop in props )
			{
				if ( prop.CanWrite )
				{
					Type propType = prop.PropertyType;
					
					String oldParent = parent;
					
					try
					{
						if ( !propType.IsPrimitive && !propType.IsArray && 
							propType != typeof(String) && propType != typeof(Guid) && 
							propType != typeof(DateTime) && propType != typeof(HttpPostedFile) &&
							!typeof(ICollection).IsAssignableFrom( propType ) )
						{
							parent += prop.Name + ".";		
						
							// if the property is an object, we look if it is already instanciated
							object value = prop.GetValue(instance, null);
							
							if (value == null) // if it's not there, we create it
							{
								value = BindObject( prop.PropertyType, paramPrefix + prop.Name, paramList, files, errorList );
								
								prop.SetValue( instance, value, null );
							}
							else // if the object already instanciated, then we use it 
							{
								BindObjectInstance( value, paramPrefix + prop.Name, paramList, files, errorList );
							}
						}
						else
						{
							string[] values = paramList.GetValues( paramPrefix + prop.Name );
							
							object value = Convert( prop.PropertyType, values, prop.Name, files, context );

							if (value != null)
							{
								prop.SetValue( instance, value, null );
							}
						}						
					}
					catch ( Exception e )
					{
						if ( errorList != null )
						{
							errorList.Add( new DataBindError( root, parent + prop.Name, e ) );
						}
						else
						{
							throw e;
						}
					}

					parent = oldParent;
				}
			}

			if ( parent == String.Empty ) root = null;

			return instance;
		}

		public static object Convert( Type desiredType, String value, String paramName, IDictionary files, IRailsEngineContext context )
		{
			return Convert(desiredType, new string[] { value }, paramName, files, context );
		}

		public static object Convert( Type desiredType, String[] values, String paramName, IDictionary files, IRailsEngineContext context )
		{
			String value = ( values != null && values.Length > 0 ) ? values[0] : null;

			if (desiredType == typeof(String))
			{
				return value;
			}
			else if (desiredType == typeof(Guid))
			{
				if (value != null)
				{
					return new Guid( value.ToString() );
				}
				else
				{
					return Guid.Empty; 
				}
			}
			else if (desiredType == typeof(UInt16))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToUInt16( value );
			}
			else if (desiredType == typeof(UInt32))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToUInt32( value );
			}
			else if (desiredType == typeof(UInt64))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToUInt64( value );
			}
			else if (desiredType == typeof(Int16))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToInt16( value );
			}
			else if (desiredType == typeof(Int32))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToInt32( value );
			}
			else if (desiredType == typeof(Int64))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToInt64( value );
			}
			else if (desiredType == typeof(Byte))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToByte( value );
			}
			else if (desiredType == typeof(SByte))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToSByte( value );
			}
			else if (desiredType == typeof(Single))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToSingle( value );
			}
			else if (desiredType == typeof(Double))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToDouble( value );
			}
			else if (desiredType == typeof(DateTime))
			{
				if (value == null)
				{
					String day = context.Params[paramName + "day"];
					String month = context.Params[paramName + "month"];
					String year = context.Params[paramName + "year"];

					if (day != null && day != null && day != null)
					{
						try
						{
							return new DateTime( 
								System.Convert.ToInt32(year), 
								System.Convert.ToInt32(month), 
								System.Convert.ToInt32(day) );
						}
						catch(Exception)
						{
							throw new ArgumentException("Invalid date");
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
				if (value == null || String.Compare( "false", value, true ) == 0)
				{
					return false;
				}

				return value != null;
			}
			else if (desiredType == typeof(HttpPostedFile))
			{
				return files[paramName];
			}
			else if (desiredType.IsArray)
			{
				return values != null ? ConvertToArray( desiredType, values, paramName, files, context ) : null;
			}
			else if ( context != null )
			{
				String message = String.Format("Ignoring argument {0} with value {1} " + 
					" as we don't know how to convert from this value to its type", paramName, value);
				context.Trace(message);
			}

			return null;
		}

		private static object ConvertToArray( Type desiredType, String[] values, String paramName, IDictionary files, IRailsEngineContext context )
		{
			Type elemType	= desiredType.GetElementType();
			Array newArray	= Array.CreateInstance(elemType, values.Length);
	
			for( int i=0; i < values.Length; i++)
			{
				newArray.SetValue( Convert(elemType, new String[] { values[i] }, paramName, files, context), i );
			}
	
			return newArray;
		}
	}
}
