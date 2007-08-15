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

namespace Castle.Windsor.Configuration.Interpreters
{
	using System;
	using System.Collections;
	using System.Configuration;

	using Castle.Core.Resource;
	using Castle.Core.Configuration;
	using Castle.MicroKernel;

	/// <summary>
	/// Provides common methods for those who wants 
	/// to implement <see cref="IConfigurationInterpreter"/>
	/// </summary>
	public abstract class AbstractInterpreter : IConfigurationInterpreter
	{
		#region Fields

		protected static readonly string ContainersNodeName = "containers";
		protected static readonly string ContainerNodeName = "container";
		protected static readonly string FacilitiesNodeName = "facilities";
		protected static readonly string FacilityNodeName = "facility";
		protected static readonly string ComponentsNodeName = "components";
		protected static readonly string BootstrapNodeName = "bootstrap";
		protected static readonly string ComponentNodeName = "component";
		protected static readonly string IncludeNodeName = "include";
		protected static readonly string PropertiesNodeName = "properties";		

		// private ImportDirectiveCollection imports = new ImportDirectiveCollection();
		private IResource source;
		private Stack resourceStack = new Stack();
		private string environmentName;

		#endregion

		#region Constructors

		public AbstractInterpreter(IResource source)
		{
			if (source == null) throw new ArgumentNullException("source", "IResource is null");

			this.source = source;

			PushResource(source);
		} 

		public AbstractInterpreter(string filename) : this(new FileResource(filename))
		{
		} 

		public AbstractInterpreter() : this(new ConfigResource())
		{
		}

		#endregion

		/// <summary>
		/// Should obtain the contents from the resource,
		/// interpret it and populate the <see cref="IConfigurationStore"/>
		/// accordingly.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="store"></param>
		public abstract void ProcessResource(IResource resource, IConfigurationStore store);

		#region Support for Resource stack

		protected void PushResource(IResource resource)
		{
			resourceStack.Push(resource);
		}

		protected void PopResource()
		{
			resourceStack.Pop();
		}

		protected IResource CurrentResource
		{
			get
			{
				if (resourceStack.Count == 0) return null;

				return resourceStack.Peek() as IResource;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Exposes the reference to <see cref="IResource"/>
		/// which the interpreter is likely to hold
		/// </summary>
		/// <value></value>
		public IResource Source
		{
			get { return source; }
		}

		/// <summary>
		/// Gets or sets the name of the environment.
		/// </summary>
		/// <value>The name of the environment.</value>
		public string EnvironmentName
		{
			get { return environmentName; }
			set { environmentName = value; }
		}

		#endregion

		#region Helpers to populate IConfigurationStore

		protected void AddFacilityConfig(IConfiguration facility, IConfigurationStore store)
		{
			AddFacilityConfig( facility.Attributes["id"], facility, store );
		}

		protected void AddComponentConfig(IConfiguration component, IConfigurationStore store)
		{
			AddComponentConfig( component.Attributes["id"], component, store );
		}

		protected void AddChildContainerConfig(string name, IConfiguration childContainer, IConfigurationStore store)
		{
			AssertValidId(name);

			// TODO: Use import collection on type attribute (if it exists)

			store.AddChildContainerConfiguration(name, childContainer);
		}

		protected void AddFacilityConfig(string id, IConfiguration facility, IConfigurationStore store)
		{
			AssertValidId(id);

			// TODO: Use import collection on type attribute (if it exists)

			store.AddFacilityConfiguration( id, facility );
		}

		protected void AddComponentConfig(string id, IConfiguration component, IConfigurationStore store)
		{
			AssertValidId(id);

			// TODO: Use import collection on type and service attribute (if they exist)
			
			store.AddComponentConfiguration( id, component );
		}

		protected void AddBootstrapComponentConfig(string id, IConfiguration component, IConfigurationStore store)
		{
			AssertValidId(id);

			// TODO: Use import collection on type and service attribute (if they exist)

			store.AddBootstrapComponentConfiguration(id, component);
		}

		private void AssertValidId(string id)
		{
			if (id == null || id.Length == 0)
			{
				string message = "Component or Facility was declared without a proper 'id' attribute";

				throw new ConfigurationErrorsException(message);
			}
		}

		#endregion
		
		protected void ProcessInclude(string uri, IConfigurationStore store)
		{
			IResource resource = store.GetResource(uri, CurrentResource);

			if (resource == null)
			{
				// TODO: Proper Exception
			}

			PushResource(resource);
			
			ProcessResource(resource, store);
			
			PopResource();
		}		
	}
}
