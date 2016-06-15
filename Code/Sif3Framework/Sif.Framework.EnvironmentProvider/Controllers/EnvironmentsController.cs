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

using Sif.Specification.Infrastructure;
using System.Net.Http;
using System.Web.Http;
using Sif.Framework.WebApi.ModelBinders;
using System;

namespace Sif.Framework.EnvironmentProvider.Controllers
{

    /// <summary>
    /// Valid single operations: POST, GET, DELETE.
    /// Valid multiple operations: none.
    /// </summary>
    [RoutePrefix("api/environments")]
    public class EnvironmentsController : Sif.Framework.Controllers.EnvironmentsController
    {
        [Route("environment")]
        [HttpPost]
        public override HttpResponseMessage Create
            (environmentType item,
             string authenticationMethod = null,
             string consumerName = null,
             string solutionId = null,
             string dataModelNamespace = null,
             string supportedInfrastructureVersion = null,
             string transport = null,
             string productName = null)
        {
            return base.Create(item, authenticationMethod, consumerName, solutionId, dataModelNamespace, supportedInfrastructureVersion, transport, productName);
        }

        [Route("{id}")]
        [HttpDelete]
        public override void Delete(Guid id, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            base.Delete(id, zone, context);
        }

    }

}
