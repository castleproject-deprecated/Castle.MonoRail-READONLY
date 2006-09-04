using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Castle.MVC.Test.Presentation;

namespace Castle.MVC.Test.Win
{
	/// <summary>
	/// Description r�sum�e de Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// Variable n�cessaire au concepteur.
		/// </summary>
		private System.ComponentModel.Container components = null;
//		private HomeController _homeController = null;
//
//		/// <summary>
//		/// Strong-typed controller property 
//		/// that returns the controller so we get nice IntelliSense 
//		/// and less casting errors.
//		/// </summary>
//		public HomeController HomeController
//		{
//			set
//			{
//				_homeController = value;
//			}
//		}

		public Form1()
		{
			//
			// Requis pour la prise en charge du Concepteur Windows Forms
			//
			InitializeComponent();

			Console.Write("eee");
			//
			// TODO : ajoutez le code du constructeur apr�s l'appel � InitializeComponent
			//
		}

		/// <summary>
		/// Nettoyage des ressources utilis�es.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Code g�n�r� par le Concepteur Windows Form
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.Size = new System.Drawing.Size(300,300);
			this.Text = "Form1";
		}
		#endregion

		/// <summary>
		/// Point d'entr�e principal de l'application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}
	}
}
