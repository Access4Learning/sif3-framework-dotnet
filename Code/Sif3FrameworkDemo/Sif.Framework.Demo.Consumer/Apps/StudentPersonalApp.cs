/*
 * Copyright 2022 Systemic Pty Ltd
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

using Microsoft.Extensions.Logging;
using Sif.Framework.Demo.Consumer.Consumers;
using Sif.Framework.Demo.Consumer.Models;
using Sif.Framework.Demo.Consumer.Utils;
using Sif.Framework.Models.Responses;
using Sif.Framework.Models.Settings;
using Sif.Framework.Services.Sessions;
using Sif.Framework.Utils;
using Sif.Specification.DataModel.Au;
using Tardigrade.Framework.Extensions;

namespace Sif.Framework.Demo.Consumer.Apps;

public class StudentPersonalApp
{
    private static readonly Random Random = new();

    private readonly ILogger<StudentPersonalApp> _logger;

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

    public StudentPersonalApp(ILogger<StudentPersonalApp> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void RunConsumer(IFrameworkSettings settings, ISessionService sessionService)
    {
        var consumer = new StudentPersonalConsumer(
            settings.ApplicationKey,
            settings.InstanceId,
            settings.UserToken,
            settings.SolutionId,
            settings,
            sessionService);
        consumer.Register();

        _logger.Information("Registered the Consumer.");

        try
        {
            IEnumerable<StudentPersonal> queriedStudents = consumer.DynamicQuery("[@id=1234]");

            foreach (StudentPersonal student in queriedStudents)
            {
                _logger.Information($"Queried student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
            }

            // Retrieve Bart Simpson using QBE.

            // TODO Uncomment once the ASP.NET Core Provider has been fully implemented.
            //_logger.Information("*** Retrieve Bart Simpson using QBE.");

            //var name = new NameOfRecordType { FamilyName = "Simpson", GivenName = "Bart" };
            //var personInfo = new PersonInfoType { Name = name };
            //var studentPersonal = new StudentPersonal { PersonInfo = personInfo };
            //IEnumerable<StudentPersonal> filteredStudents = consumer.QueryByExample(studentPersonal);

            //foreach (StudentPersonal student in filteredStudents)
            //{
            //    _logger.Information($"Filtered student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
            //}

            // Create a new student.

            _logger.Information("*** Create a new student.");

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

                _logger.Information($"Created new student {newStudent.PersonInfo.Name.GivenName} {newStudent.PersonInfo.Name.FamilyName} with ID of {retrievedNewStudent.RefId}.");
            }
            catch (UnauthorizedAccessException)
            {
                _logger.Information("Access to create a new student is rejected.");
            }

            // Create multiple new students.

            _logger.Information("*** Create multiple new students.");

            List<StudentPersonal> newStudents = CreateStudents(5);

            try
            {
                MultipleCreateResponse multipleCreateResponse = consumer.Create(newStudents);
                var count = 0;

                foreach (CreateStatus status in multipleCreateResponse.StatusRecords)
                {
                    _logger.Information("Create status code is " + status.StatusCode);

                    newStudents[count++].RefId = status.Id;
                }

                // Update multiple students.

                _logger.Information("*** Update multiple students.");

                foreach (StudentPersonal student in newStudents)
                {
                    student.PersonInfo.Name.GivenName += "o";
                }

                try
                {
                    MultipleUpdateResponse multipleUpdateResponse = consumer.Update(newStudents);

                    foreach (UpdateStatus status in multipleUpdateResponse.StatusRecords)
                    {
                        _logger.Information("Update status code is " + status.StatusCode);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.Information("Access to update multiple students is rejected.");
                }

                // Delete multiple students.

                _logger.Information("*** Delete multiple students.");

                ICollection<string> refIds =
                    multipleCreateResponse.StatusRecords.Select(status => status.Id).ToList();

                try
                {
                    MultipleDeleteResponse multipleDeleteResponse = consumer.Delete(refIds);

                    foreach (DeleteStatus status in multipleDeleteResponse.StatusRecords)
                    {
                        _logger.Information("Delete status code is " + status.StatusCode);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.Information("Access to delete multiple students is rejected.");
                }
            }
            catch (UnauthorizedAccessException)
            {
                _logger.Information("Access to create multiple new students is rejected.");
            }

            // Retrieve all students from zone "Gov" and context "Curr".

            _logger.Information("*** Retrieve all students from zone \"Gov\" and context \"Curr\".");

            // TODO Uncomment once the ASP.NET Core Provider has been fully implemented.
            //IEnumerable<StudentPersonal> students = consumer.Query(zoneId: "Gov", contextId: "Curr");
            IEnumerable<StudentPersonal> students = consumer.Query();

            foreach (StudentPersonal student in students)
            {
                _logger.Information($"Student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
            }

            if (students.Count() > 1)
            {
                // Retrieve a single student.

                _logger.Information("*** Retrieve a single student.");

                string studentId = students.ElementAt(1).RefId;
                StudentPersonal secondStudent = consumer.Query(studentId);
                _logger.Information($"Name of second student is {secondStudent.PersonInfo.Name.GivenName} {secondStudent.PersonInfo.Name.FamilyName}.");

                // Update that student and confirm.

                _logger.Information("*** Update that student and confirm.");

                secondStudent.PersonInfo.Name.GivenName = "Homer";
                secondStudent.PersonInfo.Name.FamilyName = "Simpson";

                try
                {
                    consumer.Update(secondStudent);
                    secondStudent = consumer.Query(studentId);

                    _logger.Information($"Name of second student has been changed to {secondStudent.PersonInfo.Name.GivenName} {secondStudent.PersonInfo.Name.FamilyName}.");
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.Information("Access to update a student is rejected.");
                }

                // Delete that student and confirm.

                _logger.Information("*** Delete that student and confirm.");

                try
                {
                    consumer.Delete(studentId);
                    StudentPersonal deletedStudent = consumer.Query(studentId);
                    bool studentDeleted = deletedStudent == null;

                    _logger.Information($"Student {secondStudent.PersonInfo.Name.GivenName} {secondStudent.PersonInfo.Name.FamilyName} was {(studentDeleted ? "successfully" : "NOT")} deleted.");
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.Information("Access to delete a student is rejected.");
                }
            }

            // Retrieve students based on Teaching Group using Service Paths.

            // TODO Uncomment once the ASP.NET Core Provider has been fully implemented.
            //_logger.Information("*** Retrieve students based on Teaching Group using Service Paths.");

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
            //        _logger.Information($"Student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");

            //        if (student.SIF_ExtendedElements == null || student.SIF_ExtendedElements.Length <= 0) continue;

            //        foreach (SIF_ExtendedElementsTypeSIF_ExtendedElement element in student.SIF_ExtendedElements)
            //        {
            //            foreach (string content in element.Text)
            //            {
            //                _logger.Information($"Extended element text is ...\n{content}");
            //            }
            //        }
            //    }
            //}
            //catch (UnauthorizedAccessException)
            //{
            //    _logger.Information("Access to query students by Service Path TeachingGroups/{}/StudentPersonals is rejected.");
            //}

            // Retrieve student changes since a particular point as defined by the Changes Since marker.

            _logger.Information("*** Retrieve student changes since a particular point as defined by the Changes Since marker.");

            string changesSinceMarker = consumer.GetChangesSinceMarker();
            IEnumerable<StudentPersonal> changedStudents =
                consumer.QueryChangesSince(changesSinceMarker, out string nextChangesSinceMarker);

            _logger.Information($"Iteration 1 - Student changes based on Changes Since marker - {changesSinceMarker}");

            if (changedStudents == null || !changedStudents.Any())
            {
                _logger.Information("No student changes");
            }
            else
            {
                foreach (StudentPersonal student in changedStudents)
                {
                    _logger.Information($"Student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
                }
            }

            changesSinceMarker = nextChangesSinceMarker;
            changedStudents = consumer.QueryChangesSince(changesSinceMarker, out nextChangesSinceMarker);

            _logger.Information($"Iteration 2 - Student changes based on Changes Since marker - {changesSinceMarker}");

            if (changedStudents == null || !changedStudents.Any())
            {
                _logger.Information("No student changes");
            }
            else
            {
                foreach (StudentPersonal student in changedStudents)
                {
                    _logger.Information($"Student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
                }
            }

            changesSinceMarker = nextChangesSinceMarker;
            changedStudents = consumer.QueryChangesSince(changesSinceMarker, out nextChangesSinceMarker);

            _logger.Information($"Iteration 3 - Student changes based on Changes Since marker - {changesSinceMarker}");

            if (changedStudents == null || !changedStudents.Any())
            {
                _logger.Information("No student changes");
            }
            else
            {
                foreach (StudentPersonal student in changedStudents)
                {
                    _logger.Information($"Student name is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            _logger.Information("Access to query students is rejected.");
        }
        catch (Exception e)
        {
            _logger.Error(
                $"Error running the StudentPersonal Consumer.\n{ExceptionUtils.InferErrorResponseMessage(e)}",
                e);
        }
        finally
        {
            consumer.Unregister();

            _logger.Information("Unregistered the Consumer.");
        }
    }
}