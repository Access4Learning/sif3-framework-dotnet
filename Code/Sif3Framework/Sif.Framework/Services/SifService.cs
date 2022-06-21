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

using Sif.Framework.Service.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Persistence;

namespace Sif.Framework.Service
{
    /// <inheritdoc />
    public abstract class SifService<TDto, TEntity> : ISifService<TDto>
        where TEntity : IHasUniqueIdentifier<Guid>, new()
    {
        /// <summary>
        /// Generic repository.
        /// </summary>
        protected IRepository<TEntity, Guid> Repository;

        /// <summary>
        /// Create an instance based upon the provided repository.
        /// </summary>
        /// <param name="repository">Repository associated with the service.</param>
        protected SifService(IRepository<TEntity, Guid> repository)
        {
            Repository = repository;
        }

        /// <inheritdoc cref="ISifService{TDto}.Create(TDto)" />
        public virtual Guid Create(TDto item)
        {
            TEntity model = MapperFactory.CreateInstance<TDto, TEntity>(item);

            return Repository.Create(model).Id;
        }

        /// <inheritdoc cref="ISifService{TDto}.Create(IEnumerable{TDto})" />
        public virtual void Create(IEnumerable<TDto> items)
        {
            ICollection<TEntity> models = MapperFactory.CreateInstances<TDto, TEntity>(items);

            foreach (TEntity model in models)
            {
                Repository.Create(model);
            }
        }

        /// <inheritdoc cref="ISifService{TDto}.Delete(Guid)" />
        public virtual void Delete(Guid id)
        {
            TEntity model = Repository.Retrieve(id);
            Repository.Delete(model);
        }

        /// <inheritdoc cref="ISifService{TDto}.Delete(TDto)" />
        public virtual void Delete(TDto item)
        {
            TEntity model = MapperFactory.CreateInstance<TDto, TEntity>(item);
            Repository.Delete(model);
        }

        /// <inheritdoc cref="ISifService{TDto}.Delete(IEnumerable{TDto})" />
        public virtual void Delete(IEnumerable<TDto> items)
        {
            ICollection<TEntity> models = MapperFactory.CreateInstances<TDto, TEntity>(items);

            foreach (TEntity model in models)
            {
                Repository.Delete(model);
            }
        }

        /// <inheritdoc cref="ISifService{TDto}.Retrieve(Guid)" />
        public virtual TDto Retrieve(Guid id)
        {
            TEntity model = Repository.Retrieve(id);

            return MapperFactory.CreateInstance<TEntity, TDto>(model);
        }

        /// <inheritdoc cref="ISifService{TDto}.Retrieve(TDto)" />
        public virtual ICollection<TDto> Retrieve(TDto item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="ISifService{TDto}.Retrieve()" />
        public virtual ICollection<TDto> Retrieve()
        {
            ICollection<TEntity> models = Repository.Retrieve().ToList();

            return MapperFactory.CreateInstances<TEntity, TDto>(models);
        }

        /// <inheritdoc cref="ISifService{TDto}.Update(TDto)" />
        public virtual void Update(TDto item)
        {
            TEntity model = MapperFactory.CreateInstance<TDto, TEntity>(item);
            Repository.Update(model);
        }

        /// <inheritdoc cref="ISifService{TDto}.Update(IEnumerable{TDto})" />
        public virtual void Update(IEnumerable<TDto> items)
        {
            ICollection<TEntity> models = MapperFactory.CreateInstances<TDto, TEntity>(items);

            foreach (TEntity model in models)
            {
                Repository.Update(model);
            }
        }
    }
}