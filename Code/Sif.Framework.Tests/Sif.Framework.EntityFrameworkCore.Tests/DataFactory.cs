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
    public static ApplicationInfo ApplicationInfo => new()
    {
        AdapterProduct = ProductIdentityAdapterProduct,
        ApplicationKey = "Sif3DemoConsumer",
        ApplicationProduct = ProductIdentityApplicationProduct,
        DataModelNamespace = "http://www.sifassociation.org/datamodel/au/3.4",
        SupportedInfrastructureVersion = "3.2.1",
        Transport = "REST"
    };

    public static InfrastructureService InfrastructureServiceEnvironment => new()
    {
        Name = "environment",
        Value = "http://localhost:62921/api/environments/31515b87-0e9e-4804-b09e-174b262b898a"
    };

    public static ProductIdentity ProductIdentityAdapterProduct => new()
    {
        IconUri = "https://cdn.sif.com/icons/adapters",
        ProductName = "Framework Adapters",
        ProductVersion = "10.4.3",
        VendorName = "Systemic"
    };

    public static ProductIdentity ProductIdentityApplicationProduct => new()
    {
        IconUri = "https://cdn.sif.com/icons/applications",
        ProductName = "Framework Applications",
        ProductVersion = "5.7.0",
        VendorName = "Systemic"
    };

    public static Property Property => new()
    {
        Name = "lang",
        Value = "en_AU"
    };

    public static ProvisionedZone ProvisionedZone => new()
    {
        Services = new List<Models.Infrastructure.Service> { ServiceSchool, ServiceStudent },
        SifId = "Sif3DemoZone1"
    };

    public static Right RightQuery => new() { Type = "QUERY", Value = "APPROVED" };

    public static ICollection<Right> Rights => new List<Right>
    {
        new() { Type = "CREATE", Value = "APPROVED" },
        new() { Type = "DELETE", Value = "REJECTED" },
        new() { Type = "QUERY", Value = "APPROVED" },
        new() { Type = "UPDATE", Value = "APPROVED" }
    };

    public static Models.Infrastructure.Service ServiceSchool => new()
    {
        ContextId = "DEFAULT",
        Name = "SchoolInfos",
        Rights = Rights,
        Type = "OBJECT"
    };

    public static Models.Infrastructure.Service ServiceStudent => new()
    {
        ContextId = "DEFAULT",
        Name = "StudentPersonals",
        Rights = Rights,
        Type = "OBJECT"
    };
}