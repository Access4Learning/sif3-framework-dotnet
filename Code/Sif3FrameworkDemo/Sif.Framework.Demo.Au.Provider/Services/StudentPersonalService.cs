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
using Sif.Framework.Demo.Au.Provider.Utils;
using Sif.Framework.Model.Events;
using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Query;
using Sif.Framework.Service.Providers;
using Sif.Specification.DataModel.Au;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sif.Framework.Demo.Au.Provider.Services
{
    public class StudentPersonalService : IBasicProviderService<StudentPersonal>, IChangesSinceService<List<StudentPersonal>>, IEventService<List<StudentPersonal>>
    {
        private const string changesSincePrefix = "ver.";

        private static int changesSinceNumber = 1;
        private static Random random = new Random();
        private static IDictionary<string, StudentPersonal> studentsCache = new Dictionary<string, StudentPersonal>();
        private static IDictionary<string, IDictionary<string, StudentPersonal>> studentsChangedCache = new Dictionary<string, IDictionary<string, StudentPersonal>>();

        public string ChangesSinceMarker
        {
            get
            {
                return string.Format("{0}{1}", changesSincePrefix, changesSinceNumber);
            }
        }

        public string NextChangesSinceMarker
        {
            get
            {
                changesSinceNumber++;
                return string.Format("{0}{1}", changesSincePrefix, changesSinceNumber);
            }
        }

        private static StudentPersonal CreateBartSimpson()
        {
            string[] text = new string[]
            {
                @"<MedicalCondition>
                    <ConditionID>Unique Medical Condition ID</ConditionID>
                    <Condition>Condition</Condition>
                    <Severity>Condition Severity</Severity>
                    <Details>Condition Details</Details>
                </MedicalCondition>"
            };

            SIF_ExtendedElementsTypeSIF_ExtendedElement extendedElement = new SIF_ExtendedElementsTypeSIF_ExtendedElement { Name = "MedicalConditions", Text = text };
            SIF_ExtendedElementsTypeSIF_ExtendedElement[] extendedElements = new SIF_ExtendedElementsTypeSIF_ExtendedElement[] { extendedElement };
            NameOfRecordType name = new NameOfRecordType { Type = NameOfRecordTypeType.LGL, FamilyName = "Simpson", GivenName = "Bart" };
            PersonInfoType personInfo = new PersonInfoType { Name = name };
            StudentPersonal studentPersonal = new StudentPersonal { RefId = Guid.NewGuid().ToString(), LocalId = "666", PersonInfo = personInfo, SIF_ExtendedElements = extendedElements };

            return studentPersonal;
        }

        private static StudentPersonal CreateStudent()
        {
            NameOfRecordType name = new NameOfRecordType { Type = NameOfRecordTypeType.LGL, FamilyName = RandomNameGenerator.FamilyName, GivenName = RandomNameGenerator.GivenName };
            PersonInfoType personInfo = new PersonInfoType { Name = name };
            StudentPersonal studentPersonal = new StudentPersonal { RefId = Guid.NewGuid().ToString(), LocalId = random.Next(10000, 99999).ToString(), PersonInfo = personInfo };

            return studentPersonal;
        }

        private static IDictionary<string, StudentPersonal> CreateStudents(int count)
        {
            IDictionary<string, StudentPersonal> studentPersonalsCache = new Dictionary<string, StudentPersonal>();

            if (count > 0)
            {
                StudentPersonal bartSimpson = CreateBartSimpson();
                studentPersonalsCache.Add(bartSimpson.RefId, bartSimpson);

                for (int i = 2; i <= count; i++)
                {
                    StudentPersonal studentPersonal = CreateStudent();
                    studentPersonalsCache.Add(studentPersonal.RefId, studentPersonal);
                }
            }

            return studentPersonalsCache;
        }

        static StudentPersonalService()
        {
            studentsCache = CreateStudents(20);
            studentsChangedCache.Add("ver.1", CreateStudents(10));
            studentsChangedCache.Add("ver.2", CreateStudents(3));
        }

        public StudentPersonal Create(
            StudentPersonal obj, bool?
            mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!mustUseAdvisory.HasValue || !mustUseAdvisory.Value)
            {
                string refId = Guid.NewGuid().ToString();
                obj.RefId = refId;
            }

            studentsCache.Add(obj.RefId, obj);

            return obj;
        }

        public StudentPersonal Retrieve(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!studentsCache.TryGetValue(refId, out StudentPersonal student))
            {
                student = null;
            }

            return student;
        }

        public List<StudentPersonal> Retrieve(
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            List<StudentPersonal> retrievedStudents = new List<StudentPersonal>();

            if ((zoneId == null && contextId == null) || ("Gov".Equals(zoneId) && "Curr".Equals(contextId)))
            {
                List<StudentPersonal> allStudents = new List<StudentPersonal>();

                if (requestParameters.Any(p => RequestParameterType.where.ToString().Equals(p.Name)))
                {
                    allStudents.Add(studentsCache.Values.FirstOrDefault());
                }
                else
                {
                    allStudents.AddRange(studentsCache.Values);
                }

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
            }

            return retrievedStudents;
        }

        public List<StudentPersonal> Retrieve(
            StudentPersonal obj,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            List<StudentPersonal> students = new List<StudentPersonal>();

            foreach (StudentPersonal student in studentsCache.Values)
            {
                if (student.PersonInfo.Name.FamilyName.Equals(obj.PersonInfo.Name.FamilyName) && student.PersonInfo.Name.GivenName.Equals(obj.PersonInfo.Name.GivenName))
                {
                    students.Add(student);
                }
            }

            return students;
        }

        public List<StudentPersonal> Retrieve(
            IEnumerable<EqualCondition> conditions,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            List<StudentPersonal> students = new List<StudentPersonal>
            {
                CreateBartSimpson()
            };

            return students;
        }

        public List<StudentPersonal> RetrieveChangesSince(
            string changesSinceMarker,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (string.IsNullOrEmpty(changesSinceMarker))
            {
                throw new ArgumentException(nameof(changesSinceMarker));
            }

            if (!studentsChangedCache.TryGetValue(changesSinceMarker, out IDictionary<string, StudentPersonal> students))
            {
                students = new Dictionary<string, StudentPersonal>();
            }

            return new List<StudentPersonal>(students.Values);
        }

        public void Update(
            StudentPersonal obj,
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

        public void Delete(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            studentsCache.Remove(refId);
        }

        public IEventIterator<List<StudentPersonal>> GetEventIterator(string zoneId = null, string contextId = null)
        {
            return new StudentPersonalIterator();
        }
    }
}