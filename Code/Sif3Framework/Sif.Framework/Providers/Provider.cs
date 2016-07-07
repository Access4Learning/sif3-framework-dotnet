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
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Providers;
using Sif.Framework.Utils;
using Sif.Framework.WebApi.ModelBinders;
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
        protected IObjectService<TSingle, TMultiple, string> service;

        /// <summary>
        /// Default constructor that is only available to derived instances of
        /// this class.
        /// </summary>
        protected Provider()
        {

            if (EnvironmentType.DIRECT.Equals(SettingsManager.ProviderSettings.EnvironmentType))
            {
                authService = new DirectAuthenticationService(new ApplicationRegisterService(), new EnvironmentService());
            }
            else if (EnvironmentType.BROKERED.Equals(SettingsManager.ProviderSettings.EnvironmentType))
            {
                authService = new BrokeredAuthenticationService(new ApplicationRegisterService(), new EnvironmentService());
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
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Post(TTSingle, string[], string[])">Post</see>
        /// </summary>
        public virtual IHttpActionResult Post(TSingle obj, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            if ((zone != null && zone.Length != 1) || (context != null && context.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(TSingle).Name + " as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                bool hasAdvisoryId = !string.IsNullOrWhiteSpace(obj.RefId);
                bool? mustUseAdvisory = HttpUtils.GetMustUseAdvisory(Request.Headers);

                if (hasAdvisoryId)
                {

                    if (mustUseAdvisory.HasValue && mustUseAdvisory.Value == true)
                    {
                        TSingle createdObject = service.Create(obj, mustUseAdvisory, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
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
                        TSingle createdObject = service.Create(obj, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
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
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Post(TMultiple, string[], string[])">Post</see>
        /// </summary>
        public virtual IHttpActionResult Post(TMultiple obj, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            if ((zone != null && zone.Length != 1) || (context != null && context.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(TSingle).Name + " as Zone and/or Context are invalid.");
            }

            bool? mustUseAdvisory = HttpUtils.GetMustUseAdvisory(Request.Headers);
            MultipleCreateResponse multipleCreateResponse =
                ((IProviderService<TSingle, TMultiple>)service).Create(obj, mustUseAdvisory, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
            createResponseType createResponse = MapperFactory.CreateInstance<MultipleCreateResponse, createResponseType>(multipleCreateResponse);
            IHttpActionResult result = Ok(createResponse);

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Get(TPrimaryKey, string[], string[])">Get</see>
        /// </summary>
        public virtual IHttpActionResult Get([FromUri(Name = "id")] string refId, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
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

            if ((zone != null && zone.Length != 1) || (context != null && context.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(TSingle).Name + " as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                TSingle obj = service.Retrieve(refId, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));

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
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Get(TTSingle, string[], string[])">Get</see>
        /// </summary>
        public virtual IHttpActionResult Get(TSingle obj, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
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

            if ((zone != null && zone.Length != 1) || (context != null && context.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(TSingle).Name + " as Zone and/or Context are invalid.");
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
                        objs = service.Retrieve(obj, navigationPage, navigationPageSize, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
                    }
                    else
                    {
                        objs = service.Retrieve(navigationPage, navigationPageSize, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
                    }

                }
                else
                {

                    if (hasPayload)
                    {
                        objs = service.Retrieve(obj, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
                    }
                    else
                    {
                        objs = service.Retrieve(zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
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
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Get(string, string, string, string, string, string, string[], string[])">Get</see>
        /// </summary>
        public virtual IHttpActionResult Get(string object1,
            [FromUri(Name = "id1")] string refId1,
            string object2 = null,
            [FromUri(Name = "id2")] string refId2 = null,
            string object3 = null,
            [FromUri(Name = "id3")] string refId3 = null,
            [MatrixParameter] string[] zone = null,
            [MatrixParameter] string[] context = null)
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

            if ((zone != null && zone.Length != 1) || (context != null && context.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(TSingle).Name + " as Zone and/or Context are invalid.");
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

                TMultiple objs = service.Retrieve(conditions, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));

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
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Put(TPrimaryKey, TTSingle, string[], string[])">Put</see>
        /// </summary>
        public virtual IHttpActionResult Put([FromUri(Name = "id")] string refId, TSingle obj, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
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

            if ((zone != null && zone.Length != 1) || (context != null && context.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(TSingle).Name + " as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                service.Update(obj, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
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
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Put(TMultiple, string[], string[])">Put</see>
        /// </summary>
        public virtual IHttpActionResult Put(TMultiple obj, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            if ((zone != null && zone.Length != 1) || (context != null && context.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(TSingle).Name + " as Zone and/or Context are invalid.");
            }

            MultipleUpdateResponse multipleUpdateResponse = 
                ((IProviderService<TSingle, TMultiple>)service).Update(obj, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
            updateResponseType updateResponse = MapperFactory.CreateInstance<MultipleUpdateResponse, updateResponseType>(multipleUpdateResponse);
            IHttpActionResult result = Ok(updateResponse);

            return result;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Delete(TPrimaryKey, string[], string[])">Delete</see>
        /// </summary>
        public virtual IHttpActionResult Delete([FromUri(Name = "id")] string refId, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            if ((zone != null && zone.Length != 1) || (context != null && context.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(TSingle).Name + " as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                service.Delete(refId, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
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
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Delete(deleteRequestType, string[], string[])">Delete</see>
        /// </summary>
        public virtual IHttpActionResult Delete(deleteRequestType deleteRequest, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            if ((zone != null && zone.Length != 1) || (context != null && context.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(TSingle).Name + " as Zone and/or Context are invalid.");
            }

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
                        service.Delete(deleteId.id, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
                        status.statusCode = ((int)HttpStatusCode.NoContent).ToString();
                    }
                    catch (ArgumentException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.BadRequest, typeof(TSingle).Name, "Invalid argument: id=" + deleteId.id + ".\n" + e.Message);
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (DeleteException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.BadRequest, typeof(TSingle).Name, "Request failed for object " + typeof(TSingle).Name + " with ID of " + deleteId.id + ".\n " + e.Message);
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (NotFoundException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.NotFound, typeof(TSingle).Name, "Object " + typeof(TSingle).Name + " with ID of " + deleteId.id + " not found.\n" + e.Message);
                        status.statusCode = ((int)HttpStatusCode.NotFound).ToString();
                    }
                    catch (Exception e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.InternalServerError, typeof(TSingle).Name, "Request failed for object " + typeof(TSingle).Name + " with ID of " + deleteId.id + ".\n " + e.Message);
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
