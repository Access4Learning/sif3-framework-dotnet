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

using Sif.Framework.Models.Authentication;
using Sif.Framework.Models.Exceptions;
using System;
using System.Text;

namespace Sif.Framework.Services.Authentication
{
    /// <summary>
    /// Implementation of the authorisation token service based upon Basic authentication.
    /// </summary>
    public class BasicAuthorisationTokenService : IAuthorisationTokenService
    {
        /// <summary>
        /// For Basic authentication, the Timestamp property of the AuthorisationToken will always return null.
        /// <see cref="IAuthorisationTokenService.Generate(string, string)"/>
        /// </summary>
        public AuthorisationToken Generate(string sessionToken, string sharedSecret)
        {
            if (string.IsNullOrWhiteSpace(sessionToken)) throw new ArgumentNullException(nameof(sessionToken));

            if (string.IsNullOrWhiteSpace(sharedSecret)) throw new ArgumentNullException(nameof(sharedSecret));

            var authorisationToken = new AuthorisationToken
            {
                Token =
                    AuthenticationMethod.Basic + " " +
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(sessionToken + ":" + sharedSecret)),
                Timestamp = null
            };

            return authorisationToken;
        }

        /// <summary>
        /// For Basic authentication, the Timestamp property of the AuthorisationToken is ignored.
        /// <see cref="IAuthorisationTokenService.Verify(AuthorisationToken, GetSharedSecret, out string)"/>
        /// </summary>
        public bool Verify(
            AuthorisationToken authorisationToken,
            GetSharedSecret getSharedSecret,
            out string sessionToken)
        {
            if (authorisationToken == null)
                throw new InvalidAuthorisationTokenException("Authorisation token is null.");

            if (string.IsNullOrWhiteSpace(authorisationToken.Token))
                throw new InvalidAuthorisationTokenException("The authorisation token value is null or empty.");

            if (getSharedSecret == null) throw new ArgumentNullException(nameof(getSharedSecret));

            string[] tokens = authorisationToken.Token.Split(' ');

            if (tokens.Length != 2 ||
                !AuthenticationMethod.Basic.ToString().Equals(tokens[0]) ||
                string.IsNullOrWhiteSpace(tokens[1]))
            {
                throw new InvalidAuthorisationTokenException("Authorisation token is not recognised.");
            }

            string base64EncodedString = tokens[1];
            string combinedMessage = Encoding.ASCII.GetString(Convert.FromBase64String(base64EncodedString));
            string[] nextTokens = combinedMessage.Split(':');

            if (nextTokens.Length != 2 ||
                string.IsNullOrWhiteSpace(nextTokens[0]) ||
                string.IsNullOrWhiteSpace(nextTokens[1]))
            {
                throw new InvalidAuthorisationTokenException("Authorisation token is invalid.");
            }

            string sharedSecret = nextTokens[1];
            sessionToken = nextTokens[0];

            return sharedSecret.Equals(getSharedSecret(sessionToken));
        }
    }
}