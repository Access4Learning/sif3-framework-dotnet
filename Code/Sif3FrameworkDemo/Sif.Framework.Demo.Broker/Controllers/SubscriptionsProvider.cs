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

using Sif.Framework.Demo.Broker.Models;
using Sif.Framework.Demo.Broker.Services;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Providers;
using Sif.Framework.Service.Providers;
using Sif.Framework.Utils;
using Sif.Framework.WebApi.ModelBinders;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Sif.Framework.Demo.Broker.Controllers
{

    public class SubscriptionsProvider : BasicProvider<Subscription>
    {

        protected SubscriptionsProvider() : base(new SubscriptionService())
        {
        }

        protected SubscriptionsProvider(IBasicProviderService<Subscription> service) : base(service)
        {
        }

        [NonAction]
        public override IHttpActionResult BroadcastEvents(string zoneId = null, string contextId = null)
        {
            return base.BroadcastEvents(zoneId, contextId);
        }

        public override IHttpActionResult Get([FromUri(Name = "id")] string refId, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            string sessionToken;

            if (!authenticationService.VerifyAuthenticationHeader(Request.Headers, out sessionToken))
            {
                return Unauthorized();
            }

            if (HttpUtils.HasPagingHeaders(Request.Headers))
            {
                return StatusCode(HttpStatusCode.MethodNotAllowed);
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(Subscription).Name + " as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                Subscription obj = service.Retrieve(refId, zoneId: (zoneId == null ? null : zoneId[0]), contextId: (contextId == null ? null : contextId[0]));

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
                result = BadRequest("Request failed for object " + typeof(Subscription).Name + " with ID of " + refId + ".\n " + e.Message);
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

        [NonAction]
        public override IHttpActionResult Post(List<Subscription> objs, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            return base.Post(objs, zoneId, contextId);
        }

        public override IHttpActionResult Post(Subscription obj, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            string sessionToken;

            if (!authenticationService.VerifyAuthenticationHeader(Request.Headers, out sessionToken))
            {
                return Unauthorized();
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(Subscription).Name + " as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;

            try
            {
                bool hasAdvisoryId = !string.IsNullOrWhiteSpace(obj.RefId);
                bool? mustUseAdvisory = HttpUtils.GetMustUseAdvisory(Request.Headers);

                if (mustUseAdvisory.HasValue && mustUseAdvisory.Value == true)
                {

                    if (hasAdvisoryId)
                    {
                        Subscription createdObject = service.Create(obj, mustUseAdvisory, zoneId: (zoneId == null ? null : zoneId[0]), contextId: (contextId == null ? null : contextId[0]));
                        string uri = Url.Link("DefaultApi", new { controller = TypeName, id = createdObject.RefId });
                        result = Created(uri, createdObject);
                    }
                    else
                    {
                        result = BadRequest($"Request failed for object {TypeName} as object ID is not provided, but mustUseAdvisory is true.");
                    }

                }
                else
                {
                    Subscription createdObject = service.Create(obj, zoneId: (zoneId == null ? null : zoneId[0]), contextId: (contextId == null ? null : contextId[0]));
                    string uri = Url.Link("DefaultApi", new { controller = typeof(Subscription).Name, id = createdObject.RefId });
                    result = Created(uri, createdObject);
                }

            }
            catch (AlreadyExistsException)
            {
                result = Conflict();
            }
            catch (ArgumentException e)
            {
                result = BadRequest("Object to create of type " + typeof(Subscription).Name + " is invalid.\n " + e.Message);
            }
            catch (CreateException e)
            {
                result = BadRequest("Request failed for object " + typeof(Subscription).Name + ".\n " + e.Message);
            }
            catch (RejectedException)
            {
                result = NotFound();
            }
            catch (QueryException e)
            {
                result = BadRequest("Request failed for object " + typeof(Subscription).Name + ".\n " + e.Message);
            }
            catch (Exception e)
            {
                result = InternalServerError(e);
            }

            return result;
        }

    }

}