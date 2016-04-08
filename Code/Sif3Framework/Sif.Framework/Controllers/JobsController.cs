/*
 * Crown Copyright © Department for Education (UK) 2016
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
using Sif.Framework.Service.Authentication;
using Sif.Framework.Service.Functional;
using Sif.Framework.Utils;
using Sif.Framework.WebApi.ModelBinders;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sif.Framework.Controllers
{
    public abstract class JobsController<FunctionalService> : SifController<jobType, Job> where FunctionalService : IFunctionalService<jobType, Job>
    {
        /// <summary>
        /// Create an instance.
        /// </summary>
        public JobsController(FunctionalService s)
            : base(s)
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

        IFunctionalService<jobType, Job> Service { get { return service as IFunctionalService<jobType, Job>; } }

        /// <summary>
        /// GET api/services/{functionalservice}
        /// GET api/services/{functionalservice}/{functionalservice}s
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override HttpResponseMessage Post(jobType item, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            checkAuthorisation(zone, context);

            HttpResponseMessage result;
            try
            {
                Guid id = Service.Create(item, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
                jobType job = Service.Retrieve(id, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
                
                string uri = Url.Link("FunctionalServicePathApi2", new { controller = item.name, id = id });
                result = Request.CreateResponse<jobType>(HttpStatusCode.Created, job);
                result.Headers.Location = new Uri(uri);
            }
            catch (AlreadyExistsException e)
            {
                result = Request.CreateErrorResponse(HttpStatusCode.Conflict, e);
            }
            catch (ArgumentException e)
            {
                result = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Object to create Job is invalid.\n ", e);
            }
            catch (CreateException e)
            {
                result = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request failed for Job.\n ", e);
            }
            catch (RejectedException e)
            {
                result = Request.CreateErrorResponse(HttpStatusCode.NotFound, "Create request rejected for Job with ID of " + item.id + ".\n", e);
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
        /// GET api/services/{functionalservice}
        /// </summary>
        /// <returns></returns>
        public override ICollection<jobType> Get([MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            checkAuthorisation(zone, context);
            return base.Get(zone, context);
        }

        /// <summary>
        /// GET api/services/{functionalservice}/{id}
        /// </summary>
        /// <returns></returns>
        public override jobType Get(Guid id, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            checkAuthorisation(zone, context);
            return base.Get(id, zone, context);
        }

        /// <summary>
        /// PUT api/services/{functionalservice}
        /// This operation is forbidden.
        /// </summary>
        /// <param name="id">Identifier for the object to update.</param>
        /// <param name="item">Object to update.</param>
        /// <returns>HTTP status 403.</returns>
        public override void Put(Guid id, jobType item, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            checkAuthorisation(zone, context);
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// DELETE api/services/{functionalservice}
        /// </summary>
        /// <param name="id">Identifier of the functional service to delete.</param>
        public override void Delete(Guid id, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            checkAuthorisation(zone, context);
            base.Delete(id, zone, context);
        }

        public virtual string Post(Guid id, string phaseName, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            checkAuthorisation(zone, context);
            preventPagingHeaders();

            string body = Request.Content.ReadAsStringAsync().Result;

            try
            {
                return Service.CreateToPhase(id, phaseName, body, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]), contentType: HttpUtils.getContentType(Request), accept: HttpUtils.GetAccept(Request));
            }
            catch (ArgumentException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid argument: id=" + id + ", phaseName=" + phaseName + ".\n" + e.Message));
            }
            catch (CreateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Problem creating data for " + phaseName + "@" + id + ".\n", e));
            }
            catch (RejectedException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Problem creating data for " + phaseName + "@" + id + ".\n", e));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Request failed for phase " + phaseName + " in job " + id + ".\n " + e.Message));
            }
        }

        public virtual string Get(Guid id, string phaseName, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            checkAuthorisation(zone, context);
            preventPagingHeaders();

            string body = Request.Content.ReadAsStringAsync().Result;

            try
            {
                return Service.RetrieveToPhase(id, phaseName, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]), contentType: HttpUtils.getContentType(Request), accept: HttpUtils.GetAccept(Request));
            }
            catch (ArgumentException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid argument: id=" + id + ", phaseName=" + phaseName + ".\n", e));
            }
            catch (NotFoundException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Problem retrieving data for " + phaseName + "@" + id + ".\n", e));
            }
            catch (RejectedException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Problem retrieving data for " + phaseName + "@" + id + ".\n", e));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Request failed for phase " + phaseName + " in job " + id + ".\n " + e.Message));
            }
        }

        public virtual string Put(Guid id, string phaseName, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            checkAuthorisation(zone, context);

            string body = Request.Content.ReadAsStringAsync().Result;

            try
            {
                return Service.UpdateToPhase(id, phaseName, body, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]), contentType: HttpUtils.getContentType(Request), accept: HttpUtils.GetAccept(Request));
            }
            catch (ArgumentException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid argument for Phase " + phaseName + " in job " + id + ".\n" + e.Message));
            }
            catch (UpdateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Problem updating data for " + phaseName + "@" + id + ".\n", e));
            }
            catch (RejectedException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Problem updating data for " + phaseName + "@" + id + ".\n", e));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Request failed for phase " + phaseName + " in job " + id + ".\n " + e.Message, e));
            }
        }

        public virtual string Delete(Guid id, string phaseName, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            checkAuthorisation(zone, context);
            preventPagingHeaders();

            string body = Request.Content.ReadAsStringAsync().Result;

            try
            {
                return Service.DeleteToPhase(id, phaseName, body, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]), contentType: HttpUtils.getContentType(Request), accept: HttpUtils.GetAccept(Request));
            }
            catch (ArgumentException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid argument: id=" + id + ", phaseName=" + phaseName + ".\n" + e.Message));
            }
            catch (DeleteException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Problem deleting data for " + phaseName + "@" + id + ".\n", e));
            }
            catch (RejectedException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Problem deleting data for " + phaseName + "@" + id + ".\n", e));
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Request failed for phase " + phaseName + " in job " + id + ".\n " + e.Message));
            }
        }

        protected virtual void checkAuthorisation(string[] zone, string[] context)
        {
            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            if ((zone != null && zone.Length != 1) || (context != null && context.Length != 1))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request failed for Phase as Zone and/or Context are invalid."));
            }
        }

        protected virtual void preventPagingHeaders()
        {
            if (HttpUtils.HasPagingHeaders(Request.Headers))
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }
        }
    }

}
