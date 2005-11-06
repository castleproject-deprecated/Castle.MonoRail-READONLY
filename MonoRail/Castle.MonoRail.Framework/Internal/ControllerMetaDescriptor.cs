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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Reflection;

	public abstract class BaseMetaDescriptor
	{
		private LayoutAttribute layout;
		private IList rescues;
		private IList resources = new ArrayList();

		public LayoutAttribute Layout
		{
			get { return layout; }
			set { layout = value; }
		}

		public IList Rescues
		{
			get { return rescues; }
			set { rescues = value; }
		}

		public IList Resources
		{
			get { return resources; }
		}
	}

	public class ActionMetaDescriptor : BaseMetaDescriptor
	{
		private SkipRescueAttribute skipRescue;
		private IList skipFilters = new ArrayList();

		public SkipRescueAttribute SkipRescue
		{
			get { return skipRescue; }
			set { skipRescue = value; }
		}

		public IList SkipFilters
		{
			get { return skipFilters; }
		}
	}

	/// <summary>
	/// Holds all monorail attributes a controller might 
	/// expose, so the attributes are collected only once.
	/// This is a huge performance boost. 
	/// </summary>
	[Serializable]
	public class ControllerMetaDescriptor : BaseMetaDescriptor
	{
		private DefaultActionAttribute defaultAction;
		private IList scaffoldings = new ArrayList();
		private IList helpers = new ArrayList();
		private IList filters = new ArrayList();
		private IList actionProviders = new ArrayList();
		private Hashtable actions = new Hashtable();

		public ControllerMetaDescriptor()
		{
		}

		public ActionMetaDescriptor GetAction(MethodInfo actionMethod)
		{
			ActionMetaDescriptor desc = (ActionMetaDescriptor) actions[actionMethod];

			if (desc == null)
			{
				desc = new ActionMetaDescriptor();
				actions[actionMethod] = desc;
			}

			return desc;
		}

		public DefaultActionAttribute DefaultAction
		{
			get { return defaultAction; }
			set { defaultAction = value; }
		}

		public IList Helpers
		{
			get { return helpers; }
		}

		public IList Filters
		{
			get { return filters; }
		}

		public IList Scaffoldings
		{
			get { return scaffoldings; }
		}

		public IList ActionProviders
		{
			get { return actionProviders; }
		}
	}
}
