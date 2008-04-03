﻿// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration
{
	using System;
	using System.ServiceModel.Description;
	using Castle.MicroKernel;

	internal abstract class WcfExplcitBehavior : IWcfServiceBehavior, IWcfEndpointBehavior
	{
		
		#region IWcfServiceBehavior

		public void Install(ServiceDescription description, IKernel kernel)
		{
			object behavior = GetBehaviorInstance(kernel);

			if (behavior is IServiceBehavior)
			{
				description.Behaviors.Add((IServiceBehavior)behavior);
			}
		}

		#endregion

		#region IWcfEndpointBehavior 

		public void Install(ServiceEndpoint endpoint, IKernel kernel)
		{
			object behavior = GetBehaviorInstance(kernel);

			if (behavior is IEndpointBehavior)
			{
				endpoint.Behaviors.Add((IEndpointBehavior)behavior);
			}
			else if (behavior is IOperationBehavior)
			{
				foreach (OperationDescription operation in endpoint.Contract.Operations)
				{
					operation.Behaviors.Add((IOperationBehavior)behavior);
				}
			}
		}

		#endregion

		protected abstract object GetBehaviorInstance(IKernel kernel);

		internal static WcfExplcitBehavior CreateFrom(object behavior)
		{
			if (behavior is Type)
			{
				return new WcfServiceTypeBehavior((Type)behavior);
			}
			else if (behavior is string)
			{
				return new WcfServiceKeyBehavior((string)behavior);
			}
			else
			{
				return new WcfInstanceBehavior(behavior);
			}
		}

		public void Accept(IWcfBehaviorVisitor visitor)
		{
			visitor.VisitServiceBehavior(this);
			visitor.VisitEndpointBehavior(this);
		}
	}

	#region Class: WcfServiceKeyBehavior

	internal class WcfServiceKeyBehavior : WcfExplcitBehavior
	{
		private readonly string key;

		internal WcfServiceKeyBehavior(string key)
		{
			this.key = key;
		}

		protected override object GetBehaviorInstance(IKernel kernel)
		{
			return kernel[key];
		}
	}

	#endregion

	#region Class: WcfServiceTypeBehavior

	internal class WcfServiceTypeBehavior : WcfExplcitBehavior
	{
		private readonly Type service;

		internal WcfServiceTypeBehavior(Type service)
		{
			this.service = service;
		}

		protected override object GetBehaviorInstance(IKernel kernel)
		{
			return kernel.Resolve(service);
		}
	}

	#endregion

	#region Class: WcfInstanceBehavior

	internal class WcfInstanceBehavior : WcfExplcitBehavior
	{
		private readonly object instance;

		internal WcfInstanceBehavior(object instance)
		{
			this.instance = instance;
		}

		protected override object GetBehaviorInstance(IKernel kernel)
		{
			return instance;
		}
	}

	#endregion
}
