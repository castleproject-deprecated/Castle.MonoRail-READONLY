// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Internal
{
    using System;
    using Castle.MonoRail.Framework.Helpers;

    /// <summary>
    /// Abstract class that contains the shared logic of JS Generations, seperate from
    /// the various view engine implementations
    /// </summary>
    public abstract class JSGeneratorBase
    {
        protected readonly PrototypeHelper.JSGenerator generator;

        protected JSGeneratorBase(PrototypeHelper.JSGenerator generator)
        {
            this.generator = generator;
        }

        protected object InternalInvoke(string method, params object[] args)
        {
            if (method == "el")
            {
                if (args == null || args.Length != 1)
                {
                    throw new ArgumentException("el() method must be invoked with the element name as an argument");
                }
                if (args[0] == null)
                {
                    throw new ArgumentNullException("el() method invoked with a null argument");
                }

                return CreateJSElementGenerator(
                    new PrototypeHelper.JSElementGenerator(generator, args[0].ToString()));
            }
            else if (method == "select")
            {
                if (args == null || args.Length != 1)
                {
                    throw new ArgumentException(
                        "select() method must be invoked with the element/css selection rule name as an argument");
                }
                if (args[0] == null)
                {
                    throw new ArgumentNullException("select() method invoked with a null argument");
                }

                return CreateJSCollectionGenerator(
                    new PrototypeHelper.JSCollectionGenerator(generator, args[0].ToString()));
            }

            if (generator.IsGeneratorMethod(method))
            {
                generator.Dispatch(method, args);
            }

            return CreateNullGenerator();
        }

        protected abstract object CreateNullGenerator();

        protected abstract object CreateJSCollectionGenerator(PrototypeHelper.JSCollectionGenerator collectionGenerator);

        protected abstract object CreateJSElementGenerator(PrototypeHelper.JSElementGenerator elementGenerator);

        /// <summary>
        /// Delegates to the generator
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return generator.ToString();
        }
    }
}