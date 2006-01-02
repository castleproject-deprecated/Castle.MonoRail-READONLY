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

namespace Castle.Applications.MindDump.Services
{
	using System;
	using System.Collections;


	public class MindDumpEventChannel : IMindDumpEventPublisher
	{
		private IList _subscribers = new ArrayList();

		public MindDumpEventChannel()
		{
		}

		#region IMindDumpEventPublisher Members

		public void NotifyBlogAdded(Castle.Applications.MindDump.Model.Blog blog)
		{
			foreach(IMindDumpEventSubscriber subs in _subscribers)
			{
				subs.OnBlogAdded(blog);
			}
		}

		public void NotifyBlogRemoved(Castle.Applications.MindDump.Model.Blog blog)
		{
			foreach(IMindDumpEventSubscriber subs in _subscribers)
			{
				subs.OnBlogRemoved(blog);
			}
		}

		public void AddSubcriber(IMindDumpEventSubscriber subscriber)
		{
			_subscribers.Add(subscriber);
		}

		public void RemoveSubcriber(IMindDumpEventSubscriber subscriber)
		{
			_subscribers.Remove(subscriber);
		}

		#endregion
	}
}
