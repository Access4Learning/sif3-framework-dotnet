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
using Sif.Framework.Service.Mapper;
using System;
using System.Collections.Generic;
using log4net;
using System.Reflection;

namespace Sif.Framework.Service
{

    public abstract class SifService<UI, DB> : ISifService<UI, DB> where DB : IPersistable<Guid>, new()
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected IGenericRepository<DB, Guid> repository;

        public SifService(IGenericRepository<DB, Guid> repository)
        {
            this.repository = repository;
        }

        public virtual Guid Create(UI item, string zone = null, string context = null)
        {
            DB repoItem = MapperFactory.CreateInstance<UI, DB>(item);
            return repository.Save(repoItem);
        }

        public virtual void Create(IEnumerable<UI> items, string zone = null, string context = null)
        {
            ICollection<DB> repoItems = MapperFactory.CreateInstances<UI, DB>(items);
            repository.Save(repoItems);
        }

        public virtual void Delete(Guid id, string zone = null, string context = null)
        {
            repository.Delete(id);
        }

        public virtual void Delete(UI item, string zone = null, string context = null)
        {
            DB repoItem = MapperFactory.CreateInstance<UI, DB>(item);
            repository.Delete(repoItem);
        }

        public virtual void Delete(IEnumerable<UI> items, string zone = null, string context = null)
        {
            ICollection<DB> repoItems = MapperFactory.CreateInstances<UI, DB>(items);
            repository.Delete(repoItems);
        }

        public virtual UI Retrieve(Guid id, string zone = null, string context = null)
        {
            DB repoItem = repository.Retrieve(id);
            return MapperFactory.CreateInstance<DB, UI>(repoItem);
        }

        public virtual ICollection<UI> Retrieve(UI item, string zone = null, string context = null)
        {
            DB repoItem = MapperFactory.CreateInstance<UI, DB>(item);
            ICollection<DB> repoItems = repository.Retrieve(repoItem);
            return MapperFactory.CreateInstances<DB, UI>(repoItems);
        }

        public virtual ICollection<UI> Retrieve(string zone = null, string context = null)
        {
            ICollection<DB> repoItems = repository.Retrieve();
            return MapperFactory.CreateInstances<DB, UI>(repoItems);
        }

        public virtual void Update(UI item, string zone = null, string context = null)
        {
            DB repoItem = MapperFactory.CreateInstance<UI, DB>(item);
            repository.Save(repoItem);
        }

        public virtual void Update(IEnumerable<UI> items, string zone = null, string context = null)
        {
            ICollection<DB> repoItems = MapperFactory.CreateInstances<UI, DB>(items);
            repository.Save(repoItems);
        }
    }
}
