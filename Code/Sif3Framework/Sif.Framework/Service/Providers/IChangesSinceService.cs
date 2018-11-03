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

namespace Sif.Framework.Service.Providers
{
    /// <summary>
    /// This interface defines operations associated with the "Changes Since" mechanism.
    /// </summary>
    /// <typeparam name="TMultiple">Type that defines a SIF data model object.</typeparam>
    public interface IChangesSinceService<TMultiple> : ISupportsChangesSince
    {
        /// <summary>
        /// Retrieve all objects which have changed since a given point (as indicated by the Changes Since marker).
        /// <para/>
        /// <para>The following rules specify how the collection of changed objects should be interpreted:</para>
        /// <para/>
        /// <list type="bullet">
        ///     <item>If the refId does not exist, the object is to be added.</item>
        ///     <item>If the refId exists, the object is to be updated.</item>
        ///     <item>If the object contains a refId value only, the object is to be deleted.</item>
        /// </list>
        /// </summary>
        /// <param name="changesSinceMarker">Changes Since marker.</param>
        /// <param name="pageIndex">Current paging index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <param name="requestParameters">Additional parameters associated with the request.</param>
        /// <exception cref="System.ArgumentException">One or more parameters are invalid.</exception>
        /// <exception cref="Model.Exceptions.ContentTooLargeException">Too many objects to return.</exception>
        /// <exception cref="Model.Exceptions.QueryException">Error retrieving objects.</exception>
        /// <returns>Retrieved objects.</returns>
        TMultiple RetrieveChangesSince(
            string changesSinceMarker,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters);
    }
}