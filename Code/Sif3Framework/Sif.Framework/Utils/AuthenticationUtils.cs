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

using System;
using System.Text;

namespace Sif.Framework.Utils
{

    /// <summary>
    /// Utility class for authentication operations.
    /// </summary>
    public static class AuthenticationUtils
    {

        /// <summary>
        /// Generate a session token based on the provided parameters.
        /// </summary>
        /// <param name="applicationKey">Application key.</param>
        /// <param name="instanceId">Instance identifier.</param>
        /// <param name="userToken">User token.</param>
        /// <param name="solutionId">Solution identifier.</param>
        /// <returns>Session token.</returns>
        public static string GenerateSessionToken(string applicationKey, string instanceId, string userToken, string solutionId)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(applicationKey + ":" + instanceId + ":" + userToken + ":" + solutionId));
        }

    }

}
