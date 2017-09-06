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

using Sif.Framework.Demo.EventsConnector.Models;
using Sif.Framework.Demo.EventsConnector.Services;
using Sif.Framework.Providers;
using Sif.Framework.WebApi.ModelBinders;
using System.Web.Http;
using System.Collections.Generic;

namespace Sif.Framework.Demo.EventsConnector.Controllers
{

    public class StudentPersonalsProvider : BasicProvider<StudentPersonal>
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public StudentPersonalsProvider()
            : base(new StudentPersonalService())
        {
        }

        [Route("~/api/StudentPersonals/StudentPersonal")]
        public override IHttpActionResult Post(StudentPersonal obj, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            return base.Post(obj, zoneId, contextId);
        }

        public override IHttpActionResult Post(List<StudentPersonal> objs, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {

            foreach (KeyValuePair<string, IEnumerable<string>> nameValues in Request.Headers)
            {
                if (log.IsDebugEnabled) log.Debug($"*** Header field is [{nameValues.Key}:{string.Join(",", nameValues.Value)}]");
            }

            return base.Post(objs, zoneId, contextId);
        }

    }

}