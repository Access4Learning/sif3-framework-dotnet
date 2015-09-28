/*
 * Copyright 2015 Systemic Pty Ltd
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

using Sif.Framework.Model.Persistence;
using Sif.Framework.Model.Query;
using Sif.Framework.Persistence;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Service
{

    public class GenericService<T, PK> : IGenericService<T, PK> where T : IPersistable<PK>, new()
    {
        protected IGenericRepository<T, PK> repository;

        public GenericService(IGenericRepository<T, PK> repository)
        {
            this.repository = repository;
        }

        public virtual PK Create(T obj)
        {
            return repository.Save(obj);
        }

        public virtual void Create(IEnumerable<T> objs)
        {
            repository.Save(objs);
        }

        public virtual void Delete(PK id)
        {
            repository.Delete(id);
        }

        public virtual void Delete(T obj)
        {
            repository.Delete(obj);
        }

        public virtual void Delete(IEnumerable<T> objs)
        {
            repository.Delete(objs);
        }

        public virtual T Retrieve(PK id)
        {
            return repository.Retrieve(id);
        }

        public virtual ICollection<T> Retrieve()
        {
            return repository.Retrieve();
        }

        public virtual ICollection<T> Retrieve(T obj)
        {
            return repository.Retrieve(obj);
        }

        public virtual ICollection<T> Retrieve(int pageIndex, int pageSize)
        {
            return repository.Retrieve(pageIndex, pageSize);
        }

        public virtual ICollection<T> Retrieve(IEnumerable<EqualCondition> conditions)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(T obj)
        {
            repository.Save(obj);
        }

        public virtual void Update(IEnumerable<T> objs)
        {
            repository.Save(objs);
        }

    }

}
