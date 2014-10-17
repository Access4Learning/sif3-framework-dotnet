/*
 * Copyright 2014 Systemic Pty Ltd
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

using Sif.Framework.Demo.Au.DataModel;
using Sif.Framework.Demo.Common.Utils;
using Sif.Framework.Service;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Au.Provider.Service
{

    /// <summary>
    /// 
    /// </summary>
    public class StudentPersonalService : IGenericService<StudentPersonal, Guid>
    {
        private static IDictionary<Guid, StudentPersonal> studentsCache = new Dictionary<Guid, StudentPersonal>();
        private static Random random = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static StudentPersonal CreateStudent()
        {
            Name name = new Name { Type = NameType.LGL, FamilyName = RandomNameGenerator.FamilyName, GivenName = RandomNameGenerator.GivenName };
            PersonInfo personInfo = new PersonInfo { Name = name };
            StudentPersonal studentPersonal = new StudentPersonal { Id = Guid.NewGuid(), LocalId = random.Next(10000, 99999).ToString(), PersonInfo = personInfo };

            return studentPersonal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private static IDictionary<Guid, StudentPersonal> CreateStudents(int count)
        {
            IDictionary<Guid, StudentPersonal> studentPersonalsCache = new Dictionary<Guid, StudentPersonal>();

            for (int i = 1; i <= count; i++)
            {
                StudentPersonal studentPersonal = CreateStudent();
                studentPersonalsCache.Add(studentPersonal.Id, studentPersonal);
            }

            return studentPersonalsCache;
        }
        
        /// <summary>
        /// 
        /// </summary>
        static StudentPersonalService()
        {
            studentsCache = CreateStudents(10);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objs"></param>
        public void Create(IEnumerable<StudentPersonal> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Guid Create(StudentPersonal obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objs"></param>
        public void Delete(IEnumerable<StudentPersonal> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Delete(StudentPersonal obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Delete(Guid id)
        {
            studentsCache.Remove(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICollection<StudentPersonal> Retrieve()
        {
            List<StudentPersonal> students = new List<StudentPersonal>();
            students.AddRange(studentsCache.Values);
            return students;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ICollection<StudentPersonal> Retrieve(StudentPersonal obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StudentPersonal Retrieve(Guid id)
        {
            StudentPersonal student;

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
        public void Update(IEnumerable<StudentPersonal> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Update(StudentPersonal obj)
        {

            if (studentsCache.ContainsKey(obj.Id))
            {
                studentsCache.Remove(obj.Id);
                studentsCache.Add(obj.Id, obj);
            }

        }

    }

}
