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
    /// <inheritdoc cref="IEnvironmentRegisterRepository" />
    public class EnvironmentRegisterRepository
        : GenericRepository<EnvironmentRegister, long>, IEnvironmentRegisterRepository
    {
        /// <inheritdoc cref="GenericRepository{TEntity, TKey}" />
        public EnvironmentRegisterRepository() : base(EnvironmentProviderSessionFactory.Instance)
        {
        }

        /// <inheritdoc cref="IEnvironmentRegisterRepository.RetrieveByUniqueIdentifiers(string,string,string,string)" />
        public virtual EnvironmentRegister RetrieveByUniqueIdentifiers(
            string applicationKey,
            string instanceId,
            string userToken,
            string solutionId)
        {
            if (string.IsNullOrWhiteSpace(applicationKey)) throw new ArgumentNullException(nameof(applicationKey));

            using (ISession session = SessionFactory.OpenSession())
            {
                IQueryOver<EnvironmentRegister, EnvironmentRegister> query = session.QueryOver<EnvironmentRegister>();
                query.Where(e => e.ApplicationKey == applicationKey);

                if (!string.IsNullOrWhiteSpace(instanceId))
                {
                    query.And(e => e.InstanceId == instanceId);
                }

                if (!string.IsNullOrWhiteSpace(userToken))
                {
                    query.And(e => e.UserToken == userToken);
                }

                if (!string.IsNullOrWhiteSpace(solutionId))
                {
                    query.And(e => e.SolutionId == solutionId);
                }

                try
                {
                    return query.SingleOrDefault();
                }
                catch (HibernateException e)
                {
                    throw new DuplicateFoundException(
                        $"Multiple Environment Registers exist with the combination [applicationKey:{applicationKey}|instanceId:{instanceId}|userToken:{userToken}|solutionId:{solutionId}].",
                        e);
                }
            }
        }
    }
}