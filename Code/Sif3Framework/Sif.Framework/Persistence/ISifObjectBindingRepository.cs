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

using Sif.Framework.Model.Infrastructure;
using System;
using System.Collections.Generic;
using Tardigrade.Framework.Persistence;

namespace Sif.Framework.Persistence
{
    /// <summary>
    /// Repository interface for operations associated with the SifObjectBinding object.
    /// </summary>
    public interface ISifObjectBindingRepository : IRepository<SifObjectBinding, long>
    {
        /// <summary>
        /// Retrieve SIF object bindings based upon their SIF reference and owner identifiers.
        /// </summary>
        /// <param name="refId">SIF reference identifier.</param>
        /// <param name="ownerId">Owner identifier.</param>
        /// <returns>SIF object bindings defined by the passed SIF reference and owner identifiers if exists; empty collection otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">ownerId is null or empty.</exception>
        /// <exception cref="Tardigrade.Framework.Exceptions.RepositoryException">A general repository failure occurred.</exception>
        IEnumerable<SifObjectBinding> RetrieveByBinding(Guid refId, string ownerId);

        /// <summary>
        /// Retrieve SIF object bindings based upon their SIF reference and owner identifiers.
        /// </summary>
        /// <param name="refId">SIF reference identifier.</param>
        /// <returns>SIF object bindings defined by the passed SIF reference and owner identifiers if exists; empty collection otherwise.</returns>
        /// <exception cref="Tardigrade.Framework.Exceptions.RepositoryException">A general repository failure occurred.</exception>
        IEnumerable<SifObjectBinding> RetrieveByRefId(Guid refId);
    }
}