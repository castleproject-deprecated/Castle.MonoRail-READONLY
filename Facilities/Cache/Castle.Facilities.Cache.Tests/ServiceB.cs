using System;

namespace Castle.Facilities.Cache.Tests
{
	/// <summary>
	/// Description r�sum�e de ServiceB.
	/// </summary>
	public class ServiceB : IServiceB
	{
		public ServiceB()
		{
			//
			// TODO�: ajoutez ici la logique du constructeur
			//
		}

		#region Membres de IServiceB

		public string MyMethod(string a, string b, string c)
		{
			// TODO�: ajoutez l'impl�mentation de ServiceB.MyMethod
			return a+b+c;
		}

		#endregion
	}
}
