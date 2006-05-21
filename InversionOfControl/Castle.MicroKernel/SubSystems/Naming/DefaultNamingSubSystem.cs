// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

	/// <summary>
	/// Default <see cref="INamingSubSystem"/> implementation.
	/// Keeps services and key maps as simple hash tables. Does not
	/// support a query string.
	/// </summary>
	[Serializable]
	public class DefaultNamingSubSystem : AbstractSubSystem, INamingSubSystem
	{
		/// <summary>
		/// Map(String, IHandler) to map component keys
		/// to <see cref="IHandler"/>
		/// </summary>
		[NonSerialized] 
		protected IDictionary key2Handler;

		/// <summary>
		/// Map(Type, IHandler) to map services 
		/// to <see cref="IHandler"/>
		/// </summary>
		protected IDictionary service2Handler;

		public DefaultNamingSubSystem()
		{
			key2Handler = Hashtable.Synchronized(new Hashtable());
			service2Handler = Hashtable.Synchronized(new Hashtable());
		}

		#region INamingSubSystem Members

		public virtual void Register(String key, IHandler handler)
		{
			Type service = handler.ComponentModel.Service;

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

		public virtual bool Contains(String key)
		{
			return key2Handler.Contains(key);
		}

		public virtual bool Contains(Type service)
		{
			if (service2Handler.Contains(service))
				return true;
// #if DOTNET2
// 			if (service.IsGenericType &&
// 				service2Handler.Contains(service.GetGenericTypeDefinition()))
// 				return true;
// #endif
			return false;
		}

		public virtual void UnRegister(String key)
		{
			key2Handler.Remove(key);
		}

		public virtual void UnRegister(Type service)
		{
			service2Handler.Remove(service);
		}

		public virtual int ComponentCount
		{
			get { return key2Handler.Count; }
		}

		public virtual IHandler GetHandler(String key)
		{
			if (key == null) throw new ArgumentNullException("key");

			return key2Handler[key] as IHandler;
		}

		public virtual IHandler[] GetHandlers(String query)
		{
			return new IHandler[0];
		}

		public virtual IHandler GetHandler(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			IHandler handler = service2Handler[service] as IHandler;
			
// #if DOTNET2
//            if (service.IsGenericType)
//            {
//                handler = service2Handler[service.GetGenericTypeDefinition()] as IHandler;
//                if (handler==null) return null;
//                //Registers a concrete implementation of the generic type in the kernel
//                CloneGenericHandlerForSpesificType(handler, service,null);
//                return GetHandler(service);
//            }
//#endif

			return handler;
		}

        public virtual IHandler GetHandler(String key, Type service)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (service == null) throw new ArgumentNullException("service");

            IHandler handler = key2Handler[key] as IHandler;
            
// #if DOTNET2
//             if (handler.ComponentModel.Service == service)
//                return handler;
//            if (service.IsGenericType)
//            {
//                //Registers a concrete implementation of the generic type in the kernel
//                string genericKey = CloneGenericHandlerForSpesificType(handler, service, key);
//                return GetHandler(genericKey, service);
//            }
//#endif

            return handler;
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

			    //Consider types derived from service as well
//                if (service.IsAssignableFrom(handler.ComponentModel.Service))
//                {
//                    set[handler] = null;
//                    continue;
//                }
// #if DOTNET2
//                 if (service.IsGenericType && !service.IsGenericTypeDefinition)
//                 {
//                     Type genericType = service.GetGenericTypeDefinition();
//                     //Consider types derived from service as well
//                     if(genericType .IsAssignableFrom  (handler.ComponentModel.Service))
//                     {
//                         //this will create the handler or retrieve the generic version
//                         IHandler genericHandler = GetHandler(genericType);
//                         if(genericHandler!=null)
//                             set[genericHandler] = null;
//                     }
//                 }
// #endif
            }
            
			// return (IHandler[]) new ArrayList(set.Keys).ToArray(typeof(IHandler));
			
			return (IHandler[])list.ToArray(typeof(IHandler));
		}

		public virtual IHandler[] GetAssignableHandlers(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			ArrayList list = new ArrayList();

			foreach(IHandler handler in this.GetHandlers())
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
			IHandler[] list = new IHandler[key2Handler.Values.Count];

			int index = 0;

			foreach(IHandler handler in key2Handler.Values)
			{
				list[index++] = handler;
			}

			return list;
		}

		public virtual IHandler this[Type service]
		{
			set { service2Handler[service] = value; }
		}

		public virtual IHandler this[String key]
		{
			set { key2Handler[key] = value; }
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

//#if DOTNET2
//        private string CloneGenericHandlerForSpesificType(IHandler handler, Type genericService, String key)
//       {
//            string guessedKey = key ?? GetKeyByHandler(handler);
//            String genericKey = GenericTypeNameProvider.AppendGenericTypeName(handler, genericService, guessedKey);
//            //Get the correct type from the context...
//            Type genericClass = handler.ComponentModel.Implementation;
//            Kernel.AddComponent(genericKey, genericService, genericClass);
//            return genericKey;
//        }
//
//        private String GetKeyByHandler(IHandler handler)
//        {
//            String key = null;
//            foreach (DictionaryEntry entry in key2Handler)
//            {
//                if (entry.Value == handler)
//                {
//                    key = entry.Key.ToString();
//                    break;
//                }
//            }
//            return key;
//        }
//#endif
    }
}
