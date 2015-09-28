/*
 * Copyright 2015 Systemic Pty Ltd
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
using Sif.Framework.Model.Query;
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
        public virtual List<T> Get(T item)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            IEnumerable<String> navigationPageValues;
            IEnumerable<String> navigationPageSizeValues;
            bool navigationPageFound = Request.Headers.TryGetValues("navigationPage", out navigationPageValues);
            bool navigationPageSizeFound = Request.Headers.TryGetValues("navigationPageSize", out navigationPageSizeValues);

            if (navigationPageFound && !navigationPageSizeFound)
            {
                string errorMessage = "The GET request failed for " + typeof(T).Name + " because the navigationPage request header field was found, but not the navigationPageSize.";
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

            if (!navigationPageFound && navigationPageSizeFound)
            {
                string errorMessage = "The GET request failed for " + typeof(T).Name + " because the navigationPageSize request header field was found, but not the navigationPage.";
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

            bool methodOverrideFound = Request.Headers.Contains("X-HTTP-Method-Override");
            bool payloadFound = (item != null);
            List<T> items;

            try
            {

                if (navigationPageFound && navigationPageSizeFound)
                {
                    int? navigationPage = null;
                    int? navigationPageSize = null;

                    if ((new List<String>(navigationPageValues)).Count == 1)
                    {
                        IEnumerator<String> enumerator = navigationPageValues.GetEnumerator();
                        enumerator.MoveNext();
                        navigationPage = Int32.Parse(enumerator.Current);
                    }
                    else
                    {
                        string errorMessage = "The GET request failed for " + typeof(T).Name + " because multiple values were found for the navigationPage request header field.";
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
                    }

                    if ((new List<String>(navigationPageSizeValues)).Count == 1)
                    {
                        IEnumerator<String> enumerator = navigationPageSizeValues.GetEnumerator();
                        enumerator.MoveNext();
                        navigationPageSize = Int32.Parse(enumerator.Current);
                    }
                    else
                    {
                        string errorMessage = "The GET request failed for " + typeof(T).Name + " because multiple values were found for the navigationPageSize request header field.";
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
                    }

                    items = (List<T>)service.Retrieve((int)navigationPage, (int)navigationPageSize);
                }
                else if (methodOverrideFound && payloadFound)
                {
                    items = (List<T>)service.Retrieve(item);
                }
                else
                {
                    items = (List<T>)service.Retrieve();
                }

            }
            catch (Exception e)
            {
                string errorMessage = "The GET request failed for " + typeof(T).Name + " due to the following error:\n " + e.Message;
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

            return items;
        }

        /// <summary>
        /// GET api/{object1}/{id1}/{controller}
        /// GET api/{object1}/{id1}/{object2}/{id2}/{controller}
        /// GET api/{object1}/{id1}/{object2}/{id2}/{object3}/{id3}/{controller}
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="id1"></param>
        /// <param name="object2"></param>
        /// <param name="id2"></param>
        /// <param name="object3"></param>
        /// <param name="id3"></param>
        /// <returns>Objects that meet the associated object and ID combinations.</returns>
        public virtual List<T> Get(string object1, string id1, string object2 = null, string id2 = null, string object3 = null, string id3 = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            List<T> item;

            try
            {
                EqualCondition condition = new EqualCondition() { Left = object1, Right = id1 };
                IList<EqualCondition> conditions = new List<EqualCondition>() { condition };

                if (!string.IsNullOrWhiteSpace(object2))
                {
                    conditions.Add(new EqualCondition() { Left = object2, Right = id2 });

                    if (!string.IsNullOrWhiteSpace(object3))
                    {
                        conditions.Add(new EqualCondition() { Left = object3, Right = id3 });
                    }

                }

                item = (List<T>)service.Retrieve(conditions);
            }
            catch (Exception e)
            {
                string errorMessage = "The GET request (based on Service Path parameters) failed for a " + typeof(T).Name + " due to the following error:\n " + e.Message;
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return item;
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
