// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration
{
    using System;
	using System.Collections.Generic;

	public class WcfClientModel : IWcfClientModel
	{
		private IWcfEndpoint endpoint;
		private ICollection<IWcfBehavior> behaviors;

		public WcfClientModel()
		{
		}

		public WcfClientModel(IWcfEndpoint endpoint)
		{
			Endpoint = endpoint;
		}

		#region IWcfClientModel Members

		public Type Contract
		{
			get { return endpoint.Contract; }
		}

		public IWcfEndpoint Endpoint
		{
			get { return endpoint; }
			set 
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				endpoint = value; 
			}
		}

		public ICollection<IWcfBehavior> Behaviors
		{
			get
			{
				if (behaviors == null)
				{
					behaviors = new List<IWcfBehavior>();
				}
				return behaviors;
			}
		}

		#endregion


		public WcfClientModel AddBehaviors(params object[] behaviors)
		{
			foreach (object behavior in behaviors)
			{
				Behaviors.Add(WcfExplcitBehavior.CreateFrom(behavior));
			}
			return this;
		}
	}
}

