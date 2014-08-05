/*
 * Copyright 2014 Systemic Pty Ltd
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

using Sif.Framework.Service.Infrastructure;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Controller
{

    /// <summary>
    /// This class defines a Controller for the Environment object.
    /// 
    /// Valid single operations: POST, GET, DELETE.
    /// Valid multiple operations: none.
    /// </summary>
    public abstract class EnvironmentsController : SifController<environmentType, Environment>
    {

        /// <summary>
        /// Create an instance.
        /// </summary>
        public EnvironmentsController()
            : base(new EnvironmentService())
        {

        }

        /// <summary>
        /// GET api/environments
        /// This operation is forbidden.
        /// </summary>
        /// <returns>HTTP status 403.</returns>
        public override ICollection<environmentType> Get()
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// POST api/environments
        /// This operation is forbidden.
        /// </summary>
        /// <param name="item">Object to create.</param>
        /// <returns>HTTP status 403.</returns>
        public override HttpResponseMessage Post(environmentType item)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// POST api/environments/environment
        /// </summary>
        /// <param name="item">Object to create.</param>
        /// <returns>HTTP response message indicating success or failure.</returns>
        [HttpPost]
        [Route("api/environments/environment")]
        public virtual HttpResponseMessage Create(environmentType item)
        {
            string initialToken;

            if (!VerifyInitialAuthorisationHeader(Request.Headers.Authorization, out initialToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            HttpResponseMessage responseMessage = null;

            try
            {
                Guid id = service.Create(item);
                environmentType newItem = service.Retrieve(id);
                responseMessage = Request.CreateResponse<environmentType>(HttpStatusCode.Created, newItem);
                //string uri = Url.Link("DefaultApi", new { id = id });
                //responseMessage.Headers.Location = new Uri(uri);
            }
            catch (Exception)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return responseMessage;
        }

        /// <summary>
        /// PUT api/environments/{id}
        /// This operation is forbidden (raises HTTP status 403).
        /// </summary>
        /// <param name="id">Identifier for the object to update.</param>
        /// <param name="item">Object to update.</param>
        public override void Put(Guid id, environmentType item)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

    }

}
