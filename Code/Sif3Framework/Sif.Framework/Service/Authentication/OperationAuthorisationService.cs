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
using System.Web.Http.Controllers;

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
        /// <see cref="IOperationAuthorisationService.IsAuthorised(HttpActionContext, string, RightType, RightValue)">IsAuthorised</see>
        /// </summary>
        /// <exception cref="InvalidRequestException"></exception>
        /// <exception cref="RejectedException"></exception>
        public virtual bool IsAuthorised(HttpActionContext actionContext, string serviceName, RightType permission, RightValue privilege = RightValue.APPROVED)
        {
            bool isAuthorised = true; // if something goes wrong, an exception will be thrown

            string sessionToken = CheckAuthorisation(actionContext);
            Model.Infrastructure.Environment environment = authService.GetEnvironmentBySessionToken(sessionToken);

            if (environment == null)
            {
                throw new InvalidRequestException("Request failed as could not retrieve environment XML.");
            }

            Right operationPolicy = new Right(permission, privilege);

            string[] zoneId = actionContext.ActionArguments["zoneId"] as string[];

            // retireving permissions for requester
            IDictionary<string, Right> requesterPermissions
                = GetRightsForService(actionContext, serviceName, EnvironmentUtils.GetTargetZone(environment, zoneId == null ? null : zoneId[0]));
            
            // Checking the appropriate rights
            RightsUtils.CheckRight(requesterPermissions, operationPolicy);
            
            return isAuthorised;
        }

        /// <summary>
        /// Internal method to check if the request is authorised in the given zone and context by checking the environment XML.
        /// </summary>
        /// <returns>The SessionToken if the request is authorised, otherwise an excpetion will be thrown.</returns>
        private string CheckAuthorisation(HttpActionContext actionContext)
        {
            string sessionToken = "";
            if (!authService.VerifyAuthenticationHeader(actionContext.Request.Headers, out sessionToken))
            {
                throw new RejectedException("Request is not authorized.");
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.
            string[] zoneId = actionContext.ActionArguments["zoneId"] as string[];
            string[] contextId = actionContext.ActionArguments["contextId"] as string[];

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                throw new InvalidRequestException("Request failed as Zone and/or Context are invalid.");
            }

            return sessionToken;
        }

        /// <summary>
        /// Retrieve the rights for a service in a given zone.
        /// </summary>
        /// <param name="actionContext">The action context, which encapsulates information for using the filter.</param>
        /// <param name="serviceName">The service name.</param>
        /// <param name="zone">The zone to retrieve the rights for.</param>
        /// <returns>An array of declared rights</returns>
        private IDictionary<string, Right> GetRightsForService(HttpActionContext actionContext, string serviceName, ProvisionedZone zone)
        {
            string serviceType = HttpUtils.GetHeaderValue(actionContext.Request.Headers, "serviceType");

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
