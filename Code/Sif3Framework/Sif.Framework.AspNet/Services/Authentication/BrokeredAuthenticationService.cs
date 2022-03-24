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

using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Sessions;
using Sif.Specification.Infrastructure;
using System;
using System.Net.Http.Headers;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.AspNet.Services.Authentication
{
    /// <inheritdoc />
    public class BrokeredAuthenticationService : AuthenticationService
    {
        private readonly IApplicationRegisterService _applicationRegisterService;
        private readonly IEnvironmentService _environmentService;
        private readonly ISessionService _sessionService;
        private readonly IFrameworkSettings _settings;

        /// <summary>
        /// Create an instance of this service.
        /// </summary>
        /// <param name="applicationRegisterService">Application register service.</param>
        /// <param name="environmentService">Environment service.</param>
        /// <param name="settings">Framework settings service.</param>
        /// <param name="sessionService">Session service.</param>
        /// <exception cref="ArgumentNullException">A passed parameter is null.</exception>
        public BrokeredAuthenticationService(
            IApplicationRegisterService applicationRegisterService,
            IEnvironmentService environmentService,
            IFrameworkSettings settings,
            ISessionService sessionService)
        {
            _applicationRegisterService =
                applicationRegisterService ?? throw new ArgumentNullException(nameof(applicationRegisterService));
            _environmentService = environmentService ?? throw new ArgumentNullException(nameof(environmentService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        /// <inheritdoc cref="AuthenticationService.GetEnvironmentBySessionToken(string)" />
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

            if (environment == null)
            {
                throw new InvalidSessionException();
            }

            ApplicationRegister applicationRegister =
                _applicationRegisterService.RetrieveByApplicationKey(environment.applicationInfo.applicationKey);

            return applicationRegister?.SharedSecret;
        }

        /// <inheritdoc cref="Framework.Service.Authentication.IAuthenticationService{THeaders}.VerifyAuthenticationHeader(THeaders)" />
        public override bool VerifyAuthenticationHeader(HttpRequestHeaders headers)
        {
            string storedSessionToken = _sessionService.RetrieveSessionToken(
                _settings.ApplicationKey,
                _settings.SolutionId,
                _settings.UserToken,
                _settings.UserToken);
            bool verified = VerifyAuthenticationHeader(headers, false, out string sessionToken);

            return (verified && sessionToken.Equals(storedSessionToken));
        }
    }
}