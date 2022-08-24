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

using Sif.Framework.Models.Infrastructure;
using Sif.Framework.Persistence;
using Sif.Framework.Services.Mapper;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Linq;
using Tardigrade.Framework.Exceptions;
using Environment = Sif.Framework.Models.Infrastructure.Environment;

namespace Sif.Framework.Services.Infrastructure
{
    /// <summary>
    /// Service class for Environment objects.
    /// </summary>
    public class EnvironmentDtoService : SifService<environmentType, Environment>, IEnvironmentDtoService
    {
        private readonly IEnvironmentRegisterService _environmentRegisterService;
        private readonly IEnvironmentRepository _environmentRepository;

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
                destinationZone.Properties = sourceZone.Properties.ToList();
            }

            return destinationZone;
        }

        /// <inheritdoc cref="SifService{TDto, TEntity}" />
        public EnvironmentDtoService(
            IEnvironmentRepository environmentRepository,
            IEnvironmentRegisterService environmentRegisterService)
            : base(environmentRepository)
        {
            _environmentRepository =
                environmentRepository ?? throw new ArgumentNullException(nameof(environmentRepository));
            _environmentRegisterService =
                environmentRegisterService ?? throw new ArgumentNullException(nameof(environmentRegisterService));
        }

        /// <inheritdoc cref="SifService{TDto, TEntity}.Create(TDto)" />
        public override Guid Create(environmentType item)
        {
            EnvironmentRegister environmentRegister = _environmentRegisterService.RetrieveByUniqueIdentifiers(
                item.applicationInfo.applicationKey,
                item.instanceId,
                item.userToken,
                item.solutionId);

            if (environmentRegister == null)
            {
                var errorMessage =
                    $"Environment register with [applicationKey:{item.applicationInfo.applicationKey}|solutionId:{item.solutionId ?? "<null>"}|instanceId:{item.instanceId ?? "<null>"}|userToken:{item.userToken ?? "<null>"}] does NOT exist.";
                throw new NotFoundException(errorMessage);
            }

            string sessionToken = AuthenticationUtils.GenerateSessionToken(
                item.applicationInfo.applicationKey,
                item.instanceId,
                item.userToken,
                item.solutionId);

            Environment environment = _environmentRepository.RetrieveBySessionToken(sessionToken);

            if (environment != null)
            {
                var errorMessage =
                    $"A session token already exists for environment with [applicationKey:{item.applicationInfo.applicationKey}|solutionId:{item.solutionId ?? "<null>"}|instanceId:{item.instanceId ?? "<null>"}|userToken:{item.userToken ?? "<null>"}].";
                throw new AlreadyExistsException(errorMessage);
            }

            Environment repoItem = MapperFactory.CreateInstance<environmentType, Environment>(item);

            if (environmentRegister.DefaultZone != null)
            {
                repoItem.DefaultZone = CopyDefaultZone(environmentRegister.DefaultZone);
            }

            if (environmentRegister.InfrastructureServices.Any())
            {
                repoItem.InfrastructureServices = environmentRegister.InfrastructureServices;
            }

            if (environmentRegister.ProvisionedZones.Any())
            {
                repoItem.ProvisionedZones = environmentRegister.ProvisionedZones.ToList();
            }

            repoItem.SessionToken = sessionToken;
            Guid environmentId = Repository.Create(repoItem).Id;

            if (repoItem.InfrastructureServices != null && repoItem.InfrastructureServices.Any())
            {
                InfrastructureService infrastructureService = repoItem.InfrastructureServices.FirstOrDefault(
                    i => i.Name == InfrastructureServiceNames.environment);

                if (infrastructureService != null)
                {
                    infrastructureService.Value = infrastructureService.Value + "/" + environmentId;
                    Repository.Update(repoItem);
                }
            }

            return environmentId;
        }
    }
}