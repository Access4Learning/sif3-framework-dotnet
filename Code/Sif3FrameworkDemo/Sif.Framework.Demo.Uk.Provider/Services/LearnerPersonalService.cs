/*
 * Crown Copyright © Department for Education (UK) 2016
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

using Sif.Framework.Demo.Uk.Provider.Models;
using Sif.Framework.Demo.Uk.Provider.Utils;
using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Query;
using Sif.Framework.Service.Providers;
using Sif.Specification.DataModel.Uk;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Uk.Provider.Services
{
    public class LearnerPersonalService : IBasicProviderService<LearnerPersonal>
    {
        private static IDictionary<string, LearnerPersonal> learnerCache = new Dictionary<string, LearnerPersonal>();
        private static Random random = new Random();

        private static LearnerPersonal CreateBartSimpson()
        {
            return new LearnerPersonal
            {
                RefId = Guid.NewGuid().ToString(),
                LocalId = "666",
                PersonalInformation = new PersonalInformationType
                {
                    Name = new NameType
                    {
                        Type = NameTypeType.C,
                        FamilyName = "Simpson",
                        GivenName = "Bart"
                    }
                }
            };
        }

        private static LearnerPersonal CreateLearner()
        {
            return new LearnerPersonal
            {
                RefId = Guid.NewGuid().ToString(),
                LocalId = random.Next(10000, 99999).ToString(),
                PersonalInformation = new PersonalInformationType
                {
                    Name = new NameType
                    {
                        Type = NameTypeType.C,
                        FamilyName = RandomNameGenerator.FamilyName,
                        GivenName = RandomNameGenerator.GivenName
                    }
                }
            };
        }

        private static IDictionary<string, LearnerPersonal> CreateLearner(int count)
        {
            IDictionary<string, LearnerPersonal> LearnerPersonalsCache = new Dictionary<string, LearnerPersonal>();

            if (count > 0)
            {
                LearnerPersonal bartSimpson = CreateBartSimpson();
                LearnerPersonalsCache.Add(bartSimpson.RefId, bartSimpson);

                for (int i = 2; i <= count; i++)
                {
                    LearnerPersonal LearnerPersonal = CreateLearner();
                    LearnerPersonalsCache.Add(LearnerPersonal.RefId, LearnerPersonal);
                }
            }

            return LearnerPersonalsCache;
        }

        static LearnerPersonalService()
        {
            learnerCache = CreateLearner(20);
        }

        public LearnerPersonal Create(
            LearnerPersonal obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            string refId = Guid.NewGuid().ToString();
            obj.RefId = refId;
            learnerCache.Add(refId, obj);

            return obj;
        }

        public LearnerPersonal Retrieve(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            LearnerPersonal student;

            if (!learnerCache.TryGetValue(refId, out student))
            {
                student = null;
            }

            return student;
        }

        public List<LearnerPersonal> Retrieve(
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            List<LearnerPersonal> retrievedStudents = new List<LearnerPersonal>();

            if ("Gov".Equals(zoneId) && "Curr".Equals(contextId))
            {
                List<LearnerPersonal> allStudents = new List<LearnerPersonal>();
                allStudents.AddRange(learnerCache.Values);

                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    uint index = pageIndex.Value * pageSize.Value;
                    uint count = pageSize.Value;

                    if (learnerCache.Values.Count < (index + count))
                    {
                        count = (uint)learnerCache.Values.Count - index;
                    }
                    else
                    {
                        count = pageSize.Value;
                    }

                    if (index <= learnerCache.Values.Count)
                    {
                        retrievedStudents = allStudents.GetRange((int)index, (int)count);
                    }
                }
                else
                {
                    retrievedStudents = allStudents;
                }
            }

            return retrievedStudents;
        }

        public List<LearnerPersonal> Retrieve(
            LearnerPersonal obj,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            List<LearnerPersonal> students = new List<LearnerPersonal>();

            foreach (LearnerPersonal student in learnerCache.Values)
            {
                if (student.PersonalInformation.Name.FamilyName.Equals(obj.PersonalInformation.Name.FamilyName) && student.PersonalInformation.Name.GivenName.Equals(obj.PersonalInformation.Name.GivenName))
                {
                    students.Add(student);
                }
            }

            return students;
        }

        public List<LearnerPersonal> Retrieve(
            IEnumerable<EqualCondition> conditions,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            List<LearnerPersonal> students = new List<LearnerPersonal>();
            students.Add(CreateBartSimpson());

            return students;
        }

        public void Update(
            LearnerPersonal obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (learnerCache.ContainsKey(obj.RefId))
            {
                learnerCache.Remove(obj.RefId);
                learnerCache.Add(obj.RefId, obj);
            }
        }

        public void Delete(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            learnerCache.Remove(refId);
        }
    }
}