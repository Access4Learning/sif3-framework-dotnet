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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sif.Framework.Persistence;
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.EntityFrameworkCore.Persistence
{
    /// <inheritdoc />
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
        where TEntity : class, IHasUniqueIdentifier<TKey>, new()
    {
        public virtual void Delete(TKey id)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(TEntity item)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(IEnumerable<TEntity> items)
        {
            throw new NotImplementedException();
        }

        public virtual TEntity Retrieve(TKey id)
        {
            throw new NotImplementedException();
        }

        public virtual ICollection<TEntity> Retrieve()
        {
            throw new NotImplementedException();
        }

        public virtual ICollection<TEntity> Retrieve(TEntity item)
        {
            throw new NotImplementedException();
        }

        public virtual ICollection<TEntity> Retrieve(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public virtual TKey Save(TEntity item)
        {
            throw new NotImplementedException();
        }

        public virtual void Save(IEnumerable<TEntity> items)
        {
            throw new NotImplementedException();
        }

        public virtual bool Exists(TKey id)
        {
            throw new NotImplementedException();
        }
    }
}
