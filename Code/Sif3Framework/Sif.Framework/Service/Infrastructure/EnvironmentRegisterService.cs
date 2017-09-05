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

        /**
         * This method attempts to retrive the application environment template. This works in a hierarchical manner. The lowest level of the hierarchy
         * is when the following values of the environmentKey are set: solutionID, applicationKey, userToken, instanceID.<br/>
         * In this case the instanceID is ignored in determining the actual row because according to SIF3 the instanceID is only used for cases where
         * two different devices access the same environment. The environment composition for both instances is assumed to be the same and therefore
         * the only fields to be used for the determination of the actual environment are solutionID, applicationKey, userToken.<br/> 
         * If there is no template listed for this combination then the next level is attempted: solutionID, applicationKey.<br/>
         * It must be noted that the 'solutionID' is optional. It is possible that the applicationKey is the only value provided and that there is a
         * template for that. In the DB it could have a solutionID set but the consumer doesn't provide it and only provides the applicationKey. This
         * method will also work for that case. The returned row will may or may not have the solutionID set though. It is important that in such a 
         * case it is not possible to have two solutions with the same applicationKey in the SIF3_APP_TEMPLATE table.
         * 
         * @param tx The current transaction. Cannot be null.
         * @param environmentKey The environment key data for which the application environment template shall be returned.
         * 
         * @return See desc.
         * 
         * @throws PersistenceException Could not access underlying data store.
         */
        public virtual EnvironmentRegister RetrieveByUniqueIdentifiers(string applicationKey, string instanceId, string userToken, string solutionId)
        {
            var repo = (EnvironmentRegisterRepository) repository;

            var environmentRegister =
                // Let's try the lowest level first. Remember solutionID is optional
                repo.RetrieveByUniqueIdentifiers(applicationKey, instanceId, userToken, solutionId)
                // Try the next level up. We have no result, yet.
                ?? repo.RetrieveByUniqueIdentifiers(applicationKey, null, userToken, solutionId)
                // And finally try the top level. We still have no result.
                ?? repo.RetrieveByUniqueIdentifiers(applicationKey, null, null, solutionId)
                // If we get here then there is no template listed for the given environmentKey.
                ;

            return environmentRegister;
        }

    }

}
