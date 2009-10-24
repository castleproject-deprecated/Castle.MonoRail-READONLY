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

namespace Castle.MonoRail.Framework.Extensions.ExceptionChaining
{
	using System;
	using System.Configuration;
	using Castle.Core.Configuration;
	using Castle.MonoRail.Framework.Configuration;

	/// <summary>
	/// This extension allow one to perform one or more steps
	/// in response to an exception threw by an action. 
	/// 
	/// <seealso cref="IExceptionHandler"/>
	/// 
	/// </summary>
	/// 
	/// <remarks>
	/// To successfully install this extension you must register 
	/// it on the <c>extensions</c> node and the handlers within the <c>exception</c> node:
	/// <code>
	///   &lt;monorail&gt;
	///   	&lt;extensions&gt;
	///   	  &lt;extension type="Castle.MonoRail.Framework.Extensions.ExceptionChaining.ExceptionChainingExtension, Castle.MonoRail.Framework" /&gt;
	///   	&lt;/extensions&gt;
	///   	
	///   	&lt;exception&gt;
	///   	  &lt;exceptionHandler type="Type name that implements IExceptionHandler" /&gt;
	///   	  &lt;exceptionHandler type="Type name that implements IExceptionHandler" /&gt;
	///   	&lt;/exception&gt;
	///   &lt;/monorail&gt;
	/// </code>
	/// <para>
	/// Controllers can request IExceptionProcessor through IServiceProvider
	/// and invoke the handlers to process an exception
	/// </para>
	/// <code>
	/// <![CDATA[
	/// public void BuyMercedes()
	/// {
	///		try
	///		{
	///			...
	///		}
	///		catch(Exception ex)
	///		{
	///			IExceptionProcessor exProcessor = ServiceProvider.GetService<IExceptionProcessor>();
	///			exProcessor.ProcessException(ex);
	/// 
	///			RenderView("CouldNotBuyMercedes");
	///		}
	/// }
	/// ]]>
	/// </code>
	/// </remarks>
	public class ExceptionChainingExtension : IMonoRailExtension, IExceptionProcessor
	{
		private IExceptionHandler firstHandler;

		#region IMonoRailExtension implementation

		/// <summary>
		/// Gives to the extension implementor a chance to read
		/// attributes and child nodes of the extension node
		/// </summary>
		/// <param name="node">The node that defines the MonoRail extension</param>
		public void SetExtensionConfigNode(IConfiguration node)
		{
			// Ignored
		}

		#endregion

		#region IServiceEnabledComponent implementation

		/// <summary>
		/// Services the specified provider.
		/// </summary>
		/// <param name="serviceProvider">The provider.</param>
		public void Service(IMonoRailServices serviceProvider)
		{
			ExtensionManager manager = (ExtensionManager)
									   serviceProvider.GetService(typeof(ExtensionManager));

			IMonoRailConfiguration config = (IMonoRailConfiguration)
										   serviceProvider.GetService(typeof(IMonoRailConfiguration));

			manager.ActionException += OnException;
			manager.UnhandledException += OnException;

			IConfiguration exceptionNode = config.ConfigurationSection.Children["exception"];

			foreach(IConfiguration node in exceptionNode.Children)
			{
				string typeAtt = node.Attributes["type"];

				if (typeAtt == null)
				{
					throw new MonoRailException("Configuration error: missing type attribute on exception handler configuration.");
				}

				InstallExceptionHandler(node, typeAtt);
			}
		}

		#endregion

		#region IExceptionProcessor implementation

		/// <summary>
		/// Initiates the ExceptionChainingExtension manualy
		/// </summary>
		/// <param name="exception">The exception to process</param>
		/// <param name="engineContext">The engine context.</param>
		public void ProcessException(Exception exception, IEngineContext engineContext)
		{
			if (exception == null) return;

			engineContext.LastException = exception;

			OnException(engineContext);
		}

		#endregion

		/// <summary>
		/// Called when an exception happens.
		/// </summary>
		/// <param name="context">The context.</param>
		private void OnException(IEngineContext context)
		{
			const String mrExceptionKey = "MonoRail.ExceptionHandled";

			if (context.Items.Contains(mrExceptionKey))
			{
				return;
			}

			if (firstHandler != null)
			{
				context.Items.Add(mrExceptionKey, true);

				firstHandler.Process(context);
			}
		}

		/// <summary>
		/// Installs the exception handler.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="typeName">Name of the type.</param>
		private void InstallExceptionHandler(IConfiguration node, String typeName)
		{
			IExceptionHandler handler;

			Type handlerType = TypeLoadUtil.GetType(typeName);

			if (handlerType == null)
			{
				String message = "The Type for the custom session could not be loaded. " +
				                 typeName;
				throw new ConfigurationErrorsException(message);
			}

			try
			{
				handler = (IExceptionHandler) Activator.CreateInstance(handlerType);
			}
			catch(InvalidCastException)
			{
				String message = "The Type for the custom session must " +
				                 "implement ICustomSessionFactory. " + typeName;
				throw new ConfigurationErrorsException(message);
			}

			IConfigurableHandler configurableHandler = handler as IConfigurableHandler;

			if (configurableHandler != null)
			{
				configurableHandler.Configure(node);
			}

			handler.Initialize();

			if (firstHandler == null)
			{
				firstHandler = handler;
			}
			else
			{
				IExceptionHandler navHandler = firstHandler;

				while(navHandler != null)
				{
					if (navHandler.Next == null)
					{
						navHandler.Next = handler;
						break;
					}

					navHandler = navHandler.Next;
				}
			}
		}
	}
}