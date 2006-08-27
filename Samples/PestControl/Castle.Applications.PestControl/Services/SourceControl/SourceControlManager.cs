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

namespace Castle.Applications.PestControl.Services.SourceControl
{
	using System;

	using System.Collections;

	using Castle.Core;

	using Castle.MicroKernel;

	/// <summary>
	/// Summary description for SourceControlManager.
	/// </summary>
	public class SourceControlManager : IInitializable, IDisposable
	{
		private IKernel _kernel;

		public SourceControlManager(IKernel kernel)
		{
			_kernel = kernel;
		}

		public void Initialize()
		{
		}

		public void Dispose()
		{
//			_kernel = null;
		}

		/// <summary>
		/// Queries the kernel for implementations of ISourceControl.
		/// This allow new build system to be registered even without
		/// restarting the application. A more efficient approach however
		/// is to issue this query only once, but then you lost the ability
		/// to add/remove buildsystems at runtime
		/// </summary>
		/// <remarks>
		/// This approach invokes Resolve on the handler, 
		/// but do not invoke the counter part release.
		/// </remarks>
		/// <returns></returns>
		public virtual ISourceControl[] AvailableSourceControl()
		{
			ArrayList list = new ArrayList();

			IHandler[] handlers = _kernel.GetHandlers( typeof(ISourceControl) );

			foreach(IHandler handler in handlers)
			{
				if (handler.CurrentState == HandlerState.Valid)
				{
					list.Add( handler.Resolve() );
				}
			}

			return (ISourceControl[]) list.ToArray( typeof(ISourceControl) );
		}
	}
}
