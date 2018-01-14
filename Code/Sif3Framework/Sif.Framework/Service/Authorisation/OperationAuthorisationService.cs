/*
 * Copyright 2017 Systemic Pty Ltd
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
using Sif.Framework.Service.Authentication;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Sif.Framework.Service.Authorisation
{

    /// <summary>
    /// <see cref="IOperationAuthorisationService">IOperationAuthorisationService</see>
    /// </summary>
    public class OperationAuthorisationService : IOperationAuthorisationService
    {
        /// <summary>
        /// Service used for request authentication.
        /// </summary>
        protected IAuthenticationService authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationAuthorisationService" /> class.
        /// </summary>
        public OperationAuthorisationService()
        {
            if (EnvironmentType.DIRECT.Equals(SettingsManager.ProviderSettings.EnvironmentType))
            {
                this.authService = new DirectAuthenticationService(new ApplicationRegisterService(), new EnvironmentService());
            }
            else if (EnvironmentType.BROKERED.Equals(SettingsManager.ProviderSettings.EnvironmentType))
            {
                this.authService = new BrokeredAuthenticationService(new ApplicationRegisterService(), new EnvironmentService());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationAuthorisationService" /> class.
        /// </summary>
        /// <param name="authService">An instance of either the <see cref="Sif.Framework.Service.Authentication.BrokeredAuthenticationService" /> or the 
        ///  <see cref="Sif.Framework.Service.Authentication.DirectAuthenticationService" /> classes.</param>
        public OperationAuthorisationService(IAuthenticationService authService)
        {
            this.authService = authService;
        }

        /// <summary>
        /// <see cref="IOperationAuthorisationService.IsAuthorised(HttpRequestHeaders, string, RightType, RightValue, string)">IsAuthorised</see>
        /// </summary>
        /// <exception cref="UnauthorisedRequestException">Thrown when the request not authorised.</exception>
        /// <exception cref="InvalidRequestException">Thrown when the request is invalid. eg: Zone and context configuration is incorrect.</exception>
        /// <exception cref="RejectedException">When the access in unauthorised.</exception>
        public virtual bool IsAuthorised(HttpRequestHeaders headers, string serviceName, RightType permission, RightValue privilege = RightValue.APPROVED, string zoneId = null)
        {
            bool isAuthorised = true; // if something goes wrong, an exception will be thrown

            string sessionToken = "";
            if (!authService.VerifyAuthenticationHeader(headers, out sessionToken))
            {
                throw new UnauthorisedRequestException();
            }

            Model.Infrastructure.Environment environment = authService.GetEnvironmentBySessionToken(sessionToken);

            if (environment == null)
            {
                throw new InvalidRequestException("Request failed as could not retrieve environment XML.");
            }

            Right operationPolicy = new Right(permission, privilege);
            string serviceType = HttpUtils.GetHeaderValue(headers, "serviceType");

            // retireving permissions for requester
            IDictionary<string, Right> requesterPermissions = GetRightsForService(serviceType, serviceName, EnvironmentUtils.GetTargetZone(environment, zoneId));

            // Checking the appropriate rights
            RightsUtils.CheckRight(requesterPermissions, operationPolicy);

            return isAuthorised;
        }

        /// <summary>
        /// Retrieve the rights for a service in a given zone.
        /// </summary>
        /// <param name="serviceType">The service type used for the request <see cref="Sif.Framework.Model.Infrastructure.ServiceType"/> for valid types.</param>
        /// <param name="serviceName">The service name.</param>
        /// <param name="zone">The zone to retrieve the rights for.</param>
        /// <returns>An array of declared rights</returns>
        private IDictionary<string, Right> GetRightsForService(string serviceType, string serviceName, ProvisionedZone zone)
        {
            Model.Infrastructure.Service service = (from Model.Infrastructure.Service s in zone.Services
                                                    where s.Type.Equals(serviceType) && s.Name.Equals(serviceName)
                                                    select s).FirstOrDefault();

            if (service == null)
            {
                throw new InvalidRequestException($"No Service called {serviceName} found in Environment.");
            }

            return service.Rights;
        }
    }

}
