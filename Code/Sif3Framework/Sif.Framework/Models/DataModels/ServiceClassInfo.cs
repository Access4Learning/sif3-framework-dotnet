/*
 * Crown Copyright © Department for Education (UK) 2016
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Sif.Framework.Service;
using System;
using System.Reflection;

namespace Sif.Framework.Model.DataModels
{
    /// <summary>
    /// Encapsulates a constuctor for an IService class.
    /// </summary>
    public class ServiceClassInfo
    {
        private ConstructorInfo constructor = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clazz">The IService class type</param>
        /// <param name="paramTypes">The list of parameter types used to identify a constructor, commonly Type.EmptyTypes</param>
        public ServiceClassInfo(Type clazz, Type[] paramTypes)
        {
            this.constructor = clazz.GetConstructor(paramTypes);
        }

        /// <summary>
        /// Execute the constructor and return the instantiated IService instance.
        /// </summary>
        /// <param name="args">The arguments to pass to the constructor, commonly null.</param>
        /// <returns>See description.</returns>
        public IService GetClassInstance(Object[] args = null)
        {
            return constructor.Invoke(args) as IService;
        }

        /// <summary>
        /// Returns true if this object encapsulates a non-null constructor. False otherwise.
        /// </summary>
        /// <returns>See summary.</returns>
        public Boolean HasConstructor()
        {
            return constructor != null;
        }
    }
}
