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
using Sif.Framework.Persistence;
using System;
using Tardigrade.Framework.Exceptions;
using Environment = Sif.Framework.Models.Infrastructure.Environment;

namespace Sif.Framework.NHibernate.Persistence
{
    /// <inheritdoc cref="IEnvironmentRepository" />
    public class EnvironmentRepository : GenericRepository<Environment, Guid>, IEnvironmentRepository
    {
        /// <inheritdoc cref="GenericRepository{TEntity, TKey}" />
        public EnvironmentRepository() : base(EnvironmentProviderSessionFactory.Instance)
        {
        }

        /// <inheritdoc cref="IEnvironmentRepository.RetrieveBySessionToken(string)" />
        public virtual Environment RetrieveBySessionToken(string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken)) throw new ArgumentNullException(nameof(sessionToken));

            using (ISession session = SessionFactory.OpenSession())
            {
                try
                {
                    return session.QueryOver<Environment>().Where(e => e.SessionToken == sessionToken).SingleOrDefault();
                }
                catch (HibernateException e)
                {
                    throw new DuplicateFoundException(
                        $"Multiple Environments exist with session token {sessionToken}.",
                        e);
                }
            }
        }
    }
}