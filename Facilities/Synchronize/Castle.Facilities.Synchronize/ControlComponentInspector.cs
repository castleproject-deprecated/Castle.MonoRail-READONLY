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

namespace Castle.Facilities.Synchronize
{
	using System;
	using System.Configuration;
	using System.Windows.Forms;
	using Castle.Core;
	using Castle.Core.Configuration;
	using Castle.DynamicProxy;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ModelBuilder;
	using Castle.MicroKernel.SubSystems.Conversion;
	using Castle.Windsor.Proxy;

	/// <summary>
	/// Checks for <see cref="Control"/> implementations a registers
	/// components to ensure the controls can be safely created and
	/// accessed from different threads.
	/// </summary>
	internal class ControlComponentInspector : IContributeComponentModelConstruction, IDisposable
	{
		private readonly MarshalingControl marshalingControl;
		private readonly IProxyGenerationHook controlProxyHook;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControlComponentInspector"/> class.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="config">The config.</param>
		public ControlComponentInspector(IKernel kernel, IConfiguration config)
		{
			marshalingControl = new MarshalingControl();
			controlProxyHook = ObtainProxyGenerationHook(kernel, config);

			RegisterWindowsFormsSynchronizationContext(kernel);
		}

		/// <summary>
		/// Processes <see cref="Control"/> implementations.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="model">The model.</param>
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			CheckForControlImplementation(model);
		}

		/// <summary>
		/// Releases the marshaling control.
		/// </summary>
		public void Dispose()
		{
			if (marshalingControl != null)
			{
				marshalingControl.Dispose();
			}
		}

		private void CheckForControlImplementation(ComponentModel model)
		{
			// Since controls created on different threads cannot be added to one
			// another, ensure that all controls are created on the main UI thread.

			if (typeof(Control).IsAssignableFrom(model.Implementation))
			{
				ConfigureProxyOptions(model);
				model.ExtendedProperties[Constants.MarshalControl] = marshalingControl;
				model.CustomComponentActivator = typeof(ControlComponentActivator);
			}
		}

		private void ConfigureProxyOptions(ComponentModel model)
		{
			ProxyGenerationOptions options = ProxyUtil.ObtainProxyGenerationOptions(model, true);
			options.Hook = controlProxyHook;
		}

		private static void RegisterWindowsFormsSynchronizationContext(IKernel kernel)
		{
#if DOTNET2
			WindowsFormsSynchronizationContext winFormsSyncCtx =
				WindowsFormsSynchronizationContext.Current as WindowsFormsSynchronizationContext;

			if (winFormsSyncCtx != null)
			{
				kernel.AddComponentInstance(Constants.WinFormsSyncContext, winFormsSyncCtx);
			}
#endif
		}

		private static IProxyGenerationHook ObtainProxyGenerationHook(IKernel kernel, IConfiguration config)
		{
			IProxyGenerationHook hook = null;

			if (config != null)
			{
				String hookAttrib = config.Attributes[Constants.ControlProxyHookAttrib];

				if (hookAttrib != null)
				{
					ITypeConverter converter = (ITypeConverter)
					                           kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);

					Type hookType = (Type) converter.PerformConversion(hookAttrib, typeof(Type));

					if (!typeof(IProxyGenerationHook).IsAssignableFrom(hookType))
					{
						String message = String.Format("The specified controlProxyHook does " +
						                               "not implement the interface IProxyGenerationHook. Type {0}", hookType.FullName);
#if DOTNET2
						throw new ConfigurationErrorsException(message);
#else
						throw new ConfigurationException(message);
#endif
					}

					hook = (IProxyGenerationHook) Activator.CreateInstance(hookType);
				}
			}

			if (hook == null)
			{
				hook = new ControlComponentHook();
			}

			return hook;
		}

		#region MarshalingControl

		private sealed class MarshalingControl : Control
		{
			internal MarshalingControl()
			{
				Visible = false;
				SetTopLevel(true);
				CreateControl();
				CreateHandle();
			}

			protected override void OnLayout(LayoutEventArgs levent)
			{
			}

			protected override void OnSizeChanged(EventArgs e)
			{
			}
		}

		#endregion
	}
}