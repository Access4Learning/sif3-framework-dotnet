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

using Sif.Framework.Demo.Provider.Models;
using Sif.Framework.Demo.Provider.Utils;
using Sif.Framework.Models.Events;
using Sif.Framework.Models.Parameters;
using Sif.Framework.Models.Query;
using Sif.Framework.Services.Providers;
using Sif.Specification.DataModel.Au;

namespace Sif.Framework.Demo.Provider.Services;

public class StudentPersonalService :
    IBasicProviderService<StudentPersonal>,
    IChangesSinceService<List<StudentPersonal>>,
    IEventService<List<StudentPersonal>>
{
    private const string ChangesSincePrefix = "ver.";

    private static readonly Random Random = new();
    private static readonly IDictionary<string, StudentPersonal> StudentsCache;

    private static readonly IDictionary<string, IDictionary<string, StudentPersonal>> StudentsChangedCache =
        new Dictionary<string, IDictionary<string, StudentPersonal>>();

    private static int _changesSinceNumber = 1;

    public string ChangesSinceMarker => $"{ChangesSincePrefix}{_changesSinceNumber}";

    public string NextChangesSinceMarker
    {
        get
        {
            _changesSinceNumber++;

            return $"{ChangesSincePrefix}{_changesSinceNumber}";
        }
    }

    private static StudentPersonal CreateBartSimpson()
    {
        var text = new[]
        {
            @"<MedicalCondition>
                    <ConditionID>Unique Medical Condition ID</ConditionID>
                    <Condition>Condition</Condition>
                    <Severity>Condition Severity</Severity>
                    <Details>Condition Details</Details>
                </MedicalCondition>"
        };

        var extendedElement =
            new SIF_ExtendedElementsTypeSIF_ExtendedElement { Name = "MedicalConditions", Text = text };
        SIF_ExtendedElementsTypeSIF_ExtendedElement[] extendedElements = { extendedElement };
        var name = new NameOfRecordType { Type = NameOfRecordTypeType.LGL, FamilyName = "Simpson", GivenName = "Bart" };
        var personInfo = new PersonInfoType { Name = name };

        var studentPersonal = new StudentPersonal
        {
            RefId = Guid.NewGuid().ToString(),
            LocalId = "666",
            PersonInfo = personInfo,
            SIF_ExtendedElements = extendedElements
        };

        return studentPersonal;
    }

    private static StudentPersonal CreateStudent()
    {
        var name = new NameOfRecordType
        {
            Type = NameOfRecordTypeType.LGL,
            FamilyName = RandomNameGenerator.FamilyName,
            GivenName = RandomNameGenerator.GivenName
        };

        var emails = new EmailType[]
        {
            new() { Type = AUCodeSetsEmailTypeType.Item01, Value = $"{name.GivenName}01@gmail.com" },
            new() { Type = AUCodeSetsEmailTypeType.Item02, Value = $"{name.GivenName}02@gmail.com" }
        };

        var personInfo = new PersonInfoType { Name = name, EmailList = emails };

        var studentPersonal = new StudentPersonal
        {
            RefId = Guid.NewGuid().ToString(),
            LocalId = Random.Next(10000, 99999).ToString(),
            PersonInfo = personInfo
        };

        return studentPersonal;
    }

    private static IDictionary<string, StudentPersonal> CreateStudents(int count)
    {
        IDictionary<string, StudentPersonal> studentPersonalsCache = new Dictionary<string, StudentPersonal>();

        if (count <= 0) return studentPersonalsCache;

        StudentPersonal bartSimpson = CreateBartSimpson();
        studentPersonalsCache.Add(bartSimpson.RefId, bartSimpson);

        for (var i = 2; i <= count; i++)
        {
            StudentPersonal studentPersonal = CreateStudent();
            studentPersonalsCache.Add(studentPersonal.RefId, studentPersonal);
        }

        return studentPersonalsCache;
    }

    static StudentPersonalService()
    {
        StudentsCache = CreateStudents(20);
        StudentsChangedCache.Add("ver.1", CreateStudents(10));
        StudentsChangedCache.Add("ver.2", CreateStudents(3));
    }

    public StudentPersonal Create(
        StudentPersonal obj,
        bool? mustUseAdvisory = null,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        if (!mustUseAdvisory.HasValue || !mustUseAdvisory.Value)
        {
            var refId = Guid.NewGuid().ToString();
            obj.RefId = refId;
        }

        StudentsCache.Add(obj.RefId, obj);

        return obj;
    }

    public StudentPersonal Retrieve(
        string refId,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        if (!StudentsCache.TryGetValue(refId, out StudentPersonal? student))
        {
            student = null;
        }

        return student!;
    }

    public List<StudentPersonal> Retrieve(
        uint? pageIndex = null,
        uint? pageSize = null,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        var retrievedStudents = new List<StudentPersonal>();

        if ((zoneId != null || contextId != null) && (!"Gov".Equals(zoneId) || !"Curr".Equals(contextId)))
            return retrievedStudents;

        var allStudents = new List<StudentPersonal>();

        if (requestParameters.Any(p => RequestParameterType.where.ToString().Equals(p.Name)))
        {
            allStudents.Add(StudentsCache.Values.FirstOrDefault()!);
        }
        else
        {
            allStudents.AddRange(StudentsCache.Values);
        }

        if (pageIndex.HasValue && pageSize.HasValue)
        {
            uint index = pageIndex.Value * pageSize.Value;
            uint count = pageSize.Value;

            if (StudentsCache.Values.Count < index + count)
            {
                count = (uint)StudentsCache.Values.Count - index;
            }
            else
            {
                count = pageSize.Value;
            }

            if (index <= StudentsCache.Values.Count)
            {
                retrievedStudents = allStudents.GetRange((int)index, (int)count);
            }
        }
        else
        {
            retrievedStudents = allStudents;
        }

        return retrievedStudents;
    }

    public List<StudentPersonal> Retrieve(
        StudentPersonal obj,
        uint? pageIndex = null,
        uint? pageSize = null,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        return StudentsCache.Values
            .Where(student =>
                student.PersonInfo.Name.FamilyName.Equals(obj.PersonInfo.Name.FamilyName) &&
                student.PersonInfo.Name.GivenName.Equals(obj.PersonInfo.Name.GivenName))
            .ToList();
    }

    public List<StudentPersonal> Retrieve(
        IEnumerable<EqualCondition> conditions,
        uint? pageIndex = null,
        uint? pageSize = null,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        var students = new List<StudentPersonal>
        {
            CreateBartSimpson()
        };

        return students;
    }

    public List<StudentPersonal> RetrieveChangesSince(
        string changesSinceMarker,
        uint? pageIndex = null,
        uint? pageSize = null,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        if (string.IsNullOrEmpty(changesSinceMarker))
            throw new ArgumentException("changesSinceMarker parameter is null or empty.", nameof(changesSinceMarker));

        if (!StudentsChangedCache.TryGetValue(changesSinceMarker, out IDictionary<string, StudentPersonal>? students))
        {
            students = new Dictionary<string, StudentPersonal>();
        }

        return new List<StudentPersonal>(students.Values);
    }

    public void Update(
        StudentPersonal obj,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        if (StudentsCache.ContainsKey(obj.RefId))
        {
            StudentsCache.Remove(obj.RefId);
            StudentsCache.Add(obj.RefId, obj);
        }
    }

    public void Delete(
        string refId,
        string? zoneId = null,
        string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        StudentsCache.Remove(refId);
    }

    public IEventIterator<List<StudentPersonal>> GetEventIterator(string? zoneId = null, string? contextId = null)
    {
        return new StudentPersonalIterator();
    }
}