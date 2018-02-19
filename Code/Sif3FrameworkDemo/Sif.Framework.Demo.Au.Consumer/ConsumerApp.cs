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

using Sif.Framework.Demo.Au.Consumer.Consumers;
using Sif.Framework.Demo.Au.Consumer.Models;
using Sif.Framework.Demo.Au.Consumer.Utils;
using Sif.Framework.Model.Query;
using Sif.Framework.Model.Responses;
using Sif.Framework.Utils;
using Sif.Specification.DataModel.Au;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sif.Framework.Demo.Au.Consumer
{

    class ConsumerApp
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Random random = new Random();

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

        private static bool RunDemo(string demoName)
        {
            Console.WriteLine();
            Console.Write("Would you like run the " + demoName + " demo (Y/N)? - ");
            ConsoleKeyInfo info = new ConsoleKeyInfo();

            do
            {
                info = Console.ReadKey();
            }
            while (info.Key != ConsoleKey.N && info.Key != ConsoleKey.Y && info.Key != ConsoleKey.Enter);

            Console.WriteLine();
            Console.WriteLine();

            return (info.Key == ConsoleKey.Y);
        }

        void RunStudentPersonalConsumer()
        {
            StudentPersonalConsumer studentPersonalConsumer = new StudentPersonalConsumer(
                SettingsManager.ConsumerSettings.ApplicationKey,
                SettingsManager.ConsumerSettings.InstanceId,
                SettingsManager.ConsumerSettings.UserToken,
                SettingsManager.ConsumerSettings.SolutionId);
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
                string studentID = Guid.NewGuid().ToString();
                StudentPersonal newStudent = new StudentPersonal { RefId = studentID, LocalId = "555", PersonInfo = newStudentInfo, SIF_ExtendedElements = extendedElements };

                try
                {
                    StudentPersonal retrievedNewStudent = studentPersonalConsumer.Create(newStudent, true);
                    if (log.IsInfoEnabled) log.Info($"Created new student {newStudent.PersonInfo.Name.GivenName} {newStudent.PersonInfo.Name.FamilyName} with ID of {studentID}.");
                }
                catch (UnauthorizedAccessException)
                {
                    if (log.IsInfoEnabled) log.Info($"Access to create a new student is rejected.");
                }

                // Create multiple new students.
                if (log.IsInfoEnabled) log.Info("*** Create multiple new students.");
                List<StudentPersonal> newStudents = CreateStudents(5);

                try
                {
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

                    try
                    {
                        MultipleUpdateResponse multipleUpdateResponse = studentPersonalConsumer.Update(newStudents);

                        foreach (UpdateStatus status in multipleUpdateResponse.StatusRecords)
                        {
                            if (log.IsInfoEnabled) log.Info("Update status code is " + status.StatusCode);
                        }

                    }
                    catch (UnauthorizedAccessException)
                    {
                        if (log.IsInfoEnabled) log.Info($"Access to update multiple students is rejected.");
                    }

                    // Delete multiple students.
                    if (log.IsInfoEnabled) log.Info("*** Delete multiple students.");
                    ICollection<string> refIds = new List<string>();

                    foreach (CreateStatus status in multipleCreateResponse.StatusRecords)
                    {
                        refIds.Add(status.Id);
                    }

                    try
                    {
                        MultipleDeleteResponse multipleDeleteResponse = studentPersonalConsumer.Delete(refIds);

                        foreach (DeleteStatus status in multipleDeleteResponse.StatusRecords)
                        {
                            if (log.IsInfoEnabled) log.Info("Delete status code is " + status.StatusCode);
                        }

                    }
                    catch (UnauthorizedAccessException)
                    {
                        if (log.IsInfoEnabled) log.Info($"Access to delete multiple students is rejected.");
                    }

                }
                catch (UnauthorizedAccessException)
                {
                    if (log.IsInfoEnabled) log.Info($"Access to create multiple new students is rejected.");
                }

                // Retrieve all students from zone "Gov" and context "Curr".
                if (log.IsInfoEnabled) log.Info("*** Retrieve all students from zone \"Gov\" and context \"Curr\".");
                IEnumerable<StudentPersonal> students = studentPersonalConsumer.Query(zoneId: "Gov", contextId: "Curr");

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

                    try
                    {
                        studentPersonalConsumer.Update(secondStudent);
                        secondStudent = studentPersonalConsumer.Query(studentId);
                        if (log.IsInfoEnabled) log.Info("Name of second student has been changed to " + secondStudent.PersonInfo.Name.GivenName + " " + secondStudent.PersonInfo.Name.FamilyName);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        if (log.IsInfoEnabled) log.Info($"Access to update a student is rejected.");
                    }

                    // Delete that student and confirm.
                    if (log.IsInfoEnabled) log.Info("*** Delete that student and confirm.");

                    try
                    {
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
                    catch (UnauthorizedAccessException)
                    {
                        if (log.IsInfoEnabled) log.Info($"Access to delete a student is rejected.");
                    }

                }

                // Retrieve students based on Teaching Group using Service Paths.
                if (log.IsInfoEnabled) log.Info("*** Retrieve students based on Teaching Group using Service Paths.");
                EqualCondition condition = new EqualCondition() { Left = "TeachingGroups", Right = "597ad3fe-47e7-4b2c-b919-a93c564d19d0" };
                IList<EqualCondition> conditions = new List<EqualCondition>();
                conditions.Add(condition);

                try
                {
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
                catch (UnauthorizedAccessException)
                {
                    if (log.IsInfoEnabled) log.Info("Access to query students by Service Path TeachingGroups/{}/StudentPersonals is rejected.");
                }

                // Retrieve student changes since a particular point as defined by the Changes Since marker.
                if (log.IsInfoEnabled) log.Info("*** Retrieve student changes since a particular point as defined by the Changes Since marker.");
                string changesSinceMarker = studentPersonalConsumer.GetChangesSinceMarker();
                string nextChangesSinceMarker;
                IEnumerable<StudentPersonal> changedStudents = studentPersonalConsumer.QueryChangesSince(changesSinceMarker, out nextChangesSinceMarker);
                if (log.IsInfoEnabled) log.Info("Iteration 1 - Student changes based on Changes Since marker - " + changesSinceMarker);

                if (changedStudents == null || changedStudents.Count() == 0)
                {
                    if (log.IsInfoEnabled) log.Info("No student changes");
                }
                else
                {

                    foreach (StudentPersonal student in changedStudents)
                    {
                        if (log.IsInfoEnabled) log.Info("Student name is " + student.PersonInfo.Name.GivenName + " " + student.PersonInfo.Name.FamilyName);
                    }

                }

                changesSinceMarker = nextChangesSinceMarker;
                nextChangesSinceMarker = null;
                changedStudents = studentPersonalConsumer.QueryChangesSince(changesSinceMarker, out nextChangesSinceMarker);
                if (log.IsInfoEnabled) log.Info("Iteration 2 - Student changes based on Changes Since marker - " + changesSinceMarker);

                if (changedStudents == null || changedStudents.Count() == 0)
                {
                    if (log.IsInfoEnabled) log.Info("No student changes");
                }
                else
                {

                    foreach (StudentPersonal student in changedStudents)
                    {
                        if (log.IsInfoEnabled) log.Info("Student name is " + student.PersonInfo.Name.GivenName + " " + student.PersonInfo.Name.FamilyName);
                    }

                }

                changesSinceMarker = nextChangesSinceMarker;
                nextChangesSinceMarker = null;
                changedStudents = studentPersonalConsumer.QueryChangesSince(changesSinceMarker, out nextChangesSinceMarker);
                if (log.IsInfoEnabled) log.Info("Iteration 3 - Student changes based on Changes Since marker - " + changesSinceMarker);

                if (changedStudents == null || changedStudents.Count() == 0)
                {
                    if (log.IsInfoEnabled) log.Info("No student changes");
                }
                else
                {

                    foreach (StudentPersonal student in changedStudents)
                    {
                        if (log.IsInfoEnabled) log.Info("Student name is " + student.PersonInfo.Name.GivenName + " " + student.PersonInfo.Name.FamilyName);
                    }

                }

            }
            catch (UnauthorizedAccessException)
            {
                if (log.IsInfoEnabled) log.Info($"Access to query students is rejected.");
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Error running the StudentPersonal Consumer.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
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

            if (RunDemo("Student Personal CRUD Consumer"))
            {

                try
                {
                    app.RunStudentPersonalConsumer();
                }
                catch (Exception e)
                {
                    if (log.IsErrorEnabled) log.Error("Error running the Student Personal CRUD Consumer.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
                }

            }

            Console.WriteLine();
            Console.WriteLine("********************************************************************************");
            Console.WriteLine();
            Console.WriteLine("To run the following Event Consumer demo, the following is required:");
            Console.WriteLine();
            Console.WriteLine("  1) The consumer.environment.url app setting in the SifFramework.config file needs to reference the BROKERED environment endpoint.");
            Console.WriteLine("  2) The Sif.Framework.Demo.Broker needs to be run instead of the Sif.Framework.EnvironmentProvider.");
            Console.WriteLine();
            Console.WriteLine("********************************************************************************");

            if (RunDemo("Student Personal Event Consumer"))
            {

                try
                {
                    StudentPersonalEventConsumer studentPersonalConsumer = new StudentPersonalEventConsumer(
                        SettingsManager.ConsumerSettings.ApplicationKey,
                        SettingsManager.ConsumerSettings.InstanceId,
                        SettingsManager.ConsumerSettings.UserToken,
                        SettingsManager.ConsumerSettings.SolutionId);
                    studentPersonalConsumer.Start("Sif3DemoZone1", "DEFAULT");
                    if (log.IsInfoEnabled) log.Info("Started the Event Consumer.");

                    Console.WriteLine("Press any key to stop the Event Consumer (may take several seconds to complete) ...");
                    Console.ReadKey();

                    studentPersonalConsumer.Stop();
                    if (log.IsInfoEnabled) log.Info("Stopped the Event Consumer.");
                }
                catch (Exception e)
                {
                    if (log.IsErrorEnabled) log.Error("Error running the Student Personal Event Consumer.\n" + ExceptionUtils.InferErrorResponseMessage(e), e);
                }

            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

    }

}
