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
using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Query;
using Sif.Framework.Service.Providers;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Au.Provider.Services
{
    public class SchoolInfoService : IBasicProviderService<SchoolInfo>
    {
        public SchoolInfo Create(
            SchoolInfo obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public SchoolInfo Retrieve(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public List<SchoolInfo> Retrieve(
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            SchoolInfo school = new SchoolInfo { SchoolName = "Applecross SHS" };
            List<SchoolInfo> schools = new List<SchoolInfo> { school };

            return schools;
        }

        public List<SchoolInfo> Retrieve(
            IEnumerable<EqualCondition> conditions,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            SchoolInfo school = new SchoolInfo { SchoolName = "Rossmoyne Conditional SHS" };
            List<SchoolInfo> schools = new List<SchoolInfo> { school };

            return schools;
        }

        public List<SchoolInfo> Retrieve(
            SchoolInfo obj,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public void Update(
            SchoolInfo obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public void Delete(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }
    }
}