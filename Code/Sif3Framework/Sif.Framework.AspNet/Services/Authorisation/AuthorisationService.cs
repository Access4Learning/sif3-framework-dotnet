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

using Sif.Framework.Extensions;
using Sif.Framework.Models.Exceptions;
using Sif.Framework.Models.Infrastructure;
using Sif.Framework.Services.Authentication;
using Sif.Framework.Services.Authorisation;
using Sif.Framework.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Sif.Framework.AspNet.Services.Authorisation
{
    /// <inheritdoc />
    public class AuthorisationService : IAuthorisationService<HttpRequestHeaders>
    {
        /// <summary>
        /// Service used for request authentication.
        /// </summary>
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorisationService" /> class.
        /// </summary>
        /// <param name="authenticationService">An instance of either a Brokered or Direct authentication service.</param>
        public AuthorisationService(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <inheritdoc cref="IAuthorisationService{THeaders}.IsAuthorised(THeaders, string, string, RightType, RightValue, string)" />
        public virtual bool IsAuthorised(HttpRequestHeaders headers,
            string sessionToken,
            string serviceName,
            RightType permission,
            RightValue privilege = RightValue.APPROVED,
            string zoneId = null)
        {
            var isAuthorised = true;
            Environment environment = _authenticationService.GetEnvironmentBySessionToken(sessionToken);

            if (environment == null) throw
                new InvalidSessionException("Session token does not have an associated environment definition.");

            var operationPolicy = new Right { Type = permission.ToString(), Value = privilege.ToString() };
            string serviceType = headers.GetHeaderValue("serviceType") ?? ServiceType.OBJECT.ToDescription();

            // Retrieving permissions for requester.
            ICollection<Right> requesterPermissions = GetRightsForService(
                serviceType,
                serviceName,
                environment.GetTargetZone(zoneId));

            if (requesterPermissions == null)
            {
                isAuthorised = false;
            }
            else
            {
                // Checking the appropriate rights.
                try
                {
                    RightsUtils.CheckRight(requesterPermissions, operationPolicy);
                }
                catch (RejectedException)
                {
                    isAuthorised = false;
                }
            }

            return isAuthorised;
        }

        /// <summary>
        /// Retrieve the rights for a service in a given zone.
        /// </summary>
        /// <param name="serviceType">The service type used for the request <see cref="ServiceType"/> for valid types.</param>
        /// <param name="serviceName">The service name.</param>
        /// <param name="zone">The zone to retrieve the rights for.</param>
        /// <returns>An array of declared rights</returns>
        private static ICollection<Right> GetRightsForService(
            string serviceType,
            string serviceName,
            ProvisionedZone zone)
        {
            Service service =
                (from Service s in zone.Services
                 where s.Type.Equals(serviceType) && s.Name.Equals(serviceName)
                 select s).FirstOrDefault();

            return service?.Rights;
        }
    }
}