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

using Castle.Igloo.Attributes;
using Castle.Igloo.Navigation;

namespace Castle.Igloo.Controllers
{
    /// <summary>
    /// Serves as the base class for <see cref="IController"/> implementation.
    /// <see cref="IController"/> are request scope.
    /// </summary>
    [Scope(Scope = ScopeType.Request)]
    public abstract class BaseController : IController
    {
        protected NavigationContext navigationContext = null;
        protected Messages messages = null;

        /// <summary>
        /// Sets the navigation context.
        /// </summary>
        /// <value>The navigation context.</value>
        [Inject(Name=NavigationContext.NAVIGATION_CONTEXT, Scope = ScopeType.Request)]
        public NavigationContext NavigationContext
        {
            set { navigationContext = value; }
        }

        /// <summary>
        /// Sets the request messages.
        /// </summary>
        /// <value>The request messages.</value>
        [Inject(Name = Messages.REQUEST_MESSAGES, Scope = ScopeType.Request)]
        public Messages Messages
        {
            set { messages = value; }
        }
    }
}
