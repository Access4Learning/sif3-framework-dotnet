/*
 * Copyright 2015 Systemic Pty Ltd
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
using System.Collections.Generic;
using Configuration = NHibernate.Cfg.Configuration;

namespace Sif.Framework.Model.Infrastructure
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
            Configuration configuration = new Configuration();
            configuration.Configure();
            SchemaExport schemaExport = new SchemaExport(configuration);
            schemaExport.SetOutputFile("SQLite database schema.ddl");
            schemaExport.Create(true, true);
        }

        public static Environment CreateEnvironmentRequest()
        {
            ProductIdentity adapterProduct = new ProductIdentity { IconURI = "icon url 1", ProductName = "product name 1", ProductVersion = "3.4.1", VendorName = "Systemic" };
            ProductIdentity applicationProduct = new ProductIdentity { IconURI = "icon url 2", ProductName = "product name 2", ProductVersion = "9.80", VendorName = "Systemic" };
            Property zoneCharge = new Property { Name = "charge", Value = "Negative" };
            Property zoneMaster = new Property { Name = "master", Value = "Annihilus" };
            IDictionary<string, Property> zoneProperties =
                new Dictionary<string, Property> { { zoneCharge.Name, zoneCharge }, { zoneMaster.Name, zoneMaster } };
            Zone theNegativeZone = new Zone
            {
                Description = "The Negative Zone",
                Properties = zoneProperties
            };
            ApplicationInfo applicationInfo = new ApplicationInfo
            {
                AdapterProduct = adapterProduct,
                ApplicationKey = "UnitTesting",
                ApplicationProduct = applicationProduct,
                DataModelNamespace = "http://www.sifassociation.org/au/datamodel/1.4",
                SupportedInfrastructureVersion = "3.0",
                Transport = "REST"
            };
            Environment environmentRequest = new Environment
            {
                ApplicationInfo = applicationInfo,
                AuthenticationMethod = "Basic",
                ConsumerName = "UnitTestConsumer",
                DefaultZone = theNegativeZone,
                InstanceId = "ThisInstance01",
                SessionToken = "2e5dd3ca282fc8ddb3d08dcacc407e8a",
                SolutionId = "auTestSolution",
                Type = Model.Infrastructure.EnvironmentType.DIRECT,
                UserToken = "UserToken01"
            };

            return environmentRequest;
        }

        public static Environment CreateEnvironmentResponse()
        {
            InfrastructureService environmentURL = new InfrastructureService { Name = InfrastructureServiceNames.environment, Value = "http://rest3api.sifassociation.org/api/solutions/auTestSolution/environments/5b72f2d4-7a83-4297-a71f-8b5fb26cbf14" };
            InfrastructureService provisionRequestsURL = new InfrastructureService { Name = InfrastructureServiceNames.provisionRequests, Value = "http://rest3api.sifassociation.org/api/solutions/auTestSolution/provisionRequests" };
            InfrastructureService queuesURL = new InfrastructureService { Name = InfrastructureServiceNames.queues, Value = "http://rest3api.sifassociation.org/api/solutions/auTestSolution/queues" };
            InfrastructureService requestsConnectorURL = new InfrastructureService { Name = InfrastructureServiceNames.requestsConnector, Value = "http://rest3api.sifassociation.org/api/solutions/auTestSolution/requestsConnector" };
            InfrastructureService subscriptionsURL = new InfrastructureService { Name = InfrastructureServiceNames.subscriptions, Value = "http://rest3api.sifassociation.org/api/solutions/auTestSolution/subscriptions" };
            IDictionary<InfrastructureServiceNames, InfrastructureService> infrastructureServices = new Dictionary<InfrastructureServiceNames, InfrastructureService>
            {
                { environmentURL.Name, environmentURL },
                { provisionRequestsURL.Name, provisionRequestsURL },
                { queuesURL.Name, queuesURL },
                { requestsConnectorURL.Name, requestsConnectorURL },
                { subscriptionsURL.Name, subscriptionsURL }
            };

            Right adminRight = new Right() { Type = RightType.ADMIN.ToString(), Value = RightValue.APPROVED.ToString() };
            Right createRight = new Right() { Type = RightType.CREATE.ToString(), Value = RightValue.APPROVED.ToString() };
            IDictionary<string, Right> rights = new Dictionary<string, Right> { { adminRight.Type, adminRight } };

            Infrastructure.Service studentPersonalsService = new Infrastructure.Service
            {
                ContextId = "DEFAULT",
                Name = "StudentPersonals",
                Rights = rights,
                Type = "OBJECT"
            };
            Infrastructure.Service schoolInfosService = new Infrastructure.Service
            {
                ContextId = "DEFAULT",
                Name = "SchoolInfos",
                Rights = rights,
                Type = "OBJECT"
            };
            ICollection<Infrastructure.Service> services = new SortedSet<Infrastructure.Service>
            {
                studentPersonalsService, schoolInfosService
            };

            ProvisionedZone schoolZone = new ProvisionedZone { SifId = "auSchoolTestingZone", Services = services };
            ProvisionedZone studentZone = new ProvisionedZone { SifId = "auStudentTestingZone", Services = services };

            IDictionary<string, ProvisionedZone> provisionedZones = new SortedDictionary<string, ProvisionedZone> { { schoolZone.SifId, schoolZone }, { studentZone.SifId, studentZone } };

            Environment environmentResponse = CreateEnvironmentRequest();
            environmentResponse.InfrastructureServices = infrastructureServices;
            environmentResponse.ProvisionedZones = provisionedZones;

            return environmentResponse;
        }

    }

}
