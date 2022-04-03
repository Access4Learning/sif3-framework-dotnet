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

using NHibernate;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Persistence;
using System;
using System.Collections.Generic;

namespace Sif.Framework.NHibernate.Persistence
{
    public class SifObjectBindingRepository : GenericRepository<SifObjectBinding, long>, ISifObjectBindingRepository
    {
        public SifObjectBindingRepository() : base(EnvironmentProviderSessionFactory.Instance)
        {
        }

        /// <inheritdoc cref="ISifObjectBindingRepository.RetrieveByBinding" />
        public virtual IEnumerable<SifObjectBinding> RetrieveByBinding(Guid refId, string ownerId)
        {
            if (string.IsNullOrWhiteSpace(ownerId)) throw new ArgumentNullException(nameof(ownerId));

            using (ISession session = SessionFactory.OpenSession())
            {
                return session.QueryOver<SifObjectBinding>()
                    .Where(e => e.RefId == refId)
                    .And(e => e.OwnerId == ownerId)
                    .List<SifObjectBinding>();
            }
        }

        /// <inheritdoc cref="ISifObjectBindingRepository.RetrieveByRefId(Guid)" />
        public IEnumerable<SifObjectBinding> RetrieveByRefId(Guid refId)
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                return session.QueryOver<SifObjectBinding>().Where(e => e.RefId == refId).List<SifObjectBinding>();
            }
        }
    }
}