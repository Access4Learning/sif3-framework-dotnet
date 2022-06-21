/*
 * Copyright 2022 Systemic Pty Ltd
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

using System;
using System.Collections.Generic;

namespace Sif.Framework.Service
{
    /// <summary>
    /// Service interface for CRUD operations based on Data Transfer Objects (DTOs).
    /// </summary>
    /// <typeparam name="TDto">DTO used by the presentation layer.</typeparam>
    public interface ISifService<TDto> : IService
    {
        /// <summary>
        /// Create a single object.
        /// </summary>
        /// <param name="item">Object to create.</param>
        /// <returns>Unique identifier for the created object.</returns>
        Guid Create(TDto item);

        /// <summary>
        /// Create multiple objects.
        /// </summary>
        /// <param name="items">Objects to create.</param>
        void Create(IEnumerable<TDto> items);

        /// <summary>
        /// Delete an object.
        /// </summary>
        /// <param name="id">Unique identifier for the object to delete.</param>
        void Delete(Guid id);

        /// <summary>
        /// Delete an object.
        /// </summary>
        /// <param name="item">Object to delete.</param>
        void Delete(TDto item);

        /// <summary>
        /// Delete multiple objects.
        /// </summary>
        /// <param name="items">Objects to delete.</param>
        void Delete(IEnumerable<TDto> items);

        /// <summary>
        /// Retrieve an object.
        /// </summary>
        /// <param name="id">Unique identifier for the object to retrieve.</param>
        /// <returns>Object retrieved.</returns>
        TDto Retrieve(Guid id);

        /// <summary>
        /// Retrieve objects based upon an example (filter) object.
        /// </summary>
        /// <param name="item">Example (filter) object.</param>
        /// <returns>Objects matching the example (filter) object.</returns>
        ICollection<TDto> Retrieve(TDto item);

        /// <summary>
        /// Retrieve all objects.
        /// </summary>
        /// <returns>All objects.</returns>
        ICollection<TDto> Retrieve();

        /// <summary>
        /// Update an object.
        /// </summary>
        /// <param name="item">Object to update.</param>
        void Update(TDto item);

        /// <summary>
        /// Update multiple objects.
        /// </summary>
        /// <param name="items">Objects to update.</param>
        void Update(IEnumerable<TDto> items);
    }
}