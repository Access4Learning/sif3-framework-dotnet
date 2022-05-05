﻿/*
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

using Microsoft.AspNetCore.Mvc;
using Sif.Framework.AspNetCore.Providers;
using Sif.Framework.Demo.AspNetCore.Provider.Models;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Service.Providers;
using Sif.Framework.Service.Sessions;

namespace Sif.Framework.Demo.AspNetCore.Provider.Controllers;

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

    [HttpPost("StudentPersonal")]
    public override IActionResult Post(
        StudentPersonal obj,
        [FromQuery] string? zoneId = null,
        [FromQuery] string? contextId = null)
    {
        return base.Post(obj, zoneId, contextId);
    }

    [HttpGet("BroadcastEvents")]
    public override IActionResult BroadcastEvents(string? zoneId = null, string? contextId = null)
    {
        return base.BroadcastEvents(zoneId, contextId);
    }
}