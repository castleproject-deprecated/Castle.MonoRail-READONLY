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

namespace Castle.Facilities.ActiveRecordGenerator.Action.Standard
{
	using System;
	using System.Windows.Forms;

	using Castle.Facilities.ActiveRecordGenerator.Forms;
	using Castle.Facilities.ActiveRecordGenerator.Model;


	public class NewProjectAction : IAction
	{
		private NewProjectForm _newProjectForm;

		public NewProjectAction(NewProjectForm newProjectForm)
		{
			_newProjectForm = newProjectForm;
		}

		#region IAction Members

		public object Execute(IApplicationModel model)
		{
//			if (project.IsDirty)
//			{
//				DialogResult result = MessageBox.Show("Save changes?", "Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk);
//
//				if (result == DialogResult.Cancel)
//				{
//					return null;
//				}
//				else if (result == DialogResult.Yes)
//				{
//					// Save changes
//				}
//			}

			if (_newProjectForm.ShowDialog(model.MainWindow) == DialogResult.OK)
			{
				model.CurrentProject = _newProjectForm.Project;
			}
			
			return null;
		}

		#endregion
	}
}
