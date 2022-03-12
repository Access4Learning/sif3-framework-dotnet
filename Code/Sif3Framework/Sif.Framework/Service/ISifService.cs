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
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.Service
{
    /// <summary>
    /// Service interface for CRUD operations based on Data Transfer Objects (DTOs).
    /// </summary>
    /// <typeparam name="TDto">DTO used by the presentation layer.</typeparam>
    /// <typeparam name="TEntity">Model object used by the persistence layer.</typeparam>
    public interface ISifService<TDto, TEntity> : IService where TEntity : IHasUniqueIdentifier<Guid>
    {
        /// <summary>
        /// Create a single object.
        /// </summary>
        /// <param name="item">Object to create.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>Unique identifier for the created object.</returns>
        Guid Create(TDto item, string zoneId = null, string contextId = null);

        /// <summary>
        /// Create multiple objects.
        /// </summary>
        /// <param name="items">Objects to create.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        void Create(IEnumerable<TDto> items, string zoneId = null, string contextId = null);

        /// <summary>
        /// Delete an object.
        /// </summary>
        /// <param name="id">Unique identifier for the object to delete.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        void Delete(Guid id, string zoneId = null, string contextId = null);

        /// <summary>
        /// Delete an object.
        /// </summary>
        /// <param name="item">Object to delete.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        void Delete(TDto item, string zoneId = null, string contextId = null);

        /// <summary>
        /// Delete multiple objects.
        /// </summary>
        /// <param name="items">Objects to delete.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        void Delete(IEnumerable<TDto> items, string zoneId = null, string contextId = null);

        /// <summary>
        /// Retrieve an object.
        /// </summary>
        /// <param name="id">Unique identifier for the object to retrieve.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>Object retrieved.</returns>
        TDto Retrieve(Guid id, string zoneId = null, string contextId = null);

        /// <summary>
        /// Retrieve objects based upon an example (filter) object.
        /// </summary>
        /// <param name="item">Example (filter) object.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>Objects matching the example (filter) object.</returns>
        ICollection<TDto> Retrieve(TDto item, string zoneId = null, string contextId = null);

        /// <summary>
        /// Retrieve all objects.
        /// </summary>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>All objects.</returns>
        ICollection<TDto> Retrieve(string zoneId = null, string contextId = null);

        /// <summary>
        /// Update an object.
        /// </summary>
        /// <param name="item">Object to update.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        void Update(TDto item, string zoneId = null, string contextId = null);

        /// <summary>
        /// Update multiple objects.
        /// </summary>
        /// <param name="items">Objects to update.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        void Update(IEnumerable<TDto> items, string zoneId = null, string contextId = null);
    }
}