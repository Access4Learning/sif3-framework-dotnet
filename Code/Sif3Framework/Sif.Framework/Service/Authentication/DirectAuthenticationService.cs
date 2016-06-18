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
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Service.Mapper;
using Sif.Specification.Infrastructure;

namespace Sif.Framework.Service.Authentication
{

    /// <summary>
    /// <see cref="Sif.Framework.Service.Authentication.AuthenticationService">AuthenticationService</see>
    /// </summary>
    class DirectAuthenticationService : AuthenticationService
    {
        private IApplicationRegisterService applicationRegisterService;
        private IEnvironmentService environmentService;

        public DirectAuthenticationService(IApplicationRegisterService applicationRegisterService, IEnvironmentService environmentService)
        {
            this.applicationRegisterService = applicationRegisterService;
            this.environmentService = environmentService;
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

        public override Environment GetEnvironmentBySessionToken(string sessionToken)
        {
            environmentType environment = environmentService.RetrieveBySessionToken(sessionToken);
            return MapperFactory.CreateInstance<environmentType, Environment>(environment);
        }

    }

}
