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

using log4net;
using Sif.Framework.Demo.Au.Consumer.Models;
using Sif.Framework.Model.Query;
using Sif.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sif.Framework.Demo.Au.Consumer
{

    /// <summary>
    /// 
    /// </summary>
    class ConsumerApp
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        void RunSchoolInfoConsumer()
        {
            ISchoolInfoConsumer schoolInfoConsumer = new SchoolInfoConsumer("HITS", null, "0EE41AE6-C43F-11E3-9050-E0F4DBD909AB", "HITS");
            schoolInfoConsumer.Register();

            try
            {
                ICollection<SchoolInfo> schools = schoolInfoConsumer.Retrieve();
            }
            finally
            {
                schoolInfoConsumer.Unregister();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        void RunStaffPersonalConsumer()
        {
            IStaffPersonalConsumer staffPersonalConsumer = new StaffPersonalConsumer("HITS", null, "0EE41AE6-C43F-11E3-9050-E0F4DBD909AB", "HITS");
            staffPersonalConsumer.Register();

            try
            {
                EqualCondition condition = new EqualCondition() { Left = "TeachingGroups", Right = "597ad3fe-47e7-4b2c-b919-a93c564d19d0" };
                IList<EqualCondition> conditions = new List<EqualCondition>();
                conditions.Add(condition);
                ICollection<StaffPersonal> staffPersonals = staffPersonalConsumer.Retrieve(conditions);

                foreach (StaffPersonal staffPersonal in staffPersonals)
                {
                    if (log.IsInfoEnabled) log.Info("Staff name is " + staffPersonal.PersonInfo.Name.GivenName + " " + staffPersonal.PersonInfo.Name.FamilyName);
                }

            }
            finally
            {
                staffPersonalConsumer.Unregister();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        void RunStudentPersonalConsumer()
        {
            IStudentPersonalConsumer studentPersonalConsumer = new StudentPersonalConsumer("Sif3DemoApp");
            studentPersonalConsumer.Register();
            if (log.IsInfoEnabled) log.Info("Registered the Consumer.");

            try
            {
                // Retrieve Bart Simpson.
                Name name = new Name { FamilyName = "Simpson", GivenName = "Bart" };
                PersonInfo personInfo = new PersonInfo { Name = name };
                StudentPersonal studentPersonal = new StudentPersonal { PersonInfo = personInfo };
                ICollection<StudentPersonal> filteredStudents = studentPersonalConsumer.Retrieve(studentPersonal);

                foreach (StudentPersonal student in filteredStudents)
                {
                    if (log.IsInfoEnabled) log.Info("Filtered student name is " + student.PersonInfo.Name.GivenName + " " + student.PersonInfo.Name.FamilyName);
                }

                // Create and then retrieve a new student.
                Name newStudentName = new Name() { FamilyName = "Wayne", GivenName = "Bruce", Type = NameType.LGL };
                PersonInfo newStudentInfo = new PersonInfo() { Name = newStudentName };
                StudentPersonal newStudent = new StudentPersonal() { LocalId = "555", PersonInfo = newStudentInfo };
                Guid newStudentId = studentPersonalConsumer.Create(newStudent);
                if (log.IsInfoEnabled) log.Info("Created new student " + newStudent.PersonInfo.Name.GivenName + " " + newStudent.PersonInfo.Name.FamilyName);
                StudentPersonal retrievedNewStudent = studentPersonalConsumer.Retrieve(newStudentId);
                if (log.IsInfoEnabled) log.Info("Retrieved new student " + retrievedNewStudent.PersonInfo.Name.GivenName + " " + retrievedNewStudent.PersonInfo.Name.FamilyName);

                // Retrieve all students.
                ICollection<StudentPersonal> students = studentPersonalConsumer.Retrieve();

                foreach (StudentPersonal student in students)
                {
                    if (log.IsInfoEnabled) log.Info("Student name is " + student.PersonInfo.Name.GivenName + " " + student.PersonInfo.Name.FamilyName);
                }

                // Retrieve a single student.
                Guid studentId = students.ElementAt(1).Id;
                StudentPersonal secondStudent = studentPersonalConsumer.Retrieve(studentId);
                if (log.IsInfoEnabled) log.Info("Name of second student is " + secondStudent.PersonInfo.Name.GivenName + " " + secondStudent.PersonInfo.Name.FamilyName);

                // Update that student and confirm.
                secondStudent.PersonInfo.Name.GivenName = "Homer";
                secondStudent.PersonInfo.Name.FamilyName = "Simpson";
                studentPersonalConsumer.Update(secondStudent);
                secondStudent = studentPersonalConsumer.Retrieve(studentId);
                if (log.IsInfoEnabled) log.Info("Name of second student has been changed to " + secondStudent.PersonInfo.Name.GivenName + " " + secondStudent.PersonInfo.Name.FamilyName);

                // Delete that student and confirm.
                studentPersonalConsumer.Delete(studentId);
                students = studentPersonalConsumer.Retrieve();
                bool studentDeleted = true;

                foreach (StudentPersonal student in students)
                {

                    if (studentId == student.Id)
                    {
                        studentDeleted = false;
                        break;
                    }

                }

                if (studentDeleted)
                {
                    if (log.IsInfoEnabled) log.Info("Student " + secondStudent.PersonInfo.Name.GivenName + " " + secondStudent.PersonInfo.Name.FamilyName + " was successfully deleted.");
                }
                else
                {
                    if (log.IsInfoEnabled) log.Info("Student " + secondStudent.PersonInfo.Name.GivenName + " " + secondStudent.PersonInfo.Name.FamilyName + " was NOT deleted.");
                }

                // Retrieve students based on Teaching Group.
                EqualCondition condition = new EqualCondition() { Left = "TeachingGroups", Right = "597ad3fe-47e7-4b2c-b919-a93c564d19d0" };
                IList<EqualCondition> conditions = new List<EqualCondition>();
                conditions.Add(condition);
                ICollection<StudentPersonal> teachingGroupStudents = studentPersonalConsumer.Retrieve(conditions);

                foreach (StudentPersonal student in teachingGroupStudents)
                {
                    if (log.IsInfoEnabled) log.Info("Student name is " + student.PersonInfo.Name.GivenName + " " + student.PersonInfo.Name.FamilyName);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
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
