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
using System;
using System.Linq;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the HttpHeaders class.
    /// </summary>
    public static class EnvironmentExtension
    {
        public static ProvisionedZone GetTargetZone(this Environment environment, string zoneId = null)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));

            // Return the requested zone
            if (!string.IsNullOrWhiteSpace(zoneId))
            {
                return environment.ProvisionedZones.FirstOrDefault(p => p.SifId == zoneId);
            }

            // No zone, so try getting the default zone
            if (!string.IsNullOrWhiteSpace(environment.DefaultZone.SifId))
            {
                return environment.ProvisionedZones.FirstOrDefault(p => p.SifId == environment.DefaultZone.SifId);
            }

            // No default zone defined
            // Fallback: If there is exactly one zone defined then return that
            if (environment.ProvisionedZones != null && environment.ProvisionedZones.Count == 1)
            {
                return environment.ProvisionedZones.FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// Parse the Environment object for the service URL.
        /// </summary>
        /// <param name="environment">Environment object to parse.</param>
        /// <param name="serviceType">The type of service to get a connector for.</param>
        /// <param name="connector">The type of connector to get, only has an effect when the service type is UTILITY.</param>
        /// <returns>Service URL.</returns>
        public static string ParseServiceUrl(
            this Environment environment,
            ServiceType serviceType = ServiceType.OBJECT,
            InfrastructureServiceNames connector = InfrastructureServiceNames.requestsConnector)
        {
            if (environment?.InfrastructureServices == null) return null;

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
                default:
                    name = InfrastructureServiceNames.requestsConnector;
                    break;
            }

            InfrastructureService infrastructureService =
                environment.InfrastructureServices.FirstOrDefault(i => i.Name == name);

            return infrastructureService?.Value;
        }
    }
}