/*
 * Copyright 2018 Systemic Pty Ltd
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
using Sif.Framework.Model.Settings;
using System;
using System.Configuration;
using System.Linq;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Utils
{

    /// <summary>
    /// This is a utility class for operations on the Environment object.
    /// </summary>
    static class EnvironmentUtils
    {

        /// <summary>
        /// Create an Environment instance from properties defined in the framework settings.
        /// </summary>
        /// <param name="settings">Framework settings.</param>
        /// <returns>Environment instance.</returns>
        internal static Environment LoadFromSettings(IFrameworkSettings settings)
        {
            Environment merged = new Environment();

            if (merged.ApplicationInfo == null)
            {
                merged.ApplicationInfo = new ApplicationInfo();
            }

            if (string.IsNullOrWhiteSpace(settings.ApplicationKey))
            {
                throw new ConfigurationErrorsException("An applicationKey must be defined in the Provider Environment template.");
            }
            else
            {
                merged.ApplicationInfo.ApplicationKey = settings.ApplicationKey;
            }

            if (!string.IsNullOrWhiteSpace(settings.SolutionId))
            {
                merged.SolutionId = settings.SolutionId;
            }

            if (!string.IsNullOrWhiteSpace(settings.UserToken))
            {
                merged.UserToken = settings.UserToken;
            }

            if (!string.IsNullOrWhiteSpace(settings.InstanceId))
            {
                merged.InstanceId = settings.InstanceId;
            }

            if (!string.IsNullOrWhiteSpace(settings.AuthenticationMethod))
            {
                merged.AuthenticationMethod = settings.AuthenticationMethod;
            }

            if (!string.IsNullOrWhiteSpace(settings.ConsumerName))
            {
                merged.ConsumerName = settings.ConsumerName;
            }

            if (!string.IsNullOrWhiteSpace(settings.DataModelNamespace))
            {
                merged.ApplicationInfo.DataModelNamespace = settings.DataModelNamespace;
            }

            if (!string.IsNullOrWhiteSpace(settings.SupportedInfrastructureVersion))
            {
                merged.ApplicationInfo.SupportedInfrastructureVersion = settings.SupportedInfrastructureVersion;
            }

            return merged;
        }

        /// <summary>
        /// Combine the passed in environment object with the equivalent properties defined in the framework settings
        /// (if present). Properties that already exist in the passed in the environment take precedence over the
        /// properties defined in the framework settings.
        /// </summary>
        /// <param name="environment">Environment object to check.</param>
        /// <param name="settings">Framework settings.</param>
        /// <returns>Environment object with merged properties.</returns>
        internal static Environment MergeWithSettings(Environment environment, IFrameworkSettings settings)
        {
            Environment merged;

            if (environment == null)
            {
                merged = new Environment();
            }
            else
            {
                merged = environment;
            }

            if (merged.ApplicationInfo == null)
            {
                merged.ApplicationInfo = new ApplicationInfo();
            }

            if (string.IsNullOrWhiteSpace(merged.ApplicationInfo.ApplicationKey) && settings.ApplicationKey != null)
            {
                merged.ApplicationInfo.ApplicationKey = settings.ApplicationKey;
            }

            if (string.IsNullOrWhiteSpace(merged.ApplicationInfo.ApplicationKey))
            {
                throw new ArgumentException("An applicationKey must either be provided or defined in the Consumer Environment template.", "applicationKey");
            }

            if (string.IsNullOrWhiteSpace(merged.AuthenticationMethod) && settings.AuthenticationMethod != null)
            {
                merged.AuthenticationMethod = settings.AuthenticationMethod;
            }

            if (string.IsNullOrWhiteSpace(merged.ConsumerName) && settings.ConsumerName != null)
            {
                merged.ConsumerName = settings.ConsumerName;
            }

            if (string.IsNullOrWhiteSpace(merged.ApplicationInfo.DataModelNamespace) && settings.DataModelNamespace != null)
            {
                merged.ApplicationInfo.DataModelNamespace = settings.DataModelNamespace;
            }

            if (string.IsNullOrWhiteSpace(merged.ApplicationInfo.SupportedInfrastructureVersion) && settings.SupportedInfrastructureVersion != null)
            {
                merged.ApplicationInfo.SupportedInfrastructureVersion = settings.SupportedInfrastructureVersion;
            }

            return merged;
        }

        /// <summary>
        /// Parse the Environment object for the service URL.
        /// </summary>
        /// <param name="environment">Environment object to parse.</param>
        /// <param name="serviceType">The type of service to get a connector for.</param>
        /// <param name="connector">The type of connector to get, only has an effect when the service type is UTILITY.</param>
        /// <returns>Service URL.</returns>
        internal static string ParseServiceUrl(Environment environment, ServiceType serviceType = ServiceType.OBJECT, InfrastructureServiceNames connector = InfrastructureServiceNames.requestsConnector)
        {
            if (environment == null || environment.InfrastructureServices == null)
            {
                return null;
            }

            InfrastructureServiceNames name;
            switch (serviceType)
            {
                case ServiceType.UTILITY:
                    name = connector;
                    break;
                case ServiceType.FUNCTIONAL:
                    name = InfrastructureServiceNames.servicesConnector;
                    break;
                case ServiceType.OBJECT:
                    name = InfrastructureServiceNames.requestsConnector;
                    break;
                default:
                    name = InfrastructureServiceNames.requestsConnector;
                    break;
            }

            if (environment.InfrastructureServices[name] == null ||
                environment.InfrastructureServices[name].Value == null)
            {
                return null;
            }

            return environment.InfrastructureServices[name].Value;
        }

        public static ProvisionedZone GetTargetZone(Environment environment, string zoneId = null)
        {
            // Return the requested zone
            if (StringUtils.NotEmpty(zoneId))
            {
                return environment.ProvisionedZones[zoneId];
            }

            // No zone, so try getting the default zone
            if (StringUtils.NotEmpty(environment.DefaultZone.SifId))
            {
                return environment.ProvisionedZones[environment.DefaultZone.SifId];
            }

            // No default zone defined
            // Fallback: If there is exactly one zone defined then return that
            if (environment.ProvisionedZones != null && environment.ProvisionedZones.Count == 1)
            {
                return environment.ProvisionedZones.Values.FirstOrDefault();
            }

            return null;
        }

    }

}
