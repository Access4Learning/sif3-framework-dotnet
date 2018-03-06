/*
 * Copyright 2018 Systemic Pty Ltd
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
using Sif.Framework.Model.Infrastructure;

namespace Sif.Framework.Service.Registration
{

    /// <summary>
    /// This class represents operations for registering and unregistering a service (Consumer or Provider) with an
    /// Environment service (i.e. SIF 3 Broker, Environment Provider).
    /// </summary>
    public interface IRegistrationService
    {

        /// <summary>
        /// Authorisation token generated from registration.
        /// </summary>
        AuthorisationToken AuthorisationToken { get; }

        /// <summary>
        /// Flag to indicate whether the service has been registered.
        /// </summary>
        bool Registered { get; }

        /// <summary>
        /// Register service with an Environment service (Broker or Environment Provider). This must be the first
        /// method called, and only once. Subsequent calls are ignored.
        /// </summary>
        /// <exception cref="Model.Exceptions.RegistrationException">Error occurred during registration.</exception>
        Environment Register();

        /// <summary>
        /// Register service with an Environment service (Broker or Environment Provider). This must be the first
        /// method called, and only once. Subsequent calls are ignored.
        /// </summary>
        /// <param name="environment">Environment used for registration.</param>
        /// <exception cref="Model.Exceptions.RegistrationException">Error occurred during registration.</exception>
        Environment Register(ref Environment environment);

        /// <summary>
        /// Unregister service from an Environment service (Broker or Environment Provider). This must be the last
        /// method called.
        /// </summary>
        /// <param name="deleteOnUnregister">If true, remove session data on unregister. If false, maintain session
        /// data. If null, use the setting stored in SifFramework.config.</param>
        void Unregister(bool? deleteOnUnregister = null);

    }

}
