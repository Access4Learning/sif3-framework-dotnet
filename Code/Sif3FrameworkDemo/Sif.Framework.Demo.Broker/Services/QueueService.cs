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

using Sif.Framework.Demo.Broker.Models;
using Sif.Framework.Model.Query;
using Sif.Framework.Service.Providers;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Broker.Services
{

    public class QueueService : IBasicProviderService<Queue>
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Queue Create(Queue obj, bool? mustUseAdvisory = null, string zoneId = null, string contextId = null)
        {
            if (log.IsDebugEnabled) log.Debug($"*** Queue name is {obj.name}");
            return obj;
        }

        public void Delete(string id, string zoneId = null, string contextId = null)
        {
            throw new NotImplementedException();
        }

        public Queue Retrieve(string id, string zoneId = null, string contextId = null)
        {
            throw new NotImplementedException();
        }

        public List<Queue> Retrieve(uint? pageIndex = null, uint? pageSize = null, string zoneId = null, string contextId = null)
        {
            throw new NotImplementedException();
        }

        public List<Queue> Retrieve(IEnumerable<EqualCondition> conditions, uint? pageIndex = null, uint? pageSize = null, string zoneId = null, string contextId = null)
        {
            throw new NotImplementedException();
        }

        public List<Queue> Retrieve(Queue obj, uint? pageIndex = null, uint? pageSize = null, string zoneId = null, string contextId = null)
        {
            throw new NotImplementedException();
        }

        public void Update(Queue obj, string zoneId = null, string contextId = null)
        {
            throw new NotImplementedException();
        }

    }

}