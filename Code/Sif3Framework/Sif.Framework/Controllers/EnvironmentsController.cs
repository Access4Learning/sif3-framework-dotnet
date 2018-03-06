﻿/*
 * Copyright 2017 Systemic Pty Ltd
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

using Sif.Framework.Model.Exceptions;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Utils;
using Sif.Framework.WebApi.ModelBinders;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Controllers
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
        /// Default value for the authentication method.
        /// </summary>
        protected static readonly string defaultAuthenticationMethod = "Basic";

        /// <summary>
        /// Default value for the Consumer name.
        /// </summary>
        protected static readonly string defaultConsumerName = "Sif3FrameworkConsumer";

        /// <summary>
        /// Default value for the supported infrastructure version.
        /// </summary>
        protected static readonly string defaultSupportedInfrastructureVersion = "3.0.1";

        /// <summary>
        /// Create an Environment, using default values where applicable.
        /// </summary>
        /// <param name="applicationKey">Application key.</param>
        /// <param name="authenticationMethod">Authentication method.</param>
        /// <param name="consumerName">Consumer name.</param>
        /// <param name="solutionId">Solution ID.</param>
        /// <param name="dataModelNamespace">Data model namespace.</param>
        /// <param name="supportedInfrastructureVersion">Supported infrastructure version.</param>
        /// <param name="transport">Transport.</param>
        /// <param name="productName">Product name.</param>
        /// <returns>An Environment.</returns>
        private environmentType CreateDefaultEnvironmentType
            (string applicationKey,
             string authenticationMethod = null,
             string consumerName = null,
             string solutionId = null,
             string dataModelNamespace = null,
             string supportedInfrastructureVersion = null,
             string transport = null,
             string productName = null)
        {
            applicationInfoType applicationInfo = new applicationInfoType();
            applicationInfo.applicationKey = applicationKey;

            if (!string.IsNullOrWhiteSpace(dataModelNamespace))
            {
                applicationInfo.dataModelNamespace = dataModelNamespace;
            }

            if (string.IsNullOrWhiteSpace(supportedInfrastructureVersion))
            {
                applicationInfo.supportedInfrastructureVersion = defaultSupportedInfrastructureVersion;
            }
            else
            {
                applicationInfo.supportedInfrastructureVersion = supportedInfrastructureVersion;
            }

            if (!string.IsNullOrWhiteSpace(transport))
            {
                applicationInfo.transport = transport;
            }

            if (!string.IsNullOrWhiteSpace(productName))
            {
                productIdentityType productIdentity = new productIdentityType();
                productIdentity.productName = productName;
                applicationInfo.applicationProduct = productIdentity;
            }

            environmentType environmentType = new environmentType();
            environmentType.applicationInfo = applicationInfo;

            if (string.IsNullOrWhiteSpace(authenticationMethod))
            {
                environmentType.authenticationMethod = defaultAuthenticationMethod;
            }
            else
            {
                environmentType.authenticationMethod = authenticationMethod;
            }

            if (string.IsNullOrWhiteSpace(consumerName))
            {
                environmentType.consumerName = defaultConsumerName;
            }
            else
            {
                environmentType.consumerName = consumerName;
            }

            if (!string.IsNullOrWhiteSpace(solutionId))
            {
                environmentType.solutionId = solutionId;
            }

            return environmentType;
        }

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
        /// <param name="zoneId">The zone in which to perform the request.</param>
        /// <param name="contextId">The context in which to perform the request.</param>
        /// <returns>HTTP status 403.</returns>
        public override ICollection<environmentType> Get([MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// GET /api/environments/{id}
        /// </summary>
        /// <param name="id">Identifier for the environment to retrieve.</param>
        /// <param name="zoneId">The zone in which to perform the request.</param>
        /// <param name="contextId">The context in which to perform the request.</param>
        /// <returns>Environment.</returns>
        [Route("{id}")]
        [HttpGet]
        public override environmentType Get(Guid id, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            return base.Get(id, zoneId, contextId);
        }

        /// <summary>
        /// POST api/environments
        /// This operation is forbidden.
        /// </summary>
        /// <param name="item">Object to create.</param>
        /// <param name="zoneId">The zone in which to perform the request.</param>
        /// <param name="contextId">The context in which to perform the request.</param>
        /// <returns>HTTP status 403.</returns>
        public override HttpResponseMessage Post(environmentType item, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// POST api/environments/environment
        /// </summary>
        /// <param name="item">Object to create.</param>
        /// <param name="authenticationMethod">Authentication method.</param>
        /// <param name="consumerName">Consumer name.</param>
        /// <param name="solutionId">Solution ID.</param>
        /// <param name="dataModelNamespace">Data model namespace.</param>
        /// <param name="supportedInfrastructureVersion">Supported infrastructure version.</param>
        /// <param name="transport">Transport.</param>
        /// <param name="productName">Product name.</param>
        /// <returns>HTTP response message indicating success or failure.</returns>
        [HttpPost]
        [Route("api/environments/environment")]
        public virtual HttpResponseMessage Create
            (environmentType item,
             string authenticationMethod = null,
             string consumerName = null,
             string solutionId = null,
             string dataModelNamespace = null,
             string supportedInfrastructureVersion = null,
             string transport = null,
             string productName = null)
        {
            HttpResponseMessage responseMessage = null;
            string initialToken;

            if (!authService.VerifyInitialAuthenticationHeader(Request.Headers, out initialToken))
            {
                string errorMessage = "The POST request failed for Environment creation due to invalid authentication credentials.";
                responseMessage = HttpUtils.CreateErrorResponse(Request, HttpStatusCode.Unauthorized, errorMessage);
            }
            else
            {

                if (item == null)
                {
                    item = CreateDefaultEnvironmentType(initialToken, authenticationMethod, consumerName, solutionId, dataModelNamespace, supportedInfrastructureVersion, transport, productName);
                }

                try
                {
                    Guid id = service.Create(item);
                    environmentType newItem = service.Retrieve(id);
                    responseMessage = Request.CreateResponse(HttpStatusCode.Created, newItem);
                    //string uri = Url.Link("DefaultApi", new { id = id });
                    //responseMessage.Headers.Location = new Uri(uri);
                }
                catch (AlreadyExistsException e)
                {
                    responseMessage = HttpUtils.CreateErrorResponse(Request, HttpStatusCode.Conflict, e);
                }

            }

            return responseMessage;
        }

        /// <summary>
        /// PUT api/environments/{id}
        /// This operation is forbidden (raises HTTP status 403).
        /// </summary>
        /// <param name="id">Identifier for the environment to update.</param>
        /// <param name="item">Object to update.</param>
        /// <param name="zoneId">The zone in which to perform the request.</param>
        /// <param name="contextId">The context in which to perform the request.</param>
        /// <exception cref="HttpResponseException">Exception representing HTTP status 403 Forbidden.</exception>
        public override void Put(Guid id, environmentType item, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// DELETE api/environments{id}
        /// </summary>
        /// <param name="id">Identifier for the environment to delete.</param>
        /// <param name="zoneId">The zone in which to perform the request.</param>
        /// <param name="contextId">The context in which to perform the request.</param>
        [Route("{id}")]
        [HttpDelete]
        public override void Delete(Guid id, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            base.Delete(id, zoneId, contextId);
        }

    }

}
