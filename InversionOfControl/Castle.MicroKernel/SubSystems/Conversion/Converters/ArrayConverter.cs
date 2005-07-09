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

namespace Castle.MicroKernel.SubSystems.Conversion
{
	using System;

	using Castle.Model.Configuration;


	[Serializable]
	public class ArrayConverter : AbstractTypeConverter
	{
		public ArrayConverter()
		{
		}

		public override bool CanHandleType(Type type)
		{
			if (! type.IsArray) 
			{
				return false;
			}

			return Context.Composition.CanHandleType(type.GetElementType());
		}

		public override object PerformConversion(String value, Type targetType)
		{
			throw new NotImplementedException();
		}

		public override object PerformConversion(IConfiguration configuration, Type targetType)
		{
			System.Diagnostics.Debug.Assert( targetType.IsArray );

			int count = configuration.Children.Count;
			Type itemType = targetType.GetElementType();

			Array array = Array.CreateInstance( itemType, count );

			int index = 0;
			foreach(IConfiguration itemConfig in configuration.Children)
			{
				object value = Context.Composition.PerformConversion(itemConfig.Value, itemType);
				array.SetValue( value, index++ ); 
			}

			return array;
		}
	}
}
