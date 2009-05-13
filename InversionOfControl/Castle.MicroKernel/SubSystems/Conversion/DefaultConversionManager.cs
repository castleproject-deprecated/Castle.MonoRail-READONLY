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

namespace Castle.MicroKernel.SubSystems.Conversion
{
	using System;
	using System.Collections;
	using System.Threading;
	using Castle.Core;
	using Castle.Core.Configuration;

	/// <summary>
	/// Composition of all available conversion managers
	/// </summary>
	[Serializable]
	public class DefaultConversionManager : AbstractSubSystem, IConversionManager, ITypeConverterContext
	{
		private static LocalDataStoreSlot slot = Thread.AllocateDataSlot();
		private IList converters;
		private IList standAloneConverters;

		public DefaultConversionManager()
		{
			converters = new ArrayList();
			standAloneConverters = new ArrayList();

			InitDefaultConverters();
		}

		protected virtual void InitDefaultConverters()
		{
			Add(new PrimitiveConverter());
			Add(new TimeSpanConverter());
			Add(new TypeNameConverter());
			Add(new EnumConverter());
			Add(new ListConverter());
			Add(new DictionaryConverter());
			Add(new GenericDictionaryConverter());
			Add(new GenericListConverter());
			Add(new ArrayConverter());
			Add(new ComponentConverter());
			Add(new AttributeAwareConverter());
			Add(new ComponentModelConverter());
		}

		#region IConversionManager Members

		public void Add(ITypeConverter converter)
		{
			converter.Context = this;

			converters.Add(converter);

			if (!(converter is IKernelDependentConverter))
			{
				standAloneConverters.Add(converter);
			}
		}

		public bool IsSupportedAndPrimitiveType(Type type)
		{
			foreach(ITypeConverter converter in standAloneConverters)
			{
				if (converter.CanHandleType(type)) return true;
			}

			return false;
		}

		#endregion

		#region ITypeConverter Members

		public ITypeConverterContext Context
		{
			get { return this; }
			set { throw new NotImplementedException(); }
		}

		public bool CanHandleType(Type type)
		{
			foreach(ITypeConverter converter in converters)
			{
				if (converter.CanHandleType(type)) return true;
			}

			return false;
		}

		public bool CanHandleType(Type type, IConfiguration configuration)
		{
			foreach(ITypeConverter converter in converters)
			{
				if (converter.CanHandleType(type, configuration)) return true;
			}

			return false;
		}

		public object PerformConversion(String value, Type targetType)
		{
			foreach(ITypeConverter converter in converters)
			{
				if (converter.CanHandleType(targetType))
				{
					return converter.PerformConversion(value, targetType);
				}
			}

			String message = String.Format("No converter registered to handle the type {0}",
			                               targetType.FullName);

			throw new ConverterException(message);
		}

		public object PerformConversion(IConfiguration configuration, Type targetType)
		{
			foreach(ITypeConverter converter in converters)
			{
				if (converter.CanHandleType(targetType, configuration))
				{
					return converter.PerformConversion(configuration, targetType);
				}
			}

			String message = String.Format("No converter registered to handle the type {0}",
			                               targetType.FullName);

			throw new ConverterException(message);
		}

		#endregion

		#region ITypeConverterContext Members

		IKernel ITypeConverterContext.Kernel
		{
			get { return base.Kernel; }
		}

		public void PushModel(ComponentModel model)
		{
			CurrentStack.Push(model);
		}

		public void PopModel()
		{
			CurrentStack.Pop();
		}

		public ComponentModel CurrentModel
		{
			get
			{
				if (CurrentStack.Count == 0) return null;
				else return (ComponentModel) CurrentStack.Peek();
			}
		}

		public ITypeConverter Composition
		{
			get { return this; }
		}

		#endregion

		private Stack CurrentStack
		{
			get
			{
				Stack stack = (Stack) Thread.GetData(slot);

				if (stack == null)
				{
					stack = new Stack();
					Thread.SetData(slot, stack);
				}

				return stack;
			}
		}
	}
}