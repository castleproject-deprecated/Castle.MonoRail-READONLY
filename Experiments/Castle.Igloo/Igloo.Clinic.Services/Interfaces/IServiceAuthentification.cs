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

using Igloo.Clinic.Domain;

namespace Igloo.Clinic.Services.Interfaces
{
    public interface IServiceAuthentification
    {
        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <param name="passwd">The passwd.</param>
        /// <returns></returns>
        Doctor Validate(string login, string passwd);

        /// <summary>
        /// Registers a user.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="login">The login.</param>
        /// <param name="passwd">The passwd.</param>
        void Register(string name, string login, string passwd);
    }
}
