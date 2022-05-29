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

using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Serialisation;
using Sif.Specification.Infrastructure;
using System.Collections.ObjectModel;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Demo.NetCore.Setup.Utils;

/// <summary>
/// This class is used to generate data for running the demonstration projects.
/// </summary>
internal static class DataFactory
{
    private static ApplicationRegister CreateApplicationRegister(string path)
    {
        environmentType environmentTypeRequest;
        environmentType environmentTypeResponse;

        string requestFile = path + "\\EnvironmentRequest.xml";
        string responseFile = path + "\\EnvironmentResponse.xml";

        if (!File.Exists(requestFile))
            throw new FileNotFoundException("Environment request file not found.", requestFile);

        if (!File.Exists(responseFile))
            throw new FileNotFoundException("Environment response file not found.", responseFile);

        Console.WriteLine("");
        Console.WriteLine("Processing input from: " + path);
        Console.WriteLine("Request:  " + requestFile);
        Console.WriteLine("Response: " + responseFile);
        Console.WriteLine("");

        using (FileStream xmlStream = File.OpenRead(requestFile))
        {
            environmentTypeRequest = SerialiserFactory.GetXmlSerialiser<environmentType>().Deserialise(xmlStream);
        }

        using (FileStream xmlStream = File.OpenRead(responseFile))
        {
            environmentTypeResponse = SerialiserFactory.GetXmlSerialiser<environmentType>().Deserialise(xmlStream);
        }

        Environment environmentRequest =
            MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeRequest);
        Environment environmentResponse =
            MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeResponse);
        EnvironmentRegister environmentRegister = CreateEnvironmentRegister(environmentRequest, environmentResponse);

        var applicationRegister = new ApplicationRegister
        {
            ApplicationKey = environmentRequest.ApplicationInfo.ApplicationKey,
            SharedSecret = "SecretDem0",
            EnvironmentRegisters = new Collection<EnvironmentRegister> { environmentRegister }
        };

        return applicationRegister;
    }

    public static IList<ApplicationRegister> CreateApplicationRegisters(string locale)
    {
        // Paths to look for.
        var path = $"Data files\\{locale.ToUpper()}";
        var paths = new List<string>(Directory.GetDirectories(path));

        return paths.Select(CreateApplicationRegister).ToList();
    }

    private static EnvironmentRegister CreateEnvironmentRegister(
        Environment environmentRequest,
        Environment environmentResponse)
    {
        var environmentRegister = new EnvironmentRegister
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