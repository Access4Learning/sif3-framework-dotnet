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
using System.Collections.Generic;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Infrastructure
{

    public class EnvironmentService : SifService<environmentType, Environment>, IEnvironmentService
    {

        private IDictionary<string, Property> CopyInfrastructureServices(IDictionary<string, Property> sourceInfrastructureServices)
        {
            IDictionary<string, Property> destinationInfrastructureServices = new Dictionary<string, Property>();

            if (sourceInfrastructureServices != null && sourceInfrastructureServices.Count > 0)
            {

                foreach (Property sourceInfrastructureService in sourceInfrastructureServices.Values)
                {
                    Property destinationInfrastructureService = new Property { Name = sourceInfrastructureService.Name, Value = sourceInfrastructureService.Value };
                    destinationInfrastructureServices.Add(destinationInfrastructureService.Name, destinationInfrastructureService);
                }

            }

            return destinationInfrastructureServices;
        }

        private IDictionary<RightType, Right> CopyRights(IDictionary<RightType, Right> sourceRights)
        {
            IDictionary<RightType, Right> destinationRights = new Dictionary<RightType, Right>();

            if (sourceRights != null && sourceRights.Count > 0)
            {

                foreach (Right sourceRight in sourceRights.Values)
                {
                    Right destinationRight = new Right { Type = sourceRight.Type, Value = sourceRight.Value };
                    destinationRights.Add(destinationRight.Type, destinationRight);
                }

            }

            return destinationRights;
        }

        private ICollection<Model.Infrastructure.Service> CopyServices(ICollection<Model.Infrastructure.Service> sourceServices)
        {
            ICollection<Model.Infrastructure.Service> destinationServices = new List<Model.Infrastructure.Service>();

            if (sourceServices != null && sourceServices.Count > 0)
            {

                foreach (Model.Infrastructure.Service sourceService in sourceServices)
                {
                    Model.Infrastructure.Service destinationService = new Model.Infrastructure.Service { ContextId = sourceService.ContextId, Name = sourceService.Name, Type = sourceService.Type };
                    IDictionary<RightType, Right> rights = CopyRights(sourceService.Rights);

                    if (rights.Count > 0)
                    {
                        destinationService.Rights = rights;
                    }

                    destinationServices.Add(destinationService);
                }

            }

            return destinationServices;
        }

        private IDictionary<string, ProvisionedZone> CopyProvisionedZones(IDictionary<string, ProvisionedZone> sourceProvisionedZones)
        {
            IDictionary<string, ProvisionedZone> destinationProvisionedZones = new Dictionary<string, ProvisionedZone>();

            if (sourceProvisionedZones != null && sourceProvisionedZones.Count > 0)
            {

                foreach (ProvisionedZone sourceProvisionedZone in sourceProvisionedZones.Values)
                {

                    ProvisionedZone destinationProvisionedZone = new ProvisionedZone { SifId = sourceProvisionedZone.SifId };
                    ICollection<Model.Infrastructure.Service> services = CopyServices(sourceProvisionedZone.Services);

                    if (services.Count > 0)
                    {
                        destinationProvisionedZone.Services = services;
                    }

                    destinationProvisionedZones.Add(destinationProvisionedZone.SifId, destinationProvisionedZone);
                }

            }

            return destinationProvisionedZones;
        }

        protected override IGenericRepository<Environment, Guid> GetRepository()
        {
            return new EnvironmentRepository();
        }

        public override Guid Create(environmentType item)
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

            IDictionary<string, Property> infrastructureServices = CopyInfrastructureServices(environmentRegister.InfrastructureServices);
            IDictionary<string, ProvisionedZone> provisionedZones = CopyProvisionedZones(environmentRegister.ProvisionedZones);
            Environment repoItem = MapperFactory.CreateInstance<environmentType, Environment>(item);

            if (infrastructureServices.Count > 0)
            {
                repoItem.InfrastructureServices = CopyInfrastructureServices(environmentRegister.InfrastructureServices);
            }

            if (provisionedZones.Count > 0)
            {
                repoItem.ProvisionedZones = CopyProvisionedZones(environmentRegister.ProvisionedZones);
            }

            repoItem.SessionToken = sessionToken;

            return repository.Save(repoItem);
        }

        public virtual environmentType RetrieveBySessionToken(string sessionToken)
        {
            Environment environment = ((EnvironmentRepository)repository).RetrieveBySessionToken(sessionToken);
            return MapperFactory.CreateInstance<Environment, environmentType>(environment);
        }

    }

}
