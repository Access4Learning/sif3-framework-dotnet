/*
 * Copyright 2020 Systemic Pty Ltd
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

using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Responses;

namespace Sif.Framework.Service.Providers
{
    /// <summary>
    /// In addition to the operations provided by the IProviderService interface, this interface defines operations for
    /// the creation and update of the multiple objects type.
    /// </summary>
    /// <typeparam name="TSingle">Type that defines a single object entity.</typeparam>
    /// <typeparam name="TMultiple">Type that defines a multiple objects entity.</typeparam>
    public interface IObjectProviderService<TSingle, TMultiple> : IProviderService<TSingle, TMultiple>
    {
        /// <summary>
        /// Create multiple objects.
        /// </summary>
        /// <param name="obj">Object (multiple object entity) to create.</param>
        /// <param name="mustUseAdvisory">Flag to indicate whether the object's identifier should be retained.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <param name="requestParameters">Additional parameters associated with the request.</param>
        /// <exception cref="Model.Exceptions.AlreadyExistsException">Object already exists.</exception>
        /// <exception cref="System.ArgumentException">Parameter is invalid.</exception>
        /// <exception cref="Model.Exceptions.CreateException">Error creating object.</exception>
        /// <exception cref="Model.Exceptions.RejectedException">Create operation not valid for the given object.</exception>
        /// <returns>Response containing status of each object created.</returns>
        MultipleCreateResponse Create(
            TMultiple obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters);

        /// <summary>
        /// Update multiple objects.
        /// </summary>
        /// <param name="obj">Object (multiple object entity) to update</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <param name="requestParameters">Additional parameters associated with the request.</param>
        /// <exception cref="System.ArgumentException">Parameter is invalid.</exception>
        /// <exception cref="Model.Exceptions.NotFoundException">Object to update not found.</exception>
        /// <exception cref="Model.Exceptions.UpdateException">Error updating objects.</exception>
        /// <returns>Response containing status of each object updated.</returns>
        MultipleUpdateResponse Update(
            TMultiple obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters);
    }
}