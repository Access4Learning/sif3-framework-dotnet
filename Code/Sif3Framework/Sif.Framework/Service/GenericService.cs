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

using Sif.Framework.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Models.Persistence;
using Tardigrade.Framework.Persistence;

namespace Sif.Framework.Service
{
    public class GenericService<TEntity, TKey> : IGenericService<TEntity, TKey>
        where TEntity : IHasUniqueIdentifier<TKey>, new()
    {
        protected IRepository<TEntity, TKey> Repository;

        public GenericService(IRepository<TEntity, TKey> repository)
        {
            Repository = repository;
        }

        public virtual TKey Create(TEntity item)
        {
            return Repository.Create(item).Id;
        }

        public virtual void Create(IEnumerable<TEntity> items)
        {
            foreach (TEntity item in items.OrEmptyIfNull())
            {
                Repository.Create(item);
            }
        }

        public virtual void Delete(TKey id)
        {
            TEntity item = Retrieve(id);
            Repository.Delete(item);
        }

        public virtual void Delete(TEntity item)
        {
            Repository.Delete(item);
        }

        public virtual void Delete(IEnumerable<TEntity> items)
        {
            foreach (TEntity item in items.OrEmptyIfNull())
            {
                Repository.Delete(item);
            }
        }

        public virtual TEntity Retrieve(TKey id)
        {
            return Repository.Retrieve(id);
        }

        public virtual ICollection<TEntity> Retrieve()
        {
            return Repository.Retrieve().ToList();
        }

        public virtual ICollection<TEntity> Retrieve(TEntity item)
        {
            throw new NotImplementedException();
        }

        public virtual ICollection<TEntity> Retrieve(int pageIndex, int pageSize)
        {
            var pagingContext = new PagingContext
            {
                PageIndex = (uint)pageIndex,
                PageSize = (uint)pageSize
            };

            return Repository.Retrieve(pagingContext: pagingContext).ToList();
        }

        public virtual ICollection<TEntity> Retrieve(IEnumerable<EqualCondition> conditions)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(TEntity item)
        {
            Repository.Update(item);
        }

        public virtual void Update(IEnumerable<TEntity> items)
        {
            foreach (TEntity item in items.OrEmptyIfNull())
            {
                Repository.Update(item);
            }
        }
    }
}