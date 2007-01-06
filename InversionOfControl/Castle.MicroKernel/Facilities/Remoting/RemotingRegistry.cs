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

namespace Castle.Facilities.Remoting
{
	using System;
	using System.Collections;
	using Castle.Core;
	using Castle.MicroKernel;


	public class RemotingRegistry : MarshalByRefObject, IDisposable
	{
		private readonly IKernel kernel;
		private readonly IDictionary entries = Hashtable.Synchronized(new Hashtable());

#if DOTNET2
		private readonly IDictionary genericEntries = Hashtable.Synchronized(new Hashtable());
#endif

		public RemotingRegistry(IKernel kernel)
		{
			this.kernel = kernel;
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public void AddComponentEntry(ComponentModel model)
		{
#if DOTNET2
			if (model.Service.IsGenericType)
			{
				genericEntries[model.Service] = model;
				return;
			}
#endif
			entries[model.Name] = model;
		}

		public object CreateRemoteInstance(String key)
		{
			GetModel(key);

			return kernel[key];
		}

		private ComponentModel GetModel(string key)
		{
			ComponentModel model = (ComponentModel) entries[key];

			if (model == null)
			{
				throw new KernelException(
					String.Format("No remote/available component found for key {0}", key));
			}

			return model;
		}

		public void Publish(string key)
		{
			ComponentModel model = GetModel(key);

			MarshalByRefObject mbr = (MarshalByRefObject) kernel[key];

			RemoteMarshallerActivator.Marshal(mbr, model);
		}

#if DOTNET2
		/// <summary>
		/// Used in case of generics:
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		private ComponentModel GetModel(Type serviceType)
		{
			ComponentModel model = (ComponentModel) genericEntries[serviceType];

			if (model == null)
			{
				throw new KernelException(
					String.Format("No remote/available component found for service type {0}", serviceType));
			}

			return model;
		}

		public object CreateRemoteInstance(Type serviceType)
		{
			return kernel[serviceType];
		}


		public void Publish(Type serviceType)
		{
			ComponentModel model = GetModel(serviceType);

			MarshalByRefObject mbr = (MarshalByRefObject) kernel[serviceType];

			RemoteMarshallerActivator.Marshal(mbr, model);
		}
#endif

		#region IDisposable Members

		public void Dispose()
		{
			entries.Clear();
#if DOTNET2
			genericEntries.Clear();
#endif
		}

		#endregion
	}
}
