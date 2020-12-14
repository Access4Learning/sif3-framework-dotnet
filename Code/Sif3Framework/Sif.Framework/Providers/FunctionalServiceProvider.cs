﻿/*
 * Crown Copyright © Department for Education (UK) 2016
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

using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service;
using Sif.Framework.Service.Authentication;
using Sif.Framework.Service.Functional;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Utils;
using Sif.Framework.WebApi.ModelBinders;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Providers
{
    /// <summary>
    /// Services Connector implementation
    /// </summary>
    [RoutePrefix("services")]
    public class FunctionalServiceProvider : ApiController
    {
        private readonly slf4net.ILogger log =
            slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Authentication (BROKERED or DIRECT) service against which requests will be checked.
        /// </summary>
        private readonly IAuthenticationService authService;

        /// <summary>
        /// Application settings associated with the Provider.
        /// </summary>
        protected IFrameworkSettings ProviderSettings { get; }

        /// <summary>
        /// Create an instance.
        /// </summary>
        public FunctionalServiceProvider() : this(null)
        {
        }

        /// <summary>
        /// Create an instance.
        /// </summary>
        /// <param name="settings">Provider settings. If null, Provider settings will be read from the SifFramework.config file.</param>
        protected FunctionalServiceProvider(IFrameworkSettings settings = null)
        {
            ProviderSettings = settings ?? SettingsManager.ProviderSettings;

            if (EnvironmentType.DIRECT.Equals(ProviderSettings.EnvironmentType))
            {
                authService =
                    new DirectAuthenticationService(new ApplicationRegisterService(), new EnvironmentService());
            }
            else if (EnvironmentType.BROKERED.Equals(ProviderSettings.EnvironmentType))
            {
                authService = new BrokeredAuthenticationService(
                    new ApplicationRegisterService(),
                    new EnvironmentService(),
                    settings,
                    SessionsManager.ProviderSessionService);
            }
        }

        /*-------------------*/
        /* Job operations    */
        /*-------------------*/

        /// <summary>
        /// POST services/{serviceName}/{jobName}
        /// </summary>
        [HttpPost]
        [Route("{serviceName}/{jobName}")]
        public virtual HttpResponseMessage Post(
            [FromUri] string serviceName,
            [FromUri] string jobName,
            [FromBody] jobType item,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            string sessionToken =
                CheckAuthorisation(serviceName, zoneId, contextId, new Right(RightType.CREATE, RightValue.APPROVED));
            HttpResponseMessage result;

            try
            {
                bool hasAdvisoryId = ProviderUtils.IsAdvisoryId(item.id);
                bool? _mustUseAdvisory = HttpUtils.GetMustUseAdvisory(Request.Headers);
                bool mustUseAdvisory = _mustUseAdvisory.HasValue && _mustUseAdvisory.Value;

                IFunctionalService service = GetService(serviceName);

                if (!service.AcceptJob(serviceName, jobName))
                {
                    return Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Service " + serviceName + " does not handle jobs named " + jobName);
                }

                if (!hasAdvisoryId && mustUseAdvisory)
                {
                    return Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Request requires use of advisory id, but none has been supplied.");
                }

                Guid id = service.Create(item, zoneId?[0], contextId?[0]);

                if (ProviderSettings.JobBinding)
                {
                    service.Bind(id, GetOwnerId(sessionToken));
                }

                jobType job = service.Retrieve(id, zoneId?[0], contextId?[0]);

                string uri = Url.Link("ServicesRoute", new { controller = serviceName, id });

                result = Request.CreateResponse(HttpStatusCode.Created, job);
                result.Headers.Location = new Uri(uri);
            }
            catch (AlreadyExistsException e)
            {
                result = Request.CreateErrorResponse(HttpStatusCode.Conflict, e);
            }
            catch (ArgumentException e)
            {
                result = Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    "Object to create Job is invalid.\n ",
                    e);
            }
            catch (CreateException e)
            {
                result = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request failed for Job.\n ", e);
            }
            catch (RejectedException e)
            {
                result = Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    "Create request rejected for Job" + (item.id == null ? "" : " with advisory id " + item.id) + ".\n",
                    e);
            }
            catch (QueryException e)
            {
                result = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request failed for Job.\n", e);
            }
            catch (Exception e)
            {
                result = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

            return result;
        }

        /// <summary>
        /// POST services/{serviceName}
        /// </summary>
        [HttpPost]
        [Route("{serviceName}")]
        public virtual HttpResponseMessage Post(
            [FromUri] string serviceName,
            [FromBody] jobCollectionType items,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            string sessionToken =
                CheckAuthorisation(serviceName, zoneId, contextId, new Right(RightType.CREATE, RightValue.APPROVED));
            HttpResponseMessage result;

            try
            {
                IFunctionalService service = GetService(serviceName);
                var creates = new List<createType>();

                foreach (jobType job in items.job)
                {
                    try
                    {
                        if (!service.AcceptJob(serviceName, job.name))
                        {
                            throw new ArgumentException(
                                $"Service {serviceName} does not handle jobs named {job.name}.");
                        }

                        Guid id = service.Create(job, zoneId?[0], contextId?[0]);

                        if (ProviderSettings.JobBinding)
                        {
                            service.Bind(id, GetOwnerId(sessionToken));
                        }

                        creates.Add(ProviderUtils.CreateCreate(HttpStatusCode.Created, id.ToString(), job.id));
                    }
                    catch (CreateException e)
                    {
                        ProviderUtils.CreateCreate(
                            HttpStatusCode.Conflict,
                            job.id,
                            error: ProviderUtils.CreateError(
                                HttpStatusCode.Conflict,
                                HttpStatusCode.Conflict.ToString(),
                                e.Message));
                    }
                }

                createResponseType createResponse = ProviderUtils.CreateCreateResponse(creates.ToArray());
                result = Request.CreateResponse(HttpStatusCode.Created, createResponse);
                string uri = Url.Link("ServicesRoute", new { controller = serviceName });
                result.Headers.Location = new Uri(uri);
            }
            catch (AlreadyExistsException e)
            {
                result = Request.CreateErrorResponse(HttpStatusCode.Conflict, e);
            }
            catch (ArgumentException e)
            {
                result = Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    "Object to create Jobs is invalid.\n ",
                    e);
            }
            catch (CreateException e)
            {
                result = Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    "Request failed for creating Jobs.\n ",
                    e);
            }
            catch (RejectedException e)
            {
                result = Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    "Create request rejected for creating Jobs.\n",
                    e);
            }
            catch (QueryException e)
            {
                result = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request failed for Job.\n", e);
            }
            catch (Exception e)
            {
                result = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

            return result;
        }

        /// <summary>
        /// GET services
        /// </summary>
        /// <returns>Forbidden (403)</returns>
        [HttpGet]
        [Route("")]
        public virtual HttpResponseMessage Get(
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            CheckAuthorisation(zoneId, contextId);

            return Request.CreateResponse(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// GET services/{serviceName}
        /// </summary>
        [HttpGet]
        [Route("{serviceName}")]
        public virtual ICollection<jobType> Get(
            [FromUri] string serviceName,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            string sessionToken =
                CheckAuthorisation(serviceName, zoneId, contextId, new Right(RightType.QUERY, RightValue.APPROVED));
            ICollection<jobType> items = new List<jobType>();

            try
            {
                IFunctionalService service = GetService(serviceName);
                ICollection<jobType> jobs = service.Retrieve(zoneId?[0], contextId?[0]);

                foreach (jobType job in jobs)
                {
                    if (!ProviderSettings.JobBinding || service.IsBound(Guid.Parse(job.id), GetOwnerId(sessionToken)))
                    {
                        items.Add(job);
                    }
                }
            }
            catch (Exception e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        $"The GET request failed for functional services due to the following error:\n{e.Message}"));
            }

            if (items == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return items;
        }

        /// <summary>
        /// GET services/{serviceName}/{id}
        /// </summary>
        [HttpGet]
        [Route("{serviceName}/{id}")]
        public virtual HttpResponseMessage Get(
            [FromUri] string serviceName,
            [FromUri] Guid id,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            string sessionToken =
                CheckAuthorisation(serviceName, zoneId, contextId, new Right(RightType.QUERY, RightValue.APPROVED));

            // Check that we support that provider
            // if not then throw new HttpResponseException(HttpStatusCode.NotFound);

            jobType item;

            try
            {
                IFunctionalService service = GetService(serviceName);
                item = service.Retrieve(id, zoneId?[0], contextId?[0]);

                if (ProviderSettings.JobBinding && !service.IsBound(Guid.Parse(item.id), GetOwnerId(sessionToken)))
                {
                    throw new InvalidSessionException(
                        "Request failed as one or more jobs referred to in this request do not belong to this consumer.");
                }
            }
            catch (Exception e)
            {
                string errorMessage = "The GET request failed for a " + serviceName + " job with an ID of " + id +
                                      " due to the following error:\n " + e.Message;
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK, item);

            return result;
        }

        /// <summary>
        /// PUT services/{serviceName}/{id}
        /// </summary>
        /// <returns>Forbidden (403)</returns>
        [HttpPut]
        [Route("{serviceName}/{id}")]
        public virtual HttpResponseMessage Put(
            [FromUri] string serviceName,
            [FromUri] Guid id,
            [FromBody] jobType item,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            CheckAuthorisation(serviceName, zoneId, contextId, new Right(RightType.UPDATE, RightValue.APPROVED));

            return Request.CreateResponse(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// PUT services/{serviceName}
        /// </summary>
        /// <returns>Forbidden (403)</returns>
        [HttpPut]
        [Route("{serviceName}")]
        public virtual HttpResponseMessage Put(
            [FromUri] string serviceName,
            [FromBody] jobCollectionType items,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            CheckAuthorisation(serviceName, zoneId, contextId, new Right(RightType.UPDATE, RightValue.APPROVED));

            return Request.CreateResponse(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// DELETE services/{serviceName}/{id}
        /// </summary>
        [HttpDelete]
        [Route("{serviceName}/{id}")]
        public virtual HttpResponseMessage Delete(
            [FromUri] string serviceName,
            [FromUri] Guid id,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            string sessionToken =
                CheckAuthorisation(serviceName, zoneId, contextId, new Right(RightType.DELETE, RightValue.APPROVED));

            try
            {
                IFunctionalService service = GetService(serviceName);

                if (ProviderSettings.JobBinding && !service.IsBound(id, GetOwnerId(sessionToken)))
                {
                    throw new InvalidSessionException(
                        "Request failed as one or more jobs referred to in this request do not belong to this consumer.");
                }

                service.Delete(id, zoneId?[0], contextId?[0]);

                if (ProviderSettings.JobBinding)
                {
                    service.Unbind(id);
                }
            }
            catch (Exception e)
            {
                string errorMessage = "The DELETE request failed for a " + serviceName + " job with an ID of " + id +
                                      " due to the following error:\n " + e.Message;
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.NoContent);

            return result;
        }

        /// <summary>
        /// DELETE /services/{serviceName}
        /// </summary>
        [HttpDelete]
        [Route("{serviceName}")]
        public virtual HttpResponseMessage Delete(
            [FromUri] string serviceName,
            [FromBody] deleteRequestType deleteRequest,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            string sessionToken =
                CheckAuthorisation(serviceName, zoneId, contextId, new Right(RightType.DELETE, RightValue.APPROVED));
            IFunctionalService service = GetService(serviceName);
            ICollection<deleteStatus> statuses = new List<deleteStatus>();

            foreach (deleteIdType deleteId in deleteRequest.deletes)
            {
                try
                {
                    if (ProviderSettings.JobBinding &&
                        !service.IsBound(Guid.Parse(deleteId.id), GetOwnerId(sessionToken)))
                    {
                        throw new InvalidSessionException("Request failed as job does not belong to this consumer.");
                    }

                    service.Delete(Guid.Parse(deleteId.id), zoneId?[0], contextId?[0]);

                    if (ProviderSettings.JobBinding)
                    {
                        service.Unbind(Guid.Parse(deleteId.id));
                    }

                    statuses.Add(ProviderUtils.CreateDelete(HttpStatusCode.OK, deleteId.id));
                }
                catch (ArgumentException e)
                {
                    statuses.Add(
                        ProviderUtils.CreateDelete(
                            HttpStatusCode.BadRequest,
                            deleteId.id,
                            ProviderUtils.CreateError(
                                HttpStatusCode.BadRequest,
                                serviceName,
                                "Invalid argument: id=" + deleteId.id + ".\n" + e.Message)));
                }
                catch (DeleteException e)
                {
                    statuses.Add(
                        ProviderUtils.CreateDelete(
                            HttpStatusCode.BadRequest,
                            deleteId.id,
                            ProviderUtils.CreateError(
                                HttpStatusCode.BadRequest,
                                serviceName,
                                $"Request failed for object {serviceName} with ID of {deleteId.id}.\n{e.Message}")));
                }
                catch (NotFoundException e)
                {
                    statuses.Add(
                        ProviderUtils.CreateDelete(
                            HttpStatusCode.NotFound,
                            deleteId.id,
                            ProviderUtils.CreateError(
                                HttpStatusCode.BadRequest,
                                serviceName,
                                $"Object {serviceName} with ID of {deleteId.id} not found.\n{e.Message}")));
                }
                catch (InvalidSessionException e)
                {
                    statuses.Add(
                        ProviderUtils.CreateDelete(
                            HttpStatusCode.BadRequest,
                            deleteId.id,
                            ProviderUtils.CreateError(
                                HttpStatusCode.BadRequest,
                                serviceName,
                                $"Request failed for object {serviceName} with ID of {deleteId.id}, job doesn't belong to this consumer.\n{e.Message}")));
                }
                catch (Exception e)
                {
                    statuses.Add(
                        ProviderUtils.CreateDelete(
                            HttpStatusCode.InternalServerError,
                            deleteId.id,
                            ProviderUtils.CreateError(
                                HttpStatusCode.BadRequest,
                                serviceName,
                                $"Request failed for object {serviceName} with ID of {deleteId.id}.\n{e.Message}")));
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ProviderUtils.CreateDeleteResponse(statuses.ToArray()));
        }

        /*-------------------*/
        /* Phase operations  */
        /*-------------------*/

        /// <summary>
        /// POST services/{serviceName}/{id}/{phaseName}
        /// </summary>
        [HttpPost]
        [Route("{serviceName}/{id}/{phaseName}")]
        public virtual HttpResponseMessage Post(
            [FromUri] string serviceName,
            [FromUri] Guid id,
            [FromUri] string phaseName,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            string sessionToken =
                CheckAuthorisation(serviceName, zoneId, contextId, new Right(RightType.UPDATE, RightValue.APPROVED));
            PreventPagingHeaders();
            string body = Request.Content.ReadAsStringAsync().Result;

            try
            {
                IFunctionalService service = GetService(serviceName);

                if (ProviderSettings.JobBinding && !service.IsBound(id, GetOwnerId(sessionToken)))
                {
                    throw new InvalidSessionException(
                        "Request failed as the job referred to in this request does not belong to this consumer.");
                }

                return OkResult(
                    service.CreateToPhase(
                        id,
                        phaseName,
                        body,
                        zoneId?[0],
                        contextId?[0],
                        HttpUtils.GetContentType(Request),
                        HttpUtils.GetAccept(Request)));
            }
            catch (ArgumentException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Invalid argument: id=" + id + ", phaseName=" + phaseName + ".\n" + e.Message));
            }
            catch (CreateException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Problem creating data for " + phaseName + "@" + id + ".\n",
                        e));
            }
            catch (RejectedException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        "Problem creating data for " + phaseName + "@" + id + ".\n",
                        e));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.InternalServerError,
                        "Request failed for phase " + phaseName + " in job " + id + ".\n " + e.Message));
            }
        }

        /// <summary>
        /// GET services/{serviceName}/{id}/{phaseName}
        /// </summary>
        [HttpGet]
        [Route("{serviceName}/{id}/{phaseName}")]
        public virtual HttpResponseMessage Get(
            [FromUri] string serviceName,
            [FromUri] Guid id,
            [FromUri] string phaseName,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            string sessionToken =
                CheckAuthorisation(serviceName, zoneId, contextId, new Right(RightType.UPDATE, RightValue.APPROVED));
            PreventPagingHeaders();
            string _ = Request.Content.ReadAsStringAsync().Result;

            try
            {
                IFunctionalService service = GetService(serviceName);

                if (ProviderSettings.JobBinding && !service.IsBound(id, GetOwnerId(sessionToken)))
                {
                    throw new InvalidSessionException(
                        "Request failed as the job referred to in this request does not belong to this consumer.");
                }

                return OkResult(
                    service.RetrieveToPhase(
                        id,
                        phaseName,
                        zoneId: zoneId?[0],
                        contextId: contextId?[0],
                        contentType: HttpUtils.GetContentType(Request), accept: HttpUtils.GetAccept(Request)));
            }
            catch (ArgumentException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Invalid argument: id=" + id + ", phaseName=" + phaseName + ".\n",
                        e));
            }
            catch (NotFoundException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.NotFound,
                        "Problem retrieving data for " + phaseName + "@" + id + ".\n",
                        e));
            }
            catch (RejectedException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        "Problem retrieving data for " + phaseName + "@" + id + ".\n",
                        e));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.InternalServerError,
                        "Request failed for phase " + phaseName + " in job " + id + ".\n " + e.Message));
            }
        }

        /// <summary>
        /// PUT services/{serviceName}/{id}/{phaseName}
        /// </summary>
        [HttpPut]
        [Route("{serviceName}/{id}/{phaseName}")]
        public virtual HttpResponseMessage Put(
            [FromUri] string serviceName,
            [FromUri] Guid id,
            [FromUri] string phaseName,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            string sessionToken =
                CheckAuthorisation(serviceName, zoneId, contextId, new Right(RightType.UPDATE, RightValue.APPROVED));
            string body = Request.Content.ReadAsStringAsync().Result;

            try
            {
                IFunctionalService service = GetService(serviceName);

                if (ProviderSettings.JobBinding && !service.IsBound(id, GetOwnerId(sessionToken)))
                {
                    throw new InvalidSessionException(
                        "Request failed as the job referred to in this request does not belong to this consumer.");
                }

                return OkResult(
                    service.UpdateToPhase(
                        id,
                        phaseName,
                        body,
                        zoneId?[0],
                        contextId?[0],
                        HttpUtils.GetContentType(Request), HttpUtils.GetAccept(Request)));
            }
            catch (ArgumentException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Invalid argument for Phase " + phaseName + " in job " + id + ".\n" + e.Message));
            }
            catch (UpdateException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Problem updating data for " + phaseName + "@" + id + ".\n",
                        e));
            }
            catch (RejectedException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        "Problem updating data for " + phaseName + "@" + id + ".\n",
                        e));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.InternalServerError,
                        "Request failed for phase " + phaseName + " in job " + id + ".\n " + e.Message,
                        e));
            }
        }

        /// <summary>
        /// DELETE services/{serviceName}/{id}/{phaseName}
        /// </summary>
        [HttpDelete]
        [Route("{serviceName}/{id}/{phaseName}")]
        public virtual HttpResponseMessage Delete(
            [FromUri] string serviceName,
            [FromUri] Guid id,
            [FromUri] string phaseName,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            string sessionToken =
                CheckAuthorisation(serviceName, zoneId, contextId, new Right(RightType.UPDATE, RightValue.APPROVED));
            PreventPagingHeaders();
            string body = Request.Content.ReadAsStringAsync().Result;

            try
            {
                IFunctionalService service = GetService(serviceName);

                if (ProviderSettings.JobBinding && !service.IsBound(id, GetOwnerId(sessionToken)))
                {
                    throw new InvalidSessionException(
                        "Request failed as the job referred to in this request does not belong to this consumer.");
                }

                return OkResult(
                    service.DeleteToPhase(
                        id,
                        phaseName,
                        body,
                        zoneId?[0],
                        contextId?[0],
                        HttpUtils.GetContentType(Request), HttpUtils.GetAccept(Request)));
            }
            catch (ArgumentException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Invalid argument: id=" + id + ", phaseName=" + phaseName + ".\n" + e.Message));
            }
            catch (DeleteException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Problem deleting data for " + phaseName + "@" + id + ".\n",
                        e));
            }
            catch (RejectedException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        "Problem deleting data for " + phaseName + "@" + id + ".\n",
                        e));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.InternalServerError,
                        "Request failed for phase " + phaseName + " in job " + id + ".\n " + e.Message));
            }
        }

        /*-------------------*/
        /* States operations */
        /*-------------------*/

        /// <summary>
        /// POST services/{serviceName}/{id}/{phaseName}/states/state
        /// </summary>
        [HttpPost]
        [Route("{serviceName}/{id}/{phaseName}/states/state")]
        public virtual HttpResponseMessage Post(
            [FromUri] string serviceName,
            [FromUri] Guid id,
            [FromUri] string phaseName,
            [FromBody] stateType item,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            string sessionToken =
                CheckAuthorisation(serviceName, zoneId, contextId, new Right(RightType.UPDATE, RightValue.APPROVED));
            PreventPagingHeaders();
            HttpResponseMessage result;

            try
            {
                IFunctionalService service = GetService(serviceName);

                if (ProviderSettings.JobBinding && !service.IsBound(id, GetOwnerId(sessionToken)))
                {
                    throw new InvalidSessionException(
                        "Request failed as the job referred to in this request does not belong to this consumer.");
                }

                stateType state = service.CreateToState(id, phaseName, item, zoneId?[0], contextId?[0]);
                string uri = Url.Link(
                    "ServiceStatesRoute",
                    new { controller = serviceName, id, phaseName, stateId = state.id });
                result = Request.CreateResponse(HttpStatusCode.Created, state);
                result.Headers.Location = new Uri(uri);
            }
            catch (ArgumentException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Invalid argument: id=" + id + ", phaseName=" + phaseName + ".\n" + e.Message));
            }
            catch (CreateException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Problem creating state data for " + phaseName + "@" + id + ".\n",
                        e));
            }
            catch (RejectedException e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        "Problem creating state data for " + phaseName + "@" + id + ".\n",
                        e));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.InternalServerError,
                        "Request failed for creating state for phase " + phaseName + " in job " + id + ".\n " +
                        e.Message));
            }

            return result;
        }

        /// <summary>
        /// Gets the IFunctionalService with the given name, checking to make sure that the name makes sense first.
        /// </summary>
        /// <param name="serviceName">The name of the service to find.</param>
        /// <returns>An IFunctionalService instance if found, otherwise an exception is thrown.</returns>
        protected virtual IFunctionalService GetService(string serviceName)
        {
            if (!serviceName.ToLower().EndsWith("s"))
            {
                throw new InvalidOperationException(
                    $"Found a functional service to support messages to {serviceName}, but its name isn't a plural (doesn't end in 's'). This will not work in the current framework.");
            }

            IService service = FunctionalServiceProviderFactory.GetInstance(ProviderSettings).GetProvider(serviceName);

            if (service == null)
            {
                throw new InvalidOperationException(
                    $"No functional service found to support messages to {serviceName}.");
            }

            if (!ProviderUtils.isFunctionalService(service.GetType()))
            {
                throw new InvalidOperationException(
                    $"Service ({service.GetType().Name}) found for {serviceName} is not a functional service implementation");
            }

            return service as IFunctionalService;
        }

        /// <summary>
        /// Internal method to check if the request is authorised in the given zone and context by checking the environment XML.
        /// </summary>
        /// <returns>The SessionToken if the request is authorised, otherwise an exception will be thrown.</returns>
        protected virtual string CheckAuthorisation(string[] zoneId, string[] contextId)
        {
            if (!authService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
            {
                log.Debug("Could not verify request headers.");
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Request failed as Zone and/or Context are invalid."));
            }

            return sessionToken;
        }

        /// <summary>
        /// Internal method to check if a given right is supported by the ACL.
        /// </summary>
        /// <param name="serviceName">The name of the service to check</param>
        /// <param name="zoneId">The zone to check authorization in</param>
        /// <param name="contextId">The context to check authorization in</param>
        /// <param name="right">The right to check</param>
        /// <returns>The session token if authorized, otherwise a HttpResponseException is thrown</returns>
        protected virtual string CheckAuthorisation(
            string serviceName,
            string[] zoneId,
            string[] contextId,
            Right right)
        {
            string sessionToken = CheckAuthorisation(zoneId, contextId);
            Environment environment = authService.GetEnvironmentBySessionToken(sessionToken);

            if (environment == null)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Request failed as could not retrieve environment XML."));
            }

            try
            {
                RightsUtils.CheckRight(
                    GetRights(serviceName, EnvironmentUtils.GetTargetZone(environment, zoneId?[0])),
                    right);
                log.Debug($"Functional Service {serviceName} has expected ACL ({right.Type}:{right.Value})");

                return sessionToken;
            }
            catch (RejectedException e)
            {
                string msg = "Functional Service " + serviceName +
                             " does not have sufficient access rights to perform an " + right.Type + " operation: " +
                             e.Message;
                log.Debug(msg);

                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, msg, e));
            }
        }

        /// <summary>
        /// Internal method to retrieve the rights for a service in a given zone.
        /// </summary>
        /// <param name="serviceName">The service name to look for</param>
        /// <param name="zone">The zone to retrieve the rights for</param>
        /// <returns>An array of declared rights</returns>
        private IDictionary<string, Right> GetRights(string serviceName, ProvisionedZone zone)
        {
            Model.Infrastructure.Service service =
                (from Model.Infrastructure.Service s in zone.Services
                    where s.Type.Equals(ServiceType.FUNCTIONAL.ToString()) && s.Name.Equals(serviceName)
                    select s)
                .FirstOrDefault();

            if (service == null)
            {
                string msg = "No Functional Service called " + serviceName + " found in Environment.";
                log.Debug(msg);

                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, msg));
            }

            log.Debug("Found access rights for " + service.Type + " service " + service.Name);

            return service.Rights;
        }

        /// <summary>
        /// Internal method that throws an exception with the Method Not Allowed status code if the request has paging headers.
        /// </summary>
        protected virtual void PreventPagingHeaders()
        {
            if (HttpUtils.HasPagingHeaders(Request.Headers))
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }
        }

        /// <summary>
        /// Convenience method to produce and OK response
        /// </summary>
        /// <param name="payload">The payload to send back in the response</param>
        /// <returns>A response message</returns>
        protected HttpResponseMessage OkResult(string payload = null)
        {
            if (StringUtils.IsEmpty(payload))
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            string accept = HttpUtils.GetAccept(Request);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(payload, Encoding.UTF8, accept)
            };
        }

        private string GetOwnerId(string sessionToken)
        {
            string ownerId = null;

            switch (ProviderSettings.EnvironmentType)
            {
                case EnvironmentType.DIRECT:
                    // Application key is either in header or in session token
                    ownerId = HttpUtils.GetApplicationKey(Request.Headers) ?? sessionToken;
                    break;

                case EnvironmentType.BROKERED:
                    // Application key must have been moved into sourceName property
                    ownerId = HttpUtils.GetSourceName(Request.Headers);
                    break;
            }

            if (ownerId == null)
            {
                throw new NotFoundException("Could not identify consumer.");
            }

            return ownerId;
        }
    }
}