/*
 * Copyright 2019 Systemic Pty Ltd
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
    public class FinancialQuestionnaireSubmissionService : IBasicProviderService<FinancialQuestionnaireSubmission>
    {
        private static readonly Random random = new Random();

        private static IDictionary<string, FinancialQuestionnaireSubmission> cache = new Dictionary<string, FinancialQuestionnaireSubmission>();

        private static FinancialQuestionnaireSubmission CreateObject()
        {
            FinancialQuestionnaireSubmission obj = new FinancialQuestionnaireSubmission
            {
                RefId = Guid.NewGuid().ToString(),
                FQYear = random.Next(2016, 2020).ToString(),
                ReportingAuthority = "Ballarat Diocese",
                ReportingAuthorityCommonwealthId = $"0{random.Next(12000, 12999)}"
            };

            return obj;
        }

        private static IDictionary<string, FinancialQuestionnaireSubmission> CreateObjects(int count)
        {
            IDictionary<string, FinancialQuestionnaireSubmission> objs = new Dictionary<string, FinancialQuestionnaireSubmission>();

            for (int i = 1; i <= count; i++)
            {
                FinancialQuestionnaireSubmission obj = CreateObject();
                objs.Add(obj.RefId, obj);
            }

            return objs;
        }

        static FinancialQuestionnaireSubmissionService()
        {
            cache = CreateObjects(5);
        }

        public FinancialQuestionnaireSubmission Create(
            FinancialQuestionnaireSubmission obj,
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

        public FinancialQuestionnaireSubmission Retrieve(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!cache.TryGetValue(refId, out FinancialQuestionnaireSubmission obj))
            {
                obj = null;
            }

            return obj;
        }

        public List<FinancialQuestionnaireSubmission> Retrieve(
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            List<FinancialQuestionnaireSubmission> objs = new List<FinancialQuestionnaireSubmission>();

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

        public List<FinancialQuestionnaireSubmission> Retrieve(
            FinancialQuestionnaireSubmission obj,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public List<FinancialQuestionnaireSubmission> Retrieve(
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
            FinancialQuestionnaireSubmission obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }
    }
}