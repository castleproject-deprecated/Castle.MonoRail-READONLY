using System;

namespace Castle.MVC.Test
{
	/// <summary>
	/// Description r�sum�e de IServcieA.
	/// </summary>
	public interface IServiceA
	{
		decimal MyMethod(int a , decimal c);

		string MyMethodNotcached(string a);
	}
}
