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
using Sif.Framework.Persistence;
using System;
using System.Collections.Generic;
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.Service
{
    public class GenericService<TEntity, TKey> : IGenericService<TEntity, TKey>
        where TEntity : IHasUniqueIdentifier<TKey>, new()
    {
        protected IGenericRepository<TEntity, TKey> Repository;

        public GenericService(IGenericRepository<TEntity, TKey> repository)
        {
            Repository = repository;
        }

        public virtual TKey Create(TEntity obj)
        {
            return Repository.Save(obj);
        }

        public virtual void Create(IEnumerable<TEntity> objs)
        {
            Repository.Save(objs);
        }

        public virtual void Delete(TKey id)
        {
            Repository.Delete(id);
        }

        public virtual void Delete(TEntity obj)
        {
            Repository.Delete(obj);
        }

        public virtual void Delete(IEnumerable<TEntity> objs)
        {
            Repository.Delete(objs);
        }

        public virtual TEntity Retrieve(TKey id)
        {
            return Repository.Retrieve(id);
        }

        public virtual ICollection<TEntity> Retrieve()
        {
            return Repository.Retrieve();
        }

        public virtual ICollection<TEntity> Retrieve(TEntity obj)
        {
            return Repository.Retrieve(obj);
        }

        public virtual ICollection<TEntity> Retrieve(int pageIndex, int pageSize)
        {
            return Repository.Retrieve(pageIndex, pageSize);
        }

        public virtual ICollection<TEntity> Retrieve(IEnumerable<EqualCondition> conditions)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(TEntity obj)
        {
            Repository.Save(obj);
        }

        public virtual void Update(IEnumerable<TEntity> objs)
        {
            Repository.Save(objs);
        }
    }
}