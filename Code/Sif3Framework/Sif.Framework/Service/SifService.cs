/*
 * Copyright 2017 Systemic Pty Ltd
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

using log4net;
using Sif.Framework.Model.Persistence;
using Sif.Framework.Persistence;
using Sif.Framework.Service.Mapper;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sif.Framework.Service
{

    /// <summary>
    /// <see cref="ISifService{UI, DB}"/>
    /// </summary>
    public abstract class SifService<UI, DB> : ISifService<UI, DB> where DB : IPersistable<Guid>, new()
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Generic repository.
        /// </summary>
        protected IGenericRepository<DB, Guid> repository;

        /// <summary>
        /// Create an instance based upon the provided repository.
        /// </summary>
        /// <param name="repository">Repository associated with the service.</param>
        public SifService(IGenericRepository<DB, Guid> repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// <see cref="ISifService{UI, DB}.Create(UI, string, string)"/>
        /// </summary>
        public virtual Guid Create(UI item, string zoneId = null, string contextId = null)
        {
            DB repoItem = MapperFactory.CreateInstance<UI, DB>(item);
            return repository.Save(repoItem);
        }

        /// <summary>
        /// <see cref="ISifService{UI, DB}.Create(IEnumerable{UI}, string, string)"/>
        /// </summary>
        public virtual void Create(IEnumerable<UI> items, string zoneId = null, string contextId = null)
        {
            ICollection<DB> repoItems = MapperFactory.CreateInstances<UI, DB>(items);
            repository.Save(repoItems);
        }

        /// <summary>
        /// <see cref="ISifService{UI, DB}.Delete(Guid, string, string)"/>
        /// </summary>
        public virtual void Delete(Guid id, string zoneId = null, string contextId = null)
        {
            repository.Delete(id);
        }

        /// <summary>
        /// <see cref="ISifService{UI, DB}.Delete(UI, string, string)"/>
        /// </summary>
        public virtual void Delete(UI item, string zoneId = null, string contextId = null)
        {
            DB repoItem = MapperFactory.CreateInstance<UI, DB>(item);
            repository.Delete(repoItem);
        }

        /// <summary>
        /// <see cref="ISifService{UI, DB}.Delete(IEnumerable{UI}, string, string)"/>
        /// </summary>
        public virtual void Delete(IEnumerable<UI> items, string zoneId = null, string contextId = null)
        {
            ICollection<DB> repoItems = MapperFactory.CreateInstances<UI, DB>(items);
            repository.Delete(repoItems);
        }

        /// <summary>
        /// <see cref="ISifService{UI, DB}.Retrieve(Guid, string, string)"/>
        /// </summary>
        public virtual UI Retrieve(Guid id, string zoneId = null, string contextId = null)
        {
            DB repoItem = repository.Retrieve(id);
            return MapperFactory.CreateInstance<DB, UI>(repoItem);
        }

        /// <summary>
        /// <see cref="ISifService{UI, DB}.Retrieve(UI, string, string)"/>
        /// </summary>
        public virtual ICollection<UI> Retrieve(UI item, string zoneId = null, string contextId = null)
        {
            DB repoItem = MapperFactory.CreateInstance<UI, DB>(item);
            ICollection<DB> repoItems = repository.Retrieve(repoItem);
            return MapperFactory.CreateInstances<DB, UI>(repoItems);
        }

        /// <summary>
        /// <see cref="ISifService{UI, DB}.Retrieve(string, string)"/>
        /// </summary>
        public virtual ICollection<UI> Retrieve(string zoneId = null, string contextId = null)
        {
            ICollection<DB> repoItems = repository.Retrieve();
            return MapperFactory.CreateInstances<DB, UI>(repoItems);
        }

        /// <summary>
        /// <see cref="ISifService{UI, DB}.Update(UI, string, string)"/>
        /// </summary>
        public virtual void Update(UI item, string zoneId = null, string contextId = null)
        {
            DB repoItem = MapperFactory.CreateInstance<UI, DB>(item);
            repository.Save(repoItem);
        }

        /// <summary>
        /// <see cref="ISifService{UI, DB}.Update(IEnumerable{UI}, string, string)"/>
        /// </summary>
        public virtual void Update(IEnumerable<UI> items, string zoneId = null, string contextId = null)
        {
            ICollection<DB> repoItems = MapperFactory.CreateInstances<UI, DB>(items);
            repository.Save(repoItems);
        }

    }

}
