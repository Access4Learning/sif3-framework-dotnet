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

using Sif.Framework.Demo.EventsConnector.Models;
using Sif.Framework.Demo.EventsConnector.Services;
using Sif.Framework.Providers;
using Sif.Framework.WebApi.ModelBinders;
using System.Web.Http;
using System.Collections.Generic;
using Sif.Specification.Infrastructure;
using Sif.Framework.Utils;
using System.Net;
using Sif.Framework.Model.Exceptions;
using System;
using System.Linq;

namespace Sif.Framework.Demo.EventsConnector.Controllers
{

    public class EventsProvider : BasicProvider<StudentPersonal>
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EventsProvider() : base(new StudentPersonalService())
        {
        }

        [Route("~/api/Events/Event")]
        public override IHttpActionResult Post(StudentPersonal obj, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            return base.Post(obj, zoneId, contextId);
        }

        public override IHttpActionResult Post(List<StudentPersonal> objs, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {

            foreach (KeyValuePair<string, IEnumerable<string>> nameValues in Request.Headers)
            {
                if (log.IsDebugEnabled) log.Debug($"*** Header field is [{nameValues.Key}:{string.Join(",", nameValues.Value)}]");
            }

            //return base.Post(objs, zoneId, contextId);
            string sessionToken;

            if (!authenticationService.VerifyAuthenticationHeader(Request.Headers, out sessionToken))
            {
                return Unauthorized();
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(StudentPersonal).Name + " as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;
            ICollection<createType> createStatuses = new List<createType>();

            try
            {
                bool? mustUseAdvisory = HttpUtils.GetMustUseAdvisory(Request.Headers);

                foreach (StudentPersonal obj in objs)
                {
                    bool hasAdvisoryId = !string.IsNullOrWhiteSpace(obj.RefId);
                    createType status = new createType();
                    status.advisoryId = (hasAdvisoryId ? obj.RefId : null);

                    try
                    {

                        if (mustUseAdvisory.HasValue && mustUseAdvisory.Value == true)
                        {

                            if (hasAdvisoryId)
                            {
                                status.id = service.Create(obj, mustUseAdvisory, zoneId: (zoneId == null ? null : zoneId[0]), contextId: (contextId == null ? null : contextId[0])).RefId;
                                status.statusCode = ((int)HttpStatusCode.Created).ToString();
                            }
                            else
                            {
                                status.error = ProviderUtils.CreateError(HttpStatusCode.BadRequest, typeof(StudentPersonal).Name, "Create request failed as object ID is not provided, but mustUseAdvisory is true.");
                                status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                            }

                        }
                        else
                        {
                            status.id = service.Create(obj, zoneId: (zoneId == null ? null : zoneId[0]), contextId: (contextId == null ? null : contextId[0])).RefId;
                            status.statusCode = ((int)HttpStatusCode.Created).ToString();
                        }

                    }
                    catch (AlreadyExistsException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.Conflict, typeof(StudentPersonal).Name, "Object " + typeof(StudentPersonal).Name + " with ID of " + obj.RefId + " already exists.\n" + e.Message);
                        status.statusCode = ((int)HttpStatusCode.Conflict).ToString();
                    }
                    catch (ArgumentException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.BadRequest, typeof(StudentPersonal).Name, "Object to create of type " + typeof(StudentPersonal).Name + (hasAdvisoryId ? " with ID of " + obj.RefId : "") + " is invalid.\n " + e.Message);
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (CreateException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.BadRequest, typeof(StudentPersonal).Name, "Request failed for object " + typeof(StudentPersonal).Name + (hasAdvisoryId ? " with ID of " + obj.RefId : "") + ".\n " + e.Message);
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (RejectedException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.NotFound, typeof(StudentPersonal).Name, "Create request rejected for object " + typeof(StudentPersonal).Name + " with ID of " + obj.RefId + ".\n" + e.Message);
                        status.statusCode = ((int)HttpStatusCode.Conflict).ToString();
                    }
                    catch (Exception e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.InternalServerError, typeof(StudentPersonal).Name, "Request failed for object " + typeof(StudentPersonal).Name + (hasAdvisoryId ? " with ID of " + obj.RefId : "") + ".\n " + e.Message);
                        status.statusCode = ((int)HttpStatusCode.InternalServerError).ToString();
                    }

                    createStatuses.Add(status);
                }

            }
            catch (Exception)
            {
                // Need to ignore exceptions otherwise it would not be possible to return status records of processed objects.
            }

            createResponseType createResponse = new createResponseType { creates = createStatuses.ToArray() };
            result = Ok(createResponse);

            return result;
        }

    }

}