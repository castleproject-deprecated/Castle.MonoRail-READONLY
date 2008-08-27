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
	using System.ServiceModel;
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;

	public abstract class AbstractServiceHostBuilder<M> : AbstractServiceHostBuilder, IServiceHostBuilder<M>
			where M : IWcfServiceModel
	{
		protected AbstractServiceHostBuilder(IKernel kernel)
			: base(kernel)
		{
		}

		#region IServiceHostBuilder Members

		public ServiceHost Build(ComponentModel model, M serviceModel, params Uri[] baseAddresses)
		{
			ValidateServiceModelInternal(model, serviceModel);
			ServiceHost serviceHost = CreateServiceHost(model, serviceModel, baseAddresses);
			serviceHost.Opening += delegate
			                       {
								   ConfigureServiceHost(serviceHost, serviceModel); 
								   OnOpening(serviceHost, serviceModel, model);
			                       };
			return serviceHost;
		}

		public ServiceHost Build(ComponentModel model, params Uri[] baseAddresses)
		{
			ServiceHost serviceHost = CreateServiceHost(model, baseAddresses);
			serviceHost.Opening += delegate { OnOpening(serviceHost, null, model); };
			return serviceHost;
		}

		public ServiceHost Build(Type serviceType, params Uri[] baseAddresses)
		{
			ServiceHost serviceHost = CreateServiceHost(serviceType, baseAddresses);
			serviceHost.Opening += delegate { OnOpening(serviceHost, null, null); };
			return serviceHost;
		}

		#endregion

		protected virtual void ConfigureServiceHost(ServiceHost serviceHost, M serviceModel)
		{
			foreach (IWcfEndpoint endpoint in serviceModel.Endpoints)
			{
				AddServiceEndpoint(serviceHost, endpoint);
			}
		}

		private void ValidateServiceModelInternal(ComponentModel model, M serviceModel)
		{
			ValidateServiceModel(model, serviceModel);

			foreach (IWcfEndpoint endpoint in serviceModel.Endpoints)
			{
				Type contract = endpoint.Contract;

				if (contract != null)
				{
					if (!contract.IsInterface)
					{
						throw new FacilityException("The service endpoint contract " +
							contract.FullName + " does not represent an interface.");
					}
				}
				else if (model == null || !model.Service.IsInterface)
				{
					throw new FacilityException(
						"No service endpoint contract can be implied from the component.");
				}
				else
				{
					endpoint.Contract = model.Service;
				}
			}
		}

		protected virtual void ValidateServiceModel(ComponentModel model, M serviceModel)
		{
		}

		protected abstract ServiceHost CreateServiceHost(ComponentModel model, M serviceModel,
														 params Uri[] baseAddresses);
		protected abstract ServiceHost CreateServiceHost(ComponentModel model, Uri[] baseAddresses);
		protected abstract ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses);
	}
}
