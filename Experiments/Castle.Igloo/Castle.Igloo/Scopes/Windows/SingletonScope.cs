#region Apache Notice
/*****************************************************************************
 * 
 * Castle.Igloo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

using System.Collections;
using System.Collections.Generic;
using Castle.Core;
using Castle.Igloo.Scopes;
using Castle.MicroKernel;

namespace Castle.Igloo.Contexts.Windows
{
    public class SingletonScope : IScope
    {
        private IDictionary<string, object> _map = null;
        
        public SingletonScope()
        {
            _map = new Dictionary<string, object>();
        }
        
        #region IScope Members

        /// <summary>
        /// Gets a value indicating whether this context is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the <see cref="object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get 
            {
                if (_map.ContainsKey(name))
                {
                    return _map[name];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the IScope object.
        /// </summary>
        /// <param name="name">The name of the element to add.</param>
        /// <param name="instance">The Object to use as the value of the element to add.</param>
        public void Add(string name, object instance)
        {
            _map[name] = instance;
        }

        /// <summary>
        /// Removes the element with the specified name from the IScope object.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        public void Remove(string name)
        {
            _map.Remove(name);
        }

        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name to locate in the IScope object.</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return _map.Keys.Contains(name);
        }


        /// <summary>
        /// Gets All the objects names contain in the IScope object.
        /// </summary>
        /// <value>The names.</value>
        public ICollection Names
        {
            get { return (ICollection)_map.Keys; }
        }

        /// <summary>
        /// Removes all the elements from the IScope object.
        /// </summary>
        public void Flush()
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public string ScopeType
        {
            get { return Igloo.ScopeType.Singleton; }
        }


        public void RegisterForEviction(ILifestyleManager manager, ComponentModel model, object instance)
        {
        }

        public void CheckInitialisation()
        {
        }

        #endregion
    }
}
