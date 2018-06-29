/*
 * Copyright 2018 Systemic Pty Ltd
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

using Sif.Framework.Demo.Au.Provider.Models;
using Sif.Framework.Demo.Au.Provider.Services;
using Sif.Framework.Providers;
using System.Web.Http;

namespace Sif.Framework.Demo.Au.Provider.Controllers
{
    public class StudentSchoolEnrollmentsProvider : BasicProvider<StudentSchoolEnrollment>
    {
        public StudentSchoolEnrollmentsProvider() : base(new StudentSchoolEnrollmentService())
        {
        }

        [NonAction]
        public override IHttpActionResult BroadcastEvents(string zoneId = null, string contextId = null)
        {
            return base.BroadcastEvents(zoneId, contextId);
        }
    }
}