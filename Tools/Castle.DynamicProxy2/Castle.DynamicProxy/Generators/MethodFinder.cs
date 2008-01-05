// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	/// Returns the methods implemented by a type. Use this instead of Type.GetMethods() to work around a CLR issue
	/// where duplicate MethodInfos are returned by Type.GetMethods() after a token of a generic type's method was loaded.
	/// </summary>
	public class MethodFinder
	{
		private static Hashtable _cachedMethodInfosByType = new Hashtable();
		private static object _lockObject = new object();

		public static MethodInfo[] GetAllInstanceMethods(Type type, BindingFlags flags)
		{
			if ((flags & ~(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) != 0)
				throw new ArgumentException("MethodFinder only supports the Public, NonPublic, and Instance binding flags.", "flags");

			MethodInfo[] methodsInCache;

			lock(_lockObject)
			{
				if (!_cachedMethodInfosByType.ContainsKey(type))
				{
					// We always load all instance methods into the cache, we will filter them later
					_cachedMethodInfosByType.Add(
						type,
						RemoveDuplicates (type.GetMethods(
							BindingFlags.Public | BindingFlags.NonPublic
							| BindingFlags.Instance)));
				}
				methodsInCache = (MethodInfo[]) _cachedMethodInfosByType[type];
			}
			return MakeFilteredCopy(methodsInCache, flags & (BindingFlags.Public | BindingFlags.NonPublic));
		}

		private static object RemoveDuplicates (MethodInfo[] infos)
		{
			Dictionary<MethodInfo, object> uniqueInfos = new Dictionary<MethodInfo, object> (MethodSignatureComparer.Instance);
			foreach (MethodInfo info in infos)
			{
				if (!uniqueInfos.ContainsKey (info))
					uniqueInfos.Add (info, null);
			}
			MethodInfo[] result = new MethodInfo[uniqueInfos.Count];
			uniqueInfos.Keys.CopyTo (result, 0);
			return result;
		}

		private static MethodInfo[] MakeFilteredCopy(MethodInfo[] methodsInCache, BindingFlags visibilityFlags)
		{
			if ((visibilityFlags & ~(BindingFlags.Public | BindingFlags.NonPublic)) != 0)
			{
				throw new ArgumentException("Only supports BindingFlags.Public and NonPublic.", "visibilityFlags");
			}

			bool includePublic = (visibilityFlags & BindingFlags.Public) == BindingFlags.Public;
			bool includeNonPublic = (visibilityFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic;

			// Return a copy of the cached array, only returning the public methods unless requested otherwise
			List<MethodInfo> result = new List<MethodInfo>(methodsInCache.Length);
			
			foreach(MethodInfo method in methodsInCache)
			{
				if ((method.IsPublic && includePublic) || (!method.IsPublic && includeNonPublic))
				{
					result.Add(method);
				}
			}
			
			return result.ToArray();
		}
	}
}