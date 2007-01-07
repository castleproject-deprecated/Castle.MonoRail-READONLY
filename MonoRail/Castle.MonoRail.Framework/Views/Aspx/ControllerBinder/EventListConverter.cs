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

#if DOTNET2,NET

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.ComponentModel;
	using System.Web.UI;

	public class EventListConverter : StringConverter
	{
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (context != null)
			{
				EventDescriptorCollection events = GetAvailableEvents(context);

				string[] eventNames = new string[events.Count];

				for (int i = 0; i < events.Count; ++i)
				{
					eventNames[i] = events[i].Name;
				}

				Array.Sort(eventNames);

				return new StandardValuesCollection(eventNames);
			}

			return base.GetStandardValues(context);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			if (context != null)
			{
				ActionBinding binding = context.Instance as ActionBinding;

				return (binding != null) && (binding.Parent != null);
			}

			return false;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		private EventDescriptorCollection GetAvailableEvents(ITypeDescriptorContext context)
		{
			ControllerBinding parent;
			Control control = ObtainTargetControl(context, out parent);

			if (control != null && parent != null)
			{
				return EventUtil.GetCompatibleEvents(control,
					 delegate(EventDescriptor eventDescriptor)
					 {
						 string eventName = eventDescriptor.Name;
						 return parent.ActionBindings[eventName] == null;
					 });
			}

			return new EventDescriptorCollection(null, true);
		}

		private Control ObtainTargetControl(ITypeDescriptorContext context,
											out ControllerBinding parent)
		{
			parent = null;
			Control control = null;

			ActionBinding binding = context.Instance as ActionBinding;

			if (binding != null && binding.Parent != null)
			{
				parent = binding.Parent;
				control = parent.ControlInstance;

				if (control == null)
				{
					control = FindControlInContainer(context, parent);
				}
			}

			return control;
		}

		private Control FindControlInContainer(ITypeDescriptorContext context,
											   ControllerBinding parent)
		{
			IControllerBinder binder = (IControllerBinder)context.GetService(typeof(IControllerBinder));

			if (binder != null)
			{
				return binder.FindControlWithID(parent.ControlID);
			}

			return null;
		}
	}
}

#endif
