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

using Sif.Framework.Model.Persistence;
using Sif.Framework.Service;
using Sif.Framework.Service.Authentication;
using Sif.Framework.WebApi.ModelBinders;
using Sif.Framework.Service.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sif.Framework.Controllers
{

    /// <summary>
    /// This class defines a base Controller containing common operations.
    /// </summary>
    /// <typeparam name="UI">Object type exposed at the presentation/API layer.</typeparam>
    /// <typeparam name="DB">Object type used in the business layer.</typeparam>
    public abstract class SifController<UI, DB> : ApiController
        where UI : new()
        where DB : IPersistable<Guid>, new()
    {
        protected IAuthenticationService authService;
        protected ISifService<UI, DB> service;

        /// <summary>
        /// Create an instance.
        /// </summary>
        /// <param name="service">Service used for managing conversion between the object types.</param>
        public SifController(ISifService<UI, DB> service)
        {
            this.service = service;
            authService = new DirectAuthenticationService(new ApplicationRegisterService(), new EnvironmentService());
        }

        /// <summary>
        /// DELETE api/{controller}/{id}
        /// </summary>
        /// <param name="id">Identifier of the object to delete.</param>
        /// <param name="zone">The zone in which to perform the request.</param>
        /// <param name="context">The context in which to perform the request.</param>
        public virtual void Delete(Guid id, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
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
                    service.Delete(id);
                }

            }
            catch (Exception e)
            {
                string errorMessage = "The DELETE request failed for a " + typeof(UI).Name + " with an ID of " + id + " due to the following error:\n " + e.Message;
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

        }

        /// <summary>
        /// GET api/{controller}/{id}
        /// </summary>
        /// <param name="id">Identifier of the object to retrieve.</param>
        /// <param name="zone">The zone in which to perform the request.</param>
        /// <param name="context">The context in which to perform the request.</param>
        /// <returns>Object with that identifier.</returns>
        public virtual UI Get(Guid id, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            UI item;

            try
            {
                item = service.Retrieve(id);
            }
            catch (Exception e)
            {
                string errorMessage = "The GET request failed for a " + typeof(UI).Name + " with an ID of " + id + " due to the following error:\n " + e.Message;
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return item;
        }

        /// <summary>
        /// GET api/{controller}
        /// </summary>
        /// <returns>All objects.</returns>
        public virtual ICollection<UI> Get([MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            ICollection<UI> items;

            try
            {
                items = service.Retrieve();
            }
            catch (Exception e)
            {
                string errorMessage = "The GET request failed for " + typeof(UI).Name + " due to the following error:\n " + e.Message;
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

            return items;
        }

        /// <summary>
        /// POST api/{controller}
        /// </summary>
        /// <param name="item">Object to create.</param>
        /// <param name="zone">The zone in which to perform the request.</param>
        /// <param name="context">The context in which to perform the request.</param>
        /// <returns>HTTP response message indicating success or failure.</returns>
        public virtual HttpResponseMessage Post(UI item, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            
            HttpResponseMessage responseMessage = null;

            try
            {
                Guid id = service.Create(item);
                UI newItem = service.Retrieve(id);
                responseMessage = Request.CreateResponse<UI>(HttpStatusCode.Created, newItem);
                //string uri = Url.Link("DefaultApi", new { id = id });
                //responseMessage.Headers.Location = new Uri(uri);
            }
            catch (Exception e)
            {
                string errorMessage = "The POST request failed for " + typeof(UI).Name + " due to the following error:\n " + e.Message;
                responseMessage = Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            return responseMessage;
        }

        /// <summary>
        /// PUT api/{controller}/{id}
        /// </summary>
        /// <param name="id">Identifier for the object to update.</param>
        /// <param name="zone">The zone in which to perform the request.</param>
        /// <param name="context">The context in which to perform the request.</param>
        /// <param name="item">Object to update.</param>
        public virtual void Put(Guid id, UI item, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
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
            catch (Exception e)
            {
                string errorMessage = "The PUT request failed for a " + typeof(UI).Name + " with an ID of " + id + " due to the following error:\n " + e.Message;
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

        }

    }

}
