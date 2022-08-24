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
using System.Security.Cryptography;
using System.Text;

namespace Sif.Framework.Services.Authentication
{
    /// <summary>
    /// Implementation of the authorisation token service based upon HMAC-SHA256 authentication.
    /// </summary>
    public class HmacShaAuthorisationTokenService : IAuthorisationTokenService
    {
        /// <inheritdoc cref="IAuthorisationTokenService.Generate(string, string)" />
        public AuthorisationToken Generate(string sessionToken, string sharedSecret)
        {
            if (string.IsNullOrWhiteSpace(sessionToken)) throw new ArgumentNullException(nameof(sessionToken));

            if (string.IsNullOrWhiteSpace(sharedSecret)) throw new ArgumentNullException(nameof(sharedSecret));

            var authorisationToken = new AuthorisationToken
            {
                // Generate UTC ISO 8601 date string.
                Timestamp = DateTime.UtcNow.ToString("o")
            };

            // 1. Combine the Token and current date time in UTC using ISO 8601 format.
            byte[] messageBytes = Encoding.ASCII.GetBytes(sessionToken + ":" + authorisationToken.Timestamp);
            // 2. Calculate the HMAC SHA 256 using the Consumer Secret and then Base64 encode.
            byte[] keyBytes = Encoding.ASCII.GetBytes(sharedSecret);
            string hmacSha256EncodedString;

            using (var hmacSha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashMessage = hmacSha256.ComputeHash(messageBytes);
                hmacSha256EncodedString = Convert.ToBase64String(hashMessage);
            }

            // 3. Combine the Token with the resulting string from above separated by a colon.
            string combinedMessage = sessionToken + ":" + hmacSha256EncodedString;
            // 4. Base64 encode this string.
            string base64EncodedString = Convert.ToBase64String(Encoding.ASCII.GetBytes(combinedMessage));

            // 5. Prefix this string with the Authentication Method and a space.
            authorisationToken.Token = AuthenticationMethod.SIF_HMACSHA256 + " " + base64EncodedString;

            return authorisationToken;
        }

        /// <inheritdoc cref="IAuthorisationTokenService.Verify(AuthorisationToken, GetSharedSecret, out string)" />
        public bool Verify(
            AuthorisationToken authorisationToken,
            GetSharedSecret getSharedSecret,
            out string sessionToken)
        {
            if (authorisationToken == null)
                throw new InvalidAuthorisationTokenException("Authorisation token is null.");

            if (string.IsNullOrWhiteSpace(authorisationToken.Token))
                throw new InvalidAuthorisationTokenException("The authorisation token value is null or empty.");

            if (string.IsNullOrWhiteSpace(authorisationToken.Timestamp))
                throw new InvalidAuthorisationTokenException("The authorisation token timestamp is null or empty.");

            //if (!DateTime.TryParse(authorisationToken.Timestamp, out DateTime tokenTimestamp))
            //{
            //    throw new InvalidAuthorisationTokenException("The authorisation token timestamp is not of a valid format.");
            //}

            //TimeSpan timeSpan = DateTime.UtcNow - tokenTimestamp;

            // TODO: Retrieve the token expiry limit from configuration.
            //if (timeSpan.TotalSeconds > 10)
            //{
            //    throw new InvalidAuthorisationTokenException("The authorisation token timestamp has expired.");
            //}

            if (getSharedSecret == null) throw new ArgumentNullException(nameof(getSharedSecret));

            string[] tokens = authorisationToken.Token.Split(' ');

            if (tokens.Length != 2 ||
                !AuthenticationMethod.SIF_HMACSHA256.ToString().Equals(tokens[0]) ||
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

            string hmacSha256EncodedString = nextTokens[1];
            sessionToken = nextTokens[0];
            string sharedSecret = getSharedSecret(sessionToken);

            // Recalculate the encoded HMAC SHA256 string.
            // NOTE: Currently there are no checks for the date to be in UTC ISO 8601 format.
            byte[] messageBytes = Encoding.ASCII.GetBytes(sessionToken + ":" + authorisationToken.Timestamp);
            byte[] keyBytes = Encoding.ASCII.GetBytes(sharedSecret);
            string newHmacSha256EncodedString;

            using (HMACSHA256 hmacSha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashMessage = hmacSha256.ComputeHash(messageBytes);
                newHmacSha256EncodedString = Convert.ToBase64String(hashMessage);
            }

            return hmacSha256EncodedString.Equals(newHmacSha256EncodedString);
        }
    }
}