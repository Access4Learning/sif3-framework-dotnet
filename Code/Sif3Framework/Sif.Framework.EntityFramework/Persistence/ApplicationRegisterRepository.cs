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
using Sif.Framework.Persistence;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Tardigrade.Framework.EntityFramework;
using Tardigrade.Framework.Exceptions;

namespace Sif.Framework.EntityFramework.Persistence
{
    /// <inheritdoc cref="IApplicationRegisterRepository" />
    public class ApplicationRegisterRepository : Repository<ApplicationRegister, long>, IApplicationRegisterRepository
    {
        /// <inheritdoc cref="Repository{TEntity, TKey}" />
        public ApplicationRegisterRepository(DbContext dbContext) : base(dbContext)
        {
        }

        /// <inheritdoc cref="IApplicationRegisterRepository.RetrieveByApplicationKey(string)" />
        public ApplicationRegister RetrieveByApplicationKey(string applicationKey)
        {
            if (string.IsNullOrWhiteSpace(applicationKey)) throw new ArgumentNullException(nameof(applicationKey));

            IEnumerable<ApplicationRegister> registers = Retrieve(a => a.ApplicationKey == applicationKey).ToList();

            try
            {
                return registers.SingleOrDefault();
            }
            catch (InvalidOperationException e)
            {
                throw new DuplicateFoundException(
                    $"Multiple Application Registers are associated with application key {applicationKey}.",
                    e);
            }
        }
    }
}