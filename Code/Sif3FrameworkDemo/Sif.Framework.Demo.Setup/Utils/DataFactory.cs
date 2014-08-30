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

using Sif.Framework.Demo.Provider.Models;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Serialisation;
using Sif.Specification.Infrastructure;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Sif.Framework.Demo.Setup.Utils
{

    /// <summary>
    /// This class is used to generate data for running the demos.
    /// </summary>
    static class DataFactory
    {
        private static environmentType environmentTypeRequest;
        private static environmentType environmentTypeResponse;
        private static System.Random random = new System.Random();

        /// <summary>
        /// 
        /// </summary>
        static DataFactory()
        {

            using (FileStream xmlStream = File.OpenRead("Data files\\EnvironmentRequest.xml"))
            {
                environmentTypeRequest = SerialiserFactory.GetXmlSerialiser<environmentType>().Deserialise(xmlStream);
            }

            using (FileStream xmlStream = File.OpenRead("Data files\\EnvironmentResponse.xml"))
            {
                environmentTypeResponse = SerialiserFactory.GetXmlSerialiser<environmentType>().Deserialise(xmlStream);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ApplicationRegister CreateApplicationRegister()
        {
            Environment environmentRequest = MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeRequest);
            EnvironmentRegister environmentRegister = CreateEnvironmentRegister();
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

        public static StudentPersonal CreateStudent()
        {
            Name name = new Name { Type = NameType.LGL, FamilyName = RandomNameGenerator.FamilyName, GivenName = RandomNameGenerator.GivenName };
            PersonInfo personInfo = new PersonInfo { Name = name };
            StudentPersonal studentPersonal = new StudentPersonal { LocalId = random.Next(10000, 99999).ToString(), PersonInfo = personInfo };

            return studentPersonal;
        }

        public static ICollection<StudentPersonal> CreateStudents(int count)
        {
            ICollection<StudentPersonal> students = new List<StudentPersonal>();

            for (int i = 1; i <= count; i++)
            {
                students.Add(CreateStudent());
            }

            return students;
        }

    }

}
