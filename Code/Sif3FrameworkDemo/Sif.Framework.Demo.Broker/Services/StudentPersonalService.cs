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

using Sif.Framework.Demo.Broker.Models;
using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Query;
using Sif.Framework.Service.Providers;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Broker.Services
{
    public class StudentPersonalService : IBasicProviderService<StudentPersonal>
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public StudentPersonal Create(
            StudentPersonal obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (log.IsDebugEnabled) log.Debug($"*** Student name is {obj.PersonInfo.Name.GivenName} {obj.PersonInfo.Name.FamilyName}");

            return obj;
        }

        public void Delete(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public StudentPersonal Retrieve(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public List<StudentPersonal> Retrieve(
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public List<StudentPersonal> Retrieve(
            IEnumerable<EqualCondition> conditions,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public List<StudentPersonal> Retrieve(
            StudentPersonal obj,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public void Update(
            StudentPersonal obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }
    }
}