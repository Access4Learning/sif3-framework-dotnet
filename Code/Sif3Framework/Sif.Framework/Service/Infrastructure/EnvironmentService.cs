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

using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Persistence;
using Sif.Framework.Persistence.NHibernate;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using Tardigrade.Framework.Exceptions;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Infrastructure
{
    /// <summary>
    /// Service class for Environment objects.
    /// </summary>
    public class EnvironmentService : SifService<environmentType, Environment>, IEnvironmentService
    {
        private readonly IEnvironmentRegisterService _environmentRegisterService;

        /// <summary>
        /// Create a copy of a Zone object.
        /// </summary>
        /// <param name="sourceZone">Zone object to copy.</param>
        /// <returns>New copy of the Zone object.</returns>
        private static Zone CopyDefaultZone(Zone sourceZone)
        {
            if (sourceZone == null) return null;

            var destinationZone = new Zone { Description = sourceZone.Description, SifId = sourceZone.SifId };

            if (sourceZone.Properties != null)
            {
                destinationZone.Properties = CopyProperties(sourceZone.Properties);
            }

            return destinationZone;
        }

        /// <summary>
        /// Create a copy of a dictionary of InfrastructureService objects.
        /// </summary>
        /// <param name="sourceServices">Dictionary of InfrastructureService objects to copy.</param>
        /// <returns>New copy of the dictionary of InfrastructureService objects if not null; null otherwise.</returns>
        private static IDictionary<InfrastructureServiceNames, InfrastructureService> CopyInfrastructureServices(
            IDictionary<InfrastructureServiceNames, InfrastructureService> sourceServices)
        {
            if (sourceServices == null) return null;

            var destinationServices = new Dictionary<InfrastructureServiceNames, InfrastructureService>();

            foreach (InfrastructureServiceNames key in sourceServices.Keys)
            {
                if (!sourceServices.TryGetValue(key, out InfrastructureService sourceService)) continue;

                var destinationService = new InfrastructureService
                {
                    Name = sourceService.Name,
                    Value = sourceService.Value
                };

                destinationServices.Add(key, destinationService);
            }

            return destinationServices;
        }

        /// <summary>
        /// Create a copy of a dictionary of Property objects.
        /// </summary>
        /// <param name="sourceProperties">Dictionary of Property objects to copy.</param>
        /// <returns>New copy of the dictionary of Property objects if not null; null otherwise.</returns>
        private static IDictionary<string, Property> CopyProperties(IDictionary<string, Property> sourceProperties)
        {
            if (sourceProperties == null) return null;

            var destinationProperties = new Dictionary<string, Property>();

            foreach (string key in sourceProperties.Keys)
            {
                if (!sourceProperties.TryGetValue(key, out Property sourceProperty)) continue;

                var destinationProperty = new Property
                {
                    Name = sourceProperty.Name,
                    Value = sourceProperty.Value
                };

                destinationProperties.Add(key, destinationProperty);
            }

            return destinationProperties;
        }

        /// <summary>
        /// Create a copy of a dictionary of ProvisionedZone objects.
        /// </summary>
        /// <param name="sourceZones">Dictionary of ProvisionedZone objects to copy.</param>
        /// <returns>New copy of the dictionary of ProvisionedZone objects if not null; null otherwise.</returns>
        private static IDictionary<string, ProvisionedZone> CopyProvisionedZones(
            IDictionary<string, ProvisionedZone> sourceZones)
        {
            if (sourceZones == null) return null;

            var destinationZones = new Dictionary<string, ProvisionedZone>();

            foreach (string key in sourceZones.Keys)
            {
                if (!sourceZones.TryGetValue(key, out ProvisionedZone sourceZone)) continue;

                var destinationZone = new ProvisionedZone { SifId = sourceZone.SifId };

                if (sourceZone.Services != null)
                {
                    destinationZone.Services = CopyServices(sourceZone.Services);
                }

                destinationZones.Add(key, destinationZone);
            }

            return destinationZones;
        }

        /// <summary>
        /// Create a copy of a dictionary of Right objects.
        /// </summary>
        /// <param name="sourceRights">Dictionary of Right objects to copy.</param>
        /// <returns>New copy of the dictionary of Right objects if not null; null otherwise.</returns>
        private static IDictionary<string, Right> CopyRights(IDictionary<string, Right> sourceRights)
        {
            if (sourceRights == null) return null;

            var destinationRights = new Dictionary<string, Right>();

            foreach (string key in sourceRights.Keys)
            {
                if (!sourceRights.TryGetValue(key, out Right sourceRight)) continue;

                var destinationRight = new Right
                {
                    Type = sourceRight.Type,
                    Value = sourceRight.Value
                };

                destinationRights.Add(key, destinationRight);
            }

            return destinationRights;
        }

        /// <summary>
        /// Create a copy of a collection of Service objects.
        /// </summary>
        /// <param name="sourceServices">Collection of Service objects to copy.</param>
        /// <returns>New copy of the collection of Service objects if not null; null otherwise.</returns>
        private static ICollection<Model.Infrastructure.Service> CopyServices(
            ICollection<Model.Infrastructure.Service> sourceServices)
        {
            if (sourceServices == null) return null;

            var destinationServices = new List<Model.Infrastructure.Service>();

            foreach (Model.Infrastructure.Service sourceService in sourceServices)
            {
                var destinationService = new Model.Infrastructure.Service
                {
                    ContextId = sourceService.ContextId,
                    Name = sourceService.Name,
                    Type = sourceService.Type
                };

                if (sourceService.Rights != null)
                {
                    destinationService.Rights = CopyRights(sourceService.Rights);
                }

                destinationServices.Add(destinationService);
            }

            return destinationServices;
        }

        /// <summary>
        /// Create a default instance.
        /// </summary>
        public EnvironmentService(
            IEnvironmentRepository environmentRepository,
            IEnvironmentRegisterService environmentRegisterService)
            : base(environmentRepository)
        {
            _environmentRegisterService = environmentRegisterService;
        }

        /// <inheritdoc cref="SifService{TDto, TEntity}.Create(TDto, string, string)" />
        public override Guid Create(environmentType item, string zoneId = null, string contextId = null)
        {
            EnvironmentRegister environmentRegister = _environmentRegisterService.RetrieveByUniqueIdentifiers(
                item.applicationInfo.applicationKey,
                item.instanceId,
                item.userToken,
                item.solutionId);

            if (environmentRegister == null)
            {
                string errorMessage =
                    $"Environment register with [applicationKey:{item.applicationInfo.applicationKey}|solutionId:{item.solutionId ?? "<null>"}|instanceId:{item.instanceId ?? "<null>"}|userToken:{item.userToken ?? "<null>"}] does NOT exist.";
                throw new NotFoundException(errorMessage);
            }

            string sessionToken = AuthenticationUtils.GenerateSessionToken(
                item.applicationInfo.applicationKey,
                item.instanceId,
                item.userToken,
                item.solutionId);

            environmentType environmentType = RetrieveBySessionToken(sessionToken);

            if (environmentType != null)
            {
                string errorMessage =
                    $"A session token already exists for environment with [applicationKey:{item.applicationInfo.applicationKey}|solutionId:{item.solutionId ?? "<null>"}|instanceId:{item.instanceId ?? "<null>"}|userToken:{item.userToken ?? "<null>"}].";
                throw new AlreadyExistsException(errorMessage);
            }

            IDictionary<InfrastructureServiceNames, InfrastructureService> infrastructureServices =
                CopyInfrastructureServices(environmentRegister.InfrastructureServices);
            IDictionary<string, ProvisionedZone> provisionedZones =
                CopyProvisionedZones(environmentRegister.ProvisionedZones);
            Environment repoItem = MapperFactory.CreateInstance<environmentType, Environment>(item);

            if (environmentRegister.DefaultZone != null)
            {
                repoItem.DefaultZone = CopyDefaultZone(environmentRegister.DefaultZone);
            }

            if (infrastructureServices.Count > 0)
            {
                repoItem.InfrastructureServices =
                    CopyInfrastructureServices(environmentRegister.InfrastructureServices);
            }

            if (provisionedZones.Count > 0)
            {
                repoItem.ProvisionedZones = CopyProvisionedZones(environmentRegister.ProvisionedZones);
            }

            repoItem.SessionToken = sessionToken;
            Guid environmentId = Repository.Save(repoItem);

            if (repoItem.InfrastructureServices != null && repoItem.InfrastructureServices.Count > 0)
            {
                InfrastructureService infrastructureService =
                    repoItem.InfrastructureServices[InfrastructureServiceNames.environment];

                if (infrastructureService != null)
                {
                    infrastructureService.Value = infrastructureService.Value + "/" + environmentId;
                    Repository.Save(repoItem);
                }
            }

            return environmentId;
        }

        /// <inheritdoc cref="IEnvironmentService.RetrieveBySessionToken(string)" />
        public virtual environmentType RetrieveBySessionToken(string sessionToken)
        {
            Environment environment = ((EnvironmentRepository)Repository).RetrieveBySessionToken(sessionToken);

            return MapperFactory.CreateInstance<Environment, environmentType>(environment);
        }
    }
}