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

using Sif.Framework.Model.Infrastructure;
using System.Net.Http.Headers;

namespace Sif.Framework.Service.Authentication
{

    /// <summary>
    /// This class contains operations for service authentication.
    /// </summary>
    public interface IAuthenticationService
    {

        /// <summary>
        /// Retrieve an environment by session token.
        /// </summary>
        /// <param name="sessionToken">Session token associated with the environment.</param>
        /// <returns>Environment.</returns>
        Environment GetEnvironmentBySessionToken(string sessionToken);

        /// <summary>
        /// Verify the authentication header.
        /// </summary>
        /// <param name="headers">HTTP request headers.</param>
        /// <returns>True if the authentication header is valid; false otherwise.</returns>
        bool VerifyAuthenticationHeader(HttpRequestHeaders headers);

        /// <summary>
        /// Verify the authentication header.
        /// </summary>
        /// <param name="headers">HTTP request headers.</param>
        /// <param name="sessionToken">Session token associated with the authentication header.</param>
        /// <returns>True if the authentication header is valid; false otherwise.</returns>
        bool VerifyAuthenticationHeader(HttpRequestHeaders headers, out string sessionToken);

        /// <summary>
        /// Verify the initial authentication header.
        /// </summary>
        /// <param name="headers">HTTP request headers.</param>
        /// <param name="sessionToken">Session token associated with the initial authentication header.</param>
        /// <returns>True if the initial authentication header is valid; false otherwise.</returns>
        bool VerifyInitialAuthenticationHeader(HttpRequestHeaders headers, out string sessionToken);

    }

}
