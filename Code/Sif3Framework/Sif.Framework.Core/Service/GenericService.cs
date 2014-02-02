/*
 * Copyright 2014 Systemic Pty Ltd
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
using Sif.Framework.Persistence;
using System.Collections.Generic;

namespace Sif.Framework.Service
{

    public abstract class GenericService<T> : IGenericService<T> where T : IPersistable, new()
    {
        protected IGenericRepository<T> repository;

        // Need to inject repository.
        protected abstract IGenericRepository<T> GetRepository();

        public GenericService()
        {
            repository = GetRepository();
        }

        public virtual void Delete(T obj)
        {
            repository.Delete(obj);
        }

        public virtual T Retrieve(long id)
        {
            return repository.Retrieve(id);
        }

        public virtual ICollection<T> Retrieve(T obj)
        {
            return repository.Retrieve(obj);
        }

        public virtual ICollection<T> Retrieve()
        {
            return repository.Retrieve();
        }

        public virtual long Save(T obj)
        {
            return repository.Save(obj);
        }

    }

}
