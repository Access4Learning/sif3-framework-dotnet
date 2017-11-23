/*
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

using Sif.Specification.Infrastructure;
using System.Net.Http;
using System.Web.Http;

namespace Sif.Framework.Demo.Broker.Controllers
{

    /// <summary>
    /// Valid single operations: POST, GET, DELETE.
    /// Valid multiple operations: none.
    /// </summary>
    public class EnvironmentsProvider : Framework.Controllers.EnvironmentsController
    {

        // POST api/{controller}
        [HttpPost]
        [Route("api/environments/environment")]
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

    }

}
