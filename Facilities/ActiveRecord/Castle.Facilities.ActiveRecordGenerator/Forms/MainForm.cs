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

namespace Castle.Facilities.ActiveRecordGenerator.Forms
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Data;

	using Castle.Facilities.ActiveRecordGenerator.Action;
	using Castle.Facilities.ActiveRecordGenerator.Model;

	/// <summary>
	/// Summary description for MainForm
	/// </summary>
	public class MainForm : Form, IApplicationModel
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem fileMenu;
		private System.Windows.Forms.MenuItem newMenu;
		private System.Windows.Forms.MenuItem openMenu;
		private System.Windows.Forms.MenuItem saveMenu;
		private System.Windows.Forms.MenuItem saveAsMenu;
		private System.Windows.Forms.MenuItem exitMenu;
		private System.Windows.Forms.MenuItem helpMenu;
		private System.Windows.Forms.MenuItem aboutMenu;
		private System.ComponentModel.IContainer components;

		private Hashtable _translator = new Hashtable();

		private IActionFactory _actionFactory;
		private Project _currentProject;


		public MainForm(IActionFactory actionFactory)
		{
			_actionFactory = actionFactory;

			InitializeComponent();

			InitTranslator();
		}

		private void InitTranslator()
		{
			_translator.Add(newMenu, ActionConstants.New_Project);
			_translator.Add(openMenu, ActionConstants.Open_Project);
			_translator.Add(saveMenu, ActionConstants.Save_Project);
			_translator.Add(saveAsMenu, ActionConstants.SaveAs_Project);
			_translator.Add(exitMenu, ActionConstants.Exit);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.fileMenu = new System.Windows.Forms.MenuItem();
			this.newMenu = new System.Windows.Forms.MenuItem();
			this.openMenu = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.saveMenu = new System.Windows.Forms.MenuItem();
			this.saveAsMenu = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.exitMenu = new System.Windows.Forms.MenuItem();
			this.helpMenu = new System.Windows.Forms.MenuItem();
			this.aboutMenu = new System.Windows.Forms.MenuItem();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.splitter1 = new System.Windows.Forms.Splitter();
			((System.ComponentModel.ISupportInitialize) (this.statusBarPanel1)).BeginInit();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
				{
					this.fileMenu,
					this.helpMenu
				});
			// 
			// fileMenu
			// 
			this.fileMenu.Index = 0;
			this.fileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
				{
					this.newMenu,
					this.openMenu,
					this.menuItem4,
					this.saveMenu,
					this.saveAsMenu,
					this.menuItem7,
					this.menuItem8,
					this.menuItem9,
					this.exitMenu
				});
			this.fileMenu.Text = "&File";
			// 
			// newMenu
			// 
			this.newMenu.Index = 0;
			this.newMenu.Text = "&New...";
			this.newMenu.Click += new System.EventHandler(this.Menu_Click);
			// 
			// openMenu
			// 
			this.openMenu.Index = 1;
			this.openMenu.Text = "Open...";
			this.openMenu.Click += new System.EventHandler(this.Menu_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 2;
			this.menuItem4.Text = "-";
			// 
			// saveMenu
			// 
			this.saveMenu.Index = 3;
			this.saveMenu.Text = "&Save";
			this.saveMenu.Click += new System.EventHandler(this.Menu_Click);
			// 
			// saveAsMenu
			// 
			this.saveAsMenu.Index = 4;
			this.saveAsMenu.Text = "S&ave As...";
			this.saveAsMenu.Click += new System.EventHandler(this.Menu_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 5;
			this.menuItem7.Text = "-";
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 6;
			this.menuItem8.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
				{
					this.menuItem13
				});
			this.menuItem8.Text = "Recent Projecs";
			// 
			// menuItem13
			// 
			this.menuItem13.Index = 0;
			this.menuItem13.Text = "Sample";
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 7;
			this.menuItem9.Text = "-";
			// 
			// exitMenu
			// 
			this.exitMenu.Index = 8;
			this.exitMenu.Text = "E&xit";
			this.exitMenu.Click += new System.EventHandler(this.Menu_Click);
			// 
			// helpMenu
			// 
			this.helpMenu.Index = 1;
			this.helpMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
				{
					this.aboutMenu
				});
			this.helpMenu.Text = "&Help";
			// 
			// aboutMenu
			// 
			this.aboutMenu.Index = 0;
			this.aboutMenu.Text = "About...";
			this.aboutMenu.Click += new System.EventHandler(this.Menu_Click);
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 468);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[]
				{
					this.statusBarPanel1
				});
			this.statusBar1.Size = new System.Drawing.Size(712, 22);
			this.statusBar1.TabIndex = 3;
			this.statusBar1.Text = "statusBar1";
			// 
			// statusBarPanel1
			// 
			this.statusBarPanel1.Text = "Ready.";
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(712, 468);
			this.panel1.TabIndex = 7;
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeView1.ImageIndex = -1;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectedImageIndex = -1;
			this.treeView1.Size = new System.Drawing.Size(121, 468);
			this.treeView1.TabIndex = 8;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(121, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 468);
			this.splitter1.TabIndex = 9;
			this.splitter1.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 15);
			this.ClientSize = new System.Drawing.Size(712, 490);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.statusBar1);
			this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
			this.Menu = this.mainMenu1;
			this.Name = "MainForm";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize) (this.statusBarPanel1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private void Menu_Click(object sender, System.EventArgs e)
		{
			String actionName = (String) _translator[sender];

			if (actionName != null)
			{
				IAction action = _actionFactory.Create(actionName);

				action.Execute(this);
			}
		}

		#region IApplicationModel Members

		public Project CurrentProject
		{
			get { return _currentProject; }
			set
			{
				if (OnProjectChange != null) OnProjectChange(this, _currentProject, value);
				_currentProject = value;
			}
		}

		public IWin32Window MainWindow
		{
			get { return this; }
		}

		public event ProjectDelegate OnProjectChange;

		#endregion
	}
}