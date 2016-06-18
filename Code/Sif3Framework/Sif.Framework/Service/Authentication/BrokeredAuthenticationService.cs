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
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Sessions;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System.Net.Http.Headers;

namespace Sif.Framework.Service.Authentication
{

    /// <summary>
    /// <see cref="Sif.Framework.Service.Authentication.AuthenticationService">AuthenticationService</see>
    /// </summary>
    class BrokeredAuthenticationService : AuthenticationService
    {
        private IApplicationRegisterService applicationRegisterService;
        private IEnvironmentService environmentService;
        private IFrameworkSettings settings;
        private ISessionService sessionService;

        public BrokeredAuthenticationService(IApplicationRegisterService applicationRegisterService, IEnvironmentService environmentService)
        {
            this.applicationRegisterService = applicationRegisterService;
            this.environmentService = environmentService;
            settings = SettingsManager.ProviderSettings;
            sessionService = SessionsManager.ProviderSessionService;
        }

        /// <summary>
        /// <see cref="Sif.Framework.Service.Authentication.AuthenticationService.InitialSharedSecret(System.String)">InitialSharedSecret</see>
        /// </summary>
        protected override string InitialSharedSecret(string applicationKey)
        {
            ApplicationRegister applicationRegister = applicationRegisterService.RetrieveByApplicationKey(applicationKey);
            return (applicationRegister == null ? null : applicationRegister.SharedSecret);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Service.Authentication.AuthenticationService.SharedSecret(System.String)">SharedSecret</see>
        /// </summary>
        protected override string SharedSecret(string sessionToken)
        {
            environmentType environment = environmentService.RetrieveBySessionToken(sessionToken);

            if (environment == null)
            {
                throw new InvalidSessionException();
            }

            ApplicationRegister applicationRegister = applicationRegisterService.RetrieveByApplicationKey(environment.applicationInfo.applicationKey);
            return (applicationRegister == null ? null : applicationRegister.SharedSecret);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Service.Authentication.IAuthenticationService.VerifyAuthenticationHeader(System.Net.Http.Headers.AuthenticationHeaderValue, System.Boolean)">VerifyAuthenticationHeader</see>
        /// </summary>
        public override bool VerifyAuthenticationHeader(AuthenticationHeaderValue header)
        {
            string storedSessionToken = sessionService.RetrieveSessionToken(settings.ApplicationKey, settings.SolutionId, settings.UserToken, settings.UserToken);
            string sessionToken;
            bool verified = VerifyAuthenticationHeader(header, false, out sessionToken);
            return (verified && sessionToken.Equals(storedSessionToken));
        }

        public override Environment GetEnvironmentBySessionToken(string sessionToken)
        {
            environmentType environment = environmentService.RetrieveBySessionToken(sessionToken);
            return MapperFactory.CreateInstance<environmentType, Environment>(environment);
        }

    }

}
