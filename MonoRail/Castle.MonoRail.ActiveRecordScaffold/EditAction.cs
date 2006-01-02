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

namespace Castle.MonoRail.ActiveRecordScaffold
{
	using System;

	using Castle.Components.Common.TemplateEngine;

	using Castle.MonoRail.Framework;
	
	using Castle.ActiveRecord.Framework;
	
	/// <summary>
	/// Renders an edit form
	/// </summary>
	/// <remarks>
	/// Searchs for a template named <c>edit{name}</c>
	/// </remarks>
	public class EditAction : AbstractScaffoldAction
	{
		public EditAction(Type modelType, ITemplateEngine templateEngine) : base(modelType, templateEngine)
		{
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			return String.Format(@"{0}\edit{1}", controller.Name, Model.Type.Name);
		}

		protected override void PerformActionProcess(Controller controller)
		{
			object idVal = CommonOperationUtils.ReadPkFromParams(controller, ObtainPKProperty());

			try
			{
				object instance = SupportingUtils.FindByPK( Model.Type, idVal );
				
				controller.PropertyBag["armodel"] = Model;
				controller.PropertyBag["instance"] = instance;
				controller.PropertyBag["id"] = idVal;
			}
			catch(Exception ex)
			{
				throw new ScaffoldException("Could not obtain instance by using this id", ex);
			}
		}

		protected override void RenderStandardHtml(Controller controller)
		{
			SetUpHelpers(controller);
			RenderFromTemplate("edit.vm", controller);
		}
	}
}
