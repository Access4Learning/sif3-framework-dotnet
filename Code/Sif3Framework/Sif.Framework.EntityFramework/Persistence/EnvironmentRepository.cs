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

using Sif.Framework.Persistence;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Tardigrade.Framework.EntityFramework;
using Tardigrade.Framework.Exceptions;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.EntityFramework.Persistence
{
    /// <inheritdoc cref="IEnvironmentRepository" />
    public class EnvironmentRepository : Repository<Environment, Guid>, IEnvironmentRepository
    {
        /// <inheritdoc cref="Repository{TEntity, TKey}" />
        public EnvironmentRepository(DbContext dbContext) : base(dbContext)
        {
        }

        /// <inheritdoc cref="IEnvironmentRepository.RetrieveBySessionToken(string)" />
        public Environment RetrieveBySessionToken(string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken)) throw new ArgumentNullException(nameof(sessionToken));

            IEnumerable<Environment> registers = Retrieve(a => a.SessionToken == sessionToken).ToList();

            try
            {
                return registers.SingleOrDefault();
            }
            catch (InvalidOperationException e)
            {
                throw new DuplicateFoundException($"Multiple Environments exist with session token {sessionToken}.", e);
            }
        }
    }
}