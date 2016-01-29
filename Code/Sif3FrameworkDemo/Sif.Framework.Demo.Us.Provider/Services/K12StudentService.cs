/*
 * Copyright 2015 Systemic Pty Ltd
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

    public class K12StudentService : IBasicProviderService<K12Student>
    {
        private static IDictionary<string, K12Student> studentsCache = new Dictionary<string, K12Student>();
        private static Random random = new Random();

        private static K12Student CreateStudent()
        {
            k12StudentTypeIdentityIdentification identification = new k12StudentTypeIdentityIdentification { studentId = random.Next(10000, 99999).ToString() };
            k12StudentTypeIdentityName name = new k12StudentTypeIdentityName { firstName = RandomNameGenerator.GivenName, lastName = RandomNameGenerator.FamilyName };
            k12StudentTypeIdentity identity = new k12StudentTypeIdentity { identification = identification, name = name };
            K12Student k12Student = new K12Student { refId = Guid.NewGuid().ToString(), identity = identity };

            return k12Student;
        }

        private static IDictionary<string, K12Student> CreateStudents(int count)
        {
            IDictionary<string, K12Student> k12StudentsCache = new Dictionary<string, K12Student>();

            for (int i = 1; i <= count; i++)
            {
                K12Student k12Student = CreateStudent();
                k12StudentsCache.Add(k12Student.refId, k12Student);
            }

            return k12StudentsCache;
        }

        static K12StudentService()
        {
            studentsCache = CreateStudents(10);
        }

        public K12Student Create(K12Student obj, bool? mustUseAdvisory = null, string zone = null, string context = null)
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

        public K12Student Retrieve(string refId, string zone = null, string context = null)
        {
            K12Student student;

            if (!studentsCache.TryGetValue(refId, out student))
            {
                student = null;
            }

            return student;
        }

        public List<K12Student> Retrieve(uint? pageIndex = null, uint? pageSize = null, string zone = null, string context = null)
        {
            List<K12Student> allStudents = new List<K12Student>();
            allStudents.AddRange(studentsCache.Values);
            List<K12Student> retrievedStudents = new List<K12Student>();

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

        public List<K12Student> Retrieve(K12Student obj, uint? pageIndex = null, uint? pageSize = null, string zone = null, string context = null)
        {
            throw new NotImplementedException();
        }

        public List<K12Student> Retrieve(IEnumerable<EqualCondition> conditions, uint? pageIndex = null, uint? pageSize = null, string zone = null, string context = null)
        {
            throw new NotImplementedException();
        }

        public void Update(K12Student obj, string zone = null, string context = null)
        {

            if (studentsCache.ContainsKey(obj.RefId))
            {
                studentsCache.Remove(obj.RefId);
                studentsCache.Add(obj.RefId, obj);
            }

        }

    }

}