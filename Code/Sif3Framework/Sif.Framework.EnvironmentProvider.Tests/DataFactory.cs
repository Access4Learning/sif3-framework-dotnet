/*
 * Copyright 2014 Systemic Pty Ltd
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
using Sif.Framework.Infrastructure;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Utils;
using System.Collections.ObjectModel;
using System.IO;
using Configuration = NHibernate.Cfg.Configuration;

namespace Sif.Framework.Model.Infrastructure
{

    public static class DataFactory
    {
        static environmentType environmentTypeRequest;
        static environmentType environmentTypeResponse;

        static DataFactory()
        {

            using (FileStream xmlStream = File.OpenRead("Data files\\EnvironmentRequest.xml"))
            {
                environmentTypeRequest = SerialisationUtils.XmlDeserialise<environmentType>(xmlStream);
            }

            using (FileStream xmlStream = File.OpenRead("Data files\\EnvironmentResponse.xml"))
            {
                environmentTypeResponse = SerialisationUtils.XmlDeserialise<environmentType>(xmlStream);
            }

        }

        public static ApplicationRegister CreateApplicationRegister()
        {
            Environment environmentRequest = CreateEnvironmentRequest();
            EnvironmentRegister environmentRegister = CreateEnvironmentRegister();
            ApplicationRegister applicationRegister = new ApplicationRegister
            {
                ApplicationKey = environmentRequest.ApplicationInfo.ApplicationKey,
                SharedSecret = "SecretDem0",
                EnvironmentRegisters = new Collection<EnvironmentRegister> { { environmentRegister } }
            };
            return applicationRegister;
        }

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
            return MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeRequest);
        }

        public static Environment CreateEnvironmentResponse()
        {
            return MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeResponse);
        }

        public static EnvironmentRegister CreateEnvironmentRegister()
        {
            Environment environmentRequest = MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeRequest);
            Environment environmentResponse = MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeResponse);
            EnvironmentRegister environmentRegister = new EnvironmentRegister
            {
                ApplicationKey = environmentRequest.ApplicationInfo.ApplicationKey,
                InfrastructureServices = environmentResponse.InfrastructureServices,
                InstanceId = environmentRequest.InstanceId,
                ProvisionedZones = environmentResponse.ProvisionedZones,
                SolutionId = environmentRequest.SolutionId,
                UserToken = environmentRequest.UserToken
            };

            return environmentRegister;
        }

    }

}
