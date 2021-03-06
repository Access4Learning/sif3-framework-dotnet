﻿/*
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

using Sif.Framework.Model.Infrastructure;
using System.Net.Http.Headers;

namespace Sif.Framework.Service.Authorisation
{

    /// <summary>
    /// This class contains operations for service authorisation.
    /// </summary>
    public interface IAuthorisationService
    {

        /// <summary>
        /// Verifies if the request is authorised. It takes in consideration the permissions and privilegies defined
        /// in the ACL list for the consumer.
        /// </summary>
        /// <param name="headers">HTTP request headers.</param>
        /// <param name="sessionToken">Session token.</param>
        /// <param name="serviceName">The service name to check access rights.</param>
        /// <param name="permission">The permission requested. Any of: ADMIN, CREATE, DELETE, PROVIDE, QUERY, SUBSCRIBE, UPDATE</param>
        /// <param name="privilege">The access level requested. Any of APPROVED, REJECTED, SUPPORTED</param>
        /// <param name="zoneId">The zone of the request.</param>
        /// <exception cref="Model.Exceptions.InvalidSessionException">Session token does not have an associated environment definition.</exception>
        bool IsAuthorised(HttpRequestHeaders headers,
            string sessionToken,
            string serviceName,
            RightType permission,
            RightValue privilege = RightValue.APPROVED,
            string zoneId = null);

    }

}
