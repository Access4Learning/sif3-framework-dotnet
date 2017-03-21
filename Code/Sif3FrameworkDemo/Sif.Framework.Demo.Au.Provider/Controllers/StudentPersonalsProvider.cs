﻿/*
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

using Sif.Framework.Demo.Au.Provider.Models;
using Sif.Framework.Demo.Au.Provider.Services;
using Sif.Framework.Providers;
using Sif.Framework.WebApi.ModelBinders;
using System.Web.Http;

namespace Sif.Framework.Demo.Au.Provider.Controllers
{

    public class StudentPersonalsProvider : BasicProvider<StudentPersonal>
    {

        public StudentPersonalsProvider()
            : base(new StudentPersonalService())
        {
        }

        [Route("~/api/StudentPersonals/StudentPersonal")]
        public override IHttpActionResult Post(StudentPersonal obj, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            return base.Post(obj);
        }

    }

}