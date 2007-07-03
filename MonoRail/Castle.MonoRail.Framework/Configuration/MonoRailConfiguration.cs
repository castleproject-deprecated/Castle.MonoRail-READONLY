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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Configuration;
	using System.Xml;

	/// <summary>
	/// Pendent
	/// </summary>
	public class MonoRailConfiguration : ISerializedConfig
	{
		private static readonly String SectionName = "monorail";
		private static readonly String AlternativeSectionName = "monoRail";

		private bool checkClientIsConnected, useWindsorIntegration, matchHostNameAndPath, excludeAppPath;
		private Type customFilterFactory;
		private XmlNode configurationSection;

		private SmtpConfig smtpConfig;
		private ViewEngineConfig viewEngineConfig;
		private ControllersConfig controllersConfig;
		private ViewComponentsConfig viewComponentsConfig;
		private ScaffoldConfig scaffoldConfig;

		private RoutingRuleCollection routingRules;
		private ExtensionEntryCollection extensions;
		private ServiceEntryCollection services;
		private DefaultUrlCollection defaultUrls;

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailConfiguration"/> class.
		/// </summary>
		public MonoRailConfiguration()
		{
			smtpConfig = new SmtpConfig();
			viewEngineConfig = new ViewEngineConfig();
			controllersConfig = new ControllersConfig();
			viewComponentsConfig = new ViewComponentsConfig();
			scaffoldConfig = new ScaffoldConfig();
			routingRules = new RoutingRuleCollection();
			extensions = new ExtensionEntryCollection();
			services = new ServiceEntryCollection();
			defaultUrls = new DefaultUrlCollection();

			checkClientIsConnected = false;
			matchHostNameAndPath = false;
			excludeAppPath = false;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="section"></param>
		public MonoRailConfiguration(XmlNode section) : this()
		{
			configurationSection = section;
		}

		public static MonoRailConfiguration GetConfig()
		{
			MonoRailConfiguration config =
				ConfigurationManager.GetSection(SectionName) as MonoRailConfiguration;

			if (config == null)
			{
				config =
					ConfigurationManager.GetSection(AlternativeSectionName) as MonoRailConfiguration;
			}

			if (config == null)
			{
				throw new ApplicationException("You have to provide a small configuration to use " +
				                               "MonoRail. Check the samples or the documentation");
			}

			return config;
		}

		#region ISerializedConfig implementation

		public void Deserialize(XmlNode node)
		{
			viewEngineConfig.Deserialize(node);
			smtpConfig.Deserialize(node);
			controllersConfig.Deserialize(node);
			viewComponentsConfig.Deserialize(node);
			scaffoldConfig.Deserialize(node);

			services.Deserialize(node);
			extensions.Deserialize(node);
			routingRules.Deserialize(node);
			defaultUrls.Deserialize(node);

			ProcessFilterFactoryNode(node.SelectSingleNode("customFilterFactory"));
			ProcessMatchHostNameAndPath(node.SelectSingleNode("routing"));
			ProcessExcludeAppPath(node.SelectSingleNode("routing"));

			XmlAttribute checkClientIsConnectedAtt = node.Attributes["checkClientIsConnected"];

			if (checkClientIsConnectedAtt != null && checkClientIsConnectedAtt.Value != String.Empty)
			{
				checkClientIsConnected = String.Compare(checkClientIsConnectedAtt.Value, "true", true) == 0;
			}

			XmlAttribute useWindsorAtt = node.Attributes["useWindsorIntegration"];

			if (useWindsorAtt != null && useWindsorAtt.Value != String.Empty)
			{
				useWindsorIntegration = String.Compare(useWindsorAtt.Value, "true", true) == 0;

				if (useWindsorIntegration)
				{
					ConfigureWindsorIntegration();
				}
			}
		}

		#endregion

		public SmtpConfig SmtpConfig
		{
			get { return smtpConfig; }
		}

		public ViewEngineConfig ViewEngineConfig
		{
			get { return viewEngineConfig; }
		}

		public ControllersConfig ControllersConfig
		{
			get { return controllersConfig; }
		}

		public ViewComponentsConfig ViewComponentsConfig
		{
			get { return viewComponentsConfig; }
		}

		public RoutingRuleCollection RoutingRules
		{
			get { return routingRules; }
		}

		public ExtensionEntryCollection ExtensionEntries
		{
			get { return extensions; }
		}

		public ServiceEntryCollection ServiceEntries
		{
			get { return services; }
		}

		public Type CustomFilterFactory
		{
			get { return customFilterFactory; }
		}

		public ScaffoldConfig ScaffoldConfig
		{
			get { return scaffoldConfig; }
		}

		public bool CheckClientIsConnected
		{
			get { return checkClientIsConnected; }
		}

		public bool UseWindsorIntegration
		{
			get { return useWindsorIntegration; }
		}

		public bool MatchHostNameAndPath
		{
			get { return matchHostNameAndPath; }
		}

		public bool ExcludeAppPath
		{
			get { return excludeAppPath; }
		}

		public XmlNode ConfigurationSection
		{
			get { return configurationSection; }
		}

		public DefaultUrlCollection DefaultUrls
		{
			get { return defaultUrls; }
		}

		private void ProcessFilterFactoryNode(XmlNode node)
		{
			if (node == null) return;

			XmlAttribute type = node.Attributes["type"];

			if (type == null)
			{
				String message = "The custom filter factory node must specify a 'type' attribute";
				throw new ConfigurationErrorsException(message);
			}

			customFilterFactory = TypeLoadUtil.GetType(type.Value);
		}

		private void ProcessMatchHostNameAndPath(XmlNode node)
		{
			if (node == null) return;

			XmlAttribute matchHostNameAndPathAtt = node.Attributes["matchHostNameAndPath"];

			if (matchHostNameAndPathAtt != null && matchHostNameAndPathAtt.Value != String.Empty)
			{
				matchHostNameAndPath = String.Compare(matchHostNameAndPathAtt.Value, "true", true) == 0;
			}
		}

		private void ProcessExcludeAppPath(XmlNode node)
		{
			if (node == null) return;

			// maybe a check to make sure both matchHostNameAndPathAtt & includeAppPath 
			// are not both set as that wouldn't make sense?
			XmlAttribute excludeAppPathAtt = node.Attributes["excludeAppPath"];

			if (excludeAppPathAtt != null && excludeAppPathAtt.Value != String.Empty)
			{
				excludeAppPath = String.Compare(excludeAppPathAtt.Value, "true", true) == 0;
			}
		}

		private void ConfigureWindsorIntegration()
		{
			const string windsorExtensionAssemblyName = "Castle.MonoRail.WindsorExtension";

			services.RegisterService(ServiceIdentification.ControllerTree, TypeLoadUtil.GetType(
			                                                               	TypeLoadUtil.GetEffectiveTypeName(
			                                                               		"Castle.MonoRail.WindsorExtension.ControllerTreeAccessor, " +
			                                                               		windsorExtensionAssemblyName)));

			controllersConfig.CustomControllerFactory = TypeLoadUtil.GetType(
				TypeLoadUtil.GetEffectiveTypeName("Castle.MonoRail.WindsorExtension.WindsorControllerFactory, " +
				                                  windsorExtensionAssemblyName));

			viewComponentsConfig.CustomViewComponentFactory = TypeLoadUtil.GetType(
				TypeLoadUtil.GetEffectiveTypeName("Castle.MonoRail.WindsorExtension.WindsorViewComponentFactory, " +
				                                  windsorExtensionAssemblyName));

			customFilterFactory = TypeLoadUtil.GetType(
				TypeLoadUtil.GetEffectiveTypeName("Castle.MonoRail.WindsorExtension.WindsorFilterFactory, " +
				                                  windsorExtensionAssemblyName));
		}
	}
}