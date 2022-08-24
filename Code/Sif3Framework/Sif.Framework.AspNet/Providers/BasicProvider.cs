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

using Sif.Framework.AspNet.ModelBinders;
using Sif.Framework.Extensions;
using Sif.Framework.Models.DataModels;
using Sif.Framework.Models.Exceptions;
using Sif.Framework.Models.Infrastructure;
using Sif.Framework.Models.Settings;
using Sif.Framework.Services.Infrastructure;
using Sif.Framework.Services.Providers;
using Sif.Framework.Services.Serialisation;
using Sif.Framework.Services.Sessions;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Xml.Serialization;
using Tardigrade.Framework.Exceptions;

namespace Sif.Framework.AspNet.Providers
{
    /// <summary>
    /// This is a convenience class that defines a Provider of SIF data model objects whereby the primary key is of
    /// type System.String and the multiple objects entity is represented as a list of single objects.
    /// </summary>
    /// <typeparam name="T">Type of object associated with the Service Provider.</typeparam>
    public abstract class BasicProvider<T> : Provider<T, List<T>> where T : ISifRefId<string>
    {
        /// <inheritdoc />
        protected BasicProvider(
            IBasicProviderService<T> service,
            IApplicationRegisterService applicationRegisterService,
            IEnvironmentService environmentService,
            IFrameworkSettings settings = null,
            ISessionService sessionService = null)
            : base(service, applicationRegisterService, environmentService, settings, sessionService)
        {
        }

        /// <summary>
        /// <see cref="Provider{TSingle, TMultiple}.Post(TMultiple, string[], string[])">Post</see>
        /// </summary>
        public override IHttpActionResult Post(
            List<T> items,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.
            if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.CREATE))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            ICollection<createType> createStatuses = new List<createType>();

            try
            {
                bool? mustUseAdvisory = Request.Headers.GetMustUseAdvisory();

                foreach (T item in items)
                {
                    bool hasAdvisoryId = !string.IsNullOrWhiteSpace(item.RefId);
                    var status = new createType
                    {
                        advisoryId = (hasAdvisoryId ? item.RefId : null)
                    };

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
                                status.id = Service.Create(item, mustUseAdvisory, zoneId?[0], contextId?[0]).RefId;
                                status.statusCode = ((int)HttpStatusCode.Created).ToString();
                            }
                        }
                        else
                        {
                            status.id = Service.Create(item, null, zoneId?[0], contextId?[0]).RefId;
                            status.statusCode = ((int)HttpStatusCode.Created).ToString();
                        }
                    }
                    catch (AlreadyExistsException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.Conflict,
                            TypeName,
                            $"Object {TypeName} with ID of {item.RefId} already exists.\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.Conflict).ToString();
                    }
                    catch (ArgumentException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.BadRequest,
                            TypeName,
                            $"Object to create of type {TypeName}" + (hasAdvisoryId ? $" with ID of {item.RefId}" : "") + $" is invalid.\n {e.Message}");
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (CreateException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.BadRequest,
                            TypeName,
                            $"Request failed for object {TypeName}" + (hasAdvisoryId ? $" with ID of {item.RefId}" : "") + $".\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (RejectedException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.NotFound,
                            TypeName,
                            $"Create request rejected for object {TypeName} with ID of {item.RefId}.\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.Conflict).ToString();
                    }
                    catch (Exception e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.InternalServerError,
                            TypeName,
                            $"Request failed for object {TypeName}" + (hasAdvisoryId ? $" with ID of {item.RefId}" : "") + $".\n{e.Message}");
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

        /// <summary>
        /// <see cref="Provider{TSingle, TMultiple}.Put(TMultiple, string[], string[])">Put</see>
        /// </summary>
        public override IHttpActionResult Put(
            List<T> items,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.
            if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.CREATE))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            ICollection<updateType> updateStatuses = new List<updateType>();

            try
            {
                foreach (T item in items)
                {
                    var status = new updateType
                    {
                        id = item.RefId
                    };

                    try
                    {
                        Service.Update(item, zoneId?[0], contextId?[0]);
                        status.statusCode = ((int)HttpStatusCode.NoContent).ToString();
                    }
                    catch (ArgumentException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.BadRequest,
                            TypeName,
                            $"Object to update of type {TypeName} is invalid.\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (NotFoundException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.NotFound,
                            TypeName,
                            $"Object {TypeName} with ID of {item.RefId} not found.\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.NotFound).ToString();
                    }
                    catch (UpdateException e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.BadRequest,
                            TypeName,
                            $"Request failed for object {TypeName} with ID of {item.RefId}.\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.BadRequest).ToString();
                    }
                    catch (Exception e)
                    {
                        status.error = ProviderUtils.CreateError(
                            HttpStatusCode.InternalServerError,
                            TypeName,
                            $"Request failed for object {TypeName} with ID of {item.RefId}.\n{e.Message}");
                        status.statusCode = ((int)HttpStatusCode.InternalServerError).ToString();
                    }

                    updateStatuses.Add(status);
                }
            }
            catch (Exception)
            {
                // Need to ignore exceptions otherwise it would not be possible to return status records of processed objects.
            }

            var updateResponse = new updateResponseType { updates = updateStatuses.ToArray() };

            return Ok(updateResponse);
        }

        /// <summary>
        /// <see cref="Provider{TSingle, TMultiple}.SerialiseEvents(TMultiple)">SerialiseEvents</see>
        /// </summary>
        [NonAction]
        public override string SerialiseEvents(List<T> obj)
        {
            var xmlRootAttribute = new XmlRootAttribute(TypeName + "s")
            {
                Namespace = ProviderSettings.DataModelNamespace,
                IsNullable = false
            };

            return SerialiserFactory.GetSerialiser<List<T>>(ContentType, xmlRootAttribute).Serialise(obj);
        }
    }
}