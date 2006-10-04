using System.Configuration;
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

namespace Castle.Facilities.Db4oIntegration
{
	using System;
	using System.IO;
	using System.Globalization;

	using com.db4o;
	using com.db4o.config;
	
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;


	public class ObjectContainerComponentActivator : DefaultComponentActivator
	{
		public ObjectContainerComponentActivator(ComponentModel model, IKernel kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction) : 
			base(model, kernel, onCreation, onDestruction)
		{
		}

		protected override object Instantiate( Castle.MicroKernel.CreationContext context )
		{
			SetupDb4o();

			if (Model.ExtendedProperties[Db4oFacility.HostNameKey] != null)
			{
				return OpenClient();
			}
			else
			{
				return OpenLocal();
			}
		}

		[CLSCompliant(false)]
		protected virtual ObjectContainer OpenLocal()
		{
			string databaseFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (string) Model.ExtendedProperties[Db4oFacility.DatabaseFileKey]);

			ObjectContainer container =  Db4o.OpenFile(databaseFile);

			//TODO: Remove it when db4o's team fix it.
			if (container == null)
			{
				String message = "The ObjectContainer is null. Check the permissions of your YAP file.";
#if DOTNET2
				throw new ConfigurationErrorsException(message);
#else
				throw new ConfigurationException(message);
#endif
			}
	
			return container;
		}

		[CLSCompliant(false)]
		protected virtual ObjectContainer OpenClient()
		{
			string hostName = (string) Model.ExtendedProperties[Db4oFacility.HostNameKey];
			int remotePort = (int) Model.ExtendedProperties[Db4oFacility.RemotePortKey];
			string user = (string) Model.ExtendedProperties[Db4oFacility.UserKey];
			string password = (string) Model.ExtendedProperties[Db4oFacility.PasswordKey];
	
			return Db4o.OpenClient(hostName, remotePort, user, password);
		}

		protected virtual void SetupDb4o()
		{
			Db4o.Configure().ExceptionsOnNotStorable((bool) Model.ExtendedProperties[Db4oFacility.ExceptionsOnNotStorableKey]); 

			if(Model.ExtendedProperties[Db4oFacility.CallConstructorsKey] != null)
			{
				Db4o.Configure().CallConstructors((bool) Model.ExtendedProperties[Db4oFacility.CallConstructorsKey]);
			}

			if (Model.ExtendedProperties.Contains(Db4oFacility.ActivationDepth))
			{
				Db4o.Configure().ActivationDepth((int) Model.ExtendedProperties[Db4oFacility.ActivationDepth]);
			}
	
			if (Model.ExtendedProperties.Contains(Db4oFacility.UpdateDepth))
			{
				Db4o.Configure().UpdateDepth((int) Model.ExtendedProperties[Db4oFacility.UpdateDepth]);
			}

			SetupTranslators();
		}

		protected virtual void SetupTranslators()
		{
			//TODO: Remove it when db4o's team fix it.
			Db4o.Configure().ObjectClass(typeof(CompareInfo)).Translate(new TSerializable());
		}
	}
}
