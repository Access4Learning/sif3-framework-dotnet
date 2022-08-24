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
    /// Repository interface for operations associated with the EnvironmentRegister object.
    /// </summary>
    public interface IEnvironmentRegisterRepository : IRepository<EnvironmentRegister, long>
    {
        /// <summary>
        /// Retrieve the Environment Register based upon the parameter values passed.
        /// </summary>
        /// <param name="applicationKey">Application key.</param>
        /// <param name="instanceId">Instance identifier (optional).</param>
        /// <param name="userToken">User token (optional).</param>
        /// <param name="solutionId">Solution identifier (optional).</param>
        /// <returns>Environment Register defined by the parameter values passed if exists; null otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">applicationKey is null or empty.</exception>
        /// <exception cref="Tardigrade.Framework.Exceptions.DuplicateFoundException">Multiple Environment Registers are associated with the parameter values passed.</exception>
        /// <exception cref="Tardigrade.Framework.Exceptions.RepositoryException">A general repository failure occurred.</exception>
        EnvironmentRegister RetrieveByUniqueIdentifiers(
            string applicationKey,
            string instanceId,
            string userToken,
            string solutionId);
    }
}