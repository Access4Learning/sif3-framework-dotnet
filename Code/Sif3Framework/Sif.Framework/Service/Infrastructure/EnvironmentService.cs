/*
 * Copyright 2016 Systemic Pty Ltd
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

using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Persistence.NHibernate;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Infrastructure
{

    public class EnvironmentService : SifService<environmentType, Environment>, IEnvironmentService
    {
        private Zone CopyDefaultZone(Zone sourceZone)
        {
            Zone destinationZone = null;

            if (sourceZone != null)
            {
                destinationZone = new Zone { Description = sourceZone.Description, SifId = sourceZone.SifId };

                if (sourceZone.Properties != null)
                {
                    destinationZone.Properties = CopyProperties(sourceZone.Properties);
                }

            }

            return destinationZone;
        }

        private IDictionary<InfrastructureServiceNames, InfrastructureService> CopyInfrastructureServices(IDictionary<InfrastructureServiceNames, InfrastructureService> sourceInfrastructureServices)
        {
            IDictionary<InfrastructureServiceNames, InfrastructureService> destinationInfrastructureServices = null;

            if (sourceInfrastructureServices != null)
            {
                destinationInfrastructureServices = new Dictionary<InfrastructureServiceNames, InfrastructureService>();

                foreach (InfrastructureServiceNames key in sourceInfrastructureServices.Keys)
                {
                    InfrastructureService sourceInfrastructureService;
                    sourceInfrastructureServices.TryGetValue(key, out sourceInfrastructureService);
                    InfrastructureService destinationInfrastructureService = new InfrastructureService { Name = sourceInfrastructureService.Name, Value = sourceInfrastructureService.Value };
                    destinationInfrastructureServices.Add(key, destinationInfrastructureService);
                }

            }

            return destinationInfrastructureServices;
        }

        private IDictionary<string, Property> CopyProperties(IDictionary<string, Property> sourceProperties)
        {
            IDictionary<string, Property> destinationProperties = null;

            if (sourceProperties != null)
            {
                destinationProperties = new Dictionary<string, Property>();

                foreach (string key in sourceProperties.Keys)
                {
                    Property sourceProperty;
                    sourceProperties.TryGetValue(key, out sourceProperty);
                    Property destinationProperty = new Property { Name = sourceProperty.Name, Value = sourceProperty.Value };
                    destinationProperties.Add(key, destinationProperty);
                }

            }

            return destinationProperties;
        }

        private IDictionary<string, ProvisionedZone> CopyProvisionedZones(IDictionary<string, ProvisionedZone> sourceProvisionedZones)
        {
            IDictionary<string, ProvisionedZone> destinationProvisionedZones = null;

            if (sourceProvisionedZones != null)
            {
                destinationProvisionedZones = new Dictionary<string, ProvisionedZone>();

                foreach (string key in sourceProvisionedZones.Keys)
                {
                    ProvisionedZone sourceProvisionedZone;
                    sourceProvisionedZones.TryGetValue(key, out sourceProvisionedZone);
                    ProvisionedZone destinationProvisionedZone = new ProvisionedZone { SifId = sourceProvisionedZone.SifId };

                    if (sourceProvisionedZone.Services != null)
                    {
                        destinationProvisionedZone.Services = CopyServices(sourceProvisionedZone.Services);
                    }

                    destinationProvisionedZones.Add(key, destinationProvisionedZone);
                }

            }

            return destinationProvisionedZones;
        }

        private IDictionary<string, Right> CopyRights(IDictionary<string, Right> sourceRights)
        {
            IDictionary<string, Right> destinationRights = null;

            if (sourceRights != null)
            {
                destinationRights = new Dictionary<string, Right>();

                foreach (string key in sourceRights.Keys)
                {
                    Right sourceRight;
                    sourceRights.TryGetValue(key, out sourceRight);
                    Right destinationRight = new Right { Type = sourceRight.Type, Value = sourceRight.Value };
                    destinationRights.Add(key, destinationRight);
                }

            }

            return destinationRights;
        }

        private ICollection<Model.Infrastructure.Service> CopyServices(ICollection<Model.Infrastructure.Service> sourceServices)
        {
            ICollection<Model.Infrastructure.Service> destinationServices = null;

            if (sourceServices != null)
            {
                destinationServices = new List<Model.Infrastructure.Service>();

                foreach (Model.Infrastructure.Service sourceService in sourceServices)
                {
                    Model.Infrastructure.Service destinationService = new Model.Infrastructure.Service { ContextId = sourceService.ContextId, Name = sourceService.Name, Type = sourceService.Type };

                    if (sourceService.Rights != null)
                    {
                        destinationService.Rights = CopyRights(sourceService.Rights);
                    }

                    destinationServices.Add(destinationService);
                }

            }

            return destinationServices;
        }

        public EnvironmentService()
            : base(new EnvironmentRepository())
        {

        }

        public override Guid Create(environmentType item, string zone = null, string context = null)
        {
            EnvironmentRegister environmentRegister =
                (new EnvironmentRegisterService()).RetrieveByUniqueIdentifiers
                    (item.applicationInfo.applicationKey, item.instanceId, item.userToken, item.solutionId);

            if (environmentRegister == null)
            {
                string errorMessage = String.Format("Environment with application key of {0}, solution ID of {1}, instance ID of {2} and user token of {3} does NOT exist.",
                    item.applicationInfo.applicationKey, (item.solutionId == null ? "[null]" : item.solutionId), (item.instanceId == null ? "[null]" : item.instanceId), (item.userToken == null ? "[null]" : item.userToken));
                throw new AlreadyExistsException(errorMessage);
            }

            string sessionToken = AuthenticationUtils.GenerateSessionToken(item.applicationInfo.applicationKey, item.instanceId, item.userToken, item.solutionId);

            environmentType environmentType = RetrieveBySessionToken(sessionToken);

            if (environmentType != null)
            {
                string errorMessage = String.Format("A session token already exists for environment with application key of {0}, solution ID of {1}, instance ID of {2} and user token of {3}.",
                    item.applicationInfo.applicationKey, (item.solutionId == null ? "[null]" : item.solutionId), (item.instanceId == null ? "[null]" : item.instanceId), (item.userToken == null ? "[null]" : item.userToken));
                throw new AlreadyExistsException(errorMessage);
            }

            IDictionary<InfrastructureServiceNames, InfrastructureService> infrastructureServices = CopyInfrastructureServices(environmentRegister.InfrastructureServices);
            IDictionary<string, ProvisionedZone> provisionedZones = CopyProvisionedZones(environmentRegister.ProvisionedZones);
            Environment repoItem = MapperFactory.CreateInstance<environmentType, Environment>(item);

            if (environmentRegister.DefaultZone != null)
            {
                repoItem.DefaultZone = CopyDefaultZone(environmentRegister.DefaultZone);
            }

            if (infrastructureServices.Count > 0)
            {
                repoItem.InfrastructureServices = CopyInfrastructureServices(environmentRegister.InfrastructureServices);
            }

            if (provisionedZones.Count > 0)
            {
                repoItem.ProvisionedZones = CopyProvisionedZones(environmentRegister.ProvisionedZones);
            }

            repoItem.SessionToken = sessionToken;
            Guid environmentId = repository.Save(repoItem);

            if (repoItem.InfrastructureServices.Count > 0)
            {
                InfrastructureService infrastructureService = repoItem.InfrastructureServices[InfrastructureServiceNames.environment];

                if (infrastructureService != null)
                {
                    infrastructureService.Value = infrastructureService.Value + "/" + environmentId;
                    repository.Save(repoItem);
                }

            }

            return environmentId;
        }

        public virtual environmentType RetrieveBySessionToken(string sessionToken)
        {
            Environment environment = ((EnvironmentRepository)repository).RetrieveBySessionToken(sessionToken);
            return MapperFactory.CreateInstance<Environment, environmentType>(environment);
        }

    }

}
