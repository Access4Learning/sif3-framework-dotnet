/*
 * Copyright 2017 Systemic Pty Ltd
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
using Sif.Framework.Model.Events;
using Sif.Specification.DataModel.Au;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sif.Framework.Demo.Au.Provider.Services
{

    public class StudentPersonalIterator : IEventIterator<List<StudentPersonal>>
    {
        private static Random random = new Random();
        private static int studentIndex = 0;
        private static IList<StudentPersonal> students = new List<StudentPersonal>();

        private static StudentPersonal CreateStudent()
        {

            NameOfRecordType name = new NameOfRecordType
            {
                Type = NameOfRecordTypeType.LGL,
                FamilyName = RandomNameGenerator.FamilyName,
                GivenName = RandomNameGenerator.GivenName
            };

            PersonInfoType personInfo = new PersonInfoType { Name = name };

            StudentPersonal studentPersonal = new StudentPersonal
            {
                RefId = Guid.NewGuid().ToString(),
                LocalId = random.Next(10000, 99999).ToString(),
                PersonInfo = personInfo
            };

            return studentPersonal;
        }

        private static IList<StudentPersonal> CreateStudents(int count)
        {
            IList<StudentPersonal> students = new List<StudentPersonal>();

            for (int i = 0; i < count; i++)
            {
                StudentPersonal studentPersonal = CreateStudent();
                students.Add(studentPersonal);
            }

            return students;
        }

        static StudentPersonalIterator()
        {
            students = CreateStudents(20);
        }

        public SifEvent<List<StudentPersonal>> GetNext()
        {
            SifEvent<List<StudentPersonal>> sifEvent = null;

            if (studentIndex < students.Count)
            {

                sifEvent = new SifEvent<List<StudentPersonal>>()
                {
                    EventAction = EventAction.UPDATE_FULL,
                    Id = Guid.NewGuid(),
                    SifObjects = students.Skip(studentIndex).Take(7).ToList()
                };

                studentIndex += 7;
            }

            return sifEvent;
        }

        public bool HasNext()
        {
            return (studentIndex < students.Count);
        }

    }

}