/*
 * Crown Copyright © Department for Education (UK) 2016
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

using Sif.Framework.Demo.Uk.Provider.Models;
using Sif.Framework.Demo.Uk.Provider.Services;
using Sif.Framework.Providers;
using Sif.Framework.WebApi.ModelBinders;
using System.Web.Http;

namespace Sif.Framework.Demo.Uk.Provider.Controllers
{

    public class LearnerPersonalsProvider : BasicProvider<LearnerPersonal>
    {

        public LearnerPersonalsProvider()
            : base(new LearnerPersonalService())
        {
        }

        [Route("~/api/LearnerPersonals/LearnerPersonal")]
        public override IHttpActionResult Post(LearnerPersonal obj, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            return base.Post(obj);
        }

        [NonAction]
        public override IHttpActionResult BroadcastEvents(string zoneId = null, string contextId = null)
        {
            return base.BroadcastEvents(zoneId, contextId);
        }

    }

}