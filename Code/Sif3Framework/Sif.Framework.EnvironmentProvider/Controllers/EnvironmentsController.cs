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

using Sif.Framework.Infrastructure;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Service;
using Sif.Framework.Service.Infrastructure;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Sif.Framework.EnvironmentProvider.Controllers
{

    /// <summary>
    /// Valid single operations: POST, GET, DELETE.
    /// Valid multiple operations: none.
    /// </summary>
    public class EnvironmentsController : GenericController<environmentType, Environment>
    {

        public override IGenericService<environmentType, Environment> GetService()
        {
            return new EnvironmentService();
        }

        public EnvironmentsController() : base()
        {
        }

        // GET api/{controller}
        public override IEnumerable<environmentType> Get()
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        // PUT api/{controller}/{id}
        public override void Put(int id, environmentType item)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

    }

}
