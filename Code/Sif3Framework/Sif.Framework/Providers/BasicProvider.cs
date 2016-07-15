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

using Sif.Framework.Model.DataModels;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Service.Providers;
using Sif.Framework.Utils;
using Sif.Framework.WebApi.ModelBinders;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Sif.Framework.Providers
{

    /// <summary>
    /// This is a convenience class that defines a Provider of SIF data model objects whereby the primary key is of
    /// type System.String and the multiple objects entity is represented as a list of single objects.
    /// </summary>
    /// <typeparam name="T">Type of object associated with the Service Provider.</typeparam>
    public abstract class BasicProvider<T> : Provider<T, List<T>>, IProvider<T, List<T>, string> where T : ISifRefId<string>
    {

        /// <summary>
        /// Create an instance based on the specified service.
        /// </summary>
        /// <param name="service">Service used for managing the object type.</param>
        public BasicProvider(IBasicProviderService<T> service)
            : base()
        {
            this.service = service;
        }

        /// <summary>
        /// <see cref="Provider{TSingle, TMultiple}.Post(TMultiple, string[], string[])">Post</see>
        /// </summary>
        public override IHttpActionResult Post(List<T> objs, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            if ((zone != null && zone.Length != 1) || (context != null && context.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(T).Name + " as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;
            ICollection<createType> createStatuses = new List<createType>();

            try
            {

                foreach (T obj in objs)
                {
                    bool hasAdvisoryId = !string.IsNullOrWhiteSpace(obj.RefId);
                    createType status = new createType();
                    status.advisoryId = (hasAdvisoryId ? obj.RefId : null);

                    try
                    {
                        bool? mustUseAdvisory = HttpUtils.GetMustUseAdvisory(Request.Headers);

                        if (hasAdvisoryId)
                        {

                            if (mustUseAdvisory.HasValue && mustUseAdvisory.Value == true)
                            {
                                status.id = service.Create(obj, mustUseAdvisory, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0])).RefId;
                                status.statusCode = ((int)HttpStatusCode.Created).ToString();
                            }
                            else
                            {
                                status.error = ProviderUtils.CreateError(HttpStatusCode.BadRequest, typeof(T).Name, "Create request failed as object ID provided (" + obj.RefId + "), but mustUseAdvisory is not specified or is false.");
                                status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                            }

                        }
                        else
                        {

                            if (mustUseAdvisory.HasValue && mustUseAdvisory.Value == true)
                            {
                                status.error = ProviderUtils.CreateError(HttpStatusCode.BadRequest, typeof(T).Name, "Create request failed as object ID is not provided, but mustUseAdvisory is true.");
                                status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                            }
                            else
                            {
                                status.id = service.Create(obj, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0])).RefId;
                                status.statusCode = ((int)HttpStatusCode.Created).ToString();
                            }

                        }

                    }
                    catch (AlreadyExistsException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.Conflict, typeof(T).Name, "Object " + typeof(T).Name + " with ID of " + obj.RefId + " already exists.\n" + e.Message);
                        status.statusCode = ((int)HttpStatusCode.Conflict).ToString();
                    }
                    catch (ArgumentException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.BadRequest, typeof(T).Name, "Object to create of type " + typeof(T).Name + (hasAdvisoryId ? " with ID of " + obj.RefId : "") + " is invalid.\n " + e.Message);
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (CreateException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.BadRequest, typeof(T).Name, "Request failed for object " + typeof(T).Name + (hasAdvisoryId ? " with ID of " + obj.RefId : "") + ".\n " + e.Message);
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (RejectedException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.NotFound, typeof(T).Name, "Create request rejected for object " + typeof(T).Name + " with ID of " + obj.RefId + ".\n" + e.Message);
                        status.statusCode = ((int)HttpStatusCode.Conflict).ToString();
                    }
                    catch (Exception e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.InternalServerError, typeof(T).Name, "Request failed for object " + typeof(T).Name + (hasAdvisoryId ? " with ID of " + obj.RefId : "") + ".\n " + e.Message);
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

        /// <summary>
        /// <see cref="Provider{TSingle, TMultiple}.Put(TMultiple, string[], string[])">Put</see>
        /// </summary>
        public override IHttpActionResult Put(List<T> objs, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {

            if (!authService.VerifyAuthenticationHeader(Request.Headers.Authorization))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.

            if ((zone != null && zone.Length != 1) || (context != null && context.Length != 1))
            {
                return BadRequest("Request failed for object " + typeof(T).Name + " as Zone and/or Context are invalid.");
            }

            IHttpActionResult result;
            ICollection<updateType> updateStatuses = new List<updateType>();

            try
            {

                foreach (T obj in objs)
                {
                    updateType status = new updateType();
                    status.id = obj.RefId;

                    try
                    {
                        service.Update(obj, zone: (zone == null ? null : zone[0]), context: (context == null ? null : context[0]));
                        status.statusCode = ((int)HttpStatusCode.NoContent).ToString();
                    }
                    catch (ArgumentException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.BadRequest, typeof(T).Name, "Object to update of type " + typeof(T).Name + " is invalid.\n " + e.Message);
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (NotFoundException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.NotFound, typeof(T).Name, "Object " + typeof(T).Name + " with ID of " + obj.RefId + " not found.\n" + e.Message);
                        status.statusCode = ((int)HttpStatusCode.NotFound).ToString();
                    }
                    catch (UpdateException e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.BadRequest, typeof(T).Name, "Request failed for object " + typeof(T).Name + " with ID of " + obj.RefId + ".\n " + e.Message);
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (Exception e)
                    {
                        status.error = ProviderUtils.CreateError(HttpStatusCode.InternalServerError, typeof(T).Name, "Request failed for object " + typeof(T).Name + " with ID of " + obj.RefId + ".\n " + e.Message);
                        status.statusCode = ((int)HttpStatusCode.InternalServerError).ToString();
                    }

                    updateStatuses.Add(status);
                }

            }
            catch (Exception)
            {
                // Need to ignore exceptions otherwise it would not be possible to return status records of processed objects.
            }

            updateResponseType updateResponse = new updateResponseType { updates = updateStatuses.ToArray() };
            result = Ok(updateResponse);

            return result;
        }

    }

}
