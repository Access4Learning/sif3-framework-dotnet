/*
 * Copyright 2014 Systemic Pty Ltd
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

using System;
using System.Security.Cryptography;
using System.Text;

namespace Sif.Framework.Utils
{

    public static class AuthenticationUtils
    {
        enum AuthorisationMethod { Basic, HMACSHA256 };

        public delegate string GetSharedSecret(string sessionToken);

        public static string GenerateBasicAuthorisationToken(string sessionToken, string sharedSecret)
        {

            if (String.IsNullOrWhiteSpace(sharedSecret))
            {
                throw new ArgumentNullException("sharedSecret");
            }

            if (String.IsNullOrWhiteSpace(sessionToken))
            {
                throw new ArgumentNullException("sessionToken");
            }

            return AuthorisationMethod.Basic.ToString() + " " + Convert.ToBase64String(Encoding.UTF8.GetBytes(sessionToken + ":" + sharedSecret));
        }

        public static bool VerifyBasicAuthorisationToken(string authorisationToken, GetSharedSecret getSharedSecret, out string sessionToken)
        {

            if (String.IsNullOrWhiteSpace(authorisationToken))
            {
                throw new ArgumentNullException("authorisationToken");
            }

            if (getSharedSecret == null)
            {
                throw new ArgumentNullException("getSharedSecret");
            }

            string[] tokens = authorisationToken.Split(' ');

            if (tokens.Length != 2 || !AuthorisationMethod.Basic.ToString().Equals(tokens[0]) || String.IsNullOrWhiteSpace(tokens[1]))
            {
                throw new ArgumentException("Authorisation token not recognised.", "authorisationToken");
            }

            string base64EncodedString = tokens[1];
            string combinedMessage = Encoding.ASCII.GetString(Convert.FromBase64String(base64EncodedString));
            string[] nextTokens = combinedMessage.Split(':');

            if (nextTokens.Length != 2 || String.IsNullOrWhiteSpace(nextTokens[0]) || String.IsNullOrWhiteSpace(nextTokens[1]))
            {
                throw new ArgumentException("Invalid authorisation token.", "authorisationToken");
            }

            string sharedSecret = nextTokens[1];
            sessionToken = nextTokens[0];

            return sharedSecret.Equals(getSharedSecret(sessionToken));
        }

        public static string GenerateHMACSHA256AuthorisationToken(string sessionToken, string sharedSecret, out string dateString)
        {

            if (String.IsNullOrWhiteSpace(sharedSecret))
            {
                throw new ArgumentNullException("sharedSecret");
            }

            if (String.IsNullOrWhiteSpace(sessionToken))
            {
                throw new ArgumentNullException("sessionToken");
            }

            // Generate UTC ISO 8601 date string.
            dateString = DateTime.UtcNow.ToString("o");
            // 1. Combine the Token and current date time in UTC using ISO 8601 format.
            byte[] messageBytes = Encoding.ASCII.GetBytes(sessionToken + ":" + dateString);
            // 2. Calculate the HMAC SHA 256 using the Consumer Secret and then Base64 encode.
            byte[] keyBytes = Encoding.ASCII.GetBytes(sharedSecret);
            string hmacsha256EncodedString;

            using (HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashMessage = hmacsha256.ComputeHash(messageBytes);
                hmacsha256EncodedString = Convert.ToBase64String(hashMessage);
            }

            // 3. Combine the Token with the resulting string from above separated by a colon.
            string combinedMessage = sessionToken + ":" + hmacsha256EncodedString;
            // 4. Base64 encode this string.
            string base64EncodedString = Convert.ToBase64String(Encoding.ASCII.GetBytes(combinedMessage));

            // 5. Prefix this string with the Authentication Method and a space.
            return AuthorisationMethod.HMACSHA256.ToString() + " " + base64EncodedString;
        }

        public static bool VerifyHMACSHA256AuthorisationToken(string authorisationToken, string dateString, GetSharedSecret getSharedSecret, out string sessionToken)
        {

            if (String.IsNullOrWhiteSpace(authorisationToken))
            {
                throw new ArgumentNullException("authorisationToken");
            }

            if (String.IsNullOrWhiteSpace(dateString))
            {
                throw new ArgumentNullException("dateString");
            }

            string[] tokens = authorisationToken.Split(' ');

            if (tokens.Length != 2 || !AuthorisationMethod.HMACSHA256.ToString().Equals(tokens[0]) || String.IsNullOrWhiteSpace(tokens[1]))
            {
                throw new ArgumentException("Authorisation token not recognised.", "authorisationToken");
            }

            string base64EncodedString = tokens[1];
            string combinedMessage = Encoding.ASCII.GetString(Convert.FromBase64String(base64EncodedString));
            string[] nextTokens = combinedMessage.Split(':');

            if (nextTokens.Length != 2 || String.IsNullOrWhiteSpace(nextTokens[0]) || String.IsNullOrWhiteSpace(nextTokens[1]))
            {
                throw new ArgumentException("Invalid authorisation token.", "authorisationToken");
            }

            string hmacsha256EncodedString = nextTokens[1];
            sessionToken = nextTokens[0];
            string sharedSecret = getSharedSecret(sessionToken);

            // Recalculate the encoded HMAC SHA256 string.
            // NOTE: Currently there are no checks for the date to be in UTC ISO 8601 format. I don't see how date
            // can be relevant for verification purposes.
            byte[] messageBytes = Encoding.ASCII.GetBytes(sessionToken + ":" + dateString);
            byte[] keyBytes = Encoding.ASCII.GetBytes(sharedSecret);
            string newHmacsha256EncodedString;

            using (HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashMessage = hmacsha256.ComputeHash(messageBytes);
                newHmacsha256EncodedString = Convert.ToBase64String(hashMessage);
            }

            return hmacsha256EncodedString.Equals(newHmacsha256EncodedString);
        }

        public static string GenerateSessionToken(string applicationKey, string instanceId, string userToken, string solutionId)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(applicationKey + ":" + instanceId + ":" + userToken + ":" + solutionId));
        }

    }

}
