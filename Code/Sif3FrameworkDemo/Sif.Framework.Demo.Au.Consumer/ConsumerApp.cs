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

using log4net;
using Sif.Framework.Demo.Au.Consumer.Models;
using Sif.Framework.Demo.Au.Consumer.Utils;
using Sif.Framework.Model.Query;
using Sif.Framework.Model.Responses;
using Sif.Framework.Utils;
using Sif.Specification.DataModel.Au;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sif.Framework.Demo.Au.Consumer
{

    class ConsumerApp
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Random random = new Random();

        private List<SchoolInfo> CreateSchools()
        {
            SchoolInfo applecrossHigh = new SchoolInfo { SchoolName = "Applecross SHS" };
            SchoolInfo rossmoyneHigh = new SchoolInfo { SchoolName = "Rossmoyne SHS" };
            List<SchoolInfo> schoolCollection = new List<SchoolInfo> { applecrossHigh, rossmoyneHigh };

            return schoolCollection;
        }

        private static StudentPersonal CreateStudent()
        {
            NameOfRecordType name = new NameOfRecordType { Type = NameOfRecordTypeType.LGL, FamilyName = RandomNameGenerator.FamilyName, GivenName = RandomNameGenerator.GivenName };
            PersonInfoType personInfo = new PersonInfoType { Name = name };
            StudentPersonal studentPersonal = new StudentPersonal { LocalId = random.Next(10000, 99999).ToString(), PersonInfo = personInfo };

            return studentPersonal;
        }

        private static List<StudentPersonal> CreateStudents(int count)
        {
            List<StudentPersonal> studentPersonalsCache = new List<StudentPersonal>();

            for (int i = 1; i <= count; i++)
            {
                studentPersonalsCache.Add(CreateStudent());
            }

            return studentPersonalsCache;
        }

        void RunSchoolInfoConsumer()
        {
            SchoolInfoConsumer schoolInfoConsumer = new SchoolInfoConsumer("HITS", null, "0EE41AE6-C43F-11E3-9050-E0F4DBD909AB", "HITS");
            schoolInfoConsumer.Register();

            try
            {
                // Query all schools.
                IEnumerable<SchoolInfo> allSchools = schoolInfoConsumer.Query();

                foreach (SchoolInfo school in allSchools)
                {
                    if (log.IsInfoEnabled) log.Info("School " + school.SchoolName + " has a RefId of " + school.RefId + ".");
                }

                if (log.IsInfoEnabled) log.Info("School count is " + allSchools.Count());

                // Create multiple schools.
                MultipleCreateResponse createResponse = schoolInfoConsumer.Create(CreateSchools());

                foreach (CreateStatus status in createResponse.StatusRecords)
                {
                    SchoolInfo school = schoolInfoConsumer.Query(status.Id);
                    if (log.IsInfoEnabled) log.Info("New school " + school.SchoolName + " has a RefId of " + school.RefId + ".");
                }

                // Update multiple schools.
                List<SchoolInfo> schoolsToUpdate = new List<SchoolInfo>();

                foreach (CreateStatus status in createResponse.StatusRecords)
                {
                    SchoolInfo school = schoolInfoConsumer.Query(status.Id);
                    school.SchoolName += "x";
                    schoolsToUpdate.Add(school);
                }

                MultipleUpdateResponse updateResponse = schoolInfoConsumer.Update(schoolsToUpdate);

                foreach (UpdateStatus status in updateResponse.StatusRecords)
                {
                    SchoolInfo school = schoolInfoConsumer.Query(status.Id);
                    if (log.IsInfoEnabled) log.Info("Updated school " + school.SchoolName + " has a RefId of " + school.RefId + ".");
                }

                // Delete multiple schools.
                ICollection<string> schoolsToDelete = new List<string>();

                foreach (CreateStatus status in createResponse.StatusRecords)
                {
                    schoolsToDelete.Add(status.Id);
                }

                MultipleDeleteResponse deleteResponse = schoolInfoConsumer.Delete(schoolsToDelete);

                foreach (DeleteStatus status in deleteResponse.StatusRecords)
                {
                    SchoolInfo school = schoolInfoConsumer.Query(status.Id);

                    if (school == null)
                    {
                        if (log.IsInfoEnabled) log.Info("School with RefId of " + status.Id + " has been deleted.");
                    }
                    else
                    {
                        if (log.IsInfoEnabled) log.Info("School " + school.SchoolName + " with RefId of " + school.RefId + " FAILED deletion.");
                    }

                }

            }
            finally
            {
                schoolInfoConsumer.Unregister();
            }

        }

        void RunStaffPersonalConsumer()
        {
            StaffPersonalConsumer staffPersonalConsumer = new StaffPersonalConsumer("HITS", null, "0EE41AE6-C43F-11E3-9050-E0F4DBD909AB", "HITS");
            staffPersonalConsumer.Register();

            try
            {
                EqualCondition condition = new EqualCondition() { Left = "TeachingGroups", Right = "597ad3fe-47e7-4b2c-b919-a93c564d19d0" };
                IList<EqualCondition> conditions = new List<EqualCondition>();
                conditions.Add(condition);
                IEnumerable<StaffPersonal> staffPersonals = staffPersonalConsumer.QueryByServicePath(conditions);

                foreach (StaffPersonal staffPersonal in staffPersonals)
                {
                    if (log.IsInfoEnabled) log.Info("Staff name is " + staffPersonal.PersonInfo.Name.GivenName + " " + staffPersonal.PersonInfo.Name.FamilyName);
                }

                if (log.IsInfoEnabled) log.Info("Staff count is " + staffPersonals.Count());
            }
            finally
            {
                staffPersonalConsumer.Unregister();
            }

        }

        void RunStudentPersonalConsumer()
        {
            StudentPersonalConsumer studentPersonalConsumer = new StudentPersonalConsumer("Sif3DemoApp");
            studentPersonalConsumer.Register();
            if (log.IsInfoEnabled) log.Info("Registered the Consumer.");

            try
            {
                // Retrieve Bart Simpson using QBE.
                if (log.IsInfoEnabled) log.Info("*** Retrieve Bart Simpson using QBE.");
                NameOfRecordType name = new NameOfRecordType { FamilyName = "Simpson", GivenName = "Bart" };
                PersonInfoType personInfo = new PersonInfoType { Name = name };
                StudentPersonal studentPersonal = new StudentPersonal { PersonInfo = personInfo };
                IEnumerable<StudentPersonal> filteredStudents = studentPersonalConsumer.QueryByExample(studentPersonal);

                foreach (StudentPersonal student in filteredStudents)
                {
                    if (log.IsInfoEnabled) log.Info("Filtered student name is " + student.PersonInfo.Name.GivenName + " " + student.PersonInfo.Name.FamilyName);
                }

                // Create a new student.
                if (log.IsInfoEnabled) log.Info("*** Create a new student.");
                string[] text = new string[]
                {
                    @"
                        <MedicalCondition>
                            <ConditionID>Unique Medical Condition ID</ConditionID>
                            <Condition>Condition</Condition>
                            <Severity>Condition Severity</Severity>
                            <Details>Condition Details</Details>
                        </MedicalCondition>
                    "
                };
                SIF_ExtendedElementsTypeSIF_ExtendedElement extendedElement = new SIF_ExtendedElementsTypeSIF_ExtendedElement { Name = "MedicalConditions", Text = text };
                SIF_ExtendedElementsTypeSIF_ExtendedElement[] extendedElements = new SIF_ExtendedElementsTypeSIF_ExtendedElement[] { extendedElement };
                NameOfRecordType newStudentName = new NameOfRecordType { FamilyName = "Wayne", GivenName = "Bruce", Type = NameOfRecordTypeType.LGL };
                PersonInfoType newStudentInfo = new PersonInfoType { Name = newStudentName };
                StudentPersonal newStudent = new StudentPersonal { LocalId = "555", PersonInfo = newStudentInfo, SIF_ExtendedElements = extendedElements };
                StudentPersonal retrievedNewStudent = studentPersonalConsumer.Create(newStudent);
                if (log.IsInfoEnabled) log.Info("Created new student " + newStudent.PersonInfo.Name.GivenName + " " + newStudent.PersonInfo.Name.FamilyName);

                // Create multiple new students.
                if (log.IsInfoEnabled) log.Info("*** Create multiple new students.");
                List<StudentPersonal> newStudents = CreateStudents(5);
                MultipleCreateResponse multipleCreateResponse = studentPersonalConsumer.Create(newStudents);
                int count = 0;

                foreach (CreateStatus status in multipleCreateResponse.StatusRecords)
                {
                    if (log.IsInfoEnabled) log.Info("Create status code is " + status.StatusCode);
                    newStudents[count++].RefId = status.Id;
                }

                // Update multiple students.
                if (log.IsInfoEnabled) log.Info("*** Update multiple students.");
                foreach (StudentPersonal student in newStudents)
                {
                    student.PersonInfo.Name.GivenName += "o";
                }

                MultipleUpdateResponse multipleUpdateResponse = studentPersonalConsumer.Update(newStudents);

                foreach (UpdateStatus status in multipleUpdateResponse.StatusRecords)
                {
                    if (log.IsInfoEnabled) log.Info("Update status code is " + status.StatusCode);
                }

                // Delete multiple students.
                if (log.IsInfoEnabled) log.Info("*** Delete multiple students.");
                ICollection<string> refIds = new List<string>();

                foreach (CreateStatus status in multipleCreateResponse.StatusRecords)
                {
                    refIds.Add(status.Id);
                }

                MultipleDeleteResponse multipleDeleteResponse = studentPersonalConsumer.Delete(refIds);

                foreach (DeleteStatus status in multipleDeleteResponse.StatusRecords)
                {
                    if (log.IsInfoEnabled) log.Info("Delete status code is " + status.StatusCode);
                }

                // Retrieve all students from zone "Gov" and context "Curr".
                if (log.IsInfoEnabled) log.Info("*** Retrieve all students from zone \"Gov\" and context \"Curr\".");
                IEnumerable<StudentPersonal> students = studentPersonalConsumer.Query(zone: "Gov", context: "Curr");

                foreach (StudentPersonal student in students)
                {
                    if (log.IsInfoEnabled) log.Info("Student name is " + student.PersonInfo.Name.GivenName + " " + student.PersonInfo.Name.FamilyName);
                }

                if (students.Count() > 1)
                {

                    // Retrieve a single student.
                    if (log.IsInfoEnabled) log.Info("*** Retrieve a single student.");
                    string studentId = students.ElementAt(1).RefId;
                    StudentPersonal secondStudent = studentPersonalConsumer.Query(studentId);
                    if (log.IsInfoEnabled) log.Info("Name of second student is " + secondStudent.PersonInfo.Name.GivenName + " " + secondStudent.PersonInfo.Name.FamilyName);

                    // Update that student and confirm.
                    if (log.IsInfoEnabled) log.Info("*** Update that student and confirm.");
                    secondStudent.PersonInfo.Name.GivenName = "Homer";
                    secondStudent.PersonInfo.Name.FamilyName = "Simpson";
                    studentPersonalConsumer.Update(secondStudent);
                    secondStudent = studentPersonalConsumer.Query(studentId);
                    if (log.IsInfoEnabled) log.Info("Name of second student has been changed to " + secondStudent.PersonInfo.Name.GivenName + " " + secondStudent.PersonInfo.Name.FamilyName);

                    // Delete that student and confirm.
                    if (log.IsInfoEnabled) log.Info("*** Delete that student and confirm.");
                    studentPersonalConsumer.Delete(studentId);
                    StudentPersonal deletedStudent = studentPersonalConsumer.Query(studentId);
                    bool studentDeleted = (deletedStudent == null ? true : false);

                    if (studentDeleted)
                    {
                        if (log.IsInfoEnabled) log.Info("Student " + secondStudent.PersonInfo.Name.GivenName + " " + secondStudent.PersonInfo.Name.FamilyName + " was successfully deleted.");
                    }
                    else
                    {
                        if (log.IsInfoEnabled) log.Info("Student " + secondStudent.PersonInfo.Name.GivenName + " " + secondStudent.PersonInfo.Name.FamilyName + " was NOT deleted.");
                    }

                }

                // Retrieve students based on Teaching Group using Service Paths.
                if (log.IsInfoEnabled) log.Info("*** Retrieve students based on Teaching Group using Service Paths.");
                EqualCondition condition = new EqualCondition() { Left = "TeachingGroups", Right = "597ad3fe-47e7-4b2c-b919-a93c564d19d0" };
                IList<EqualCondition> conditions = new List<EqualCondition>();
                conditions.Add(condition);
                IEnumerable<StudentPersonal> teachingGroupStudents = studentPersonalConsumer.QueryByServicePath(conditions);

                foreach (StudentPersonal student in teachingGroupStudents)
                {
                    if (log.IsInfoEnabled) log.Info("Student name is " + student.PersonInfo.Name.GivenName + " " + student.PersonInfo.Name.FamilyName);

                    if (student.SIF_ExtendedElements != null && student.SIF_ExtendedElements.Length > 0)
                    {

                        foreach (SIF_ExtendedElementsTypeSIF_ExtendedElement element in student.SIF_ExtendedElements)
                        {

                            foreach (string content in element.Text)
                            {
                                if (log.IsInfoEnabled) log.Info("Extended element text is ...\n" + content);
                            }

                        }

                    }

                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                studentPersonalConsumer.Unregister();
                if (log.IsInfoEnabled) log.Info("Unregistered the Consumer.");
            }

        }

        static void Main(string[] args)
        {
            ConsumerApp app = new ConsumerApp();

            try
            {
                if ("HITS".Equals(SettingsManager.ConsumerSettings.ApplicationKey))
                {
                    app.RunSchoolInfoConsumer();
                    app.RunStaffPersonalConsumer();
                }
                else if ("Sif3DemoApp".Equals(SettingsManager.ConsumerSettings.ApplicationKey))
                {
                    app.RunStudentPersonalConsumer();
                }

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Error running the " + SettingsManager.ConsumerSettings.ApplicationKey + "  Consumer.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

    }

}
