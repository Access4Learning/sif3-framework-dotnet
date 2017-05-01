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

namespace Sif.Framework.Service.Authentication
{

    /// <summary>
    /// Delegate method for retrieving the shared secret used for authentication.
    /// </summary>
    /// <param name="sessionToken">Session token used to retrieve the shared secret.</param>
    /// <returns>Shared secret.</returns>
    public delegate string GetSharedSecret(string sessionToken);

    /// <summary>
    /// This interface defines operations associated with authorisation tokens.
    /// </summary>
    interface IAuthorisationTokenService
    {

        /// <summary>
        /// Generate an authorisation token.
        /// </summary>
        /// <param name="sessionToken">Session token.</param>
        /// <param name="sharedSecret">Shared secret.</param>
        /// <returns>Authorisation token.</returns>
        /// <exception cref="System.ArgumentNullException">Either sessionToken or sharedSecret is null or empty.</exception>
        AuthorisationToken Generate(string sessionToken, string sharedSecret);

        /// <summary>
        /// Verify an authorisation token.
        /// </summary>
        /// <param name="authorisationToken">Authorisation token to verify.</param>
        /// <param name="getSharedSecret">Delegate method for retrieving the shared secret.</param>
        /// <param name="sessionToken">Session token extracted from the authorisation token.</param>
        /// <returns>True if the authorisation token is valid; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">getSharedSecret is null.</exception>
        /// <exception cref="Model.Exceptions.InvalidAuthorisationTokenException">authorisationToken is null, not recognised or invalid.</exception>
        bool Verify(AuthorisationToken authorisationToken, GetSharedSecret getSharedSecret, out string sessionToken);

    }

}
