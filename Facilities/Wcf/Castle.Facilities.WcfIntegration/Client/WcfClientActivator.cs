// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Reflection;
	using System.ServiceModel;
	using System.Threading;
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.Proxy;

	public class WcfClientActivator : DefaultComponentActivator
	{
		private ChannelCreator createChannel;

		#region ClientChannelBuilder Delegate Fields

		private delegate ChannelCreator CreateChannelDelegate(
			IKernel kernel, IWcfClientModel clientModel, ComponentModel model);

		private static readonly MethodInfo createChannelMethod =
			typeof(WcfClientActivator).GetMethod("CreateChannelCreatorInternal",
				BindingFlags.NonPublic | BindingFlags.Static, null,
					new Type[] { typeof(IKernel), typeof(IWcfClientModel),
						typeof(ComponentModel) }, null
				);

		private static readonly Dictionary<Type, CreateChannelDelegate>
			createChannelCache = new Dictionary<Type, CreateChannelDelegate>();

		private static ReaderWriterLock locker = new ReaderWriterLock();

		#endregion

		public WcfClientActivator(ComponentModel model, IKernel kernel,
			ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
			: base(model, kernel, onCreation, onDestruction)
		{
		}

		protected override object InternalCreate(CreationContext context)
		{
			object instance = Instantiate(context);

			ApplyCommissionConcerns(instance);

			return instance;
		}

		protected override object Instantiate(CreationContext context)
		{
			object instance = CreateChannel();

			try
			{
				ProxyOptions options = ProxyUtil.ObtainProxyOptions(Model, true);
				options.AddAdditionalInterfaces(typeof(IContextChannel));			
				instance = Kernel.ProxyFactory.Create(Kernel, instance, Model, context);
			}
			catch (Exception ex)
			{
				throw new ComponentActivatorException("WcfClientActivator: could not proxy service " +
					Model.Service.FullName, ex);
			}

			return instance;
		}

		private IWcfClientModel ObtainClientModel(ComponentModel model)
		{
			IWcfClientModel clientModel = (IWcfClientModel)
				model.ExtendedProperties[WcfConstants.ClientModelKey];

			ValidateClientModel(clientModel, model);

			return clientModel;
		}

		private void ValidateClientModel(IWcfClientModel clientModel, ComponentModel model)
		{
			Type contract;

			if (model != null)
			{
				contract = model.Service;
			}
			else if (clientModel.Contract != null)
			{
				contract = clientModel.Contract;
			}
			else
			{
				throw new FacilityException(
					"The client endpoint does not specify a contract.");
			}

			if ((model != null) && (clientModel.Contract != null) &&
				(clientModel.Contract != model.Service))
			{
				throw new FacilityException("The client endpoint contract " +
					clientModel.Contract.FullName + " conflicts with the expected contaxt" +
					model.Service.FullName + ".");
			}

			if (clientModel.Endpoint == null)
			{
				throw new FacilityException("The client model requires an endpoint.");
			}
		}

		private object CreateChannel()
		{
			if (createChannel == null)
			{
				createChannel = CreateChannelCreator(Kernel, Model);
			}

			return createChannel();
		}

		private ChannelCreator CreateChannelCreator(IKernel kernel, ComponentModel model)
		{
			CreateChannelDelegate createChannelDelegate;

			IWcfClientModel clientModel = ObtainClientModel(model);

			try
			{
				locker.AcquireReaderLock(Timeout.Infinite);

				Type clientModelType = clientModel.GetType();

				if (!createChannelCache.TryGetValue(clientModelType, out createChannelDelegate))
				{
					locker.UpgradeToWriterLock(Timeout.Infinite);

					if (!createChannelCache.TryGetValue(clientModelType, out createChannelDelegate))
					{
						createChannelDelegate = (CreateChannelDelegate)
							Delegate.CreateDelegate(typeof(CreateChannelDelegate),
								createChannelMethod.MakeGenericMethod(clientModelType));
						createChannelCache.Add(clientModelType, createChannelDelegate);
					}
				}
			}
			finally
			{
				locker.ReleaseLock();
			}

			return createChannelDelegate(kernel, clientModel, model);
		}

		private static ChannelCreator CreateChannelCreatorInternal<M>(
			IKernel kernel, IWcfClientModel clientModel, ComponentModel model)
			where M : IWcfClientModel
		{
			IWcfBurden burden;
			IClientChannelBuilder<M> channelBuilder = kernel.Resolve<IClientChannelBuilder<M>>();
			ChannelCreator creator = channelBuilder.GetChannelCreator((M) clientModel, model.Service, out burden);
			model.ExtendedProperties[WcfConstants.ChannelCreatorKey] = creator;
			model.ExtendedProperties[WcfConstants.ClientBurdenKey] = burden;
			return creator;
		}
	}
}
