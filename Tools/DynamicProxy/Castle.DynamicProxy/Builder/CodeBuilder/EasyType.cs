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

namespace Castle.DynamicProxy.Builder.CodeBuilder
{
	using System;
	using System.Reflection;

	using Castle.DynamicProxy.Builder.CodeGenerators;
	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
using System.Collections;

	/// <summary>
	/// Summary description for EasyType.
	/// </summary>
	[CLSCompliant(false)]
	public class EasyType : AbstractEasyType
	{
        static IDictionary assemblySigning = new Hashtable();

		protected EasyType() : base()
		{
		}

		public EasyType( ModuleScope modulescope, String name, Type baseType, Type[] interfaces ) : 
			this(modulescope, name, baseType, interfaces, false)
		{
		}

		public EasyType( ModuleScope modulescope, String name, Type baseType, Type[] interfaces, bool serializable ) : this()
		{
			TypeAttributes flags = 
				TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;

			if (serializable)
			{
				flags |= TypeAttributes.Serializable;
			}
            bool isAssemblySigned = IsAssemblySigned(baseType);
            _typebuilder = modulescope.ObtainDynamicModule(isAssemblySigned).DefineType( 
				name, flags, baseType, interfaces );
		}

        private bool IsAssemblySigned(Type baseType)
        {
            lock(assemblySigning)
            {
                if(assemblySigning.Contains(baseType.Assembly)==false)
                {
                    bool isSigned = baseType.Assembly.GetName().GetPublicKey().Length != 0;
                    assemblySigning.Add(baseType.Assembly, isSigned );
                }
                return (bool)assemblySigning[baseType.Assembly];
            }
        }

		public EasyType( ModuleScope modulescope, String name ) : this(modulescope, name, typeof(object), new Type[0])
		{
		}

		public EasyCallable CreateCallable( ReturnReferenceExpression returnType, 
			params ArgumentReference[] args )
		{
			EasyCallable nested = new EasyCallable( this, IncrementAndGetCounterValue, 
				returnType, args );
			_nested.Add( nested );
			return nested;
		}

		public EasyCallable CreateCallable( Type returnType, params ParameterInfo[] args )
		{
			EasyCallable nested = new EasyCallable( this, IncrementAndGetCounterValue, 
				new ReturnReferenceExpression(returnType), ArgumentsUtil.ConvertToArgumentReference(args) );
			_nested.Add( nested );
			return nested;
		}
	}
}
