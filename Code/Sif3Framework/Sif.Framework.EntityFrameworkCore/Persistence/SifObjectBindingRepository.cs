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
using Sif.Framework.Models.Infrastructure;
using Sif.Framework.Persistence;
using Tardigrade.Framework.EntityFrameworkCore;

namespace Sif.Framework.EntityFrameworkCore.Persistence;

/// <inheritdoc cref="ISifObjectBindingRepository" />
public class SifObjectBindingRepository : Repository<SifObjectBinding, long>, ISifObjectBindingRepository
{
    /// <inheritdoc cref="Repository{TEntity, TKey}" />
    public SifObjectBindingRepository(DbContext dbContext) : base(dbContext)
    {
    }

    /// <inheritdoc cref="ISifObjectBindingRepository.RetrieveByBinding" />
    public IEnumerable<SifObjectBinding> RetrieveByBinding(Guid refId, string ownerId)
    {
        if (string.IsNullOrWhiteSpace(ownerId)) throw new ArgumentNullException(nameof(ownerId));

        return Retrieve(s => s.RefId == refId && s.OwnerId == ownerId);
    }

    /// <inheritdoc cref="ISifObjectBindingRepository.RetrieveByRefId" />
    public IEnumerable<SifObjectBinding> RetrieveByRefId(Guid refId)
    {
        return Retrieve(s => s.RefId == refId);
    }
}