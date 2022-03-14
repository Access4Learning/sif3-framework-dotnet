﻿/*
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

using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Service.Mapper;
using Sif.Specification.Infrastructure;

namespace Sif.Framework.Service.Authentication
{
    /// <inheritdoc />
    public class DirectAuthenticationService : AuthenticationService
    {
        private readonly IApplicationRegisterService _applicationRegisterService;
        private readonly IEnvironmentService _environmentService;

        public DirectAuthenticationService(
            IApplicationRegisterService applicationRegisterService,
            IEnvironmentService environmentService)
        {
            _applicationRegisterService = applicationRegisterService;
            _environmentService = environmentService;
        }

        /// <inheritdoc cref="IAuthenticationService.GetEnvironmentBySessionToken(string)" />
        public override Environment GetEnvironmentBySessionToken(string sessionToken)
        {
            environmentType environment = _environmentService.RetrieveBySessionToken(sessionToken);

            return MapperFactory.CreateInstance<environmentType, Environment>(environment);
        }

        /// <inheritdoc cref="AuthenticationService.InitialSharedSecret(string)" />
        protected override string InitialSharedSecret(string applicationKey)
        {
            ApplicationRegister applicationRegister =
                _applicationRegisterService.RetrieveByApplicationKey(applicationKey);

            return applicationRegister?.SharedSecret;
        }

        /// <inheritdoc cref="AuthenticationService.SharedSecret(string)" />
        protected override string SharedSecret(string sessionToken)
        {
            environmentType environment = _environmentService.RetrieveBySessionToken(sessionToken);

            if (environment == null) throw new InvalidSessionException();

            ApplicationRegister applicationRegister =
                _applicationRegisterService.RetrieveByApplicationKey(environment.applicationInfo.applicationKey);

            return applicationRegister?.SharedSecret;
        }
    }
}