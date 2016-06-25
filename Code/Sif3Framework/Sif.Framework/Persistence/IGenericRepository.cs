/*
 * Copyright 2015 Systemic Pty Ltd
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

    /// <summary>
    /// This interface defines CRUD operations associated with persistence to a repository.
    /// </summary>
    /// <typeparam name="T">Type of model object to be persisted.</typeparam>
    /// <typeparam name="PK">Primary key type of the model object.</typeparam>
    public interface IGenericRepository<T, PK> where T : IPersistable<PK>
    {

        /// <summary>
        /// Delete the object using it's unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the object.</param>
        void Delete(PK id);

        /// <summary>
        /// Delete the object.
        /// </summary>
        /// <param name="obj">Object to delete.</param>
        /// <exception cref="System.ArgumentNullException">obj parameter is null.</exception>
        void Delete(T obj);

        /// <summary>
        /// Delete multiple objects.
        /// </summary>
        /// <param name="objs">Objects delete.</param>
        /// <exception cref="System.ArgumentNullException">objs parameter is null.</exception>
        void Delete(IEnumerable<T> objs);

        /// <summary>
        /// Retrieve the object based upon it's unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the object.</param>
        /// <returns>Object defined by the passed unique identifier.</returns>
        T Retrieve(PK id);

        /// <summary>
        /// Retrieve all objects.
        /// </summary>
        /// <returns>All objects.</returns>
        ICollection<T> Retrieve();

        /// <summary>
        /// Retrieve all objects based upon the example instance.
        /// </summary>
        /// <param name="obj">Example instance to match on.</param>
        /// <returns>All objects that match the example instance.</returns>
        ICollection<T> Retrieve(T obj);

        /// <summary>
        /// Retrieve a range of objects.
        /// </summary>
        /// <param name="pageIndex">The "page" index for the next set of objects to retrieve (starts at 0).</param>
        /// <param name="pageSize">The number of objects to retrieve for the "page".</param>
        /// <returns>All objects.</returns>
        ICollection<T> Retrieve(int pageIndex, int pageSize);

        /// <summary>
        /// Save the object.
        /// </summary>
        /// <param name="obj">Object to save.</param>
        /// <returns>Unique identifier for the object.</returns>
        /// <exception cref="System.ArgumentNullException">obj parameter is null.</exception>
        PK Save(T obj);

        /// <summary>
        /// Save multiple objects.
        /// </summary>
        /// <param name="objs">Objects to save.</param>
        /// <exception cref="System.ArgumentNullException">obj parameter is null.</exception>
        void Save(IEnumerable<T> objs);

        /// <summary>
        /// Check if an id exists.
        /// </summary>
        /// <param name="id">Id to check.</param>
        /// <returns>True if the id exists in the repository, false otherwise.</returns>
        bool Exists(PK id);
    }

}
