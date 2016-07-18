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

using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Registration
{

    /// <summary>
    /// <see cref="Sif.Framework.Service.Registration.IRegistrationService">IRegistrationService</see>
    /// </summary>
    public class NoRegistrationService : IRegistrationService
    {

        /// <summary>
        /// <see cref="Sif.Framework.Service.Registration.IRegistrationService.AuthorisationToken">AuthorisationToken</see>
        /// </summary>
        public string AuthorisationToken { get; private set; }

        /// <summary>
        /// <see cref="Sif.Framework.Service.Registration.IRegistrationService.Registered">Registered</see>
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
        /// <see cref="Sif.Framework.Service.Registration.IRegistrationService.Register()">Register</see>
        /// </summary>
        public Environment Register()
        {
            Registered = true;
            return null;
        }

        /// <summary>
        /// <see cref="Sif.Framework.Service.Registration.IRegistrationService.Register(Sif.Framework.Model.Infrastructure.Environment)">Register</see>
        /// </summary>
        public Environment Register(ref Environment environment)
        {
            Registered = true;
            return null;
        }

        /// <summary>
        /// <see cref="Sif.Framework.Service.Registration.IRegistrationService.Unregister(bool?)">Unregister</see>
        /// </summary>
        public void Unregister(bool? deleteOnUnregister = null)
        {
            Registered = false;
        }

    }

}
