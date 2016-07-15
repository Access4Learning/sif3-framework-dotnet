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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

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
        public static ICollection<ApplicationRegister> CreateApplicationRegisters(string locale)
        {
            IList<ApplicationRegister> registers = new List<ApplicationRegister>();

            string rootPath = "Data files\\" + locale.ToUpper();

            // Paths to look for
            List<string> paths = new List<string>(Directory.GetDirectories(rootPath));
            paths.Insert(0, rootPath);

            foreach (string path in paths)
            {
                ApplicationRegister register = CreateApplicationRegister(path);
                if (register != null)
                {
                    registers.Add(register);
                }
            }

            return registers;
        }

        private static ApplicationRegister CreateApplicationRegister(string path)
        {
            environmentType environmentTypeRequest;
            environmentType environmentTypeResponse;

            string request = @path + "\\EnvironmentRequest.xml";
            string response = @path + "\\EnvironmentResponse.xml";

            if (!File.Exists(request) || !File.Exists(response))
            {
                return null;
            }

            Console.WriteLine("");
            Console.WriteLine("Processsing input from: " + path);
            Console.WriteLine("Request:  " + request);
            Console.WriteLine("Response: " + response);
            Console.WriteLine("");

            using (FileStream xmlStream = File.OpenRead(request))
            {
                environmentTypeRequest = SerialiserFactory.GetXmlSerialiser<environmentType>().Deserialise(xmlStream);
            }

            using (FileStream xmlStream = File.OpenRead(response))
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
                DefaultZone = environmentResponse.DefaultZone,
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
