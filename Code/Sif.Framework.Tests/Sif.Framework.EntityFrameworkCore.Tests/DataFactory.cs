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

using Sif.Framework.EntityFrameworkCore.Tests.Models.Infrastructure;
using System.Collections.Generic;

namespace Sif.Framework.EntityFrameworkCore.Tests;

public static class DataFactory
{
    public static ProvisionedZone ProvisionedZone => new()
    {
        Services = new List<Models.Infrastructure.Service> { SchoolService, StudentService },
        SifId = "Sif3DemoZone1"
    };

    public static Right QueryRight => new() { Type = "QUERY", Value = "APPROVED" };

    public static ICollection<Right> Rights => new List<Right>
    {
        new() { Type = "CREATE", Value = "APPROVED" },
        new() { Type = "DELETE", Value = "REJECTED" },
        new() { Type = "QUERY", Value = "APPROVED" },
        new() { Type = "UPDATE", Value = "APPROVED" }
    };

    public static Models.Infrastructure.Service SchoolService => new()
    {
        ContextId = "DEFAULT",
        Name = "SchoolInfos",
        Rights = Rights,
        Type = "OBJECT"
    };

    public static Models.Infrastructure.Service StudentService => new()
    {
        ContextId = "DEFAULT",
        Name = "StudentPersonals",
        Rights = Rights,
        Type = "OBJECT"
    };
}