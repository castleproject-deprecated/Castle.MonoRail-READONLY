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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections;
	using System.Reflection.Emit;

	/// <summary>
	/// Provides appropriate Ldind.X opcode for 
	/// the type of primitive value to be loaded indirectly.
	/// </summary>
	public sealed class LdindOpCodesDictionary : DictionaryBase
	{
		private static readonly LdindOpCodesDictionary _dict = new LdindOpCodesDictionary();

		private static readonly OpCode _emptyOpCode = new OpCode();

		private LdindOpCodesDictionary() : base()
		{
			Dictionary[typeof(bool)] = OpCodes.Ldind_I1;
			Dictionary[typeof(char)] = OpCodes.Ldind_I2;
			Dictionary[typeof(SByte)] = OpCodes.Ldind_I1;
			Dictionary[typeof(Int16)] = OpCodes.Ldind_I2;
			Dictionary[typeof(Int32)] = OpCodes.Ldind_I4;
			Dictionary[typeof(Int64)] = OpCodes.Ldind_I8;
			Dictionary[typeof(float)] = OpCodes.Ldind_R4;
			Dictionary[typeof(double)] = OpCodes.Ldind_R8;
			Dictionary[typeof(byte)] = OpCodes.Ldind_U1;
			Dictionary[typeof(UInt16)] = OpCodes.Ldind_U2;
			Dictionary[typeof(UInt32)] = OpCodes.Ldind_U4;
			Dictionary[typeof(UInt64)] = OpCodes.Ldind_I8;
		}

		public OpCode this[Type type]
		{
			get
			{
				if (Dictionary.Contains(type))
				{
					return (OpCode) Dictionary[type];
				}
				return EmptyOpCode;
			}
		}

		public static LdindOpCodesDictionary Instance
		{
			get { return _dict; }
		}

		public static OpCode EmptyOpCode
		{
			get { return _emptyOpCode; }
		}
	}
}
