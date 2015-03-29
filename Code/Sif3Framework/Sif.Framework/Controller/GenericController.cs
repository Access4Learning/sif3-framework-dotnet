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

using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Persistence;
using Sif.Framework.Service;
using Sif.Framework.Service.Authentication;
using Sif.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sif.Framework.Controller
{

    /// <summary>
    /// This class defines a base Controller containing common operations.
    /// </summary>
    /// <typeparam name="T">Type of object managed by the Controller.</typeparam>
    /// <typeparam name="PK">Primary key type of the object.</typeparam>
    public abstract class GenericController<T, PK> : ApiController where T : IPersistable<PK>
    {
        protected IAuthenticationService authService; 
        protected IGenericService<T, PK> service;

        /// <summary>
        /// Create an instance.
        /// </summary>
        /// <param name="service">Service used for managing the object type.</param>
        public GenericController(IGenericService<T, PK> service)
        {
            this.service = service;

            if (EnvironmentType.DIRECT.Equals(SettingsManager.ProviderSettings.EnvironmentType))
            {
                authService = new DirectAuthenticationService();
            }
            else if (EnvironmentType.BROKERED.Equals(SettingsManager.ProviderSettings.EnvironmentType))
            {
                authService = new BrokeredAuthenticationService();
            }

        }

        /// <summary>
        /// DELETE api/{controller}/{id}
        /// </summary>
        /// <param name="id">Identifier of the object to delete.</param>
        public virtual void Delete(PK id)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            try
            {
                T item = service.Retrieve(id);

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
                string errorMessage = "The DELETE request failed for a " + typeof(T).Name + " with an ID of " + id + " due to the following error:\n " + e.Message;
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

        }

        /// <summary>
        /// DELETE api/{controller}
        /// </summary>
        /// <param name="ids">Collection of identifiers for the objects to delete.</param>
        public virtual void Delete(IEnumerable<PK> ids)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// GET api/{controller}/{id}
        /// </summary>
        /// <param name="id">Identifier of the object to retrieve.</param>
        /// <returns>Object with that identifier.</returns>
        public virtual T Get(PK id)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            T item;

            try
            {
                item = service.Retrieve(id);
            }
            catch (Exception e)
            {
                string errorMessage = "The GET request failed for a " + typeof(T).Name + " with an ID of " + id + " due to the following error:\n " + e.Message;
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
        public virtual List<T> Get()
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            List<T> items;

            try
            {
                items = (List<T>)service.Retrieve();
            }
            catch (Exception e)
            {
                string errorMessage = "The GET request failed for " + typeof(T).Name + " due to the following error:\n " + e.Message;
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

            return items;
        }

        /// <summary>
        /// POST api/{controller}
        /// </summary>
        /// <param name="item">Object to create.</param>
        /// <returns>HTTP response message indicating success or failure.</returns>
        public virtual HttpResponseMessage Post(T item)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            HttpResponseMessage responseMessage = null;

            try
            {
                PK id = service.Create(item);
                T newItem = service.Retrieve(id);
                responseMessage = Request.CreateResponse<T>(HttpStatusCode.Created, newItem);
                //string uri = Url.Link("DefaultApi", new { id = id });
                //responseMessage.Headers.Location = new Uri(uri);
            }
            catch (Exception e)
            {
                string errorMessage = "The POST request failed for " + typeof(T).Name + " due to the following error:\n " + e.Message;
                responseMessage = Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            return responseMessage;
        }

        /// <summary>
        /// POST api/{controller}
        /// </summary>
        /// <param name="items">Objects to create.</param>
        /// <returns>HTTP response message indicating success or failure.</returns>
        public virtual HttpResponseMessage Post(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// PUT api/{controller}/{id}
        /// </summary>
        /// <param name="id">Identifier for the object to update.</param>
        /// <param name="item">Object to update.</param>
        public virtual void Put(PK id, T item)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            try
            {

                if (ModelState.IsValid)
                {
                    T existingItem = service.Retrieve(id);

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
                string errorMessage = "The PUT request failed for a " + typeof(T).Name + " with an ID of " + id + " due to the following error:\n " + e.Message;
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

        }

        /// <summary>
        /// PUT api/{controller}
        /// </summary>
        /// <param name="items">Objects to update.</param>
        public virtual void Put(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

    }

}
