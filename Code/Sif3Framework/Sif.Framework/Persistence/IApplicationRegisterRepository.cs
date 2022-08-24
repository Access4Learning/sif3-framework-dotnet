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

using Sif.Framework.Models.Infrastructure;
using Tardigrade.Framework.Persistence;

namespace Sif.Framework.Persistence
{
    /// <summary>
    /// Repository interface for operations associated with the ApplicationRegister object.
    /// </summary>
    public interface IApplicationRegisterRepository : IRepository<ApplicationRegister, long>
    {
        /// <summary>
        /// Retrieve the Application Register based upon it's application key.
        /// </summary>
        /// <param name="applicationKey">Application key for the Application Register.</param>
        /// <returns>Application Register defined by the passed application key if exists; null otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">Parameter is null or empty.</exception>
        /// <exception cref="Tardigrade.Framework.Exceptions.DuplicateFoundException">Multiple Application Registers are associated with the application key.</exception>
        /// <exception cref="Tardigrade.Framework.Exceptions.RepositoryException">A general repository failure occurred.</exception>
        ApplicationRegister RetrieveByApplicationKey(string applicationKey);
    }
}