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

using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Persistence.NHibernate;

namespace Sif.Framework.Service.Infrastructure
{

    public class EnvironmentRegisterService : GenericService<EnvironmentRegister, long>, IEnvironmentRegisterService
    {

        public EnvironmentRegisterService()
            : base(new EnvironmentRegisterRepository())
        {
        }

        /// <summary>
        /// <para>
        /// This method attempts to retrieve the application environment template in a hierarchical manner. The lowest
        /// level of the hierarchy is when the following keys are set: solutionId, applicationKey, userToken and
        /// instanceId. In this case the instanceId is ignored as it is only used for cases where two different
        /// devices access the same environment. The environment composition for both instances is assumed to be the
        /// same, and therefore the only fields to be used for the determination of the actual environment are
        /// solutionId, applicationKey and userToken.</para>
        /// <para>
        /// If there is no environment template listed for this combination, then the next level is attempted:
        /// solutionId and applicationKey. It must be noted that solutionId is optional and therefore it is possible
        /// that the applicationKey is the only value provided. The database could have a solutionId set but the
        /// Consumer may only provide an applicationKey. This method will also work for that case. The returned row
        /// may or may not have the solutionId set though. It is important that in such a case, it is not possible to
        /// have two solutions with the same applicationKey.</para>
        /// </summary>
        /// <param name="applicationKey">Application key.</param>
        /// <param name="instanceId">Instance ID.</param>
        /// <param name="userToken">User token.</param>
        /// <param name="solutionId">Solution ID.</param>
        /// <returns>Environment template that matches the specified keys.</returns>
        public virtual EnvironmentRegister RetrieveByUniqueIdentifiers(string applicationKey, string instanceId, string userToken, string solutionId)
        {
            EnvironmentRegisterRepository repo = (EnvironmentRegisterRepository)repository;

            EnvironmentRegister environmentRegister =
                // Let's try the lowest level first. Remember solutionId is optional.
                repo.RetrieveByUniqueIdentifiers(applicationKey, instanceId, userToken, solutionId)
                // Try the next level up. We have no result, yet.
                ?? repo.RetrieveByUniqueIdentifiers(applicationKey, null, userToken, solutionId)
                // And finally try the top level. We still have no result.
                ?? repo.RetrieveByUniqueIdentifiers(applicationKey, null, null, solutionId)
                // If we get here then there is no template listed for the given keys.
                ;

            return environmentRegister;
        }

    }

}
