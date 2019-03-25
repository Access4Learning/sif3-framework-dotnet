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
using System.Linq;

namespace Sif.Framework.Demo.Broker.Services
{
    public class SubscriptionService : IBasicProviderService<Subscription>
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static IDictionary<string, Subscription> subscriptionsCache = new Dictionary<string, Subscription>();

        public Subscription Create(
            Subscription obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (obj == null)
            {
                obj = new Subscription();
            }

            obj.id = Guid.NewGuid().ToString();
            obj.zoneId = zoneId;
            obj.contextId = contextId;
            subscriptionsCache.Add(obj.id, obj);
            if (log.IsDebugEnabled) log.Debug($"*** Created Subscription with ID of {obj.id}.");

            return obj;
        }

        public void Delete(
            string id,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (id != null && subscriptionsCache.ContainsKey(id))
            {
                subscriptionsCache.Remove(id);
            }
        }

        public Subscription Retrieve(
            string id,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            Subscription queue = null;

            if (id != null && subscriptionsCache.ContainsKey(id))
            {
                subscriptionsCache.TryGetValue(id, out queue);
            }

            return queue;
        }

        public List<Subscription> Retrieve(
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            return subscriptionsCache.Values.ToList();
        }

        public List<Subscription> Retrieve(
            IEnumerable<EqualCondition> conditions,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public List<Subscription> Retrieve(
            Subscription obj,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public void Update(
            Subscription obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }
    }
}