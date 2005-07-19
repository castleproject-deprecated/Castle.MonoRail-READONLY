namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.Reflection;
	using NVelocity.App.Events;
	using NVelocity.Context;
	using NVelocity.Exception;
	using NVelocity.Util.Introspection;

	/// <summary>
	/// ASTMethod.java
	///
	/// Method support for references :  $foo.method()
	///
	/// NOTE :
	///
	/// introspection is now done at render time.
	///
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	/// <version> $Id: ASTMethod.cs,v 1.5 2003/10/27 13:54:10 corts Exp $ </version>
	public class ASTMethod : SimpleNode
	{
		private String methodName = "";
		private int paramCount = 0;
		private Object[] params_Renamed;

		public ASTMethod(int id) : base(id)
		{
		}

		public ASTMethod(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object jjtAccept(ParserVisitor visitor, Object data)
		{
			return visitor.visit(this, data);
		}

		/// <summary>
		/// simple init - init our subtree and get what we can from
		/// the AST
		/// </summary>
		public override Object init(InternalContextAdapter context, Object data)
		{
			base.init(context, data);

			/*
	    *  this is about all we can do
	    */

			methodName = FirstToken.image;
			paramCount = jjtGetNumChildren() - 1;
			params_Renamed = new Object[paramCount];

			return data;
		}

		/// <summary>
		/// does the instrospection of the class for the method needed.
		/// Note, as this calls value() on the args if any, this must
		/// only be called at execute() / render() time.
		///
		/// NOTE: this will try to flip the case of the first character for
		/// convience (compatability with Java version).  If there are no arguments,
		/// it will also try to find a property with the same name (also flipping first character).
		/// </summary>
		private Object doIntrospection(InternalContextAdapter context, Type data)
		{
			/*
	    *  Now the parameters have to be processed, there
	    *  may be references contained within that need
	    *  to be introspected.
	    */
			for (int j = 0; j < paramCount; j++)
			{
				params_Renamed[j] = jjtGetChild(j + 1).Value(context);
			}

			String methodNameUsed = methodName;
			MethodInfo m = rsvc.Introspector.getMethod(data, methodNameUsed, params_Renamed);
			PropertyInfo p = null;
			if (m == null)
			{
				methodNameUsed = methodName.Substring(0, 1).ToUpper() + methodName.Substring(1);
				m = rsvc.Introspector.getMethod(data, methodNameUsed, params_Renamed);
				if (m == null)
				{
					methodNameUsed = methodName.Substring(0, 1).ToLower() + methodName.Substring(1);
					m = rsvc.Introspector.getMethod(data, methodNameUsed, params_Renamed);

					// if there are no arguments, look for a property
					if (m == null && paramCount == 0)
					{
						methodNameUsed = methodName;
						p = rsvc.Introspector.getProperty(data, methodNameUsed);
						if (p == null)
						{
							methodNameUsed = methodName.Substring(0, 1).ToUpper() + methodName.Substring(1);
							p = rsvc.Introspector.getProperty(data, methodNameUsed);
							if (p == null)
							{
								methodNameUsed = methodName.Substring(0, 1).ToLower() + methodName.Substring(1);
								p = rsvc.Introspector.getProperty(data, methodNameUsed);
							}
						}
					}
				}
			}

			// if a method was found, return it.  Otherwise, return whatever was found with a property, may be null
			if (m != null)
			{
				return m;
			}
			else
			{
				return p;
			}
		}

		/// <summary>
		/// invokes the method.  Returns null if a problem, the
		/// actual return if the method returns something, or
		/// an empty string "" if the method returns void
		/// </summary>
		public override Object execute(Object o, InternalContextAdapter context)
		{
			/*
	    *  new strategy (strategery!) for introspection. Since we want 
	    *  to be thread- as well as context-safe, we *must* do it now,
	    *  at execution time.  There can be no in-node caching,
	    *  but if we are careful, we can do it in the context.
	    */

			MethodInfo method = null;
			PropertyInfo property = null;
			bool preparedAlready = false;

			try
			{
				/*
				*   check the cache 
				*/

				IntrospectionCacheData icd = context.ICacheGet(this);
				Type c = o.GetType();

				/*
				*  like ASTIdentifier, if we have cache information, and the
				*  Class of Object o is the same as that in the cache, we are
				*  safe.
				*/

				if (icd != null && icd.contextData == c)
				{
					// We blindly assume that if there's an array, 
					// we're dealing with a "params array[] args" and 
					// if so, we create an array with an appropriate size

					int arrayArg = int.MaxValue;

					for (int j = 0; j < paramCount; j++)
					{
						if (params_Renamed[j] != null && params_Renamed[j].GetType().IsArray)
						{
							Array array = Array.CreateInstance( 
								params_Renamed[j].GetType().GetElementType(), paramCount - j );
							
							params_Renamed[j] = array;
							
							arrayArg = j; break;
						}
					}

					/*
					* sadly, we do need recalc the values of the args, as this can 
					* change from visit to visit
					*/

					for (int j = 0; j < paramCount; j++)
					{
						if (j >= arrayArg)
						{
							Array array = (Array) params_Renamed[arrayArg];
							
							array.SetValue( jjtGetChild(j + 1).Value(context), j - arrayArg );
						}
						else
						{
							params_Renamed[j] = jjtGetChild(j + 1).Value(context);
						}
					}

					preparedAlready = true;

					/*
					* and get the method from the cache
					*/
					if (icd.thingy is MethodInfo)
					{
						method = (MethodInfo) icd.thingy;
					}
					if (icd.thingy is PropertyInfo)
					{
						property = (PropertyInfo) icd.thingy;
					}

				}
				else
				{
					/*
					*  otherwise, do the introspection, and then
					*  cache it
					*/

					Object obj = doIntrospection(context, c);

					if (obj is MethodInfo)
					{
						method = (MethodInfo) obj;
					}
					if (obj is PropertyInfo)
					{
						property = (PropertyInfo) obj;
					}

					if (obj != null)
					{
						icd = new IntrospectionCacheData();
						icd.contextData = c;
						icd.thingy = obj;
						context.ICachePut(this, icd);
					}
				}

				/*
		*  if we still haven't gotten the method, either we are calling 
		*  a method that doesn't exist (which is fine...)  or I screwed
		*  it up.
		*/

				if (method == null && property == null)
				{
					return null;
				}
			}
			catch (MethodInvocationException mie)
			{
				/*
		*  this can come from the doIntrospection(), as the arg values
		*  are evaluated to find the right method signature.  We just
		*  want to propogate it here, not do anything fancy
		*/

				throw mie;
			}
			catch (Exception e)
			{
				/*
		*  can come from the doIntropection() also, from Introspector
		*/

				rsvc.error("ASTMethod.execute() : exception from introspection : " + e);
				return null;
			}

			try
			{
				/*
		*  get the returned object.  It may be null, and that is
		*  valid for something declared with a void return type.
		*  Since the caller is expecting something to be returned,
		*  as long as things are peachy, we can return an empty 
		*  String so ASTReference() correctly figures out that
		*  all is well.
		*/

				Object obj = null;

				if (method != null)
				{
					if (!preparedAlready)
					{
						ParameterInfo[] methodArgs = method.GetParameters();

						int indexOfParamArray = -1;

						for (int i = 0; i < methodArgs.Length; ++i)
						{
							ParameterInfo paramInfo = methodArgs[i];

							if (paramInfo.IsDefined( typeof(ParamArrayAttribute), false ))
							{
								indexOfParamArray = i; break;
							}
						}

						if (indexOfParamArray != -1)
						{
							Type arrayParamType = methodArgs[indexOfParamArray].ParameterType;

							object[] newParams = new object[ methodArgs.Length ];

							Array.Copy( params_Renamed, newParams, methodArgs.Length - 1 );

							if (params_Renamed.Length < (indexOfParamArray + 1))
							{
								newParams[indexOfParamArray] = Array.CreateInstance( 
									arrayParamType.GetElementType(), 0 );
							}
							else
							{
								Array args = Array.CreateInstance( arrayParamType.GetElementType(), (params_Renamed.Length + 1) - newParams.Length );

								Array.Copy( params_Renamed, methodArgs.Length - 1, args, 0, args.Length );

								newParams[indexOfParamArray] = args;
							}

							params_Renamed = newParams;
						}
					}

					obj = method.Invoke(o, params_Renamed);

					if (obj == null && method.ReturnType == System.Type.GetType("System.Void"))
					{
						obj = String.Empty;
					}
				}
				else
				{
					obj = property.GetValue(o, null);
				}

				return obj;
			}
			catch (TargetInvocationException ite)
			{
				/*
		*  In the event that the invocation of the method
		*  itself throws an exception, we want to catch that
		*  wrap it, and throw.  We don't log here as we want to figure
		*  out which reference threw the exception, so do that 
		*  above
		*/

				EventCartridge ec = context.EventCartridge;

				/*
		*  if we have an event cartridge, see if it wants to veto
		*  also, let non-Exception Throwables go...
		*/

				if (ec != null && ite.GetBaseException() is Exception)
				{
					try
					{
						return ec.methodException(o.GetType(), methodName, (Exception) ite.GetBaseException());
					}
					catch (Exception e)
					{
						throw new MethodInvocationException("Invocation of method '" + methodName + "' in  " + o.GetType() + " threw exception " + e.GetType() + " : " + e.Message, e, methodName);
					}
				}
				else
				{
					/*
		    * no event cartridge to override. Just throw
		    */

					throw new MethodInvocationException("Invocation of method '" + methodName + "' in  " + o.GetType() + " threw exception " + ite.GetBaseException().GetType() + " : " + ite.GetBaseException().Message, ite.GetBaseException(), methodName);
				}
			}
			catch (Exception e)
			{
				rsvc.error("ASTMethod.execute() : exception invoking method '" + methodName + "' in " + o.GetType() + " : " + e);
				return null;
			}
		}


	}
}