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
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Registration
{

    /// <summary>
    /// <see cref="IRegistrationService">IRegistrationService</see>
    /// </summary>
    public class NoRegistrationService : IRegistrationService
    {

        /// <summary>
        /// <see cref="IRegistrationService.AuthorisationToken">AuthorisationToken</see>
        /// </summary>
        public AuthorisationToken AuthorisationToken { get; private set; }

        /// <summary>
        /// <see cref="IRegistrationService.Registered">Registered</see>
        /// </summary>
        public bool Registered { get; private set; }

        /// <summary>
        /// Create an instance.
        /// </summary>
        public NoRegistrationService()
        {
            AuthorisationToken = null;
            Registered = false;
        }

        /// <summary>
        /// <see cref="IRegistrationService.Register()">Register</see>
        /// </summary>
        public Environment Register()
        {
            Registered = true;
            return null;
        }

        /// <summary>
        /// <see cref="IRegistrationService.Register(ref Environment)">Register</see>
        /// </summary>
        public Environment Register(ref Environment environment)
        {
            Registered = true;
            return null;
        }

        /// <summary>
        /// <see cref="IRegistrationService.Unregister(bool?)">Unregister</see>
        /// </summary>
        public void Unregister(bool? deleteOnUnregister = null)
        {
            Registered = false;
        }

    }

}
