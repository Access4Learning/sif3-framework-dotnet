using Microsoft.AspNetCore.Mvc;
using Sif.Framework.AspNetCore.Providers;
using Sif.Framework.Models.Settings;
using Sif.Framework.Services.Infrastructure;
using Sif.Framework.Services.Providers;
using Sif.Framework.Services.Sessions;
using Sif.Framework.Training.TestProvider.Models;

namespace Sif.Framework.Training.TestProvider.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentPersonalsController : BasicProvider<StudentPersonal>
{
    public StudentPersonalsController(
        IBasicProviderService<StudentPersonal> service,
        IApplicationRegisterService applicationRegisterService,
        IEnvironmentService environmentService,
        IFrameworkSettings? settings = null,
        ISessionService? sessionService = null)
        : base(service, applicationRegisterService, environmentService, settings, sessionService)
    {
    }

    [NonAction]
    public override IActionResult BroadcastEvents(string? zoneId = null, string? contextId = null)
    {
        return base.BroadcastEvents(zoneId, contextId);
    }

    [HttpPost("StudentPersonal")]
    public override IActionResult Post(
        StudentPersonal obj,
        [FromQuery] string? zoneId = null,
        [FromQuery] string? contextId = null)
    {
        return base.Post(obj, zoneId, contextId);
    }
}