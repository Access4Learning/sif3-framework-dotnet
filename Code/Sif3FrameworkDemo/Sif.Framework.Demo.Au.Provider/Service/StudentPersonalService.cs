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

using Sif.Framework.Demo.Au.Provider.Models;
using Sif.Framework.Demo.Au.Provider.Utils;
using Sif.Framework.Service;
using Sif.Specification.DataModel.Au;
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
            NameOfRecordType name = new NameOfRecordType { Type = NameOfRecordTypeType.LGL, FamilyName = RandomNameGenerator.FamilyName, GivenName = RandomNameGenerator.GivenName };
            PersonInfoType personInfo = new PersonInfoType { Name = name };
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
            studentsCache = CreateStudents(20);
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
            Guid id = Guid.NewGuid();
            obj.Id = id;
            studentsCache.Add(id, obj);
            return id;
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
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ICollection<StudentPersonal> Retrieve(int pageIndex, int pageSize)
        {
            List<StudentPersonal> allStudents = new List<StudentPersonal>();
            allStudents.AddRange(studentsCache.Values);
            List<StudentPersonal> retrievedStudents = new List<StudentPersonal>();

            int index = pageIndex * pageSize;
            int count = pageSize;

            if (studentsCache.Values.Count < (index + count))
            {
                count = studentsCache.Values.Count - index;
            }
            else
            {
                count = pageSize;
            }

            if (index <= studentsCache.Values.Count)
            {
                retrievedStudents = allStudents.GetRange(index, count);
            }

            return retrievedStudents;
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
