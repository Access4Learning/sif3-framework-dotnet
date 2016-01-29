/*
 * Copyright 2016 Systemic Pty Ltd
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

using Sif.Framework.Model.DataModels;
using Sif.Framework.Model.Query;
using Sif.Framework.Model.Responses;
using System.Collections.Generic;

namespace Sif.Framework.Consumers
{

    /// <summary>
    /// This interface defines the operations available for Consumers of SIF data model objects.
    /// <para>Note that due to XML serialisation constraints, the TSingle and TMultiple generic types should not be
    /// interfaces. For instance, TMultiple should NOT be of type IEnumerable.</para>
    /// </summary>
    /// <typeparam name="TSingle">Type that defines a single object entity.</typeparam>
    /// <typeparam name="TMultiple">Type that defines a multiple objects entity.</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the SIF data model object.</typeparam>
    public interface IConsumer<TSingle, TMultiple, TPrimaryKey> : IPayloadSerialisable<TSingle, TMultiple>
    {

        /// <summary>
        /// This method must be called before any other.
        /// </summary>
        void Register();

        /// <summary>
        /// This method should be called on completion.
        /// </summary>
        /// <param name="deleteOnUnregister">True to remove session data on unregister; false to leave session data.</param>
        void Unregister(bool? deleteOnUnregister = null);

        /// <summary>
        /// Create a single object.
        /// <para>POST /StudentPersonals/StudentPersonal</para>
        /// </summary>
        /// <param name="obj">Object to create.</param>
        /// <param name="zone">Zone associated with the request.</param>
        /// <param name="context">Zone context.</param>
        /// <returns>Created object.</returns>
        TSingle Create(TSingle obj, string zone = null, string context = null);

        /// <summary>
        /// Create multiple objects.
        /// <para>POST /StudentPersonals</para>
        /// </summary>
        /// <param name="obj">Object (multiple object entity) to create.</param>
        /// <param name="zone">Zone associated with the request.</param>
        /// <param name="context">Zone context.</param>
        /// <returns>Response containing status of each object created.</returns>
        MultipleCreateResponse Create(TMultiple obj, string zone = null, string context = null);

        /// <summary>
        /// Retrieve a single object.
        /// <para>GET /StudentPersonals/{id}</para>
        /// </summary>
        /// <param name="refId">SIF identifier of the object.</param>
        /// <param name="zone">Zone associated with the request.</param>
        /// <param name="context">Zone context.</param>
        /// <returns>Retrieved object.</returns>
        TSingle Query(TPrimaryKey refId, string zone = null, string context = null);

        /// <summary>
        /// Retrieve all objects.
        /// <para>GET /StudentPersonals</para>
        /// </summary>
        /// <param name="navigationPage">Current paging index.</param>
        /// <param name="navigationPageSize">Page size.</param>
        /// <param name="zone">Zone associated with the request.</param>
        /// <param name="context">Zone context.</param>
        /// <returns>Retrieved objects.</returns>
        TMultiple Query(uint? navigationPage = null, uint? navigationPageSize = null, string zone = null, string context = null);

        /// <summary>
        /// Retrieve multiple objects using Query by Example.
        /// <para>POST /StudentPersonals (methodOverride: GET)</para>
        /// </summary>
        /// <param name="obj">Example object.</param>
        /// <param name="navigationPage">Current paging index.</param>
        /// <param name="navigationPageSize">Page size.</param>
        /// <param name="zone">Zone associated with the request.</param>
        /// <param name="context">Zone context.</param>
        /// <returns>Retrieved objects.</returns>
        TMultiple QueryByExample(TSingle obj, uint? navigationPage = null, uint? navigationPageSize = null, string zone = null, string context = null);

        /// <summary>
        /// Retrieve multiple objects using Service Paths.
        /// <para>GET api/{object1}/{id1}/{controller}</para>
        /// <para>GET api/{object1}/{id1}/{object2}/{id2}/{controller}</para>
        /// <para>GET api/{object1}/{id1}/{object2}/{id2}/{object3}/{id3}/{controller}</para>
        /// </summary>
        /// <param name="conditions">Service Path conditions.</param>
        /// <param name="navigationPage">Current paging index.</param>
        /// <param name="navigationPageSize">Page size.</param>
        /// <param name="zone">Zone associated with the request.</param>
        /// <param name="context">Zone context.</param>
        /// <returns>Retrieved objects.</returns>
        TMultiple QueryByServicePath(IEnumerable<EqualCondition> conditions, uint? navigationPage = null, uint? navigationPageSize = null, string zone = null, string context = null);

        /// <summary>
        /// Update a single object.
        /// <para>PUT /StudentPersonals/{id}</para>
        /// </summary>
        /// <param name="obj">Object to update.</param>
        /// <param name="zone">Zone associated with the request.</param>
        /// <param name="context">Zone context.</param>
        void Update(TSingle obj, string zone = null, string context = null);

        /// <summary>
        /// Update multiple objects.
        /// <para>PUT /StudentPersonals</para>
        /// </summary>
        /// <param name="obj">Object (multiple object entity) to update.</param>
        /// <param name="zone">Zone associated with the request.</param>
        /// <param name="context">Zone context.</param>
        /// <returns>Response containing status of each object updated.</returns>
        MultipleUpdateResponse Update(TMultiple obj, string zone = null, string context = null);

        /// <summary>
        /// Delete a single object.
        /// <para>DELETE /StudentPersonals/{id}</para>
        /// </summary>
        /// <param name="refId">SIF identifier of the object.</param>
        /// <param name="zone">Zone associated with the request.</param>
        /// <param name="context">Zone context.</param>
        void Delete(TPrimaryKey refId, string zone = null, string context = null);

        /// <summary>
        /// Delete multiple objects.
        /// <para>PUT /StudentPersonals (methodOverride: DELETE)</para>
        /// </summary>
        /// <param name="refIds">SIF identifiers of the objects.</param>
        /// <param name="zone">Zone associated with the request.</param>
        /// <param name="context">Zone context.</param>
        /// <returns>Response containing status of each object deleted.</returns>
        MultipleDeleteResponse Delete(IEnumerable<TPrimaryKey> refIds, string zone = null, string context = null);

    }

}
