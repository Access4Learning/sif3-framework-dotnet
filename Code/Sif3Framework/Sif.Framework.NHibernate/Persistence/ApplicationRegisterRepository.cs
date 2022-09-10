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
using Sif.Framework.Models.Infrastructure;
using Sif.Framework.Persistence;
using System;
using Tardigrade.Framework.Exceptions;

namespace Sif.Framework.NHibernate.Persistence
{
    /// <inheritdoc cref="IApplicationRegisterRepository" />
    public class ApplicationRegisterRepository
        : GenericRepository<ApplicationRegister, long>, IApplicationRegisterRepository
    {
        /// <inheritdoc cref="GenericRepository{TEntity, TKey}" />
        public ApplicationRegisterRepository() : base(EnvironmentProviderSessionFactory.Instance)
        {
        }

        /// <inheritdoc cref="IApplicationRegisterRepository.RetrieveByApplicationKey(string)" />
        public virtual ApplicationRegister RetrieveByApplicationKey(string applicationKey)
        {
            if (string.IsNullOrWhiteSpace(applicationKey)) throw new ArgumentNullException(nameof(applicationKey));

            using (ISession session = SessionFactory.OpenSession())
            {
                try
                {
                    return session.QueryOver<ApplicationRegister>()
                        .Where(e => e.ApplicationKey == applicationKey)
                        .SingleOrDefault();
                }
                catch (HibernateException e)
                {
                    throw new DuplicateFoundException(
                        $"Multiple Application Registers exist with application key {applicationKey}.",
                        e);
                }
            }
        }
    }
}