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

namespace Castle.ActiveRecord.Generator.Actions
{
	using System;
	using System.Windows.Forms;


	public class ExitAction : AbstractAction
	{
		private MenuItem _item;

		public ExitAction()
		{
		}

		public override void Install(IWorkspace workspace, object parentMenu, object parentGroup)
		{
			base.Install(workspace, parentMenu, parentGroup);

			_item = new MenuItem("Exit");
			_item.Click += new EventHandler(OnExit);

			(parentMenu as MenuItem).MenuItems.Add(_item);
		}

		private void OnExit(object sender, EventArgs e)
		{
			// TODO: Confirmation
			Application.Exit();
		}
	}
}
