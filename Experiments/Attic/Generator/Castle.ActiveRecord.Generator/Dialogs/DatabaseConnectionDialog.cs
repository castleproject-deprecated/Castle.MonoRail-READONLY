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

namespace Castle.ActiveRecord.Generator.Dialogs
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;

	/// <summary>
	/// Summary description for DatabaseConnectionDialog.
	/// </summary>
	public class DatabaseConnectionDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.TextBox connectionString;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DatabaseConnectionDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

#if DEBUG
//			textBox1.Text = "MyAlias";
//			connectionString.Text = "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=test;Data Source=.";
#endif
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
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.connectionString = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.button3 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(360, 160);
			this.button1.Name = "button1";
			this.button1.TabIndex = 4;
			this.button1.Text = "OK";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(272, 160);
			this.button2.Name = "button2";
			this.button2.TabIndex = 1;
			this.button2.Text = "Cancel";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.TabIndex = 0;
			this.label1.Text = "Alias:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(128, 24);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(160, 21);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "";
			// 
			// connectionString
			// 
			this.connectionString.Location = new System.Drawing.Point(128, 64);
			this.connectionString.Name = "connectionString";
			this.connectionString.Size = new System.Drawing.Size(272, 21);
			this.connectionString.TabIndex = 3;
			this.connectionString.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 64);
			this.label2.Name = "label2";
			this.label2.TabIndex = 2;
			this.label2.Text = "Connection String:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(408, 64);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(24, 23);
			this.button3.TabIndex = 6;
			this.button3.Text = "...";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// DatabaseConnectionDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(448, 194);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.connectionString);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "DatabaseConnectionDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Add Database Connection";
			this.ResumeLayout(false);

		}
		#endregion

		private void button3_Click(object sender, System.EventArgs e)
		{
			MSDASC.DataLinks dataLinks = new MSDASC.DataLinksClass();
			ADODB._Connection connection;

			if (connectionString.Text == String.Empty)
			{
				try
				{
					connection = (ADODB._Connection) dataLinks.PromptNew();

					connectionString.Text = connection.ConnectionString.ToString();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
			}
			else
			{
				connection = new ADODB.ConnectionClass();
				connection.ConnectionString = connectionString.Text;

				object oConnection = connection;
				
				try
				{
					if (dataLinks.PromptEdit(ref oConnection))
					{
						connectionString.Text = connection.ConnectionString;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			// TODO: Validate fields

			DialogResult = DialogResult.OK;
			this.Close();
		}

		public String Alias
		{
			get { return textBox1.Text; }
		}

		public String ConnectionString
		{
			get { return connectionString.Text; }
		}
	}
}
