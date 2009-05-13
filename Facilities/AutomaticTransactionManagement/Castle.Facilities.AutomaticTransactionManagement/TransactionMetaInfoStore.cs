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

namespace Castle.Facilities.AutomaticTransactionManagement
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Reflection;

	using Castle.MicroKernel.Facilities;
	using Castle.Core.Configuration;
	using Castle.Services.Transaction;

	/// <summary>
	/// Pendent
	/// </summary>
	public class TransactionMetaInfoStore : MarshalByRefObject
	{
		private static readonly String TransactionModeAtt = "transactionMode";
		private static readonly String IsolationModeAtt = "isolationLevel";

		private readonly IDictionary type2MetaInfo = new HybridDictionary();

		#region MarshalByRefObject overrides

		public override object InitializeLifetimeService()
		{
			return null;
		}

		#endregion

		public TransactionMetaInfo CreateMetaFromType(Type implementation)
		{
			TransactionMetaInfo metaInfo = new TransactionMetaInfo();

			PopulateMetaInfoFromType(metaInfo, implementation);

			Register(implementation, metaInfo);

			return metaInfo;
		}

		private void PopulateMetaInfoFromType(TransactionMetaInfo metaInfo, Type implementation)
		{
			if (implementation == typeof(object) || implementation == typeof(MarshalByRefObject)) return;

			MethodInfo[] methods = implementation.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

			foreach (MethodInfo method in methods)
			{
				object[] atts = method.GetCustomAttributes(typeof(TransactionAttribute), true);

				if (atts.Length != 0)
				{
					metaInfo.Add(method, atts[0] as TransactionAttribute);
				}
			}

			PopulateMetaInfoFromType(metaInfo, implementation.BaseType);
		}

		public TransactionMetaInfo CreateMetaFromConfig(Type implementation, MethodInfo[] methods, IConfiguration config)
		{
			TransactionMetaInfo metaInfo = GetMetaFor(implementation);

			if (metaInfo == null)
			{
				metaInfo = new TransactionMetaInfo();
			}

			foreach (MethodInfo method in methods)
			{
				String transactionMode = config.Attributes[TransactionModeAtt];
				String isolationLevel = config.Attributes[IsolationModeAtt];

				TransactionMode mode = ObtainTransactionMode(implementation, method, transactionMode);
				IsolationMode level = ObtainIsolation(implementation, method, isolationLevel);

				metaInfo.Add(method, new TransactionAttribute(mode, level));
			}

			Register(implementation, metaInfo);

			return metaInfo;
		}

		public TransactionMetaInfo GetMetaFor(Type implementation)
		{
			return (TransactionMetaInfo)type2MetaInfo[implementation];
		}

		private TransactionMode ObtainTransactionMode(Type implementation, MethodInfo method, string mode)
		{
			if (mode == null)
			{
				return TransactionMode.Unspecified;
			}

			try
			{
				return (TransactionMode)Enum.Parse(typeof(TransactionMode), mode, true);
			}
			catch (Exception)
			{
				String[] values = (String[])Enum.GetValues(typeof(TransactionMode));

				String message = String.Format("The configuration for the class {0}, " +
					"method {1}, has specified {2} on {3} attribute which is not supported. " +
					"The possible values are {4}",
					implementation.FullName, method.Name, mode, TransactionModeAtt, String.Join(", ", values));

				throw new FacilityException(message);
			}
		}

		private IsolationMode ObtainIsolation(Type implementation, MethodInfo method, string level)
		{
			if (level == null)
			{
				return IsolationMode.Unspecified;
			}

			try
			{
				return (IsolationMode)Enum.Parse(typeof(IsolationMode), level, true);
			}
			catch (Exception)
			{
				String[] values = (String[])Enum.GetValues(typeof(TransactionMode));

				String message = String.Format("The configuration for the class {0}, " +
					"method {1}, has specified {2} on {3} attribute which is not supported. " +
					"The possible values are {4}",
					implementation.FullName, method.Name, level, IsolationModeAtt, String.Join(", ", values));

				throw new FacilityException(message);
			}
		}

		private void Register(Type implementation, TransactionMetaInfo metaInfo)
		{
			type2MetaInfo[implementation] = metaInfo;
		}
	}

	public class TransactionMetaInfo : MarshalByRefObject
	{
		private readonly Dictionary<MethodInfo, TransactionAttribute> method2Att;
		private readonly Dictionary<MethodInfo, String> notTransactionalCache;
		private readonly object locker = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionMetaInfo"/> class.
		/// </summary>
		public TransactionMetaInfo()
		{
			method2Att = new Dictionary<MethodInfo, TransactionAttribute>();
			notTransactionalCache = new Dictionary<MethodInfo, String>();
		}

		#region MarshalByRefObject overrides

		public override object InitializeLifetimeService()
		{
			return null;
		}

		#endregion

		public void Add(MethodInfo method, TransactionAttribute attribute)
		{
			method2Att[method] = attribute;
		}

		public MethodInfo[] Methods
		{
			get
			{
				MethodInfo[] methods = new MethodInfo[method2Att.Count];
				method2Att.Keys.CopyTo(methods, 0);
				return methods;
			}
		}

		public bool Contains(MethodInfo info)
		{
			lock(locker)
			{
				if (method2Att.ContainsKey(info)) return true;
				if (notTransactionalCache.ContainsKey(info)) return false;

				if (info.DeclaringType.IsGenericType || info.IsGenericMethod)
				{
					return IsGenericMethodTransactional(info);
				}

				return false;
			}
		}

		public TransactionAttribute GetTransactionAttributeFor(MethodInfo methodInfo)
		{
			return method2Att[methodInfo];
		}

		private bool IsGenericMethodTransactional(MethodInfo info)
		{
			object[] atts = info.GetCustomAttributes(typeof(TransactionAttribute), true);

			if (atts.Length != 0)
			{
				Add(info, atts[0] as TransactionAttribute);
				return true;
			}
			else
			{
				notTransactionalCache[info] = string.Empty;
			}

			return false;
		}
	}
}
