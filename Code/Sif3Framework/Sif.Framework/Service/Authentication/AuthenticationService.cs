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

using Sif.Framework.Model.Authentication;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Utils;
using System;
using System.Net.Http.Headers;

namespace Sif.Framework.Service.Authentication
{

    /// <summary>
    /// <see cref="IAuthenticationService">IAuthenticationService</see>
    /// </summary>
    abstract class AuthenticationService : IAuthenticationService
    {

        /// <summary>
        /// <see cref="IAuthenticationService.GetEnvironmentBySessionToken(string)">GetEnvironmentBySessionToken</see>
        /// </summary>
        public abstract Model.Infrastructure.Environment GetEnvironmentBySessionToken(string sessionToken);

        /// <summary>
        /// Retrieve the shared secret value associated with the passed in application key.
        /// </summary>
        /// <param name="applicationKey">Application key associated with the shared secret.</param>
        /// <returns>Shared secret value.</returns>
        protected abstract string InitialSharedSecret(string applicationKey);

        /// <summary>
        /// Retrieve the shared secret value associated with the passed in session token.
        /// </summary>
        /// <param name="sessionToken">Session token associated with the shared secret.</param>
        /// <returns>Shared secret value.</returns>
        /// <exception cref="InvalidSessionException">sessionToken is invalid.</exception>
        protected abstract string SharedSecret(string sessionToken);

        /// <summary>
        /// Verify the authentication header.
        /// </summary>
        /// <param name="headers">HTTP request headers.</param>
        /// <param name="initial">Flag to indicate whether this is the initial verification call.</param>
        /// <param name="sessionToken">Session token associated with the authentication header.</param>
        /// <returns>True if the initial authentication header is valid; false otherwise.</returns>
        protected bool VerifyAuthenticationHeader(HttpRequestHeaders headers, bool initial, out string sessionToken)
        {
            bool verified = false;
            string sessionTokenChecked = null;

            if (headers != null && headers.Authorization != null)
            {
                GetSharedSecret sharedSecret;

                if (initial)
                {
                    sharedSecret = InitialSharedSecret;
                }
                else
                {
                    sharedSecret = SharedSecret;
                }

                try
                {

                    if (AuthenticationMethod.Basic.ToString().Equals(headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))
                    {
                        AuthorisationToken authorisationToken = new AuthorisationToken { Token = headers.Authorization.ToString() };
                        IAuthorisationTokenService authorisationTokenService = new BasicAuthorisationTokenService();
                        verified = authorisationTokenService.Verify(authorisationToken, sharedSecret, out sessionTokenChecked);
                    }
                    else if (AuthenticationMethod.SIF_HMACSHA256.ToString().Equals(headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))
                    {
                        string timestamp = HttpUtils.GetTimestamp(headers);
                        AuthorisationToken authorisationToken = new AuthorisationToken { Token = headers.Authorization.ToString(), Timestamp = timestamp };
                        IAuthorisationTokenService authorisationTokenService = new HmacShaAuthorisationTokenService();
                        verified = authorisationTokenService.Verify(authorisationToken, sharedSecret, out sessionTokenChecked);
                    }

                }
                catch (InvalidSessionException)
                {
                    verified = false;
                }

            }

            sessionToken = sessionTokenChecked;

            return verified;
        }

        /// <summary>
        /// <see cref="IAuthenticationService.VerifyAuthenticationHeader(HttpRequestHeaders)">VerifyAuthenticationHeader</see>
        /// </summary>
        public virtual bool VerifyAuthenticationHeader(HttpRequestHeaders headers)
        {
            string sessionToken;
            return VerifyAuthenticationHeader(headers, false, out sessionToken);
        }

        /// <summary>
        /// <see cref="IAuthenticationService.VerifyAuthenticationHeader(HttpRequestHeaders, out string)">VerifyAuthenticationHeader</see>
        /// </summary>
        public virtual bool VerifyAuthenticationHeader(HttpRequestHeaders headers, out string sessionToken)
        {
            return VerifyAuthenticationHeader(headers, false, out sessionToken);
        }

        /// <summary>
        /// <see cref="IAuthenticationService.VerifyInitialAuthenticationHeader(HttpRequestHeaders, out string)">VerifyAuthenticationHeader</see>
        /// </summary>
        public virtual bool VerifyInitialAuthenticationHeader(HttpRequestHeaders header, out string sessionToken)
        {
            return VerifyAuthenticationHeader(header, true, out sessionToken);
        }

    }

}
