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

using System;
using System.Collections;
using System.Diagnostics;
using Castle.Igloo.Contexts;
using Castle.Igloo.Util;

namespace Castle.Igloo.Contexts.Web
{
    /// <summary>
    /// Represents a session context, acts as a proxy to the <see cref="ISessionScope"/>.
    /// </summary>
    //[Scope(Scope = ScopeType.Application)]
    public sealed class WebSessionScope : ISessionScope
    {
        public const string SESSION_INVALID = "_SESSION_INVALID_";

        #region ISessionContext Members

        /// <summary>
        /// Gets a value indicating whether this context is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return WebUtil.GetCurrentHttpContext().Session != null; }
        }
        
        /// <summary>
        /// Gets the <see cref="Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get { return WebUtil.GetCurrentHttpContext().Session[name]; }
        }

        /// <summary>
        /// Gets the <see cref="Object"/> with the specified type.
        /// </summary>
        /// <value></value>
        public object this[Type clazz]
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the IScope object.
        /// </summary>
        /// <param name="name">The name of the element to add.</param>
        /// <param name="value">The Object to use as the value of the element to add.</param>
        public void Add(string name, object value)
        {
            Trace.WriteLine("Add to session Context : " + name);

            WebUtil.GetCurrentHttpContext().Session.Add(name, value);
        }

        /// <summary>
        /// Removes the element with the specified name from the IScope object.
        /// </summary>
        /// <param name="name">The name of the element to remove.</param>
        public void Remove(string name)
        {
            Trace.WriteLine("Remove from session Context : " + name);

            WebUtil.GetCurrentHttpContext().Session.Remove(name);
        }

        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name to locate in the IScope object.</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            IEnumerator enumerator = WebUtil.GetCurrentHttpContext().Session.Keys.GetEnumerator();
            while ( enumerator.MoveNext() )
            {
                 string key = (string) enumerator.Current;
                if (key==name)
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Gets All the objects names contain in the IScope object.
        /// </summary>
        /// <value>The names.</value>
        public ICollection Names
        {
            get {  return WebUtil.GetCurrentHttpContext().Session.Keys; }
        }

        /// <summary>
        /// Removes all the element from the IScope object.
        /// </summary>
        public void Flush()
        {
            Trace.WriteLine("Flush session Context.");

            WebUtil.GetCurrentHttpContext().Session.Clear();
        }

        /// <summary>
        /// Abandons the current session.
        /// </summary>
        public void Abandon()
        {
            // Mark Session as invalid
            Add(SESSION_INVALID, true);
        }

        /// <summary>
        /// Gets the type of the scope.
        /// </summary>
        /// <value>The type of the scope.</value>
        public string ScopeType
        {
            get { return Igloo.ScopeType.Session; }
        }

        #endregion
    }
}