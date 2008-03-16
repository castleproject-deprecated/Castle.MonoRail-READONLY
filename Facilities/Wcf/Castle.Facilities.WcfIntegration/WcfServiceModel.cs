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

    public class WcfServiceModel
    {
        private ICollection<string> baseAddresses;
        private ICollection<IWcfEndpoint> endpoints;

		public ICollection<string> BaseAddresses
		{
			get
			{
				if (baseAddresses == null)
				{
					baseAddresses = new List<string>();
				}
				return baseAddresses;
			}
			set { baseAddresses = value; }
		}

        public WcfServiceModel AddBaseAddresses(params string[] baseAddresses)
        {
            foreach (string baseAddress in baseAddresses)
            {
                BaseAddresses.Add(baseAddress);
            }
            return this;
        }

		public ICollection<IWcfEndpoint> Endpoints
		{
			get
			{
				if (endpoints == null)
				{
					endpoints = new List<IWcfEndpoint>();
				}
				return endpoints;
			}
			set { endpoints = value; }
		}

        public WcfServiceModel AddEndpoints(params IWcfEndpoint[] endpoints)
        {
            foreach (IWcfEndpoint endpoint in endpoints)
            {
                Endpoints.Add(endpoint);
            }
            return this;
        }

        public Uri[] GetBaseAddressesUris()
        {
            int i = 0;
            Uri[] baseAddressUris = new Uri[BaseAddresses.Count];
            foreach (string baseAddress in BaseAddresses)
            {
                baseAddressUris[i++] = new Uri(baseAddress, UriKind.Absolute);
            }
            return baseAddressUris;
        }
    }
}

