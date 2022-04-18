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

    public static ApplicationRegister ApplicationRegister => new()
    {
        ApplicationKey = "Sif3DemoConsumer",
        EnvironmentRegisters = new List<EnvironmentRegister> { EnvironmentRegister },
        SharedSecret = "SecretDem0"
    };

    public static Environment Environment => new()
    {
        ApplicationInfo = ApplicationInfo,
        AuthenticationMethod = "Basic",
        ConsumerName = "TemplateDemoConsumer",
        DefaultZone = Zone,
        InfrastructureServices = InfrastructureServices,
        InstanceId = "device-037",
        ProvisionedZones = new List<ProvisionedZone> { ProvisionedZone },
        SessionToken = "U2lmM0RlbW9Db25zdW1lcjo6OlNpZjNGcmFtZXdvcms=",
        SolutionId = "Sif3Framework",
        Type = EnvironmentType.DIRECT,
        UserToken = "6621a7d1-a66d-419b-8d3f-80c4a305ad83"
    };

    public static EnvironmentRegister EnvironmentRegister => new()
    {
        ApplicationKey = "Sif3DemoConsumer",
        DefaultZone = Zone,
        InfrastructureServices = InfrastructureServices,
        InstanceId = "device-044",
        ProvisionedZones = new List<ProvisionedZone> { ProvisionedZone },
        SolutionId = "Sif3Framework",
        UserToken = "6621a7d1-a66d-419b-8d3f-80c4a305ad83"
    };

    public static InfrastructureService InfrastructureServiceEnvironment => new()
    {
        Name = InfrastructureServiceNames.environment,
        Value = "http://localhost:62921/api/environments/31515b87-0e9e-4804-b09e-174b262b898a"
    };

    public static ICollection<InfrastructureService> InfrastructureServices => new List<InfrastructureService>
    {
        InfrastructureServiceEnvironment,
        new() { Name = InfrastructureServiceNames.provisionRequests, Value = "http://localhost:62921/api/provision" },
        new() { Name = InfrastructureServiceNames.requestsConnector, Value = "http://localhost:50617/api" },
        new() { Name = InfrastructureServiceNames.servicesConnector, Value = "http://localhost:50617/services" },
        new() { Name = InfrastructureServiceNames.eventsConnector, Value = "http://localhost:62837/api" },
        new() { Name = InfrastructureServiceNames.queues, Value = "http://localhost:59586/api/queues" },
        new() { Name = InfrastructureServiceNames.subscriptions, Value = "http://localhost:59586/api/subscriptions" }
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

    public static Property PropertyLang => new()
    {
        Name = "lang",
        Value = "en_AU"
    };

    public static Property PropertyLocale => new()
    {
        Name = "locale",
        Value = "AU"
    };

    public static ProvisionedZone ProvisionedZone => new()
    {
        Services = new List<Models.Infrastructure.Service> { ServiceSchool, ServiceStudent },
        SifId = "Sif3DemoZone1"
    };

    public static Right RightQuery => new() { Type = "QUERY", Value = "APPROVED" };

    public static ICollection<Right> Rights => new List<Right>
    {
        RightQuery,
        new() { Type = "CREATE", Value = "APPROVED" },
        new() { Type = "DELETE", Value = "REJECTED" },
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

    public static Zone Zone => new()
    {
        Description = "SIF3 demo default zone",
        Properties = new List<Property> { PropertyLang, PropertyLocale },
        SifId = "Sif3DemoZone1"
    };
}