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

using Sif.Framework.Extensions;
using Sif.Framework.Model.DataModels;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Query;
using Sif.Framework.Model.Responses;
using Sif.Framework.Service;
using Sif.Framework.Service.Authentication;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Providers;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Sif.Framework.Providers
{

    /// <summary>
    /// This class defines a Provider of SIF data model objects.
    /// </summary>
    /// <typeparam name="TSingle">Type that defines a single object entity.</typeparam>
    /// <typeparam name="TMultiple">Type that defines a multiple objects entity.</typeparam>
    public abstract class Provider<TSingle, TMultiple> : ApiController, IProvider<TSingle, TMultiple, string> where TSingle : ISifRefId<string>
    {
        protected IAuthenticationService authService;
        protected IService<TSingle, TMultiple, string> service;

        /// <summary>
        /// Default constructor that is only available to derived instances of
        /// this class.
        /// </summary>
        protected Provider()
        {

            if (EnvironmentType.DIRECT.Equals(SettingsManager.ProviderSettings.EnvironmentType))
            {
                authService = new DirectAuthenticationService();
            }
            else if (EnvironmentType.BROKERED.Equals(SettingsManager.ProviderSettings.EnvironmentType))
            {
                authService = new BrokeredAuthenticationService();
            }

        }

        /// <summary>
        /// Create an instance based on the specified service.
        /// </summary>
        /// <param name="service">Service used for managing the object type.</param>
        public Provider(IProviderService<TSingle, TMultiple> service)
            : base()
        {
            this.service = service;
        }

        /// <summary>
        /// Convenience method for creating an error record.
        /// </summary>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Error record.</returns>
        protected errorType CreateError(HttpStatusCode statusCode, string message = null)
        {
            errorType error = new errorType();
            error.id = Guid.NewGuid().ToString();
            error.code = (uint)statusCode;
            error.scope = typeof(TSingle).Name;

            if (string.IsNullOrWhiteSpace(message))
            {
                error.message = message.Trim();
            }

            return error;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Post(TTSingle)">Post</see>
        /// </summary>
        public virtual IHttpActionResult Post(TSingle obj)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            IHttpActionResult result;

            try
            {
                bool hasAdvisoryId = !string.IsNullOrWhiteSpace(obj.RefId);
                bool? mustUseAdvisory = HttpUtils.GetMustUseAdvisory(Request.Headers);

                if (hasAdvisoryId)
                {

                    if (mustUseAdvisory.HasValue && mustUseAdvisory.Value == true)
                    {
                        TSingle createdObject = service.Create(obj, mustUseAdvisory);
                        string uri = Url.Link("DefaultApi", new { controller = typeof(TSingle).Name, id = createdObject.RefId });
                        result = Created(uri, createdObject);
                    }
                    else
                    {
                        result = BadRequest("Request failed for object " + typeof(TSingle).Name + " as object ID provided (" + obj.RefId + "), but mustUseAdvisory is not specified or is false.");
                    }

                }
                else
                {

                    if (mustUseAdvisory.HasValue && mustUseAdvisory.Value == true)
                    {
                        result = BadRequest("Request failed for object " + typeof(TSingle).Name + " as object ID is not provided, but mustUseAdvisory is true.");
                    }
                    else
                    {
                        TSingle createdObject = service.Create(obj);
                        string uri = Url.Link("DefaultApi", new { controller = typeof(TSingle).Name, id = createdObject.RefId });
                        result = Created(uri, createdObject);
                    }

                }

            }
            catch (AlreadyExistsException)
            {
                result = Conflict();
            }
            catch (ArgumentException e)
            {
                result = BadRequest("Object to create of type " + typeof(TSingle).Name + " is invalid.\n " + e.Message);
            }
            catch (CreateException e)
            {
                result = BadRequest("Request failed for object " + typeof(TSingle).Name + ".\n " + e.Message);
            }
            catch (RejectedException e)
            {
                result = this.NotFound("Create request rejected for object " + typeof(TSingle).Name + " with ID of " + obj.RefId + ".\n" + e.Message);
            }
            catch (QueryException e)
            {
                result = BadRequest("Request failed for object " + typeof(TSingle).Name + ".\n " + e.Message);
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Post(TMultiple)">Post</see>
        /// </summary>
        public virtual IHttpActionResult Post(TMultiple obj)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            bool? mustUseAdvisory = HttpUtils.GetMustUseAdvisory(Request.Headers);
            MultipleCreateResponse multipleCreateResponse = ((IProviderService<TSingle, TMultiple>)service).Create(obj, mustUseAdvisory);
            createResponseType createResponse = MapperFactory.CreateInstance<MultipleCreateResponse, createResponseType>(multipleCreateResponse);
            IHttpActionResult result = Ok(createResponse);

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Get(string)">Get</see>
        /// </summary>
        public virtual IHttpActionResult Get([FromUri(Name = "id")] string refId)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            if (HttpUtils.HasPagingHeaders(Request.Headers))
            {
                return StatusCode(HttpStatusCode.MethodNotAllowed);
            }

            IHttpActionResult result;

            try
            {
                TSingle obj = service.Retrieve(refId);

                if (obj == null)
                {
                    result = NotFound();
                }
                else
                {
                    result = Ok(obj);
                }

            }
            catch (ArgumentException e)
            {
                result = BadRequest("Invalid argument: id=" + refId + ".\n" + e.Message);
            }
            catch (QueryException e)
            {
                result = BadRequest("Request failed for object " + typeof(TSingle).Name + " with ID of " + refId + ".\n " + e.Message);
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Get(TSingle)">Get</see>
        /// </summary>
        public virtual IHttpActionResult Get(TSingle obj)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            string errorMessage;

            if (!HttpUtils.ValidatePagingParameters(Request.Headers, out errorMessage))
            {
                return BadRequest(errorMessage);
            }

            bool hasPayload = (obj != null);

            if (HttpUtils.HasMethodOverrideHeader(Request.Headers) && !hasPayload)
            {
                return BadRequest("GET (Query by Example) request failed due to missing payload.");
            }

            IHttpActionResult result;

            try
            {
                uint? navigationPage = HttpUtils.GetNavigationPage(Request.Headers);
                uint? navigationPageSize = HttpUtils.GetNavigationPageSize(Request.Headers);
                TMultiple objs;

                if (navigationPage.HasValue && navigationPageSize.HasValue)
                {

                    if (hasPayload)
                    {
                        objs = service.Retrieve(obj, navigationPage, navigationPageSize);
                    }
                    else
                    {
                        objs = service.Retrieve(navigationPage, navigationPageSize);
                    }

                }
                else
                {

                    if (hasPayload)
                    {
                        objs = service.Retrieve(obj);
                    }
                    else
                    {
                        objs = service.Retrieve();
                    }

                }

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
                result = BadRequest("Example object or navigation paging headers are invalid.\n" + e.Message);
            }
            catch (QueryException e)
            {
                result = BadRequest("Query by Example GET request failed for object " + typeof(TSingle).Name + ".\n " + e.Message);
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
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Get(string, string, string, string, string, string)">Get</see>
        /// </summary>
        public virtual IHttpActionResult Get(string object1, [FromUri(Name = "id1")] string refId1, string object2 = null, [FromUri(Name = "id2")] string refId2 = null, string object3 = null, [FromUri(Name = "id3")] string refId3 = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            // Check ACLs and return StatusCode(HttpStatusCode.BadRequest) if parameters do not match a recognised Service Path.

            string errorMessage;

            if (!HttpUtils.ValidatePagingParameters(Request.Headers, out errorMessage))
            {
                return BadRequest(errorMessage);
            }

            IHttpActionResult result;

            try
            {
                IList<EqualCondition> conditions = new List<EqualCondition>() { new EqualCondition() { Left = object1, Right = refId1 } };

                if (!string.IsNullOrWhiteSpace(object2))
                {
                    conditions.Add(new EqualCondition() { Left = object2, Right = refId2 });

                    if (!string.IsNullOrWhiteSpace(object3))
                    {
                        conditions.Add(new EqualCondition() { Left = object3, Right = refId3 });
                    }

                }

                TMultiple objs = service.Retrieve(conditions);

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
                result = BadRequest("One or more conditions are invalid.\n" + e.Message);
            }
            catch (QueryException e)
            {
                result = BadRequest("Service Path GET request failed for object " + typeof(TSingle).Name + ".\n " + e.Message);
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
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Put(string, T)">Put</see>
        /// </summary>
        public virtual IHttpActionResult Put([FromUri(Name = "id")] string refId, TSingle obj)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            if (string.IsNullOrWhiteSpace(refId) || obj == null || !refId.Equals(obj.RefId))
            {
                return BadRequest("The refId in the update request does not match the SIF identifier of the object provided.");
            }

            IHttpActionResult result;

            try
            {
                service.Update(obj);
                result = StatusCode(HttpStatusCode.NoContent);
            }
            catch (ArgumentException e)
            {
                result = BadRequest("Object to update of type " + typeof(TSingle).Name + " is invalid.\n " + e.Message);
            }
            catch (NotFoundException e)
            {
                result = this.NotFound("Object " + typeof(TSingle).Name + " with ID of " + refId + " not found.\n" + e.Message);
            }
            catch (UpdateException e)
            {
                result = BadRequest("Request failed for object " + typeof(TSingle).Name + " with ID of " + refId + ".\n " + e.Message);
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Put(TMultiple)">Put</see>
        /// </summary>
        public virtual IHttpActionResult Put(TMultiple obj)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            MultipleUpdateResponse multipleUpdateResponse = ((IProviderService<TSingle, TMultiple>)service).Update(obj);
            updateResponseType updateResponse = MapperFactory.CreateInstance<MultipleUpdateResponse, updateResponseType>(multipleUpdateResponse);
            IHttpActionResult result = Ok(updateResponse);

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Delete(string)">Delete</see>
        /// </summary>
        public virtual IHttpActionResult Delete([FromUri(Name = "id")] string refId)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            IHttpActionResult result;

            try
            {
                service.Delete(refId);
                result = StatusCode(HttpStatusCode.NoContent);
            }
            catch (ArgumentException e)
            {
                result = BadRequest("Invalid argument: id=" + refId + ".\n" + e.Message);
            }
            catch (DeleteException e)
            {
                result = BadRequest("Request failed for object " + typeof(TSingle).Name + " with ID of " + refId + ".\n " + e.Message);
            }
            catch (NotFoundException e)
            {
                result = this.NotFound("Object " + typeof(TSingle).Name + " with ID of " + refId + " not found.\n" + e.Message);
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Delete(deleteRequestType)">Delete</see>
        /// </summary>
        public virtual IHttpActionResult Delete(deleteRequestType deleteRequest)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            IHttpActionResult result;
            ICollection<deleteStatus> deleteStatuses = new List<deleteStatus>();

            try
            {

                foreach (deleteIdType deleteId in deleteRequest.deletes)
                {
                    deleteStatus status = new deleteStatus();
                    status.id = deleteId.id;

                    try
                    {
                        service.Delete(deleteId.id);
                        status.statusCode = ((int)HttpStatusCode.NoContent).ToString();
                    }
                    catch (ArgumentException e)
                    {
                        status.error = CreateError(HttpStatusCode.BadRequest, "Invalid argument: id=" + deleteId.id + ".\n" + e.Message);
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (DeleteException e)
                    {
                        status.error = CreateError(HttpStatusCode.BadRequest, "Request failed for object " + typeof(TSingle).Name + " with ID of " + deleteId.id + ".\n " + e.Message);
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (NotFoundException e)
                    {
                        status.error = CreateError(HttpStatusCode.NotFound, "Object " + typeof(TSingle).Name + " with ID of " + deleteId.id + " not found.\n" + e.Message);
                        status.statusCode = ((int)HttpStatusCode.NotFound).ToString();
                    }
                    catch (Exception e)
                    {
                        status.error = CreateError(HttpStatusCode.InternalServerError, "Request failed for object " + typeof(TSingle).Name + " with ID of " + deleteId.id + ".\n " + e.Message);
                        status.statusCode = ((int)HttpStatusCode.InternalServerError).ToString();
                    }

                    deleteStatuses.Add(status);
                }

            }
            catch (Exception)
            {
                // Need to ignore exceptions otherwise it would not be possible to return status records of processed objects.
            }

            deleteResponseType deleteResponse = new deleteResponseType { deletes = deleteStatuses.ToArray() };
            result = Ok(deleteResponse);

            return result;
        }

    }

}
