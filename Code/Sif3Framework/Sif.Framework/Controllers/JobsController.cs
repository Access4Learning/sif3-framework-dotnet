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

using log4net;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Service.Authentication;
using Sif.Framework.Service.Functional;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Utils;
using Sif.Framework.WebApi.ModelBinders;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace Sif.Framework.Controllers
{
    public abstract class JobsController<FunctionalService> : SifController<jobType, Job> where FunctionalService : BasicFunctionalService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

        BasicFunctionalService Service { get { return service as BasicFunctionalService; } }

        public override HttpResponseMessage Post(jobType item, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            checkAuthorisation(zone, context, new Right(RightType.CREATE, RightValue.APPROVED));

            HttpResponseMessage result;
            try
            {
                Guid id = Service.Create(item, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
                jobType job = Service.Retrieve(id, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
                
                string uri = Url.Link("DefaultApi", new { controller = item.name, id = id });
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
            checkAuthorisation(zone, context, new Right(RightType.QUERY, RightValue.APPROVED));
            return base.Get(zone, context);
        }

        /// <summary>
        /// GET api/services/{functionalservice}/{id}
        /// </summary>
        /// <returns></returns>
        public override jobType Get(Guid id, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            checkAuthorisation(zone, context, new Right(RightType.QUERY, RightValue.APPROVED));
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
            checkAuthorisation(zone, context, new Right(RightType.DELETE, RightValue.APPROVED));
            base.Delete(id, zone, context);
        }

        public virtual string Post(Guid id, string phaseName, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            checkAuthorisation(zone, context, new Right(RightType.CREATE, RightValue.APPROVED));
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
            checkAuthorisation(zone, context, new Right(RightType.QUERY, RightValue.APPROVED));
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
            checkAuthorisation(zone, context, new Right(RightType.UPDATE, RightValue.APPROVED));

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
            checkAuthorisation(zone, context, new Right(RightType.DELETE, RightValue.APPROVED));
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

        protected virtual string checkAuthorisation(string[] zone, string[] context)
        {
            string sessionToken = "";
            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization, out sessionToken))
            {
                log.Debug("Could not verify request headers. Session token was " + sessionToken);
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            if ((zone != null && zone.Length != 1) || (context != null && context.Length != 1))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request failed as Zone and/or Context are invalid."));
            }

            log.Debug("Zone: " + zone);
            log.Debug("context: " + context);
            log.Debug("Session: " + sessionToken);

            return sessionToken;
        }

        protected virtual void checkAuthorisation(string[] zone, string[] context, Right right)
        {
            string sessionToken = checkAuthorisation(zone, context);
            environmentType environment = (new EnvironmentService()).RetrieveBySessionToken(sessionToken);
            checkRights(getRights(getProvisionedZone(environment, zone)), right);
        }

        private provisionedZoneType getProvisionedZone(environmentType environment, string[] zones)
        {
            string zone = null;

            if (zones == null)
            {
                // Null zone, assuming default
                if (environment.defaultZone != null)
                {
                    zone = environment.defaultZone.id;
                    log.Debug("Using defined default zone ID ");
                }

                log.Debug("Zone not passed nor specified in the environment");

                // No default defined, so if there is exactly one zone defined we can just return that
                if (environment.provisionedZones != null && environment.provisionedZones.Length == 1)
                {
                    log.Debug("Assuming use of only declared zone " + environment.provisionedZones[0].id);
                    return environment.provisionedZones[0];
                }
            } else
            {
                zone = zones[0];
            }

            log.Debug("Looking for zone with ID " + zone);

            foreach (provisionedZoneType pzone in environment.provisionedZones)
            {
                if (pzone.id.Equals(zone))
                {
                    log.Debug("Found the zone " + pzone.id);
                    return pzone;
                }
            }

            string msg = "Request failed as Zone is invalid.";
            log.Debug(msg);
            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, msg));
        }

        private rightType[] getRights(provisionedZoneType zone)
        {
            foreach (serviceType service in zone.services)
            {
                log.Debug("Inspecting access rights for " + service.type + " service " + service.name);
                if (service.type.Equals(ServiceType.FUNCTIONAL.ToString()) && service.name.Equals(Service.TypeName))
                {
                    log.Debug("Found what we were looking for!");
                    return service.rights;
                }
            }
            string msg = "Request failed as no FUNCTIONAL service found with the name " + Service.TypeName + ".";
            log.Debug(msg);
            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, msg));
        }

        private void checkRights(rightType[] rights, Right right)
        {
            foreach (rightType r in rights)
            {
                if (r.type.Equals(right.Type) && r.Value.Equals(right.Value))
                {
                    log.Debug(r.type + " has the expected value " + right.Value);
                    return;
                }
            }
            string msg = "Request failed as FUNCTIONAL service " + Service.TypeName + " does not have sufficient access rights to perform an " + right.Type + " operation";
            log.Debug(msg);
            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, msg));
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
