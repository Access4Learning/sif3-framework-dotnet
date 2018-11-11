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

using Sif.Framework.Demo.Us.Provider.Models;
using Sif.Framework.Demo.Us.Provider.Utils;
using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Query;
using Sif.Framework.Service.Providers;
using Sif.Specification.DataModel.Us;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Us.Provider.Services
{
    public class XStudentService : IBasicProviderService<xStudent>
    {
        private static IDictionary<string, xStudent> studentsCache = new Dictionary<string, xStudent>();
        private static Random random = new Random();

        private static xStudent CreateStudent()
        {
            xPersonNameType name = new xPersonNameType { familyName = RandomNameGenerator.GivenName, givenName = RandomNameGenerator.FamilyName };
            xStudent student = new xStudent { refId = Guid.NewGuid().ToString(), localId = random.Next(10000, 99999).ToString(), name = name };

            return student;
        }

        private static IDictionary<string, xStudent> CreateStudents(int count)
        {
            IDictionary<string, xStudent> studentsCache = new Dictionary<string, xStudent>();

            for (int i = 1; i <= count; i++)
            {
                xStudent student = CreateStudent();
                studentsCache.Add(student.refId, student);
            }

            return studentsCache;
        }

        static XStudentService()
        {
            studentsCache = CreateStudents(10);
        }

        public xStudent Create(
            xStudent obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            string refId = Guid.NewGuid().ToString();
            obj.RefId = refId;
            studentsCache.Add(refId, obj);

            return obj;
        }

        public void Delete(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            studentsCache.Remove(refId);
        }

        public xStudent Retrieve(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            xStudent student;

            if (!studentsCache.TryGetValue(refId, out student))
            {
                student = null;
            }

            return student;
        }

        public List<xStudent> Retrieve(
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            List<xStudent> allStudents = new List<xStudent>();
            allStudents.AddRange(studentsCache.Values);
            List<xStudent> retrievedStudents = new List<xStudent>();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                uint index = pageIndex.Value * pageSize.Value;
                uint count = pageSize.Value;

                if (studentsCache.Values.Count < (index + count))
                {
                    count = (uint)studentsCache.Values.Count - index;
                }
                else
                {
                    count = pageSize.Value;
                }

                if (index <= studentsCache.Values.Count)
                {
                    retrievedStudents = allStudents.GetRange((int)index, (int)count);
                }
            }
            else
            {
                retrievedStudents = allStudents;
            }

            return retrievedStudents;
        }

        public List<xStudent> Retrieve(
            xStudent obj,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public List<xStudent> Retrieve(
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
            xStudent obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (studentsCache.ContainsKey(obj.RefId))
            {
                studentsCache.Remove(obj.RefId);
                studentsCache.Add(obj.RefId, obj);
            }
        }
    }
}