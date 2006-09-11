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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Configuration;
	using System.Xml;

	public enum ServiceIdentification
	{
		Custom,
		ControllerFactory,
		ViewEngine,
		ViewComponentFactory,
		ViewSourceLoader,
		FilterFactory,
		EmailSender,
		ControllerDescriptorProvider,
		ResourceDescriptorProvider,
		RescueDescriptorProvider,
		LayoutDescriptorProvider,
		HelperDescriptorProvider,
		FilterDescriptorProvider,
		ResourceFactory,
		EmailTemplateService,
		ControllerTree,
		CacheProvider,
		ScaffoldingSupport
	}

	public class ServiceEntry : ISerializedConfig
	{
		private ServiceIdentification serviceType;
		private Type service;
		
		#region ISerializedConfig implementation

		public void Deserialize(XmlNode section)
		{
			XmlAttribute idAtt = section.Attributes["id"];
			XmlAttribute typeAtt = section.Attributes["type"];
			
			if (idAtt == null || idAtt.Value == String.Empty)
			{
				throw new ConfigurationException("To add a service, please specify the 'id' attribute. " + 
					"Check the documentation for more information");
			}

			if (typeAtt == null || typeAtt.Value == String.Empty)
			{
				throw new ConfigurationException("To add a service, please specify the 'type' attribute. " + 
					"Check the documentation for more information");
			}
			
			try
			{
				serviceType = (ServiceIdentification) 
					Enum.Parse(typeof(ServiceIdentification), idAtt.Value, true);
			}
			catch(Exception ex)
			{
				throw new ConfigurationException("Invalid service id: " + idAtt.Value, ex);
			}
			
			service = TypeLoadUtil.GetType(typeAtt.Value);
		}
		
		#endregion

		public ServiceIdentification ServiceType
		{
			get { return serviceType; }
		}

		public Type Service
		{
			get { return service; }
		}
	}
}
