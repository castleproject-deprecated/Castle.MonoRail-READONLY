using System;
using System.Collections;

namespace Castle.Facilities.Cache
{
	/// <summary>
	/// Description r�sum�e de CaheConfigHolder.
	/// </summary>
	public class CacheConfigHolder
	{
		Hashtable _impl2Config = new Hashtable();

		public void Register(Type implementation, CacheConfig config)
		{
			_impl2Config[implementation] = config;
		}

		public CacheConfig GetConfig(Type implementation)
		{
			return _impl2Config[implementation] as CacheConfig;
		}
	}
}
