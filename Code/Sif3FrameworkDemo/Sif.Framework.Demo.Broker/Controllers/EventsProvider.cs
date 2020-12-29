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

using Sif.Framework.Demo.Broker.Models;
using Sif.Framework.Demo.Broker.Services;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Providers;
using Sif.Framework.Utils;
using Sif.Framework.WebApi.ModelBinders;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Sif.Framework.Demo.Broker.Controllers
{
    public class EventsProvider : BasicProvider<StudentPersonal>
    {
        private readonly slf4net.ILogger log =
            slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EventsProvider() : base(new StudentPersonalService(), SettingsManager.ProviderSettings)
        {
        }

        [Route("~/api/Events/Event")]
        public override IHttpActionResult Post(
            StudentPersonal obj,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            return base.Post(obj, zoneId, contextId);
        }

        public override IHttpActionResult Post(
            List<StudentPersonal> objs,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> nameValues in Request.Headers)
            {
                if (log.IsDebugEnabled)
                    log.Debug($"*** Header field is [{nameValues.Key}:{string.Join(",", nameValues.Value)}]");
            }

            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string _))
            {
                return Unauthorized();
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            ICollection<createType> createStatuses = new List<createType>();

            try
            {
                bool? mustUseAdvisory = HttpUtils.GetMustUseAdvisory(Request.Headers);

                foreach (StudentPersonal obj in objs)
                {
                    bool hasAdvisoryId = !string.IsNullOrWhiteSpace(obj.RefId);
                    var status = new createType { advisoryId = (hasAdvisoryId ? obj.RefId : null) };

                    try
                    {
                        if (mustUseAdvisory.HasValue)
                        {
                            if (mustUseAdvisory.Value && !hasAdvisoryId)
                            {
                                status.error = ProviderUtils.CreateError(
                                    HttpStatusCode.BadRequest,
                                    TypeName,
                                    "Create request failed as object ID is not provided, but mustUseAdvisory is true.");
                                status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                            }
                            else
                            {
                                status.id = Service.Create(obj, mustUseAdvisory, zoneId?[0], contextId?[0]).RefId;
                                status.statusCode = ((int)HttpStatusCode.Created).ToString();
                            }
                        }
                        else
                        {
                            status.id = Service.Create(obj, null, zoneId?[0], contextId?[0]).RefId;
                            status.statusCode = ((int)HttpStatusCode.Created).ToString();
                        }
                    }
                    catch (AlreadyExistsException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.Conflict,
                            TypeName,
                            $"Object {TypeName} with ID of {obj.RefId} already exists.\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.Conflict).ToString();
                    }
                    catch (ArgumentException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.BadRequest,
                            TypeName,
                            $"Object to create of type {TypeName}" +
                            (hasAdvisoryId ? $" with ID of {obj.RefId}" : "") + $" is invalid.\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (CreateException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.BadRequest,
                            TypeName,
                            $"Request failed for object {TypeName}" +
                            (hasAdvisoryId ? $" with ID of {obj.RefId}" : "") + $".\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (RejectedException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.NotFound,
                            TypeName,
                            $"Create request rejected for object {TypeName} with ID of {obj.RefId}.\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.Conflict).ToString();
                    }
                    catch (Exception e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.InternalServerError,
                            TypeName,
                            $"Request failed for object {TypeName}" +
                            (hasAdvisoryId ? $" with ID of {obj.RefId}" : "") + $".\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.InternalServerError).ToString();
                    }

                    createStatuses.Add(status);
                }
            }
            catch (Exception)
            {
                // Need to ignore exceptions otherwise it would not be possible to return status records of processed objects.
            }

            var createResponse = new createResponseType { creates = createStatuses.ToArray() };

            return Ok(createResponse);
        }
    }
}