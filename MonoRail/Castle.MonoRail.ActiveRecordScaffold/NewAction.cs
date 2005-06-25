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

namespace Castle.MonoRail.ActiveRecordScaffold
{
	using System;
	using System.Text;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Internal;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;


	public class NewAction : AbstractScaffoldAction
	{
		public NewAction( Type modelType ) : base(modelType)
		{
		}

		public override void Execute(Controller controller)
		{
			ActiveRecordModel model = ActiveRecordBase._GetModel( modelType );

			if (model == null)
			{
				throw new ScaffoldException("Specified type isn't an ActiveRecord type or the ActiveRecord " + 
					"framework wasn't started properly. Did you forget about the Initialize method?");
			}

			if (controller.LayoutName == null)
			{
				controller.LayoutName = "Scaffold";
			}

			String name = model.Type.Name;
			String viewName = String.Format(@"{0}\new{1}", controller.Name, name);

			if (controller.HasTemplate(viewName))
			{
				// The programmer provided a custom view

				controller.PropertyBag.Add( "armodel", model );

				controller.RenderView(controller.Name, "new" + name);
			}
			else
			{
				object instance = Activator.CreateInstance( model.Type );
				GenerateHtml(name, model, instance, controller);
			}
		}
	}
}
