// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Generator.Parts
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using WeifenLuo.WinFormsUI;

	using Castle.ActiveRecord.Generator.Actions;
	using Castle.ActiveRecord.Generator.Components;
	using Castle.ActiveRecord.Generator.Components.Database;

	/// <summary>
	/// Summary description for AvailableShapes.
	/// </summary>
	public class AvailableShapes : DockContent, ISubWorkspace
	{
		private Netron.GraphLib.UI.GraphShapesView graphShapesView1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AvailableShapes()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			graphShapesView1.AddLibrary("Castle.ActiveRecord.Generator.exe");
		}

		public AvailableShapes(Model _model) : this()
		{
		}

		public IWin32Window ActiveWindow
		{
			get { throw new NotImplementedException(); }
		}

		public MainMenu MainMenu
		{
			get { throw new NotImplementedException(); }
		}

		public ToolBar MainToolBar
		{
			get { throw new NotImplementedException(); }
		}

		public StatusBar MainStatusBar
		{
			get { throw new NotImplementedException(); }
		}

		public DockPanel MainDockManager
		{
			get { throw new NotImplementedException(); }
		}

		public IWorkspace ParentWorkspace
		{
			get { return null; }
			set { ; }
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.graphShapesView1 = new Netron.GraphLib.UI.GraphShapesView();
			((System.ComponentModel.ISupportInitialize)(this.graphShapesView1)).BeginInit();
			this.SuspendLayout();
			// 
			// graphShapesView1
			// 
			this.graphShapesView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.graphShapesView1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.graphShapesView1.Location = new System.Drawing.Point(0, 0);
			this.graphShapesView1.Name = "graphShapesView1";
			this.graphShapesView1.Size = new System.Drawing.Size(246, 321);
			this.graphShapesView1.TabIndex = 0;
			this.graphShapesView1.View = System.Windows.Forms.View.LargeIcon;
			// 
			// AvailableShapes
			// 
			this.DockableAreas = ((WeifenLuo.WinFormsUI.DockAreas)(((((WeifenLuo.WinFormsUI.DockAreas.Float | WeifenLuo.WinFormsUI.DockAreas.DockLeft) 
				| WeifenLuo.WinFormsUI.DockAreas.DockRight) 
				| WeifenLuo.WinFormsUI.DockAreas.DockTop) 
				| WeifenLuo.WinFormsUI.DockAreas.DockBottom)));
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(246, 323);
			this.Controls.Add(this.graphShapesView1);
			this.DockPadding.Bottom = 2;
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.HideOnClose = true;
			this.Name = "AvailableShapes";
			this.ShowHint = WeifenLuo.WinFormsUI.DockState.DockRight;
			this.Text = "ActiveRecord Components";
			((System.ComponentModel.ISupportInitialize)(this.graphShapesView1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
