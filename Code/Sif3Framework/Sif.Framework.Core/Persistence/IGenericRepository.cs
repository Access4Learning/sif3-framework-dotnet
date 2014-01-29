/*
 * Copyright 2014 Systemic Pty Ltd
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

using Sif.Framework.Model.Persistence;
using System.Collections.Generic;

namespace Sif.Framework.Persistence
{

    public interface IGenericRepository<T> where T : IPersistable
    {

        /// <summary>
        /// Delete the object.
        /// </summary>
        /// <param name="obj">Object to delete.</param>
        /// <exception cref="System.ArgumentNullException">obj parameter is null.</exception>
        void Delete(T obj);

        /// <summary>
        /// Delete multiple object.
        /// </summary>
        /// <param name="objs">Objects delete.</param>
        /// <exception cref="System.ArgumentNullException">obj parameter is null.</exception>
        void Delete(ICollection<T> objs);

        /// <summary>
        /// Retrieve the object based upon it's unique identifier.
        /// </summary>
        /// <param name="objId">Unique identifier for the object.</param>
        /// <returns>Object defined by the passed unique identifier.</returns>
        T Retrieve(long objId);

        /// <summary>
        /// Retrieve all objects based upon the example instance.
        /// </summary>
        /// <returns>All objects that match the example instance.</returns>
        ICollection<T> Retrieve(T obj);

        /// <summary>
        /// Retrieve all objects.
        /// </summary>
        /// <returns>All objects.</returns>
        ICollection<T> Retrieve();

        /// <summary>
        /// Save the object.
        /// </summary>
        /// <param name="obj">Object to save.</param>
        /// <returns>Unique identifier for the object.</returns>
        /// <exception cref="System.ArgumentNullException">obj parameter is null.</exception>
        long Save(T obj);

        /// <summary>
        /// Save multiple objects.
        /// </summary>
        /// <param name="objs">Objects to save.</param>
        /// <exception cref="System.ArgumentNullException">obj parameter is null.</exception>
        void Save(ICollection<T> objs);

    }

}
