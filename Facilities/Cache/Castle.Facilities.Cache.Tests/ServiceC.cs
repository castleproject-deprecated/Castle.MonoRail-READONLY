using System;

namespace Castle.Facilities.Cache.Tests
{
	/// <summary>
	/// Description r�sum�e de ServiceC.
	/// </summary>
	[Cache("FifoCacheManager")]
	public class ServiceC : IServiceC
	{

		#region Membres de IServiceC

		[Cache]
		public decimal MyMethod(int a , decimal c)
		{
			decimal ret = a+c;

			Console.Write(ret.ToString() + Environment.TickCount.ToString());
			return (ret);
		}

		#endregion
	}
}
