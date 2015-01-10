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
using Sif.Framework.Service;
using Sif.Specification.DataModel.Us;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Us.Provider.Service
{

    /// <summary>
    /// 
    /// </summary>
    public class K12StudentService : IGenericService<K12Student, string>
    {
        private static IDictionary<string, K12Student> studentsCache = new Dictionary<string, K12Student>();
        private static Random random = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static K12Student CreateStudent()
        {
            k12StudentTypeIdentityIdentification identification = new k12StudentTypeIdentityIdentification { studentId = random.Next(10000, 99999).ToString() };
            k12StudentTypeIdentityName name = new k12StudentTypeIdentityName { firstName = RandomNameGenerator.GivenName, lastName = RandomNameGenerator.FamilyName };
            k12StudentTypeIdentity identity = new k12StudentTypeIdentity { identification = identification, name = name };
            K12Student k12Student = new K12Student { refId = Guid.NewGuid().ToString(), identity = identity };

            return k12Student;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        static K12StudentService()
        {
            studentsCache = CreateStudents(10);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objs"></param>
        public void Create(IEnumerable<K12Student> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Create(K12Student obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objs"></param>
        public void Delete(IEnumerable<K12Student> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Delete(K12Student obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            studentsCache.Remove(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICollection<K12Student> Retrieve()
        {
            List<K12Student> students = new List<K12Student>();
            students.AddRange(studentsCache.Values);
            return students;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ICollection<K12Student> Retrieve(K12Student obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public K12Student Retrieve(string id)
        {
            K12Student student;

            if (!studentsCache.TryGetValue(id, out student))
            {
                student = null;
            }

            return student;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objs"></param>
        public void Update(IEnumerable<K12Student> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Update(K12Student obj)
        {

            if (studentsCache.ContainsKey(obj.Id))
            {
                studentsCache.Remove(obj.Id);
                studentsCache.Add(obj.Id, obj);
            }

        }

    }

}