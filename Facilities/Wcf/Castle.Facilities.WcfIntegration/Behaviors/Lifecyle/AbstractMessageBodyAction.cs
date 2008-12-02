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
	using System.ServiceModel.Channels;
	using System.Xml;

	public abstract class AbstractMessageBodyAction<T> : AbstractExtension<T>, IMessageBodyAction
		where T : MessageLifecycleBehavior<T>
	{
		private readonly MessageLifecycle lifecycle;

		protected AbstractMessageBodyAction(MessageLifecycle lifecycle)
		{
			this.lifecycle = lifecycle;
		}

		public MessageLifecycle Lifecycle
		{
			get { return lifecycle; }
		}

		public virtual bool ShouldPerform(MessageLifecycle lifecycle)
		{
			return (lifecycle & this.lifecycle) > 0;			
		}

		public abstract bool Perform(Message message, XmlDocument body, MessageLifecycle lifecycle);
	}

	public abstract class AbstractMessageBodyAction : AbstractMessageBodyAction<MessageLifecycleBehavior>
	{
		protected AbstractMessageBodyAction(MessageLifecycle lifecycle)
			: base(lifecycle)
		{
		}
	}
}
