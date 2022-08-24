using Sif.Framework.Consumers;
using Sif.Framework.Models.Settings;
using Sif.Framework.Services.Sessions;
using Sif.Framework.Training.TestConsumer.Models;

namespace Sif.Framework.Training.TestConsumer.Consumers;

public class StudentPersonalConsumer : BasicConsumer<StudentPersonal>
{
    public StudentPersonalConsumer(
        string applicationKey,
        string? instanceId = null,
        string? userToken = null,
        string? solutionId = null,
        IFrameworkSettings? settings = null,
        ISessionService? sessionService = null)
        : base(applicationKey, instanceId, userToken, solutionId, settings, sessionService)
    {
    }
}