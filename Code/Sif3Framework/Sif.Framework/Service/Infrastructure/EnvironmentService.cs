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
using Sif.Framework.Service.Mapper;
using Sif.Specification.Infrastructure;
using System;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Services;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Infrastructure
{
    /// <summary>
    /// Service class for Environment objects.
    /// </summary>
    public class EnvironmentService : ObjectService<Environment, Guid>, IEnvironmentService
    {
        /// <inheritdoc cref="ObjectService{TEntity, TKey}" />
        public EnvironmentService(IRepository<Environment, Guid> repository) : base(repository)
        {
        }

        /// <inheritdoc cref="IEnvironmentService.RetrieveBySessionToken(string)" />
        public virtual environmentType RetrieveBySessionToken(string sessionToken)
        {
            Environment environment = ((IEnvironmentRepository)Repository).RetrieveBySessionToken(sessionToken);

            return MapperFactory.CreateInstance<Environment, environmentType>(environment);
        }
    }
}