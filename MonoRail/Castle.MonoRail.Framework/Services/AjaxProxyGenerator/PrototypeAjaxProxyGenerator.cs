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

namespace Castle.MonoRail.Framework.Services.AjaxProxyGenerator
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.Reflection;
	using System.Text;
	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Provides a service which generates a <em>JavaScript</em> block, that
	/// can be used to call Ajax actions on the controller. This JavaScript will
	/// use the <em>Prototype</em> syntax.
	/// </summary>
	public class PrototypeAjaxProxyGenerator : IAjaxProxyGenerator, IServiceEnabledComponent
	{
		private static readonly Hashtable ajaxProxyCache = Hashtable.Synchronized(new Hashtable());

		private static readonly Type ARFetcherType =
			TypeLoadUtil.GetType(
				TypeLoadUtil.GetEffectiveTypeName(
					"Castle.MonoRail.ActiveRecordSupport.ARFetchAttribute, Castle.MonoRail.ActiveRecordSupport"), true);

		private IControllerDescriptorProvider controllerDescriptorBuilder;

		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;

		public PrototypeAjaxProxyGenerator()
		{
		}

		#region IServiceEnabledComponent implementation

		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public void Service(IServiceProvider provider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(PrototypeAjaxProxyGenerator));
			}

			this.controllerDescriptorBuilder =
				(IControllerDescriptorProvider) provider.GetService(typeof(IControllerDescriptorProvider));
		}

		#endregion

		/// <summary>
		/// Generates an AJAX JavaScript proxy for a given controller.
		/// </summary>
		/// <param name="context">The context of the current request</param>
		/// <param name="proxyName">Name of the javascript proxy object</param>
		/// <param name="controller">Controller which will be target of the proxy</param>
		/// <param name="area">area which the controller belongs to</param>
		public String GenerateJSProxy(IRailsEngineContext context, string proxyName, string area, string controller)
		{
			String nl = Environment.NewLine;
			String cacheKey = (area + "|" + controller).ToLower(CultureInfo.InvariantCulture);
			String result = (String) ajaxProxyCache[cacheKey];

			if (result == null)
			{
				logger.Debug("Ajax Proxy for area: '{0}', controller: '{1}' was not found. Generating a new one", area, controller);
				IControllerTree controllerTree = (IControllerTree) context.GetService(typeof(IControllerTree));
				Type controllerType = controllerTree.GetController(area, controller);

				if (controllerType == null)
				{
					logger.Fatal("Controller not found for the area and controller specified");
					throw new RailsException("Controller not found with Area: '{0}', Name: '{1}'", area, controller);
				}

				String baseUrl = context.ApplicationPath + "/";

				if (area != null && area != String.Empty)
				{
					baseUrl += area + "/";
				}

				baseUrl += controller + "/";

				// TODO: develop a smarter function generation, inspecting the return
				// value of the action and generating a proxy that does the same.
				// also, think on a proxy pattern for the Ajax.Updater.

				StringBuilder functions = new StringBuilder(1024);

				functions.Append("{");

				ControllerMetaDescriptor metaDescriptor = controllerDescriptorBuilder.BuildDescriptor(controllerType);

				bool commaNeeded = false;

				foreach(MethodInfo ajaxActionMethod in metaDescriptor.AjaxActions)
				{
					if (!commaNeeded) commaNeeded = true;
					else functions.Append(',');

					String methodName = ajaxActionMethod.Name;

					AjaxActionAttribute ajaxActionAtt =
						(AjaxActionAttribute) GetSingleAttribute(ajaxActionMethod, typeof(AjaxActionAttribute), true);

					String url = baseUrl + methodName + "." + context.UrlInfo.Extension;
					String functionName = ToCamelCase(ajaxActionAtt.Name != null ? ajaxActionAtt.Name : methodName);

					functions.AppendFormat(nl + "\t{0}: function(", functionName);

					StringBuilder parameters = new StringBuilder("_=");

					foreach(ParameterInfo pi in ajaxActionMethod.GetParameters())
					{
						string paramName = GetParameterName(pi);

						functions.AppendFormat("{0}, ", paramName);
						parameters.AppendFormat("\\x26{0}='+{0}+'", paramName);
					}

					functions.Append("callback)").Append(nl).Append("\t{").Append(nl);
					functions.AppendFormat("\t\tvar r=new Ajax.Request('{0}', " +
					                       "{{parameters: '{1}', asynchronous: !!callback, onComplete: callback}}); " + nl +
					                       "\t\tif(!callback) return r.transport.responseText;" + nl,
					                       url, parameters);
					functions.Append("\t}").Append(nl);
				}

				functions.Length -= 1;
				functions.Append("}").Append(nl);

				ajaxProxyCache[cacheKey] = result = functions.ToString();
			}

			return @"<script type=""text/javascript"">" + nl +
			       "var " + proxyName + " =" + nl + result + "</script>";
		}

		private string GetParameterName(ParameterInfo pi)
		{
			String paramName = null;

			object parameterAttribute;

			// change the parameter name, if using [DataBind]
			parameterAttribute = GetSingleAttribute(pi, typeof(DataBindAttribute), true);
			if (parameterAttribute != null)
			{
				paramName = ((DataBindAttribute) parameterAttribute).Prefix;
			}

			if (ARFetcherType != null)
			{
				// change the parameter name, if using [ARFetch]
				parameterAttribute = GetSingleAttribute(pi, ARFetcherType, true);
				if (parameterAttribute != null)
				{
					paramName = Convert.ToString(GetPropertyValue(parameterAttribute, "RequestParameterName"));
				}
			}

			// use the default parameter name, if none of the parameter binders define a new name
			if (paramName == null || paramName.Length == 0)
				paramName = pi.Name;

			return ToCamelCase(paramName);
		}

		#region Utility Methods

		private object GetPropertyValue(object obj, string propName)
		{
			if (obj == null)
				return null;

			PropertyInfo propertyInfo = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
			return propertyInfo.GetValue(obj, null);
		}

		private object GetSingleAttribute(ICustomAttributeProvider obj, Type attributeType, bool inherit)
		{
			object[] attributes = obj.GetCustomAttributes(attributeType, inherit);
			return (attributes.Length > 0 ? attributes[0] : null);
		}

		private string ToCamelCase(string value)
		{
			if (value == null || value.Length == 0)
				return value;

			return Char.ToLower(value[0], CultureInfo.InvariantCulture)
			       + (value.Length > 0 ? value.Substring(1) : null);
		}

		#endregion
	}
}