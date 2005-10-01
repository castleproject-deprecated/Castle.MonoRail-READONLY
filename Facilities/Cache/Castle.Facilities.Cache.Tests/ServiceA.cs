using System;

namespace Castle.Facilities.Cache.Tests
{
	/// <summary>
	/// Description r�sum�e de ServiceA.
	/// </summary>
	[Cache]
	public class ServiceA : IServiceA
	{
		public ServiceA()
		{
			//
			// TODO�: ajoutez ici la logique du constructeur
			//
		}

		#region Membres de IServiceA

		[Cache]
		public decimal MyMethod(int a , decimal c)
		{
			return (a+c);
		}

		#endregion
	}
}
