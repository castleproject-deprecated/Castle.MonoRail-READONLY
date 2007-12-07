// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.SubSystems.Naming
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Threading;

	/// <summary>
	/// Default <see cref="INamingSubSystem"/> implementation.
	/// Keeps services map as a simple hash table.
	/// Keeps key map as a list dictionary to maintain order.
	/// Does not support a query string.
	/// </summary>
	[Serializable]
	public class DefaultNamingSubSystem : AbstractSubSystem, INamingSubSystem
	{
		/// <summary>
		/// Map(String, IHandler) to map component keys
		/// to <see cref="IHandler"/>
		/// Items in this dictionary are sorted in insertion order.
		/// </summary>
		protected IDictionary key2Handler;

		/// <summary>
		/// Map(Type, IHandler) to map a service
		/// to <see cref="IHandler"/>.
		/// If there is more than a single service of the type, only the first
		/// registered services is stored in this dictionary.
		/// It serve as a fast lookup for the common case of having a single handler for 
		/// a type.
		/// </summary>
		protected IDictionary service2Handler;

		private readonly ReaderWriterLock locker = new ReaderWriterLock();

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultNamingSubSystem"/> class.
		/// </summary>
		public DefaultNamingSubSystem()
		{
			key2Handler = new ListDictionary();
			service2Handler = new Hashtable();
		}

		#region INamingSubSystem Members

		public virtual void Register(String key, IHandler handler)
		{
			Type service = handler.ComponentModel.Service;

			try
			{
				locker.AcquireWriterLock(Timeout.Infinite);
				if (key2Handler.Contains(key))
				{
					throw new ComponentRegistrationException(
						String.Format("There is a component already registered for the given key {0}", key));
				}

				if (!service2Handler.Contains(service))
				{
					this[service] = handler;
				}

				this[key] = handler;
			}
			finally
			{
				locker.ReleaseLock();
			}
		}

		public virtual bool Contains(String key)
		{
			try
			{
				locker.AcquireReaderLock(Timeout.Infinite);
				return key2Handler.Contains(key);
			}
			finally
			{
				locker.ReleaseLock();
			}
		}

		public virtual bool Contains(Type service)
		{
			try
			{
				locker.AcquireReaderLock(Timeout.Infinite);
				return service2Handler.Contains(service);
			}
			finally
			{
				locker.ReleaseLock();
			}
		}

		public virtual void UnRegister(String key)
		{
			try
			{
				locker.AcquireWriterLock(Timeout.Infinite);
				key2Handler.Remove(key);
			}
			finally
			{
				locker.ReleaseLock();
			}
		}

		public virtual void UnRegister(Type service)
		{
			try
			{
				locker.AcquireWriterLock(Timeout.Infinite);
				service2Handler.Remove(service);
			}
			finally
			{
				locker.ReleaseLock();
			}
		}

		public virtual int ComponentCount
		{
			get
			{
				try
				{
					locker.AcquireReaderLock(Timeout.Infinite);
					return key2Handler.Count;
				}
				finally
				{
					locker.ReleaseLock();
				}
			}
		}

		public virtual IHandler GetHandler(String key)
		{
			if (key == null) throw new ArgumentNullException("key");

			try
			{
				locker.AcquireReaderLock(Timeout.Infinite);
				return key2Handler[key] as IHandler;
			}
			finally
			{
				locker.ReleaseLock();
			}
		}

		public virtual IHandler[] GetHandlers(String query)
		{
			throw new NotImplementedException();
		}

		public virtual IHandler GetHandler(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			try
			{
				locker.AcquireReaderLock(Timeout.Infinite);
				IHandler handler = service2Handler[service] as IHandler;

				return handler;
			}
			finally
			{
				locker.ReleaseLock();
			}
		}

		public virtual IHandler GetHandler(String key, Type service)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (service == null) throw new ArgumentNullException("service");

			try
			{
				locker.AcquireReaderLock(Timeout.Infinite);
				IHandler handler = key2Handler[key] as IHandler;

				return handler;
			}
			finally
			{
				locker.ReleaseLock();
			}
		}

		public virtual IHandler[] GetHandlers(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			ArrayList list = new ArrayList();

			foreach(IHandler handler in GetHandlers())
			{
				if (service == handler.ComponentModel.Service)
				{
					list.Add(handler);
				}
			}

			return (IHandler[]) list.ToArray(typeof(IHandler));
		}

		public virtual IHandler[] GetAssignableHandlers(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			ArrayList list = new ArrayList();

			foreach(IHandler handler in GetHandlers())
			{
				if (service.IsAssignableFrom(handler.ComponentModel.Service))
				{
					list.Add(handler);
				}
			}

			return (IHandler[]) list.ToArray(typeof(IHandler));
		}

		public virtual IHandler[] GetHandlers()
		{
			try
			{
				locker.AcquireReaderLock(Timeout.Infinite);
				IHandler[] list = new IHandler[key2Handler.Values.Count];

				int index = 0;

				foreach (IHandler handler in key2Handler.Values)
				{
					list[index++] = handler;
				}

				return list;
			}
			finally
			{
				locker.ReleaseLock();
			}
		}

		public virtual IHandler this[Type service]
		{
			set
			{
				try
				{
					locker.AcquireWriterLock(Timeout.Infinite);
					service2Handler[service] = value;
				}
				finally
				{
					locker.ReleaseLock();
				}
			}
		}

		public virtual IHandler this[String key]
		{
			set
			{
				try
				{
					locker.AcquireWriterLock(Timeout.Infinite);
					key2Handler[key] = value;
				}
				finally
				{
					locker.ReleaseLock();
				}
			}
		}

		public IDictionary GetKey2Handler()
		{
			return key2Handler;
		}

		public IDictionary GetService2Handler()
		{
			return service2Handler;
		}

		#endregion
	}
}
