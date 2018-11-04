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
    public class FQReportingObjectService : IBasicProviderService<FQReportingObject>
    {
        private static readonly Random random = new Random();

        private static IDictionary<string, FQReportingObject> cache = new Dictionary<string, FQReportingObject>();

        private static FQReportingObject CreateObject()
        {
            FQReportingObject obj = new FQReportingObject
            {
                RefId = Guid.NewGuid().ToString(),
                FQYear = random.Next(2016, 2020).ToString(),
                ReportingAuthorityCommonwealthId = $"0{random.Next(12000, 12999)}",
                LocalId = $"0{random.Next(1010000, 1019999)}",
                StateProvinceId = random.Next(45640000, 45649999).ToString(),
                CommonwealthId = random.Next(12000, 12999).ToString(),
                ACARAId = random.Next(99000, 99999).ToString(),
                EntityName = $"John {random.Next(11, 39)} College"
            };

            return obj;
        }

        private static IDictionary<string, FQReportingObject> CreateObjects(int count)
        {
            IDictionary<string, FQReportingObject> objs = new Dictionary<string, FQReportingObject>();

            for (int i = 1; i <= count; i++)
            {
                FQReportingObject obj = CreateObject();
                objs.Add(obj.RefId, obj);
            }

            return objs;
        }

        static FQReportingObjectService()
        {
            cache = CreateObjects(5);
        }

        public FQReportingObject Create(
            FQReportingObject obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!mustUseAdvisory.HasValue || !mustUseAdvisory.Value)
            {
                string refId = Guid.NewGuid().ToString();
                obj.RefId = refId;
            }

            cache.Add(obj.RefId, obj);

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

        public FQReportingObject Retrieve(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!cache.TryGetValue(refId, out FQReportingObject obj))
            {
                obj = null;
            }

            return obj;
        }

        public List<FQReportingObject> Retrieve(
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            List<FQReportingObject> objs = new List<FQReportingObject>();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                uint index = pageIndex.Value * pageSize.Value;
                uint count = pageSize.Value;

                if (cache.Values.Count < (index + count))
                {
                    count = (uint)cache.Values.Count - index;
                }

                if (index <= cache.Values.Count)
                {
                    objs.AddRange(cache.Values);
                    objs = objs.GetRange((int)index, (int)count);
                }
            }
            else
            {
                objs.AddRange(cache.Values);
            }

            return objs;
        }

        public List<FQReportingObject> Retrieve(
            FQReportingObject obj,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public List<FQReportingObject> Retrieve(
            IEnumerable<EqualCondition> conditions,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public void Update(
            FQReportingObject obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }
    }
}