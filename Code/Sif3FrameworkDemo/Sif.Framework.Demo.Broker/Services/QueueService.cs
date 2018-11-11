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
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sif.Framework.Demo.Broker.Services
{
    public class QueueService : IBasicProviderService<Queue>
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static IDictionary<string, Queue> queuesCache = new Dictionary<string, Queue>();

        public Queue Create(
            Queue obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (obj == null)
            {
                obj = new Queue();
            }

            obj.id = Guid.NewGuid().ToString();
            obj.polling = queueTypePolling.IMMEDIATE;
            obj.pollingSpecified = true;
            obj.queueUri = $@"http://localhost:59586/api/queues/{obj.id}/messages";
            obj.idleTimeout = 0;
            obj.idleTimeoutSpecified = true;
            obj.created = DateTime.UtcNow;
            obj.createdSpecified = true;
            obj.lastAccessed = obj.created;
            obj.lastAccessedSpecified = true;
            obj.lastModified = obj.created;
            obj.lastModifiedSpecified = true;
            obj.messageCount = (uint)queuesCache.Count;
            queuesCache.Add(obj.id, obj);
            if (log.IsDebugEnabled) log.Debug($"*** Created Queue with ID of {obj.id}.");

            return obj;
        }

        public void Delete(
            string id,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (id != null && queuesCache.ContainsKey(id))
            {
                queuesCache.Remove(id);
            }
        }

        public Queue Retrieve(
            string id,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            Queue queue = null;

            if (id != null && queuesCache.ContainsKey(id))
            {
                queuesCache.TryGetValue(id, out queue);
            }

            return queue;
        }

        public List<Queue> Retrieve(
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            return queuesCache.Values.ToList();
        }

        public List<Queue> Retrieve(
            IEnumerable<EqualCondition> conditions,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public List<Queue> Retrieve(
            Queue obj,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public void Update(
            Queue obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }
    }
}