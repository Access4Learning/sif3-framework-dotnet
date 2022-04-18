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

using NHibernate.Tool.hbm2ddl;
using Sif.Framework.Model.Infrastructure;
using System;
using System.Collections.Generic;
using Configuration = NHibernate.Cfg.Configuration;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Tests
{
    /// <summary>
    /// This class is used to generate test data for unit testing.
    /// </summary>
    public static class DataFactory
    {
        /// <summary>
        /// Generate the database schema, apply it to the database and save the DDL used into a file.
        /// </summary>
        public static void CreateDatabase()
        {
            var configuration = new Configuration();
            configuration.Configure();
            var schemaExport = new SchemaExport(configuration);
            schemaExport.SetOutputFile("SQLite database schema.ddl");
            schemaExport.Create(true, true);
        }

        /// <summary>
        /// Create an Environment request populated with dummy data.
        /// </summary>
        /// <returns>Environment request.</returns>
        public static Environment CreateEnvironmentRequest()
        {
            var adapterProduct = new ProductIdentity
            {
                IconUri = "icon url 1",
                ProductName = "product name 1",
                ProductVersion = "3.4.1",
                VendorName = "Systemic"
            };

            var applicationProduct = new ProductIdentity
            {
                IconUri = "icon url 2",
                ProductName = "product name 2",
                ProductVersion = "9.80",
                VendorName = "Systemic"
            };

            var zoneCharge = new Property { Name = "charge", Value = "Negative" };
            var zoneMaster = new Property { Name = "master", Value = "Annihilus" };
            ICollection<Property> zoneProperties = new List<Property> { zoneCharge, zoneMaster };
            var theNegativeZone = new Zone { Description = "The Negative Zone", Properties = zoneProperties };

            var applicationInfo = new ApplicationInfo
            {
                AdapterProduct = adapterProduct,
                ApplicationKey = "UnitTesting",
                ApplicationProduct = applicationProduct,
                DataModelNamespace = "http://www.sifassociation.org/datamodel/au/3.4",
                SupportedInfrastructureVersion = "3.2",
                Transport = "REST"
            };

            var environmentRequest = new Environment
            {
                ApplicationInfo = applicationInfo,
                AuthenticationMethod = "Basic",
                ConsumerName = "UnitTestConsumer",
                DefaultZone = theNegativeZone,
                InstanceId = "ThisInstance01",
                SessionToken = Guid.NewGuid().ToString(),
                SolutionId = "auTestSolution",
                Type = EnvironmentType.DIRECT,
                UserToken = "UserToken01"
            };

            return environmentRequest;
        }

        /// <summary>
        /// Create an Environment response populated with dummy data.
        /// </summary>
        /// <returns>Environment response.</returns>
        public static Environment CreateEnvironmentResponse()
        {
            var environmentUrl = new InfrastructureService
            {
                Name = InfrastructureServiceNames.environment,
                Value = "http://rest3api.sifassociation.org/api/solutions/auTestSolution/environments/5b72f2d4-7a83-4297-a71f-8b5fb26cbf14"
            };

            var provisionRequestsUrl = new InfrastructureService
            {
                Name = InfrastructureServiceNames.provisionRequests,
                Value = "http://rest3api.sifassociation.org/api/solutions/auTestSolution/provisionRequests"
            };

            var queuesUrl = new InfrastructureService
            {
                Name = InfrastructureServiceNames.queues,
                Value = "http://rest3api.sifassociation.org/api/solutions/auTestSolution/queues"
            };

            var requestsConnectorUrl = new InfrastructureService
            {
                Name = InfrastructureServiceNames.requestsConnector,
                Value = "http://rest3api.sifassociation.org/api/solutions/auTestSolution/requestsConnector"
            };

            var subscriptionsUrl = new InfrastructureService
            {
                Name = InfrastructureServiceNames.subscriptions,
                Value = "http://rest3api.sifassociation.org/api/solutions/auTestSolution/subscriptions"
            };

            ICollection<InfrastructureService> infrastructureServices = new List<InfrastructureService>
            {
                environmentUrl,
                provisionRequestsUrl,
                queuesUrl,
                requestsConnectorUrl,
                subscriptionsUrl
            };

            var adminRight = new Right { Type = RightType.ADMIN.ToString(), Value = RightValue.APPROVED.ToString() };
            var createRight = new Right { Type = RightType.CREATE.ToString(), Value = RightValue.APPROVED.ToString() };
            ICollection<Right> rights = new List<Right> { adminRight, createRight };

            var studentPersonalsService = new Model.Infrastructure.Service
            {
                ContextId = "DEFAULT",
                Name = "StudentPersonals",
                Rights = rights,
                Type = "OBJECT"
            };

            var schoolInfosService = new Model.Infrastructure.Service
            {
                ContextId = "DEFAULT",
                Name = "SchoolInfos",
                Rights = rights,
                Type = "OBJECT"
            };

            ICollection<Model.Infrastructure.Service> services = new List<Model.Infrastructure.Service>
            {
                studentPersonalsService,
                schoolInfosService
            };

            var schoolZone = new ProvisionedZone { SifId = "auSchoolTestingZone", Services = services };
            var studentZone = new ProvisionedZone { SifId = "auStudentTestingZone", Services = services };

            IDictionary<string, ProvisionedZone> provisionedZones = new SortedDictionary<string, ProvisionedZone>
            {
                { schoolZone.SifId, schoolZone },
                { studentZone.SifId, studentZone }
            };

            Environment environmentResponse = CreateEnvironmentRequest();
            environmentResponse.InfrastructureServices = infrastructureServices;
            environmentResponse.ProvisionedZones = provisionedZones;

            return environmentResponse;
        }
    }
}