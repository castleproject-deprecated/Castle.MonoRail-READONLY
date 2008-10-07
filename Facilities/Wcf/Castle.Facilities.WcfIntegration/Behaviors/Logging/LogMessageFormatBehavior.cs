﻿// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration.Behaviors
{
	using System;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Description;
	using System.ServiceModel.Dispatcher;

	public class LogMessageFormatBehavior : IEndpointBehavior
	{
		private readonly string messageFormat;
		private readonly IFormatProvider formatProvider;

		public LogMessageFormatBehavior(string messageFormat)
		{
			this.messageFormat = messageFormat;
		}

		public LogMessageFormatBehavior(IFormatProvider formatProvider, string messageFormat)
			: this(messageFormat)
		{
			this.formatProvider = formatProvider;
		}

		public string MessageFormat
		{
			get { return messageFormat; }
		}

		public IFormatProvider FormatProvider
		{
			get { return formatProvider; }
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}
	}
}
