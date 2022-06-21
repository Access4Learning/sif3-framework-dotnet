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

using Sif.Framework.Model.Infrastructure;
using Tardigrade.Framework.Services;

namespace Sif.Framework.Service.Infrastructure
{
    /// <summary>
    /// Service interface for operations associated with the ApplicationRegister object.
    /// </summary>
    public interface IApplicationRegisterService : IObjectService<ApplicationRegister, long>
    {
        /// <summary>
        /// Retrieve an Application Register based upon the application key provided.
        /// </summary>
        /// <param name="applicationKey">Application key.</param>
        /// <returns>Application Register associated with the application key if found; null otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">Parameter is null or empty.</exception>
        /// <exception cref="System.InvalidOperationException">Multiple Application Registers are associated with the application key.</exception>
        ApplicationRegister RetrieveByApplicationKey(string applicationKey);
    }
}