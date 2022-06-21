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

using System;
using Tardigrade.Framework.Services;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Infrastructure
{
    /// <summary>
    /// Service interface for operations associated with the Environment object.
    /// </summary>
    public interface IEnvironmentService : IObjectService<Environment, Guid>
    {
        /// <summary>
        /// Retrieve an Environment based upon the session token provided.
        /// </summary>
        /// <param name="sessionToken">Session token.</param>
        /// <returns>Environment associated with the session token.</returns>
        Environment RetrieveBySessionToken(string sessionToken);
    }
}