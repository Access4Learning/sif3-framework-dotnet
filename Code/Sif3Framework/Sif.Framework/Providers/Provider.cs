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

using Sif.Framework.Extensions;
using Sif.Framework.Model.Authentication;
using Sif.Framework.Model.DataModels;
using Sif.Framework.Model.Events;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Query;
using Sif.Framework.Model.Requests;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Authentication;
using Sif.Framework.Service.Authorisation;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Service.Providers;
using Sif.Framework.Service.Registration;
using Sif.Framework.Utils;
using Sif.Framework.WebApi.ModelBinders;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sif.Framework.Providers
{
    /// <summary>
    /// This class defines a Provider of SIF data model objects.
    /// </summary>
    /// <typeparam name="TSingle">Type that defines a single object entity.</typeparam>
    /// <typeparam name="TMultiple">Type that defines a multiple objects entity.</typeparam>
    public abstract class Provider<TSingle, TMultiple>
        : ApiController, IProvider<TSingle, TMultiple, string>, IEventPayloadSerialisable<TMultiple>
        where TSingle : ISifRefId<string>
    {
        /// <summary>
        /// Accepted content type (XML or JSON) for a message payload.
        /// </summary>
        protected Accept Accept => ProviderSettings.Accept;

        /// <summary>
        /// Service used for request authentication.
        /// </summary>
        protected IAuthenticationService AuthenticationService { get; }

        /// <summary>
        /// Service used for request authorisation.
        /// </summary>
        protected IAuthorisationService AuthorisationService { get; }

        /// <summary>
        /// Content type (XML or JSON) of the message payload.
        /// </summary>
        protected ContentType ContentType => ProviderSettings.ContentType;

        /// <summary>
        /// Application settings associated with the Provider.
        /// </summary>
        protected IFrameworkSettings ProviderSettings { get; }

        /// <summary>
        /// Object service associated with this Provider.
        /// </summary>
        protected IProviderService<TSingle, TMultiple> Service { get; }

        /// <summary>
        /// Name of the SIF data model that the Provider is based on, e.g. SchoolInfo, StudentPersonal, etc.
        /// </summary>
        protected virtual string TypeName => typeof(TSingle).Name;

        /// <summary>
        /// Create an instance based on the specified service.
        /// </summary>
        /// <param name="service">Service used for managing the object type.</param>
        /// <param name="settings">Provider settings. If null, Provider settings will be read from the SifFramework.config file.</param>
        protected Provider(IProviderService<TSingle, TMultiple> service, IFrameworkSettings settings = null)
        {
            Service = service;
            ProviderSettings = settings ?? SettingsManager.ProviderSettings;

            if (EnvironmentType.DIRECT.Equals(ProviderSettings.EnvironmentType))
            {
                AuthenticationService =
                    new DirectAuthenticationService(new ApplicationRegisterService(), new EnvironmentService());
            }
            else if (EnvironmentType.BROKERED.Equals(ProviderSettings.EnvironmentType))
            {
                AuthenticationService = new BrokeredAuthenticationService(
                    new ApplicationRegisterService(),
                    new EnvironmentService(),
                    settings,
                    SessionsManager.ProviderSessionService);
            }

            AuthorisationService = new AuthorisationService(AuthenticationService);
        }

        /// <summary>
        /// Get the query parameters associated with the HTTP Request.
        /// </summary>
        /// <param name="request">HTTP Request.</param>
        /// <returns>Query parameters if found; an empty collection otherwise.</returns>
        protected RequestParameter[] GetQueryParameters(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request
                .GetQueryNameValuePairs()
                .Select(kv => new RequestParameter(kv.Key, ConveyanceType.QueryParameter, kv.Value))
                .ToArray();
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Post(TTSingle, string[], string[])">Post</see>
        /// </summary>
        public virtual IHttpActionResult Post(
            TSingle obj,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.
            if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.CREATE))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                bool hasAdvisoryId = !string.IsNullOrWhiteSpace(obj.RefId);
                bool? mustUseAdvisory = HttpUtils.GetMustUseAdvisory(Request.Headers);

                if (mustUseAdvisory.HasValue)
                {
                    if (mustUseAdvisory.Value && !hasAdvisoryId)
                    {
                        result = BadRequest(
                            $"Request failed for object {TypeName} as object ID is not provided, but mustUseAdvisory is true.");
                    }
                    else
                    {
                        RequestParameter[] requestParameters = GetQueryParameters(Request);
                        TSingle createdObject =
                            Service.Create(obj, mustUseAdvisory, zoneId?[0], contextId?[0], requestParameters);
                        string uri = Url.Link("DefaultApi", new { controller = TypeName, id = createdObject.RefId });
                        result = Created(uri, createdObject);
                    }
                }
                else
                {
                    RequestParameter[] requestParameters = GetQueryParameters(Request);
                    TSingle createdObject = Service.Create(obj, null, zoneId?[0], contextId?[0], requestParameters);
                    string uri = Url.Link("DefaultApi", new { controller = TypeName, id = createdObject.RefId });
                    result = Created(uri, createdObject);
                }
            }
            catch (AlreadyExistsException)
            {
                result = Conflict();
            }
            catch (ArgumentException e)
            {
                result = BadRequest($"Object to create of type {TypeName} is invalid.\n{e.Message}");
            }
            catch (CreateException e)
            {
                result = BadRequest($"Request failed for object {TypeName}.\n{e.Message}");
            }
            catch (RejectedException e)
            {
                result = this.NotFound(
                    $"Create request rejected for object {TypeName} with ID of {obj.RefId}.\n{e.Message}");
            }
            catch (QueryException e)
            {
                result = BadRequest($"Request failed for object {TypeName}.\n{e.Message}");
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Post(TMultiple, string[], string[])">Post</see>
        /// </summary>
        public abstract IHttpActionResult Post(TMultiple obj, string[] zoneId = null, string[] contextId = null);

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Get(TPrimaryKey, string[], string[])">Get</see>
        /// </summary>
        public virtual IHttpActionResult Get(
            [FromUri(Name = "id")] string refId,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.
            if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.QUERY))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (HttpUtils.HasPagingHeaders(Request.Headers))
            {
                return StatusCode(HttpStatusCode.MethodNotAllowed);
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                RequestParameter[] requestParameters = GetQueryParameters(Request);
                TSingle obj = Service.Retrieve(refId, zoneId?[0], contextId?[0], requestParameters);

                if (obj == null)
                {
                    result = StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    result = Ok(obj);
                }
            }
            catch (ArgumentException e)
            {
                result = BadRequest($"Invalid argument: id={refId}.\n{e.Message}");
            }
            catch (QueryException e)
            {
                result = BadRequest($"Request failed for object {TypeName} with ID of {refId}.\n{e.Message}");
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        /// <summary>
        /// Retrieve all objects.
        /// </summary>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <exception cref="ArgumentException">One or more parameters of the Provider service call are invalid.</exception>
        /// <exception cref="ContentTooLargeException">Too many objects to return.</exception>
        /// <exception cref="QueryException">Error retrieving objects.</exception>
        /// <exception cref="Exception">Catch all for exceptions thrown by the implementation of the Provider service interface.</exception>
        /// <returns>All objects.</returns>
        private IHttpActionResult GetAll(string zoneId, string contextId)
        {
            if (HttpUtils.HasMethodOverrideHeader(Request.Headers))
            {
                return BadRequest("GET (Query by Example) request failed due to missing payload.");
            }

            uint? navigationPage = HttpUtils.GetNavigationPage(Request.Headers);
            uint? navigationPageSize = HttpUtils.GetNavigationPageSize(Request.Headers);
            RequestParameter[] requestParameters = GetQueryParameters(Request);
            TMultiple objs =
                Service.Retrieve(navigationPage, navigationPageSize, zoneId, contextId, requestParameters);
            IHttpActionResult result;

            if (objs == null)
            {
                result = StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                result = Ok(objs);
            }

            return result;
        }

        /// <summary>
        /// Retrieve objects based on the Changes Since marker.
        /// </summary>
        /// <param name="changesSinceMarker">Changes Since marker.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <exception cref="ArgumentException">One or more parameters of the Provider service call are invalid.</exception>
        /// <exception cref="ContentTooLargeException">Too many objects to return.</exception>
        /// <exception cref="QueryException">Error retrieving objects.</exception>
        /// <exception cref="Exception">Catch all for exceptions thrown by the implementation of the Provider service interface.</exception>
        /// <returns>Objects associated with the Changes Since marker.</returns>
        private IHttpActionResult GetChangesSince(string changesSinceMarker, string zoneId, string contextId)
        {
            if (HttpUtils.HasMethodOverrideHeader(Request.Headers))
            {
                return BadRequest("The Changes Since marker is not applicable for a GET (Query by Example) request.");
            }

            bool changesSinceRequested = !string.IsNullOrWhiteSpace(changesSinceMarker);
            var changesSinceService = Service as IChangesSinceService<TMultiple>;
            bool changesSinceSupported = (changesSinceService != null);

            if (changesSinceRequested && !changesSinceSupported)
            {
                return BadRequest("The Changes Since request is not supported.");
            }

            uint? navigationPage = HttpUtils.GetNavigationPage(Request.Headers);
            uint? navigationPageSize = HttpUtils.GetNavigationPageSize(Request.Headers);
            RequestParameter[] requestParameters = GetQueryParameters(Request);
            TMultiple objs = changesSinceService.RetrieveChangesSince(
                changesSinceMarker,
                navigationPage,
                navigationPageSize,
                zoneId,
                contextId,
                requestParameters);
            IHttpActionResult result;

            if (objs == null)
            {
                result = StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                result = Ok(objs);
            }

            bool pagedRequest = navigationPage.HasValue && navigationPageSize.HasValue;
            bool firstPage = (navigationPage.HasValue && navigationPage.Value == 1);

            // Changes Since marker is only returned for non-paged requests or the first page of a paged request.
            if (!pagedRequest || firstPage)
            {
                try
                {
                    result = result.AddHeader(
                        "changesSinceMarker",
                        changesSinceService.NextChangesSinceMarker ?? string.Empty);
                }
                catch (Exception)
                {
                    throw new QueryException(
                        "Implementation to retrieve the next Changes Since marker returned an error.");
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieve objects using Query by Example.
        /// </summary>
        /// <param name="obj">Example object.</param>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        /// <exception cref="ArgumentException">One or more parameters of the Provider service call are invalid.</exception>
        /// <exception cref="ContentTooLargeException">Too many objects to return.</exception>
        /// <exception cref="QueryException">Error retrieving objects.</exception>
        /// <exception cref="Exception">Catch all for exceptions thrown by the implementation of the Provider service interface.</exception>
        /// <returns>Objects which match the Query by Example.</returns>
        private IHttpActionResult GetQueryByExample(TSingle obj, string zoneId, string contextId)
        {
            if (!HttpUtils.HasMethodOverrideHeader(Request.Headers))
            {
                return BadRequest("GET (Query by Example) request failed due to a missing method override header.");
            }

            uint? navigationPage = HttpUtils.GetNavigationPage(Request.Headers);
            uint? navigationPageSize = HttpUtils.GetNavigationPageSize(Request.Headers);
            RequestParameter[] requestParameters = GetQueryParameters(Request);
            TMultiple objs =
                Service.Retrieve(obj, navigationPage, navigationPageSize, zoneId, contextId, requestParameters);
            IHttpActionResult result;

            if (objs == null)
            {
                result = StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                result = Ok(objs);
            }

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Get(TTSingle, string, string[], string[])">Get</see>
        /// </summary>
        public virtual IHttpActionResult Get(
            TSingle obj,
            string changesSinceMarker = null,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.
            if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.QUERY))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (!HttpUtils.ValidatePagingParameters(Request.Headers, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                if (obj == null)
                {
                    result = changesSinceMarker == null
                        ? GetAll(zoneId?[0], contextId?[0])
                        : GetChangesSince(changesSinceMarker, zoneId?[0], contextId?[0]);
                }
                else
                {
                    result = GetQueryByExample(obj, zoneId?[0], contextId?[0]);
                }
            }
            catch (ArgumentException e)
            {
                result = BadRequest($"One or more parameters of the GET request are invalid.\n{e.Message}");
            }
            catch (QueryException e)
            {
                result = BadRequest($"GET request failed for object {TypeName}.\n{e.Message}");
            }
            catch (ContentTooLargeException)
            {
                result = StatusCode(HttpStatusCode.RequestEntityTooLarge);
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Get(string, string, string, string, string, string, string[], string[])">Get</see>
        /// </summary>
        public virtual IHttpActionResult Get(
            string object1,
            [FromUri(Name = "id1")] string refId1,
            string object2 = null,
            [FromUri(Name = "id2")] string refId2 = null,
            string object3 = null,
            [FromUri(Name = "id3")] string refId3 = null,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
            {
                return Unauthorized();
            }

            string serviceName;

            if (object3 != null)
            {
                serviceName = $"{object1}/{{}}/{object2}/{{}}/{object3}/{{}}/{TypeName}s";
            }
            else if (object2 != null)
            {
                serviceName = $"{object1}/{{}}/{object2}/{{}}/{TypeName}s";
            }
            else
            {
                serviceName = $"{object1}/{{}}/{TypeName}s";
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.
            if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, serviceName, RightType.QUERY))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (!HttpUtils.ValidatePagingParameters(Request.Headers, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                IList<EqualCondition> conditions =
                    new List<EqualCondition>() { new EqualCondition() { Left = object1, Right = refId1 } };

                if (!string.IsNullOrWhiteSpace(object2))
                {
                    conditions.Add(new EqualCondition() { Left = object2, Right = refId2 });

                    if (!string.IsNullOrWhiteSpace(object3))
                    {
                        conditions.Add(new EqualCondition() { Left = object3, Right = refId3 });
                    }
                }

                uint? navigationPage = HttpUtils.GetNavigationPage(Request.Headers);
                uint? navigationPageSize = HttpUtils.GetNavigationPageSize(Request.Headers);
                RequestParameter[] requestParameters = GetQueryParameters(Request);
                TMultiple objs = Service.Retrieve(
                    conditions,
                    navigationPage,
                    navigationPageSize,
                    zoneId?[0],
                    contextId?[0],
                    requestParameters);

                if (objs == null)
                {
                    result = StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    result = Ok(objs);
                }
            }
            catch (ArgumentException e)
            {
                result = BadRequest($"One or more conditions are invalid.\n{e.Message}");
            }
            catch (QueryException e)
            {
                result = BadRequest($"Service Path GET request failed for object {TypeName}.\n{e.Message}");
            }
            catch (ContentTooLargeException)
            {
                result = StatusCode(HttpStatusCode.RequestEntityTooLarge);
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Put(TPrimaryKey, TTSingle, string[], string[])">Put</see>
        /// </summary>
        public virtual IHttpActionResult Put(
            [FromUri(Name = "id")] string refId,
            TSingle obj,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.
            if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.UPDATE))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (string.IsNullOrWhiteSpace(refId) || obj == null || !refId.Equals(obj.RefId))
            {
                return BadRequest(
                    "The refId in the update request does not match the SIF identifier of the object provided.");
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                RequestParameter[] requestParameters = GetQueryParameters(Request);
                Service.Update(obj, zoneId?[0], contextId?[0], requestParameters);
                result = StatusCode(HttpStatusCode.NoContent);
            }
            catch (ArgumentException e)
            {
                result = BadRequest($"Object to update of type {TypeName} is invalid.\n{e.Message}");
            }
            catch (NotFoundException e)
            {
                result = this.NotFound($"Object {TypeName} with ID of {refId} not found.\n{e.Message}");
            }
            catch (UpdateException e)
            {
                result = BadRequest($"Request failed for object {TypeName} with ID of {refId}.\n{e.Message}");
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Put(TMultiple, string[], string[])">Put</see>
        /// </summary>
        public abstract IHttpActionResult Put(TMultiple obj, string[] zoneId = null, string[] contextId = null);

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Delete(TPrimaryKey, string[], string[])">Delete</see>
        /// </summary>
        public virtual IHttpActionResult Delete(
            [FromUri(Name = "id")] string refId,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.
            if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.DELETE))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                RequestParameter[] requestParameters = GetQueryParameters(Request);
                Service.Delete(refId, zoneId?[0], contextId?[0], requestParameters);
                result = StatusCode(HttpStatusCode.NoContent);
            }
            catch (ArgumentException e)
            {
                result = BadRequest($"Invalid argument: id={refId}.\n{e.Message}");
            }
            catch (DeleteException e)
            {
                result = BadRequest($"Request failed for object {TypeName} with ID of {refId}.\n{e.Message}");
            }
            catch (NotFoundException e)
            {
                result = this.NotFound($"Object {TypeName} with ID of {refId} not found.\n{e.Message}");
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Delete(deleteRequestType, string[], string[])">Delete</see>
        /// </summary>
        public virtual IHttpActionResult Delete(
            deleteRequestType deleteRequest,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.
            if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.DELETE))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            ICollection<deleteStatus> deleteStatuses = new List<deleteStatus>();

            try
            {
                foreach (deleteIdType deleteId in deleteRequest.deletes)
                {
                    var status = new deleteStatus
                    {
                        id = deleteId.id
                    };

                    try
                    {
                        RequestParameter[] requestParameters = GetQueryParameters(Request);
                        Service.Delete(deleteId.id, zoneId?[0], contextId?[0], requestParameters);
                        status.statusCode = ((int)HttpStatusCode.NoContent).ToString();
                    }
                    catch (ArgumentException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.BadRequest,
                            TypeName,
                            $"Invalid argument: id={deleteId.id}.\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (DeleteException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.BadRequest,
                            TypeName,
                            $"Request failed for object {TypeName} with ID of {deleteId.id}.\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (NotFoundException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.NotFound,
                            TypeName,
                            $"Object {TypeName} with ID of {deleteId.id} not found.\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.NotFound).ToString();
                    }
                    catch (Exception e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.InternalServerError,
                            TypeName,
                            $"Request failed for object {TypeName} with ID of {deleteId.id}.\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.InternalServerError).ToString();
                    }

                    deleteStatuses.Add(status);
                }
            }
            catch (Exception)
            {
                // Need to ignore exceptions otherwise it would not be possible to return status records of processed objects.
            }

            var deleteResponse = new deleteResponseType { deletes = deleteStatuses.ToArray() };

            return Ok(deleteResponse);
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Head(string[], string[])">Head</see>
        /// </summary>
        [HttpHead]
        public virtual IHttpActionResult Head(
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.
            if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.QUERY))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (!HttpUtils.ValidatePagingParameters(Request.Headers, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                result = GetAll(zoneId?[0], contextId?[0]).ClearContent();

                if (Service is ISupportsChangesSince supportsChangesSince)
                {
                    result = result.AddHeader(
                        "changesSinceMarker",
                        supportsChangesSince.ChangesSinceMarker ?? string.Empty);
                }
            }
            catch (ArgumentException e)
            {
                result = BadRequest(
                    $"One or more parameters of the GET request (associated with the HEAD request) are invalid.\n{e.Message}");
            }
            catch (QueryException e)
            {
                result = BadRequest($"HEAD request failed for object {TypeName}.\n{e.Message}");
            }
            catch (ContentTooLargeException)
            {
                result = StatusCode(HttpStatusCode.RequestEntityTooLarge);
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.BroadcastEvents(string, string)">BroadcastEvents</see>
        /// </summary>
        [HttpGet]
        public virtual IHttpActionResult BroadcastEvents(string zoneId = null, string contextId = null)
        {
            var eventService = Service as IEventService<TMultiple>;
            bool eventsSupported = (eventService != null);

            if (!eventsSupported)
            {
                return BadRequest("Support for SIF Events has not been implemented.");
            }

            IHttpActionResult result;

            try
            {
                IRegistrationService registrationService = RegistrationManager.GetProviderRegistrationService(
                    ProviderSettings,
                    SessionsManager.ProviderSessionService);

                if (registrationService is NoRegistrationService)
                {
                    result = BadRequest("SIF Events are only supported in a BROKERED environment.");
                }
                else
                {
                    IEventIterator<TMultiple> eventIterator = eventService.GetEventIterator(zoneId, contextId);

                    if (eventIterator == null)
                    {
                        result = BadRequest("SIF Events implementation is not valid.");
                    }
                    else
                    {
                        Model.Infrastructure.Environment environment = registrationService.Register();

                        // Retrieve the current Authorisation Token.
                        AuthorisationToken token = registrationService.AuthorisationToken;

                        // Retrieve the EventsConnector endpoint URL.
                        string url = EnvironmentUtils.ParseServiceUrl(
                            environment,
                            ServiceType.UTILITY,
                            InfrastructureServiceNames.eventsConnector);

                        while (eventIterator.HasNext())
                        {
                            SifEvent<TMultiple> sifEvent = eventIterator.GetNext();

                            var requestHeaders = new NameValueCollection()
                            {
                                { EventParameterType.eventAction.ToDescription(), sifEvent.EventAction.ToDescription() },
                                { EventParameterType.messageId.ToDescription(), sifEvent.Id.ToString() },
                                { EventParameterType.messageType.ToDescription(), "EVENT" },
                                { EventParameterType.serviceName.ToDescription(), $"{TypeName}s" }
                            };

                            switch (sifEvent.EventAction)
                            {
                                case EventAction.UPDATE_FULL:
                                    requestHeaders.Add(EventParameterType.Replacement.ToDescription(), "FULL");
                                    break;

                                case EventAction.UPDATE_PARTIAL:
                                    requestHeaders.Add(EventParameterType.Replacement.ToDescription(), "PARTIAL");
                                    break;
                            }

                            string requestBody = SerialiseEvents(sifEvent.SifObjects);
                            HttpUtils.PostRequest(
                                url,
                                token,
                                requestBody,
                                ProviderSettings.CompressPayload,
                                contentTypeOverride: ContentType.ToDescription(),
                                acceptOverride: Accept.ToDescription(),
                                requestHeaders: requestHeaders);
                        }
                    }

                    result = Ok();
                }
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        /// <summary>
        /// <see cref="IEventPayloadSerialisable{TMultiple}.SerialiseEvents(TMultiple)"/>
        /// </summary>
        public abstract string SerialiseEvents(TMultiple obj);
    }
}