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

using Sif.Framework.Model.Persistence;
using Sif.Framework.Service;
using Sif.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Sif.Framework.EnvironmentProvider.Controllers
{

    public abstract class GenericController<UI, DB> : ApiController
        where UI : new()
        where DB : IPersistable, new()
    {
        private IGenericService<UI, DB> service;

        string ConsumerSecret(string token)
        {
            return "SecretDem0";
        }

        private bool VerifyAuthorisationHeader(AuthenticationHeaderValue header)
        {
            bool verified = false;
            string scheme = header.Scheme;

            if ("Basic".Equals(scheme))
            {
                AuthenticationUtils.GetConsumerSecret consumerSecret = ConsumerSecret;
                string sessionToken;
                verified = AuthenticationUtils.VerifyBasicAuthorisationToken(header.ToString(), consumerSecret, out sessionToken);
            }
            else if ("SIF_HMACSHA256".Equals(scheme))
            {
                verified = true;
            }

            return verified;
        }

        // Need to inject repository.
        [NonAction]
        public abstract IGenericService<UI, DB> GetService();

        public GenericController()
        {
            service = GetService();
        }

        // DELETE api/{controller}/{id}
        public virtual void Delete(int id)
        {

            if (!VerifyAuthorisationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            try
            {
                UI item = service.Retrieve(id);

                if (item == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                else
                {
                    service.Delete(item);
                }

            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

        }

        // GET api/{controller}/{id}
        public virtual UI Get(int id)
        {

            if (!VerifyAuthorisationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            UI item;

            try
            {
                item = service.Retrieve(id);
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return item;
        }

        // GET api/{controller}
        public virtual IEnumerable<UI> Get()
        {

            if (!VerifyAuthorisationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            IEnumerable<UI> items;

            try
            {
                items = service.Retrieve();
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return items;
        }

        // POST api/{controller}
        public virtual HttpResponseMessage Post(UI item)
        {

            if (!VerifyAuthorisationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            
            HttpResponseMessage responseMessage = null;

            try
            {
                long id = service.Create(item);
                responseMessage = Request.CreateResponse<UI>(HttpStatusCode.Created, item);
                string uri = Url.Link("DefaultApi", new { id = id });
                responseMessage.Headers.Location = new Uri(uri);
            }
            catch (Exception)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return responseMessage;
        }

        // PUT api/{controller}/{id}
        public virtual void Put(int id, UI item)
        {

            if (!VerifyAuthorisationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            try
            {

                if (ModelState.IsValid)
                {
                    UI existingItem = service.Retrieve(id);

                    if (existingItem == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        service.Update(item);
                    }

                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

        }

    }

}
