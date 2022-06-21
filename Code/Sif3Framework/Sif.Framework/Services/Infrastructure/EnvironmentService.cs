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
using Sif.Framework.Utils;
using System;
using System.Linq;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Services;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Infrastructure
{
    /// <summary>
    /// Service class for Environment objects.
    /// </summary>
    public class EnvironmentService : ObjectService<Environment, Guid>, IEnvironmentService
    {
        private readonly IEnvironmentRegisterService _environmentRegisterService;
        private readonly IEnvironmentRepository _repository;

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

        /// <inheritdoc cref="ObjectService{TEntity, TKey}" />
        public EnvironmentService(IEnvironmentRepository repository, IEnvironmentRegisterService environmentRegisterService) : base(repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _environmentRegisterService =
                environmentRegisterService ?? throw new ArgumentNullException(nameof(environmentRegisterService));
        }

        /// <inheritdoc cref="IObjectService{TEntity,TKey}.Create(TEntity)" />
        public override Environment Create(Environment item)
        {
            EnvironmentRegister environmentRegister = _environmentRegisterService.RetrieveByUniqueIdentifiers(
                item.ApplicationInfo.ApplicationKey,
                item.InstanceId,
                item.UserToken,
                item.SolutionId);

            if (environmentRegister == null)
            {
                string errorMessage =
                    $"Environment register with [applicationKey:{item.ApplicationInfo.ApplicationKey}|solutionId:{item.SolutionId ?? "<null>"}|instanceId:{item.InstanceId ?? "<null>"}|userToken:{item.UserToken ?? "<null>"}] does NOT exist.";
                throw new NotFoundException(errorMessage);
            }

            string sessionToken = AuthenticationUtils.GenerateSessionToken(
                item.ApplicationInfo.ApplicationKey,
                item.InstanceId,
                item.UserToken,
                item.SolutionId);

            try
            {
                Environment environment = RetrieveBySessionToken(sessionToken);

                if (environment != null)
                {
                    var errorMessage =
                        $"A session token already exists for environment with [applicationKey:{item.ApplicationInfo.ApplicationKey}|solutionId:{item.SolutionId ?? "<null>"}|instanceId:{item.InstanceId ?? "<null>"}|userToken:{item.UserToken ?? "<null>"}].";
                    throw new AlreadyExistsException(errorMessage);
                }

                if (environmentRegister.DefaultZone != null)
                {
                    item.DefaultZone = CopyDefaultZone(environmentRegister.DefaultZone);
                }

                if (environmentRegister.InfrastructureServices?.Any() ?? false)
                {
                    item.InfrastructureServices = environmentRegister.InfrastructureServices;
                }

                if (environmentRegister.ProvisionedZones?.Any() ?? false)
                {
                    item.ProvisionedZones = environmentRegister.ProvisionedZones.ToList();
                }

                item.SessionToken = sessionToken;
                Environment created = Repository.Create(item);

                if (item.InfrastructureServices?.Any() ?? false)
                {
                    InfrastructureService infrastructureService =
                        item.InfrastructureServices.FirstOrDefault(
                            i => i.Name == InfrastructureServiceNames.environment);

                    if (infrastructureService != null)
                    {
                        infrastructureService.Value = infrastructureService.Value + "/" + created.Id;
                        Repository.Update(item);
                    }
                }

                return created;
            }
            catch (Exception e) when (e is DuplicateFoundException || e is RepositoryException)
            {
                throw new ServiceException(e.Message, e);
            }
        }

        /// <inheritdoc cref="IEnvironmentService.RetrieveBySessionToken(string)" />
        public virtual Environment RetrieveBySessionToken(string sessionToken)
        {
            return _repository.RetrieveBySessionToken(sessionToken);
        }
    }
}