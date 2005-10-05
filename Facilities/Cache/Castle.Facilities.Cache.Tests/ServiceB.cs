using System;

namespace Castle.Facilities.Cache.Tests
{
	/// <summary>
	/// Description r�sum�e de ServiceB.
	/// </summary>
	public class ServiceB : IServiceB
	{

		#region IServiceB members

		public string MyMethodA(string a, string b, string c)
		{
			string ret = a+b+c;

			Console.Write( ret.ToString() + Environment.TickCount.ToString() );
			return ret;
		}

		public void MyMethodB()
		{
			Console.Write( Environment.TickCount.ToString() );
		}

		#endregion
	}
}
