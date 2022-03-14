/*
 * Copyright 2022 Systemic Pty Ltd
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

using Sif.Framework.Persistence.NHibernate;
using Sif.Framework.Service;
using Sif.Framework.Service.Authentication;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.WebApi.ModelBinders;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.AspNet.Controllers
{
    /// <summary>
    /// This class defines a base Controller containing common operations.
    /// </summary>
    /// <typeparam name="TDto">Object type exposed at the presentation/API layer.</typeparam>
    /// <typeparam name="TEntity">Object type used in the business layer.</typeparam>
    public abstract class SifController<TDto, TEntity> : ApiController
        where TDto : new()
        where TEntity : IHasUniqueIdentifier<Guid>, new()
    {
        /// <summary>
        /// Authentication service.
        /// </summary>
        protected IAuthenticationService AuthService;

        /// <summary>
        /// Service for SIF Object operations.
        /// </summary>
        protected ISifService<TDto, TEntity> Service;

        /// <summary>
        /// Create an instance.
        /// </summary>
        /// <param name="service">Service used for managing conversion between the object types.</param>
        protected SifController(ISifService<TDto, TEntity> service)
        {
            Service = service;
            AuthService = new DirectAuthenticationService(
                new ApplicationRegisterService(),
                new EnvironmentService(new EnvironmentRepository()));
        }

        /// <summary>
        /// DELETE api/{controller}/{id}
        /// </summary>
        /// <param name="id">Identifier of the object to delete.</param>
        /// <param name="zoneId">The zone in which to perform the request.</param>
        /// <param name="contextId">The context in which to perform the request.</param>
        public virtual void Delete(
            Guid id,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthService.VerifyAuthenticationHeader(Request.Headers))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            try
            {
                TDto item = Service.Retrieve(id);

                if (item == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                Service.Delete(id);
            }
            catch (Exception e)
            {
                var errorMessage =
                    $"The DELETE request failed for a {typeof(TDto).Name} with an ID of {id} due to the following error:\n{e.Message}";

                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }
        }

        /// <summary>
        /// GET api/{controller}/{id}
        /// </summary>
        /// <param name="id">Identifier of the object to retrieve.</param>
        /// <param name="zoneId">The zone in which to perform the request.</param>
        /// <param name="contextId">The context in which to perform the request.</param>
        /// <returns>Object with that identifier.</returns>
        public virtual TDto Get(
            Guid id,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthService.VerifyAuthenticationHeader(Request.Headers))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            TDto item;

            try
            {
                item = Service.Retrieve(id);
            }
            catch (Exception e)
            {
                var errorMessage =
                    $"The GET request failed for a {typeof(TDto).Name} with an ID of {id} due to the following error:\n{e.Message}";

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
        public virtual ICollection<TDto> Get(
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthService.VerifyAuthenticationHeader(Request.Headers))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            ICollection<TDto> items;

            try
            {
                items = Service.Retrieve();
            }
            catch (Exception e)
            {
                var errorMessage =
                    $"The GET request failed for {typeof(TDto).Name} due to the following error:\n{e.Message}";

                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }

            return items;
        }

        /// <summary>
        /// POST api/{controller}
        /// </summary>
        /// <param name="item">Object to create.</param>
        /// <param name="zoneId">The zone in which to perform the request.</param>
        /// <param name="contextId">The context in which to perform the request.</param>
        /// <returns>HTTP response message indicating success or failure.</returns>
        public virtual HttpResponseMessage Post(
            TDto item,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthService.VerifyAuthenticationHeader(Request.Headers))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            HttpResponseMessage responseMessage;

            try
            {
                Guid id = Service.Create(item);
                TDto newItem = Service.Retrieve(id);
                responseMessage = Request.CreateResponse(HttpStatusCode.Created, newItem);
                //string uri = Url.Link("DefaultApi", new { id = id });
                //responseMessage.Headers.Location = new Uri(uri);
            }
            catch (Exception e)
            {
                var errorMessage =
                    $"The POST request failed for {typeof(TDto).Name} due to the following error:\n{e.Message}";

                responseMessage = Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            return responseMessage;
        }

        /// <summary>
        /// PUT api/{controller}/{id}
        /// </summary>
        /// <param name="id">Identifier for the object to update.</param>
        /// <param name="zoneId">The zone in which to perform the request.</param>
        /// <param name="contextId">The context in which to perform the request.</param>
        /// <param name="item">Object to update.</param>
        public virtual void Put(
            Guid id,
            TDto item,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthService.VerifyAuthenticationHeader(Request.Headers))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    TDto existingItem = Service.Retrieve(id);

                    if (existingItem == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }

                    Service.Update(item);
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception e)
            {
                var errorMessage =
                    $"The PUT request failed for a {typeof(TDto).Name} with an ID of {id} due to the following error:\n{e.Message}";

                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }
        }
    }
}