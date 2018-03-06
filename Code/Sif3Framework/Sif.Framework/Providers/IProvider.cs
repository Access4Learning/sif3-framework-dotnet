/*
 * Copyright 2017 Systemic Pty Ltd
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

using Sif.Specification.Infrastructure;
using System.Web.Http;

namespace Sif.Framework.Providers
{

    /// <summary>
    /// This interface defines the operations available for Providers of SIF data model objects.
    /// </summary>
    /// <typeparam name="TSingle">Type that defines a single object entity.</typeparam>
    /// <typeparam name="TMultiple">Type that defines a multiple objects entity.</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the SIF data model object.</typeparam>
    interface IProvider<TSingle, TMultiple, TPrimaryKey>
    {

        /// <summary>
        /// Create a single object.
        /// <para>POST api/{controller}/TSingle</para>
        /// <para/>
        /// <para>201 - Success, object created</para>
        /// <para>400 - Failue, bad request</para>
        /// <para>401 - Failure, unauthorised</para>
        /// <para>403 - Failure, forbidden</para>
        /// <para>404 - Failure, not found (reject mustUseAdvisory)</para>
        /// <para>409 - Failure, state conflict (already exists)</para>
        /// <para>500 - Failure, internal service error</para>
        /// </summary>
        /// <param name="obj">Object to create.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>Object created (TSingle).</returns>
        IHttpActionResult Post(TSingle obj, string[] zoneId = null, string[] contextId = null);

        /// <summary>
        /// Create multiple objects.
        /// <para>POST api/{controller}</para>
        /// <para/>
        /// <para>200 - Success, ok</para>
        /// <para>400 - Failue, bad request</para>
        /// <para>401 - Failure, unauthorised</para>
        /// <para>403 - Failure, forbidden</para>
        /// <para>500 - Failure, internal service error</para>
        /// </summary>
        /// <param name="obj">Object (multiple object entity) to create.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>Response containing status of each object created (createResponseType).</returns>
        IHttpActionResult Post(TMultiple obj, string[] zoneId = null, string[] contextId = null);

        /// <summary>
        /// Retrieve a single object.
        /// <para>GET api/{controller}/{id}</para>
        /// <para/>
        /// <para>200 - Success, ok</para>
        /// <para>400 - Failue, bad request</para>
        /// <para>401 - Failure, unauthorised</para>
        /// <para>403 - Failure, forbidden</para>
        /// <para>404 - Failure, not found</para>
        /// <para>405 - Failure, method not allowed (paged query request issued)</para>
        /// <para>500 - Failure, internal service error</para>
        /// </summary>
        /// <param name="refId">SIF identifier of the object.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>Object with that SIF identifier (TSingle).</returns>
        IHttpActionResult Get(TPrimaryKey refId, string[] zoneId = null, string[] contextId = null);

        /// <summary>
        /// Retrieve all objects.
        /// <para>GET api/{controller} -> where obj is null</para>
        /// <para/>
        /// <para>Retrieve multiple objects using Query by Example.</para>
        /// <para>GET api/{controller} -> POST api/{controller} where methodOverride=GET and obj is not null</para>vvvv
        /// <para/>
        /// <para>200 - Success, ok</para>
        /// <para>204 - Success, no content</para>
        /// <para>400 - Failue, bad request</para>
        /// <para>401 - Failure, unauthorised</para>
        /// <para>403 - Failure, forbidden</para>
        /// <para>413 - Failure, response too large</para>
        /// <para>500 - Failure, internal service error</para>
        /// </summary>
        /// <param name="obj">Example object to base the query on.</param>
        /// <param name="changesSinceMarker">Changes Since marker.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>All objects, objects that match the properties of the example object or no objects (TMultiple).</returns>
        IHttpActionResult Get(TSingle obj, string changesSinceMarker = null, string[] zoneId = null, string[] contextId = null);

        /// <summary>
        /// Retrieve multiple objects using Service Paths.
        /// <para>GET api/{object1}/{id1}/{controller}</para>
        /// <para>GET api/{object1}/{id1}/{object2}/{id2}/{controller}</para>
        /// <para>GET api/{object1}/{id1}/{object2}/{id2}/{object3}/{id3}/{controller}</para>
        /// <para/>
        /// <para>This method implements Service Paths with up to 3 levels of association.</para>
        /// <para/>
        /// <para>200 - Success, ok</para>
        /// <para>204 - Success, no content</para>
        /// <para>400 - Failue, bad request</para>
        /// <para>401 - Failure, unauthorised</para>
        /// <para>403 - Failure, forbidden</para>
        /// <para>413 - Failure, response too large</para>
        /// <para>500 - Failure, internal service error</para>
        /// </summary>
        /// <param name="object1">Associated object.</param>
        /// <param name="id1">Identifier of associated object.</param>
        /// <param name="object2">Associated object.</param>
        /// <param name="id2">Identifier of associated object.</param>
        /// <param name="object3">Associated object.</param>
        /// <param name="id3">Identifier of associated object.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>Objects that meet the associated object and identifier pairs (TMultiple).</returns>
        IHttpActionResult Get(string object1, string id1, string object2 = null, string id2 = null, string object3 = null, string id3 = null, string[] zoneId = null, string[] contextId = null);

        /// <summary>
        /// Update a single object.
        /// <para>PUT api/{controller}/{id}</para>
        /// <para/>
        /// <para>204 - Success, no content</para>
        /// <para>400 - Failue, bad request</para>
        /// <para>401 - Failure, unauthorised</para>
        /// <para>403 - Failure, forbidden</para>
        /// <para>404 - Failure, not found</para>
        /// <para>500 - Failure, internal service error</para>
        /// </summary>
        /// <param name="refId">SIF identifier of the object.</param>
        /// <param name="obj">Object to update.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>Result of the update request (void).</returns>
        IHttpActionResult Put(TPrimaryKey refId, TSingle obj, string[] zoneId = null, string[] contextId = null);

        /// <summary>
        /// Update multiple objects.
        /// <para>PUT api/{controller}</para>
        /// <para/>
        /// <para>200 - Success, ok</para>
        /// <para>400 - Failue, bad request</para>
        /// <para>401 - Failure, unauthorised</para>
        /// <para>403 - Failure, forbidden</para>
        /// <para>500 - Failure, internal service error</para>
        /// </summary>
        /// <param name="obj">Object (multiple object entity) to update.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>Response containing status of each object updated (updateResponseType).</returns>
        IHttpActionResult Put(TMultiple obj, string[] zoneId = null, string[] contextId = null);

        /// <summary>
        /// Delete a single object.
        /// <para>DELETE api/{controller}/{id}</para>
        /// <para/>
        /// <para>204 - Success, no content</para>
        /// <para>400 - Failue, bad request</para>
        /// <para>401 - Failure, unauthorised</para>
        /// <para>403 - Failure, forbidden</para>
        /// <para>404 - Failure, not found</para>
        /// <para>500 - Failure, internal service error</para>
        /// </summary>
        /// <param name="refId">SIF identifier of the object.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>Result of the delete request (void).</returns>
        IHttpActionResult Delete(TPrimaryKey refId, string[] zoneId = null, string[] contextId = null);

        /// <summary>
        /// Delete multiple objects.
        /// <para>DELETE api/{controller} -> PUT api/{controller} with methodOverride=DELETE</para>vv
        /// <para/>
        /// <para>200 - Success, ok</para>
        /// <para>400 - Failue, bad request</para>
        /// <para>401 - Failure, unauthorised</para>
        /// <para>403 - Failure, forbidden</para>
        /// <para>500 - Failure, internal service error</para>
        /// </summary>
        /// <param name="deleteRequest">Request containing a collection of SIF identifiers of the objects to delete.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>Response containing status of each object deleted (deleteResponseType).</returns>
        IHttpActionResult Delete(deleteRequestType deleteRequest, string[] zoneId = null, string[] contextId = null);

        /// <summary>
        /// Retrieve the message headers associated with a call to retrieve all objects.
        /// <para>HEAD api/{controller}</para>
        /// <para/>
        /// <para>200 - Success, ok</para>
        /// <para>204 - Success, no content</para>
        /// <para>400 - Failue, bad request</para>
        /// <para>401 - Failure, unauthorised</para>
        /// <para>403 - Failure, forbidden</para>
        /// <para>413 - Failure, response too large</para>
        /// <para>500 - Failure, internal service error</para>
        /// </summary>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>No objects, just message header information (void).</returns>
        IHttpActionResult Head(string[] zoneId = null, string[] contextId = null);

        /// <summary>
        /// Broadcast SIF Events.
        /// <para/>
        /// <para>200 - Success, ok</para>
        /// <para>400 - Failue, bad request</para>
        /// <para>401 - Failure, unauthorised</para>
        /// <para>500 - Failure, internal service error</para>
        /// </summary>
        /// <param name="zoneId">Zone associated with the SIF Events.</param>
        /// <param name="contextId">Zone context.</param>
        /// <returns>Ok</returns>
        IHttpActionResult BroadcastEvents(string zoneId = null, string contextId = null);

    }

}
