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

using Sif.Framework.Demo.AspNetCore.Provider.Models;
using Sif.Framework.Demo.AspNetCore.Provider.Utils;
using Sif.Framework.Model.Events;
using Sif.Specification.DataModel.Au;

namespace Sif.Framework.Demo.AspNetCore.Provider.Services;

public class StudentPersonalIterator : IEventIterator<List<StudentPersonal>>
{
    private static readonly Random Random = new();
    private static readonly IList<StudentPersonal> Students;

    private static int _studentIndex;

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
            RefId = Guid.NewGuid().ToString(),
            LocalId = Random.Next(10000, 99999).ToString(),
            PersonInfo = personInfo
        };

        return studentPersonal;
    }

    private static IList<StudentPersonal> CreateStudents(int count)
    {
        IList<StudentPersonal> students = new List<StudentPersonal>();

        for (var i = 0; i < count; i++)
        {
            StudentPersonal studentPersonal = CreateStudent();
            students.Add(studentPersonal);
        }

        return students;
    }

    static StudentPersonalIterator()
    {
        Students = CreateStudents(20);
    }

    public SifEvent<List<StudentPersonal>>? GetNext()
    {
        if (_studentIndex >= Students.Count) return null;

        var sifEvent = new SifEvent<List<StudentPersonal>>
        {
            EventAction = EventAction.UPDATE_FULL,
            Id = Guid.NewGuid(),
            SifObjects = Students.Skip(_studentIndex).Take(7).ToList()
        };

        _studentIndex += 7;

        return sifEvent;
    }

    public bool HasNext()
    {
        return _studentIndex < Students.Count;
    }
}