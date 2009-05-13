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

namespace Castle.Facilities.Cache
{
	using Castle.MicroKernel.Facilities;

	/// <summary>
	/// Summary description for CacheFacility.
	/// </summary>
	public class CacheFacility : AbstractFacility
	{
		/// <summary>
		/// The custom initialization for the Facility.
		/// </summary>
		/// <remarks>It must be overriden.</remarks>
		protected override void Init()
		{
			Kernel.AddComponent( "cache.interceptor", typeof(CacheInterceptor) );
			Kernel.AddComponent( "cache.configHolder", typeof(CacheConfigHolder) );
			Kernel.ComponentModelBuilder.AddContributor( new CacheComponentInspector() );
		}

	}
}
