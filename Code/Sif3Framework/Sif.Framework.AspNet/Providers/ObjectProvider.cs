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
using Sif.Framework.Model.DataModels;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Responses;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Providers;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Service.Sessions;
using Sif.Specification.Infrastructure;
using System.Net;
using System.Web.Http;

namespace Sif.Framework.AspNet.Providers
{
    /// <summary>
    /// This class defines a Provider of SIF data model objects whereby the primary key is of type System.String.
    /// </summary>
    /// <typeparam name="TSingle">Type that defines a single object entity.</typeparam>
    /// <typeparam name="TMultiple">Type that defines a multiple objects entity.</typeparam>
    public abstract class ObjectProvider<TSingle, TMultiple>
        : Provider<TSingle, TMultiple> where TSingle : ISifRefId<string>
    {
        private readonly IObjectProviderService<TSingle, TMultiple> service;

        /// <inheritdoc />
        protected ObjectProvider(
            IObjectProviderService<TSingle, TMultiple> service,
            IApplicationRegisterService applicationRegisterService,
            IEnvironmentService environmentService,
            IFrameworkSettings settings = null,
            ISessionService sessionService = null)
            : base(service, applicationRegisterService, environmentService, settings, sessionService)
        {
            this.service = service;
        }

        /// <inheritdoc cref="IProvider{TSingle,TMultiple,TPrimaryKey}.Post(TMultiple, string[], string[])" />
        public override IHttpActionResult Post(
            TMultiple obj,
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

            bool? mustUseAdvisory = Request.Headers.GetMustUseAdvisory();
            RequestParameter[] requestParameters = GetQueryParameters(Request);
            MultipleCreateResponse multipleCreateResponse = service.Create(
                obj,
                mustUseAdvisory,
                zoneId?[0],
                contextId?[0],
                requestParameters);
            createResponseType createResponse =
                MapperFactory.CreateInstance<MultipleCreateResponse, createResponseType>(multipleCreateResponse);

            return Ok(createResponse);
        }

        /// <inheritdoc cref="IProvider{TSingle,TMultiple,TPrimaryKey}.Put(TMultiple, string[], string[])" />
        public override IHttpActionResult Put(
            TMultiple obj,
            [MatrixParameter] string[] zoneId = null,
            [MatrixParameter] string[] contextId = null)
        {
            if (!AuthenticationService.VerifyAuthenticationHeader(Request.Headers, out string sessionToken))
            {
                return Unauthorized();
            }

            // Check ACLs and return StatusCode(HttpStatusCode.Forbidden) if appropriate.
            if (!AuthorisationService.IsAuthorised(Request.Headers, sessionToken, $"{TypeName}s", RightType.UPDATE))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if ((zoneId != null && zoneId.Length != 1) || (contextId != null && contextId.Length != 1))
            {
                return BadRequest($"Request failed for object {TypeName} as Zone and/or Context are invalid.");
            }

            RequestParameter[] requestParameters = GetQueryParameters(Request);
            MultipleUpdateResponse multipleUpdateResponse = service.Update(
                obj,
                zoneId?[0],
                contextId?[0],
                requestParameters);
            updateResponseType updateResponse =
                MapperFactory.CreateInstance<MultipleUpdateResponse, updateResponseType>(multipleUpdateResponse);

            return Ok(updateResponse);
        }

        /// <inheritdoc cref="Provider{TSingle, TMultiple}.SerialiseEvents(TMultiple)" />
        [NonAction]
        public override string SerialiseEvents(TMultiple obj)
        {
            return SerialiserFactory.GetSerialiser<TMultiple>(ContentType).Serialise(obj);
        }
    }
}