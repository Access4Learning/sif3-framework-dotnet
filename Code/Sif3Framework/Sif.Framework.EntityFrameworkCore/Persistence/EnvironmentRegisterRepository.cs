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

using Microsoft.EntityFrameworkCore;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Persistence;
using Tardigrade.Framework.EntityFrameworkCore;
using Tardigrade.Framework.Exceptions;

namespace Sif.Framework.EntityFrameworkCore.Persistence;

/// <inheritdoc cref="IEnvironmentRegisterRepository" />
public class EnvironmentRegisterRepository : Repository<EnvironmentRegister, long>, IEnvironmentRegisterRepository
{
    /// <inheritdoc cref="Repository{TEntity, TKey}" />
    public EnvironmentRegisterRepository(DbContext dbContext) : base(dbContext)
    {
    }

    /// <inheritdoc cref="IEnvironmentRegisterRepository.RetrieveByUniqueIdentifiers" />
    public EnvironmentRegister? RetrieveByUniqueIdentifiers(
        string applicationKey,
        string? instanceId,
        string? userToken,
        string? solutionId)
    {
        if (string.IsNullOrWhiteSpace(applicationKey)) throw new ArgumentNullException(nameof(applicationKey));

        IEnumerable<EnvironmentRegister> registers = Retrieve(e =>
                e.ApplicationKey == applicationKey &&
                e.InstanceId == instanceId &&
                e.UserToken == userToken &&
                e.SolutionId == solutionId)
            .ToList();

        try
        {
            return registers.SingleOrDefault();
        }
        catch (InvalidOperationException e)
        {
            throw new DuplicateFoundException(
                $"Multiple Environment Registers exist with the combination [applicationKey:{applicationKey}|instanceId:{instanceId}|userToken:{userToken}|solutionId:{solutionId}].",
                e);
        }
    }
}