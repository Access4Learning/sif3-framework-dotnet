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
using Tardigrade.Framework.Persistence;
using Environment = Sif.Framework.Models.Infrastructure.Environment;

namespace Sif.Framework.Persistence
{
    /// <summary>
    /// Repository interface for operations associated with the Environment object.
    /// </summary>
    public interface IEnvironmentRepository : IRepository<Environment, Guid>
    {
        /// <summary>
        /// Retrieve the Environment based upon it's session token.
        /// </summary>
        /// <param name="sessionToken">Session token for the Environment.</param>
        /// <returns>Environment defined by the passed session token if exists; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">Parameter is null or empty.</exception>
        /// <exception cref="Tardigrade.Framework.Exceptions.DuplicateFoundException">Multiple Environments are associated with the session token.</exception>
        /// <exception cref="Tardigrade.Framework.Exceptions.RepositoryException">A general repository failure occurred.</exception>
        Environment RetrieveBySessionToken(string sessionToken);
    }
}