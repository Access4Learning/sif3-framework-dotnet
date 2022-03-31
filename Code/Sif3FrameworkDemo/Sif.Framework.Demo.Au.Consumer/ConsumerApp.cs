/*
 * Copyright 2021 Systemic Pty Ltd
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
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Sessions;
using Sif.Framework.Utils;
using Sif.Specification.DataModel.Au;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sif.Framework.Demo.Au.Consumer
{
    internal class ConsumerApp : ConsoleApp
    {
        private static readonly slf4net.ILogger Log =
            slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Random Random = new Random();

        private static StudentPersonal CreateStudent()
        {
            var name = new NameOfRecordType
            {
                Type = NameOfRecordTypeType.LGL,
                FamilyName = RandomNameGenerator.FamilyName,
                GivenName = RandomNameGenerator.GivenName
            };

            var personInfo = new PersonInfoType { Name = name };

            var studentPersonal = new StudentPersonal
            {
                LocalId = Random.Next(10000, 99999).ToString(),
                PersonInfo = personInfo
            };

            return studentPersonal;
        }

        private static List<StudentPersonal> CreateStudents(int count)
        {
            var studentPersonalsCache = new List<StudentPersonal>();

            for (var i = 1; i <= count; i++)
            {
                studentPersonalsCache.Add(CreateStudent());
            }

            return studentPersonalsCache;
        }

        private static void RunConsumer(IFrameworkSettings settings, ISessionService sessionService)
        {
            var consumer = new StudentPersonalConsumer(
                settings.ApplicationKey,
                settings.InstanceId,
                settings.UserToken,
                settings.SolutionId,
                settings,
                sessionService);
            consumer.Register();

            if (Log.IsInfoEnabled) Log.Info("Registered the Consumer.");

            try
            {
                IEnumerable<StudentPersonal> queriedStudents = consumer.DynamicQuery("[@id=1234]");

                foreach (StudentPersonal student in queriedStudents)
                {
                    if (Log.IsInfoEnabled)
                        Log.Info($"Queried student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
                }

                // Retrieve Bart Simpson using QBE.

                // TODO Uncomment once the ASP.NET Core Provider has been fully implemented.
                //if (Log.IsInfoEnabled) Log.Info("*** Retrieve Bart Simpson using QBE.");

                //var name = new NameOfRecordType { FamilyName = "Simpson", GivenName = "Bart" };
                //var personInfo = new PersonInfoType { Name = name };
                //var studentPersonal = new StudentPersonal { PersonInfo = personInfo };
                //IEnumerable<StudentPersonal> filteredStudents = consumer.QueryByExample(studentPersonal);

                //foreach (StudentPersonal student in filteredStudents)
                //{
                //    if (Log.IsInfoEnabled)
                //        Log.Info($"Filtered student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
                //}

                // Create a new student.

                if (Log.IsInfoEnabled) Log.Info("*** Create a new student.");

                var text = new[]
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

                var extendedElement = new SIF_ExtendedElementsTypeSIF_ExtendedElement
                {
                    Name = "MedicalConditions",
                    Text = text
                };

                SIF_ExtendedElementsTypeSIF_ExtendedElement[] extendedElements = { extendedElement };

                var newStudentName = new NameOfRecordType
                {
                    FamilyName = "Wayne",
                    GivenName = "Bruce",
                    Type = NameOfRecordTypeType.LGL
                };

                var newStudentInfo = new PersonInfoType { Name = newStudentName };

                var newStudent = new StudentPersonal
                {
                    RefId = Guid.NewGuid().ToString(),
                    LocalId = "555",
                    PersonInfo = newStudentInfo,
                    SIF_ExtendedElements = extendedElements
                };

                try
                {
                    StudentPersonal retrievedNewStudent = consumer.Create(newStudent, true);

                    if (Log.IsInfoEnabled)
                        Log.Info($"Created new student {newStudent.PersonInfo.Name.GivenName} {newStudent.PersonInfo.Name.FamilyName} with ID of {retrievedNewStudent.RefId}.");
                }
                catch (UnauthorizedAccessException)
                {
                    if (Log.IsInfoEnabled) Log.Info("Access to create a new student is rejected.");
                }

                // Create multiple new students.

                // TODO Uncomment once the ASP.NET Core Provider has been fully implemented.
                //if (Log.IsInfoEnabled) Log.Info("*** Create multiple new students.");

                //List<StudentPersonal> newStudents = CreateStudents(5);

                //try
                //{
                //    MultipleCreateResponse multipleCreateResponse = consumer.Create(newStudents);
                //    var count = 0;

                //    foreach (CreateStatus status in multipleCreateResponse.StatusRecords)
                //    {
                //        if (Log.IsInfoEnabled) Log.Info("Create status code is " + status.StatusCode);

                //        newStudents[count++].RefId = status.Id;
                //    }

                //    // Update multiple students.

                //    if (Log.IsInfoEnabled) Log.Info("*** Update multiple students.");

                //    foreach (StudentPersonal student in newStudents)
                //    {
                //        student.PersonInfo.Name.GivenName += "o";
                //    }

                //    try
                //    {
                //        MultipleUpdateResponse multipleUpdateResponse = consumer.Update(newStudents);

                //        foreach (UpdateStatus status in multipleUpdateResponse.StatusRecords)
                //        {
                //            if (Log.IsInfoEnabled) Log.Info("Update status code is " + status.StatusCode);
                //        }
                //    }
                //    catch (UnauthorizedAccessException)
                //    {
                //        if (Log.IsInfoEnabled) Log.Info("Access to update multiple students is rejected.");
                //    }

                //    // Delete multiple students.

                //    if (Log.IsInfoEnabled) Log.Info("*** Delete multiple students.");

                //    ICollection<string> refIds =
                //        multipleCreateResponse.StatusRecords.Select(status => status.Id).ToList();

                //    try
                //    {
                //        MultipleDeleteResponse multipleDeleteResponse = consumer.Delete(refIds);

                //        foreach (DeleteStatus status in multipleDeleteResponse.StatusRecords)
                //        {
                //            if (Log.IsInfoEnabled) Log.Info("Delete status code is " + status.StatusCode);
                //        }
                //    }
                //    catch (UnauthorizedAccessException)
                //    {
                //        if (Log.IsInfoEnabled) Log.Info("Access to delete multiple students is rejected.");
                //    }
                //}
                //catch (UnauthorizedAccessException)
                //{
                //    if (Log.IsInfoEnabled) Log.Info("Access to create multiple new students is rejected.");
                //}

                // Retrieve all students from zone "Gov" and context "Curr".

                if (Log.IsInfoEnabled) Log.Info("*** Retrieve all students from zone \"Gov\" and context \"Curr\".");

                // TODO Uncomment once the ASP.NET Core Provider has been fully implemented.
                //IEnumerable<StudentPersonal> students = consumer.Query(zoneId: "Gov", contextId: "Curr");
                IEnumerable<StudentPersonal> students = consumer.Query();

                foreach (StudentPersonal student in students)
                {
                    if (Log.IsInfoEnabled)
                        Log.Info($"Student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
                }

                if (students.Count() > 1)
                {
                    // Retrieve a single student.

                    if (Log.IsInfoEnabled) Log.Info("*** Retrieve a single student.");

                    string studentId = students.ElementAt(1).RefId;
                    StudentPersonal secondStudent = consumer.Query(studentId);
                    if (Log.IsInfoEnabled)
                        Log.Info($"Name of second student is {secondStudent.PersonInfo.Name.GivenName} {secondStudent.PersonInfo.Name.FamilyName}.");

                    // Update that student and confirm.

                    if (Log.IsInfoEnabled) Log.Info("*** Update that student and confirm.");

                    secondStudent.PersonInfo.Name.GivenName = "Homer";
                    secondStudent.PersonInfo.Name.FamilyName = "Simpson";

                    try
                    {
                        consumer.Update(secondStudent);
                        secondStudent = consumer.Query(studentId);

                        if (Log.IsInfoEnabled)
                            Log.Info($"Name of second student has been changed to {secondStudent.PersonInfo.Name.GivenName} {secondStudent.PersonInfo.Name.FamilyName}.");
                    }
                    catch (UnauthorizedAccessException)
                    {
                        if (Log.IsInfoEnabled) Log.Info("Access to update a student is rejected.");
                    }

                    // Delete that student and confirm.

                    if (Log.IsInfoEnabled) Log.Info("*** Delete that student and confirm.");

                    try
                    {
                        consumer.Delete(studentId);
                        StudentPersonal deletedStudent = consumer.Query(studentId);
                        bool studentDeleted = deletedStudent == null;

                        if (studentDeleted)
                        {
                            if (Log.IsInfoEnabled)
                                Log.Info($"Student {secondStudent.PersonInfo.Name.GivenName} {secondStudent.PersonInfo.Name.FamilyName} was successfully deleted.");
                        }
                        else
                        {
                            if (Log.IsInfoEnabled)
                                Log.Info($"Student {secondStudent.PersonInfo.Name.GivenName} {secondStudent.PersonInfo.Name.FamilyName} was NOT deleted.");
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        if (Log.IsInfoEnabled) Log.Info("Access to delete a student is rejected.");
                    }
                }

                // Retrieve students based on Teaching Group using Service Paths.

                // TODO Uncomment once the ASP.NET Core Provider has been fully implemented.
                //if (Log.IsInfoEnabled) Log.Info("*** Retrieve students based on Teaching Group using Service Paths.");

                //var condition = new EqualCondition()
                //{
                //    Left = "TeachingGroups",
                //    Right = "597ad3fe-47e7-4b2c-b919-a93c564d19d0"
                //};

                //IList<EqualCondition> conditions = new List<EqualCondition> { condition };

                //try
                //{
                //    IEnumerable<StudentPersonal> teachingGroupStudents = consumer.QueryByServicePath(conditions);

                //    foreach (StudentPersonal student in teachingGroupStudents)
                //    {
                //        if (Log.IsInfoEnabled)
                //            Log.Info($"Student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");

                //        if (student.SIF_ExtendedElements == null || student.SIF_ExtendedElements.Length <= 0) continue;

                //        foreach (SIF_ExtendedElementsTypeSIF_ExtendedElement element in student.SIF_ExtendedElements)
                //        {
                //            foreach (string content in element.Text)
                //            {
                //                if (Log.IsInfoEnabled) Log.Info($"Extended element text is ...\n{content}");
                //            }
                //        }
                //    }
                //}
                //catch (UnauthorizedAccessException)
                //{
                //    if (Log.IsInfoEnabled)
                //        Log.Info("Access to query students by Service Path TeachingGroups/{}/StudentPersonals is rejected.");
                //}

                // Retrieve student changes since a particular point as defined by the Changes Since marker.

                if (Log.IsInfoEnabled)
                    Log.Info("*** Retrieve student changes since a particular point as defined by the Changes Since marker.");

                string changesSinceMarker = consumer.GetChangesSinceMarker();
                IEnumerable<StudentPersonal> changedStudents =
                    consumer.QueryChangesSince(changesSinceMarker, out string nextChangesSinceMarker);

                if (Log.IsInfoEnabled)
                    Log.Info($"Iteration 1 - Student changes based on Changes Since marker - {changesSinceMarker}");

                if (changedStudents == null || !changedStudents.Any())
                {
                    if (Log.IsInfoEnabled) Log.Info("No student changes");
                }
                else
                {
                    foreach (StudentPersonal student in changedStudents)
                    {
                        if (Log.IsInfoEnabled)
                            Log.Info($"Student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
                    }
                }

                changesSinceMarker = nextChangesSinceMarker;
                changedStudents = consumer.QueryChangesSince(changesSinceMarker, out nextChangesSinceMarker);

                if (Log.IsInfoEnabled)
                    Log.Info($"Iteration 2 - Student changes based on Changes Since marker - {changesSinceMarker}");

                if (changedStudents == null || !changedStudents.Any())
                {
                    if (Log.IsInfoEnabled) Log.Info("No student changes");
                }
                else
                {
                    foreach (StudentPersonal student in changedStudents)
                    {
                        if (Log.IsInfoEnabled)
                            Log.Info($"Student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
                    }
                }

                changesSinceMarker = nextChangesSinceMarker;
                changedStudents = consumer.QueryChangesSince(changesSinceMarker, out nextChangesSinceMarker);

                if (Log.IsInfoEnabled)
                    Log.Info($"Iteration 3 - Student changes based on Changes Since marker - {changesSinceMarker}");

                if (changedStudents == null || !changedStudents.Any())
                {
                    if (Log.IsInfoEnabled) Log.Info("No student changes");
                }
                else
                {
                    foreach (StudentPersonal student in changedStudents)
                    {
                        if (Log.IsInfoEnabled)
                            Log.Info($"Student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                if (Log.IsInfoEnabled) Log.Info("Access to query students is rejected.");
            }
            catch (Exception e)
            {
                if (Log.IsErrorEnabled)
                    Log.Error(
                        $"Error running the StudentPersonal Consumer.\n{ExceptionUtils.InferErrorResponseMessage(e)}",
                        e);
            }
            finally
            {
                consumer.Unregister();

                if (Log.IsInfoEnabled) Log.Info("Unregistered the Consumer.");
            }
        }

        public static void Main()
        {
            try
            {
                SettingsSource source = SelectFrameworkConfigSource();
                RunConsumer(GetSettings(source), GetSessionService(source));
            }
            catch (Exception e)
            {
                if (Log.IsErrorEnabled)
                    Log.Error(
                        $"Error running the Student Personal CRUD Consumer.\n{ExceptionUtils.InferErrorResponseMessage(e)}",
                        e);
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }
    }
}