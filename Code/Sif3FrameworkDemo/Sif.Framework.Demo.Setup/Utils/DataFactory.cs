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

using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Serialisation;
using Sif.Specification.Infrastructure;
using System.Collections.ObjectModel;
using System.IO;

namespace Sif.Framework.Demo.Setup.Utils
{

    /// <summary>
    /// This class is used to generate data for running the demonstration projects.
    /// </summary>
    static class DataFactory
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ApplicationRegister CreateApplicationRegister(string locale)
        {
            environmentType environmentTypeRequest;
            environmentType environmentTypeResponse;

            using (FileStream xmlStream = File.OpenRead("Data files\\" + locale.ToUpper() + "\\EnvironmentRequest.xml"))
            {
                environmentTypeRequest = SerialiserFactory.GetXmlSerialiser<environmentType>().Deserialise(xmlStream);
            }

            using (FileStream xmlStream = File.OpenRead("Data files\\" + locale.ToUpper() + "\\EnvironmentResponse.xml"))
            {
                environmentTypeResponse = SerialiserFactory.GetXmlSerialiser<environmentType>().Deserialise(xmlStream);
            }

            Environment environmentRequest = MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeRequest);
            Environment environmentResponse = MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeResponse);
            EnvironmentRegister environmentRegister = CreateEnvironmentRegister(environmentRequest, environmentResponse);
            ApplicationRegister applicationRegister = new ApplicationRegister
            {
                ApplicationKey = environmentRequest.ApplicationInfo.ApplicationKey,
                SharedSecret = "SecretDem0",
                EnvironmentRegisters = new Collection<EnvironmentRegister> { { environmentRegister } }
            };

            return applicationRegister;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static EnvironmentRegister CreateEnvironmentRegister(Environment environmentRequest, Environment environmentResponse)
        {
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
