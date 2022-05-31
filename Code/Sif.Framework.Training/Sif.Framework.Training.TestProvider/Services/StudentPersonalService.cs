using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Query;
using Sif.Framework.Service.Providers;
using Sif.Framework.Training.TestProvider.Models;
using Sif.Specification.DataModel.Au;

namespace Sif.Framework.Training.TestProvider.Services;

public class StudentPersonalService : IBasicProviderService<StudentPersonal>
{
    private static StudentPersonal CreateStudent()
    {
        var name = new NameOfRecordType
        {
            Type = NameOfRecordTypeType.LGL,
            FamilyName = "Simpson",
            GivenName = "Homer"
        };

        var emails = new EmailType[]
        {
            new() { Type = AUCodeSetsEmailTypeType.Item02, Value = $"{name.GivenName}@gmail.com" }
        };

        var personInfo = new PersonInfoType { Name = name, EmailList = emails };

        var studentPersonal = new StudentPersonal
        {
            RefId = Guid.NewGuid().ToString(),
            LocalId = "99999",
            PersonInfo = personInfo
        };

        return studentPersonal;
    }

    public StudentPersonal Create(StudentPersonal obj, bool? mustUseAdvisory = null, string? zoneId = null,
        string? contextId = null, params RequestParameter[] requestParameters)
    {
        throw new NotImplementedException();
    }

    public StudentPersonal Retrieve(string refId, string? zoneId = null, string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        return CreateStudent();
    }

    public List<StudentPersonal> Retrieve(uint? pageIndex = null, uint? pageSize = null, string? zoneId = null,
        string? contextId = null, params RequestParameter[] requestParameters)
    {
        return new List<StudentPersonal> { CreateStudent() };
    }

    public List<StudentPersonal> Retrieve(IEnumerable<EqualCondition> conditions, uint? pageIndex = null,
        uint? pageSize = null, string? zoneId = null, string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        return new List<StudentPersonal> { CreateStudent() };
    }

    public List<StudentPersonal> Retrieve(StudentPersonal obj, uint? pageIndex = null, uint? pageSize = null,
        string? zoneId = null, string? contextId = null, params RequestParameter[] requestParameters)
    {
        return new List<StudentPersonal> { CreateStudent() };
    }

    public void Update(StudentPersonal obj, string? zoneId = null, string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        throw new NotImplementedException();
    }

    public void Delete(string refId, string? zoneId = null, string? contextId = null,
        params RequestParameter[] requestParameters)
    {
        throw new NotImplementedException();
    }
}