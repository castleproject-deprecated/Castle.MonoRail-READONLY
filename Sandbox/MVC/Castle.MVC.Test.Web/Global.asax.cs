using System;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using Castle.MVC.Navigation;
using Castle.MVC.StatePersister;
using Castle.MVC.States;
using Castle.MVC.Test.Presentation;
using Castle.MVC.Views;
using Castle.Windsor;

namespace Castle.MVC.Test.Web 
{
	/// <summary>
	/// Description r�sum�e de Global.
	/// </summary>
	public class Global : HttpApplication, IContainerAccessor
	{
		/// <summary>
		/// Variable n�cessaire au concepteur.
		/// </summary>
		private IContainer components = null;

		private static WindsorContainer _container;

		#region IContainerAccessor implementation

		public IWindsorContainer Container
		{
			get { return _container; }
		}

		#endregion

		public Global()
		{
			InitializeComponent();
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{
			_container = new MyContainer( Assembly.GetExecutingAssembly() );
		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_Error(Object sender, EventArgs e)
		{

		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{
			_container.Dispose();
		}
			
		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent()
		{    
			this.components = new Container();
		}
		#endregion
	}
}

