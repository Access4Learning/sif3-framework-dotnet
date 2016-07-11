/*
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

using Sif.Framework.Demo.Us.Provider.Models;
using Sif.Framework.Demo.Us.Provider.Utils;
using Sif.Framework.Model.Query;
using Sif.Framework.Service.Providers;
using Sif.Specification.DataModel.Us;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Us.Provider.Services
{

    public class XStudentService : IBasicProviderService<XStudent>
    {
        private static IDictionary<string, XStudent> studentsCache = new Dictionary<string, XStudent>();
        private static Random random = new Random();

        private static XStudent CreateStudent()
        {
            xPersonNameType name = new xPersonNameType { familyName = RandomNameGenerator.GivenName, givenName = RandomNameGenerator.FamilyName };
            XStudent student = new XStudent { refId = Guid.NewGuid().ToString(), localId = random.Next(10000, 99999).ToString(), name = name };

            return student;
        }

        private static IDictionary<string, XStudent> CreateStudents(int count)
        {
            IDictionary<string, XStudent> studentsCache = new Dictionary<string, XStudent>();

            for (int i = 1; i <= count; i++)
            {
                XStudent student = CreateStudent();
                studentsCache.Add(student.refId, student);
            }

            return studentsCache;
        }

        static XStudentService()
        {
            studentsCache = CreateStudents(10);
        }

        public XStudent Create(XStudent obj, bool? mustUseAdvisory = null, string zone = null, string context = null)
        {
            string refId = Guid.NewGuid().ToString();
            obj.RefId = refId;
            studentsCache.Add(refId, obj);

            return obj;
        }

        public void Delete(string refId, string zone = null, string context = null)
        {
            studentsCache.Remove(refId);
        }

        public XStudent Retrieve(string refId, string zone = null, string context = null)
        {
            XStudent student;

            if (!studentsCache.TryGetValue(refId, out student))
            {
                student = null;
            }

            return student;
        }

        public List<XStudent> Retrieve(uint? pageIndex = null, uint? pageSize = null, string zone = null, string context = null)
        {
            List<XStudent> allStudents = new List<XStudent>();
            allStudents.AddRange(studentsCache.Values);
            List<XStudent> retrievedStudents = new List<XStudent>();

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

        public List<XStudent> Retrieve(XStudent obj, uint? pageIndex = null, uint? pageSize = null, string zone = null, string context = null)
        {
            throw new NotImplementedException();
        }

        public List<XStudent> Retrieve(IEnumerable<EqualCondition> conditions, uint? pageIndex = null, uint? pageSize = null, string zone = null, string context = null)
        {
            throw new NotImplementedException();
        }

        public void Update(XStudent obj, string zone = null, string context = null)
        {

            if (studentsCache.ContainsKey(obj.RefId))
            {
                studentsCache.Remove(obj.RefId);
                studentsCache.Add(obj.RefId, obj);
            }

        }
    }
}