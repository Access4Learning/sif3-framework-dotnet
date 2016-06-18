/*
 * Copyright 2015 Systemic Pty Ltd
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
using Sif.Framework.Utils;
using System.Net.Http.Headers;

namespace Sif.Framework.Service.Authentication
{

    /// <summary>
    /// <see cref="Sif.Framework.Service.Authentication.IAuthenticationService">IAuthenticationService</see>
    /// </summary>
    abstract class AuthenticationService : IAuthenticationService
    {

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
        protected abstract string SharedSecret(string sessionToken);

        /// <summary>
        /// Verify the authentication header.
        /// </summary>
        /// <param name="header">Authentication header.</param>
        /// <param name="initial">Flag to indicate whether this is the initial verification call.</param>
        /// <param name="sessionToken">Session token associated with the authentication header.</param>
        /// <returns>True if the initial authentication header is valid; false otherwise.</returns>
        protected bool VerifyAuthenticationHeader(AuthenticationHeaderValue header, bool initial, out string sessionToken)
        {
            bool verified = false;
            string sessionTokenChecked = null;

            if (header != null)
            {
                AuthenticationUtils.GetSharedSecret sharedSecret;

                if (initial)
                {
                    sharedSecret = InitialSharedSecret;
                }
                else
                {
                    sharedSecret = SharedSecret;
                }

                if ("Basic".Equals(header.Scheme))
                {
                    verified = AuthenticationUtils.VerifyBasicAuthorisationToken(header.ToString(), sharedSecret, out sessionTokenChecked);
                }
                else if ("SIF_HMACSHA256".Equals(header.Scheme))
                {
                    verified = true;
                }

            }

            sessionToken = sessionTokenChecked;

            return verified;
        }

        /// <summary>
        /// <see cref="Sif.Framework.Service.Authentication.IAuthenticationService.VerifyAuthenticationHeader(System.Net.Http.Headers.AuthenticationHeaderValue, System.Boolean, System.String)">VerifyAuthenticationHeader</see>
        /// </summary>
        public virtual bool VerifyAuthenticationHeader(AuthenticationHeaderValue header)
        {
            string sessionToken;
            return VerifyAuthenticationHeader(header, false, out sessionToken);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Service.Authentication.IAuthenticationService.VerifyAuthenticationHeader(System.Net.Http.Headers.AuthenticationHeaderValue, System.Boolean, System.String)">VerifyAuthenticationHeader</see>
        /// </summary>
        public virtual bool VerifyAuthenticationHeader(AuthenticationHeaderValue header, out string sessionToken)
        {
            return VerifyAuthenticationHeader(header, false, out sessionToken);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Service.Authentication.IAuthenticationService.VerifyAuthenticationHeader(System.Net.Http.Headers.AuthenticationHeaderValue, System.Boolean)">VerifyAuthenticationHeader</see>
        /// </summary>
        public virtual bool VerifyInitialAuthenticationHeader(AuthenticationHeaderValue header, out string sessionToken)
        {
            return VerifyAuthenticationHeader(header, true, out sessionToken);
        }

        public abstract Environment GetEnvironmentBySessionToken(string sessionToken);

    }

}
