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
using System;
using System.Collections.Generic;
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.Service
{
    /// <inheritdoc />
    public abstract class SifService<TDto, TEntity> : ISifService<TDto, TEntity>
        where TEntity : IHasUniqueIdentifier<Guid>, new()
    {
        /// <summary>
        /// Generic repository.
        /// </summary>
        protected IGenericRepository<TEntity, Guid> Repository;

        /// <summary>
        /// Create an instance based upon the provided repository.
        /// </summary>
        /// <param name="repository">Repository associated with the service.</param>
        protected SifService(IGenericRepository<TEntity, Guid> repository)
        {
            Repository = repository;
        }

        /// <inheritdoc cref="ISifService{TDto, TEntity}.Create(TDto, string, string)" />
        public virtual Guid Create(TDto item, string zoneId = null, string contextId = null)
        {
            TEntity repoItem = MapperFactory.CreateInstance<TDto, TEntity>(item);

            return Repository.Save(repoItem);
        }

        /// <inheritdoc cref="ISifService{TDto, TEntity}.Create(IEnumerable{TDto}, string, string)" />
        public virtual void Create(IEnumerable<TDto> items, string zoneId = null, string contextId = null)
        {
            ICollection<TEntity> repoItems = MapperFactory.CreateInstances<TDto, TEntity>(items);
            Repository.Save(repoItems);
        }

        /// <inheritdoc cref="ISifService{TDto, TEntity}.Delete(Guid, string, string)" />
        public virtual void Delete(Guid id, string zoneId = null, string contextId = null)
        {
            Repository.Delete(id);
        }

        /// <inheritdoc cref="ISifService{TDto, TEntity}.Delete(TDto, string, string)" />
        public virtual void Delete(TDto item, string zoneId = null, string contextId = null)
        {
            TEntity repoItem = MapperFactory.CreateInstance<TDto, TEntity>(item);
            Repository.Delete(repoItem);
        }

        /// <inheritdoc cref="ISifService{TDto, TEntity}.Delete(IEnumerable{TDto}, string, string)" />
        public virtual void Delete(IEnumerable<TDto> items, string zoneId = null, string contextId = null)
        {
            ICollection<TEntity> repoItems = MapperFactory.CreateInstances<TDto, TEntity>(items);
            Repository.Delete(repoItems);
        }

        /// <inheritdoc cref="ISifService{TDto, TEntity}.Retrieve(Guid, string, string)" />
        public virtual TDto Retrieve(Guid id, string zoneId = null, string contextId = null)
        {
            TEntity repoItem = Repository.Retrieve(id);

            return MapperFactory.CreateInstance<TEntity, TDto>(repoItem);
        }

        /// <inheritdoc cref="ISifService{TDto, TEntity}.Retrieve(TDto, string, string)" />
        public virtual ICollection<TDto> Retrieve(TDto item, string zoneId = null, string contextId = null)
        {
            TEntity repoItem = MapperFactory.CreateInstance<TDto, TEntity>(item);
            ICollection<TEntity> repoItems = Repository.Retrieve(repoItem);

            return MapperFactory.CreateInstances<TEntity, TDto>(repoItems);
        }

        /// <inheritdoc cref="ISifService{TDto, TEntity}.Retrieve(string, string)" />
        public virtual ICollection<TDto> Retrieve(string zoneId = null, string contextId = null)
        {
            ICollection<TEntity> repoItems = Repository.Retrieve();

            return MapperFactory.CreateInstances<TEntity, TDto>(repoItems);
        }

        /// <inheritdoc cref="ISifService{TDto, TEntity}.Update(TDto, string, string)" />
        public virtual void Update(TDto item, string zoneId = null, string contextId = null)
        {
            TEntity repoItem = MapperFactory.CreateInstance<TDto, TEntity>(item);
            Repository.Save(repoItem);
        }

        /// <inheritdoc cref="ISifService{TDto, TEntity}.Update(IEnumerable{TDto}, string, string)" />
        public virtual void Update(IEnumerable<TDto> items, string zoneId = null, string contextId = null)
        {
            ICollection<TEntity> repoItems = MapperFactory.CreateInstances<TDto, TEntity>(items);
            Repository.Save(repoItems);
        }
    }
}