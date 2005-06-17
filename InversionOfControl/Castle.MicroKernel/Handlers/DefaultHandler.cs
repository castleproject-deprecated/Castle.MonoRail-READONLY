// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Handlers
{
	using System;

	using Castle.Model;

	/// <summary>
	/// Summary description for DefaultHandler.
	/// </summary>
	[Serializable]
	public class DefaultHandler : AbstractHandler
	{
		public DefaultHandler(ComponentModel model) : base(model)
		{
		}

		#region IHandler Members

		public override object Resolve()
		{
			if (CurrentState == HandlerState.WaitingDependency)
			{
				String message = 
					String.Format("Can't create component as it has dependencies to be satisfied. {0}", 
						ObtainDependencyDetails() );

				throw new HandlerException(message);
			}
			
			return _lifestyleManager.Resolve();
		}

		public override void Release(object instance)
		{
			_lifestyleManager.Release( instance );
		}

		#endregion
	}
}