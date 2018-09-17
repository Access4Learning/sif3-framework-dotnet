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
using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Query;
using Sif.Framework.Service.Providers;
using Sif.Specification.DataModel.Au;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Au.Provider.Services
{
    public class StudentSchoolEnrollmentService : IBasicProviderService<StudentSchoolEnrollment>
    {
        private static IDictionary<string, StudentSchoolEnrollment> enrollmentsCache = new Dictionary<string, StudentSchoolEnrollment>();

        private static StudentSchoolEnrollment CreateEnrollment()
        {
            StudentSchoolEnrollment enrollment = new StudentSchoolEnrollment
            {
                RefId = Guid.NewGuid().ToString(),
                YearLevel = new YearLevelType
                {
                    Code = AUCodeSetsYearLevelCodeType.Item10
                }
            };

            return enrollment;
        }

        private static IDictionary<string, StudentSchoolEnrollment> CreateEnrollments(int count)
        {
            IDictionary<string, StudentSchoolEnrollment> enrollmentsCache = new Dictionary<string, StudentSchoolEnrollment>();

            for (int i = 1; i <= count; i++)
            {
                StudentSchoolEnrollment enrollment = CreateEnrollment();
                enrollmentsCache.Add(enrollment.RefId, enrollment);
            }

            return enrollmentsCache;
        }

        static StudentSchoolEnrollmentService()
        {
            enrollmentsCache = CreateEnrollments(5);
        }

        public StudentSchoolEnrollment Create(
            StudentSchoolEnrollment obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public void Delete(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public StudentSchoolEnrollment Retrieve(
            string refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public List<StudentSchoolEnrollment> Retrieve(
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }

        public List<StudentSchoolEnrollment> Retrieve(
            StudentSchoolEnrollment obj,
            uint? pageIndex = null,
            uint? pageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            List<StudentSchoolEnrollment> enrollments = new List<StudentSchoolEnrollment>();

            foreach (StudentSchoolEnrollment enrollment in enrollmentsCache.Values)
            {
                if (enrollment.YearLevel.Code.Equals(obj?.YearLevel.Code))
                {
                    enrollments.Add(enrollment);
                }
            }

            return enrollments;
        }

        public List<StudentSchoolEnrollment> Retrieve(
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
            StudentSchoolEnrollment obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            throw new NotImplementedException();
        }
    }
}