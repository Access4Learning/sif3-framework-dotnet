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

using NHibernate;
using Sif.Framework.Model.Infrastructure;
using System;

namespace Sif.Framework.Persistence.NHibernate
{

    public class EnvironmentRegisterRepository : GenericRepository<EnvironmentRegister, long>, IEnvironmentRegisterRepository
    {

        public EnvironmentRegisterRepository()
            : base(EnvironmentProviderSessionFactory.Instance)
        {

        }

        public virtual EnvironmentRegister RetrieveByUniqueIdentifiers(string applicationKey, string instanceId, string userToken, string solutionId)
        {

            if (string.IsNullOrWhiteSpace(applicationKey))
            {
                throw new ArgumentNullException("applicationKey");
            }

            using (ISession session = sessionFactory.OpenSession())
            {
                var query = session.QueryOver<EnvironmentRegister>();
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

                return query.SingleOrDefault();
            }

        }

    }

}
