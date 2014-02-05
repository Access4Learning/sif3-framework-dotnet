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

using Sif.Framework.Infrastructure;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Persistence;
using Sif.Framework.Persistence.NHibernate;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Utils;
using System;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Infrastructure
{

    public class EnvironmentService : SifService<environmentType, Environment>, IEnvironmentService
    {

        protected override ISifRepository<Environment> GetRepository()
        {
            return new EnvironmentRepository();
        }

        public override long Create(environmentType item)
        {
            EnvironmentRegister environmentRegister =
                (new EnvironmentRegisterService()).RetrieveByUniqueIdentifiers
                    (item.applicationInfo.applicationKey, item.instanceId, item.userToken, item.solutionId);

            if (environmentRegister == null)
            {
                throw new ArgumentException("item");
            }

            string sessionToken = AuthenticationUtils.GenerateSessionToken(item.applicationInfo.applicationKey, item.instanceId, item.userToken, item.solutionId);

            environmentType environmentType = RetrieveBySessionToken(sessionToken);

            if (environmentType != null)
            {
                throw new ArgumentException("A session token already exists for environment.", "item");
            }

            Environment repoItem = MapperFactory.CreateInstance<environmentType, Environment>(item);
            repoItem.InfrastructureServices = environmentRegister.InfrastructureServices;
            repoItem.ProvisionedZones = environmentRegister.ProvisionedZones;
            repoItem.SessionToken = sessionToken;
            repoItem.SifId = Guid.NewGuid().ToString();

            return repository.Save(repoItem);
        }

        public virtual environmentType RetrieveBySessionToken(string sessionToken)
        {
            Environment environment = ((EnvironmentRepository)repository).RetrieveBySessionToken(sessionToken);
            return MapperFactory.CreateInstance<Environment, environmentType>(environment);
        }

    }

}
