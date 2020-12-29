/*
 * Copyright 2020 Systemic Pty Ltd
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
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Responses;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Providers;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using Sif.Framework.WebApi.ModelBinders;
using Sif.Specification.Infrastructure;
using System.Net;
using System.Web.Http;

namespace Sif.Framework.Providers
{
    public abstract class ObjectProvider<TSingle, TMultiple>
        : Provider<TSingle, TMultiple> where TSingle : ISifRefId<string>
    {
        private readonly IObjectProviderService<TSingle, TMultiple> service;

        /// <summary>
        /// Create an instance based on the specified service.
        /// </summary>
        /// <param name="service">Service used for managing the object type.</param>
        /// <param name="settings">Provider settings. If null, Provider settings will be read from the SifFramework.config file.</param>
        protected ObjectProvider(IObjectProviderService<TSingle, TMultiple> service, IFrameworkSettings settings = null)
            : base(service, settings)
        {
            this.service = service;
        }

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Post(TMultiple, string[], string[])">Post</see>
        /// </summary>
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

            bool? mustUseAdvisory = HttpUtils.GetMustUseAdvisory(Request.Headers);
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

        /// <summary>
        /// <see cref="IProvider{TTSingle,TMultiple,TPrimaryKey}.Put(TMultiple, string[], string[])">Put</see>
        /// </summary>
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

        /// <summary>
        /// <see cref="Provider{TSingle, TMultiple}.SerialiseEvents(TMultiple)">SerialiseEvents</see>
        /// </summary>
        [NonAction]
        public override string SerialiseEvents(TMultiple obj)
        {
            return SerialiserFactory.GetSerialiser<TMultiple>(ContentType).Serialise(obj);
        }
    }
}