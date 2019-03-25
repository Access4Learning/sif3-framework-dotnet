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

using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Query;
using System.Collections.Generic;

namespace Sif.Framework.Service
{
    /// <summary>
    /// This interface defines the services available on SIF data model objects.
    /// </summary>
    /// <typeparam name="TSingle">Type that defines a single object entity.</typeparam>
    /// <typeparam name="TMultiple">Type that defines a multiple objects entity.</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the SIF data model object.</typeparam>
    public interface IObjectService<TSingle, TMultiple, TPrimaryKey> : IService
    {
        /// <summary>
        /// Create an object.
        /// </summary>
        /// <param name="obj">Object to create.</param>
        /// <param name="mustUseAdvisory">Flag to indicate whether the object's identifier should be retained.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <param name="requestParameters">Additional parameters associated with the request.</param>
        /// <exception cref="Model.Exceptions.AlreadyExistsException">Object already exists.</exception>
        /// <exception cref="System.ArgumentException">Parameter is invalid.</exception>
        /// <exception cref="Model.Exceptions.CreateException">Error creating object.</exception>
        /// <exception cref="Model.Exceptions.RejectedException">Create operation not valid for the given object.</exception>
        /// <returns>Created object.</returns>
        TSingle Create(
            TSingle obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters);

        /// <summary>
        /// Delete an object.
        /// </summary>
        /// <param name="refId">SIF identifier of the object.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <param name="requestParameters">Additional parameters associated with the request.</param>
        /// <exception cref="System.ArgumentException">Parameter is invalid.</exception>
        /// <exception cref="Model.Exceptions.DeleteException">Error deleting object.</exception>
        /// <exception cref="Model.Exceptions.NotFoundException">Object to delete not found.</exception>
        void Delete(
            TPrimaryKey refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters);

        /// <summary>
        /// Retrieve an object.
        /// </summary>
        /// <param name="refId">SIF identifier of the object.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <param name="requestParameters">Additional parameters associated with the request.</param>
        /// <exception cref="System.ArgumentException">Parameter is invalid.</exception>
        /// <exception cref="Model.Exceptions.QueryException">Error retrieving object.</exception>
        /// <returns>Retrieved object.</returns>
        TSingle Retrieve(
            TPrimaryKey refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters);

        /// <summary>
        /// Retrieve all objects (paged query).
        /// </summary>
        /// <param name="pageIndex">Current paging index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <param name="requestParameters">Additional parameters associated with the request.</param>
        /// <exception cref="System.ArgumentException">One or more parameters are invalid.</exception>
        /// <exception cref="Model.Exceptions.ContentTooLargeException">Too many objects to return.</exception>
        /// <exception cref="Model.Exceptions.QueryException">Error retrieving objects.</exception>
        /// <returns>Retrieved objects.</returns>
        TMultiple Retrieve(
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters);

        /// <summary>
        /// Retrieve multiple objects using Query by Example.
        /// </summary>
        /// <param name="obj">Example object.</param>
        /// <param name="pageIndex">Current paging index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <param name="requestParameters">Additional parameters associated with the request.</param>
        /// <exception cref="System.ArgumentException">One or more parameters are invalid.</exception>
        /// <exception cref="Model.Exceptions.ContentTooLargeException">Too many objects to return.</exception>
        /// <exception cref="Model.Exceptions.QueryException">Error retrieving objects.</exception>
        /// <returns>Retrieved objects.</returns>
        TMultiple Retrieve(
            TSingle obj,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters);

        /// <summary>
        /// Retrieve multiple objects using Service Paths.
        /// </summary>
        /// <param name="conditions">Service Path conditions</param>
        /// <param name="pageIndex">A (nullable) page index.</param>
        /// <param name="pageSize">A (nullable) size of elements in a page.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <param name="requestParameters">Additional parameters associated with the request.</param>
        /// <exception cref="System.ArgumentException">Parameter is invalid.</exception>
        /// <exception cref="Model.Exceptions.ContentTooLargeException">Too many objects to return.</exception>
        /// <exception cref="Model.Exceptions.QueryException">Error retrieving objects.</exception>
        /// <returns>Retrieved objects.</returns>
        TMultiple Retrieve(
            IEnumerable<EqualCondition> conditions,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters);

        /// <summary>
        /// Update an object.
        /// </summary>
        /// <param name="obj">Object to update</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <param name="requestParameters">Additional parameters associated with the request.</param>
        /// <exception cref="System.ArgumentException">Parameter is invalid.</exception>
        /// <exception cref="Model.Exceptions.NotFoundException">Object to update not found.</exception>
        /// <exception cref="Model.Exceptions.UpdateException">Error updating objects.</exception>
        void Update(
            TSingle obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters);
    }
}