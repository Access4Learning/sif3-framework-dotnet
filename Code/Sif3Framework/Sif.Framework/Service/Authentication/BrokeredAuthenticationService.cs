﻿/*
 * Copyright 2021 Systemic Pty Ltd
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

namespace Sif.Framework.Service.Authentication
{
    /// <summary>
    /// <see cref="AuthenticationService">AuthenticationService</see>
    /// </summary>
    internal class BrokeredAuthenticationService : AuthenticationService
    {
        private readonly IApplicationRegisterService applicationRegisterService;
        private readonly IEnvironmentService environmentService;
        private readonly IFrameworkSettings settings;
        private readonly ISessionService sessionService;

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
            this.applicationRegisterService = applicationRegisterService ?? throw new ArgumentNullException(nameof(applicationRegisterService));
            this.environmentService = environmentService ?? throw new ArgumentNullException(nameof(environmentService));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        /// <summary>
        /// <see cref="AuthenticationService.GetEnvironmentBySessionToken(string)">GetEnvironmentBySessionToken</see>
        /// </summary>
        public override Environment GetEnvironmentBySessionToken(string sessionToken)
        {
            environmentType environment = environmentService.RetrieveBySessionToken(sessionToken);

            return MapperFactory.CreateInstance<environmentType, Environment>(environment);
        }

        /// <summary>
        /// <see cref="AuthenticationService.InitialSharedSecret(string)">InitialSharedSecret</see>
        /// </summary>
        protected override string InitialSharedSecret(string applicationKey)
        {
            ApplicationRegister applicationRegister =
                applicationRegisterService.RetrieveByApplicationKey(applicationKey);

            return applicationRegister?.SharedSecret;
        }

        /// <summary>
        /// <see cref="AuthenticationService.SharedSecret(string)">SharedSecret</see>
        /// </summary>
        protected override string SharedSecret(string sessionToken)
        {
            environmentType environment = environmentService.RetrieveBySessionToken(sessionToken);

            if (environment == null)
            {
                throw new InvalidSessionException();
            }

            ApplicationRegister applicationRegister =
                applicationRegisterService.RetrieveByApplicationKey(environment.applicationInfo.applicationKey);

            return applicationRegister?.SharedSecret;
        }

        /// <summary>
        /// <see cref="IAuthenticationService.VerifyAuthenticationHeader(HttpRequestHeaders)">VerifyAuthenticationHeader</see>
        /// </summary>
        public override bool VerifyAuthenticationHeader(HttpRequestHeaders headers)
        {
            string storedSessionToken = sessionService.RetrieveSessionToken(
                settings.ApplicationKey,
                settings.SolutionId,
                settings.UserToken,
                settings.UserToken);
            bool verified = VerifyAuthenticationHeader(headers, false, out string sessionToken);

            return (verified && sessionToken.Equals(storedSessionToken));
        }
    }
}