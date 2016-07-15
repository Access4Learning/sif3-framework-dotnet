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

using Sif.Framework.Model.Infrastructure;

namespace Sif.Framework.Model.Settings
{

    /// <summary>
    /// This interface represents SIF Framework settings.
    /// </summary>
    public interface IFrameworkSettings
    {

        /// <summary>
        /// Application key if it exists; null otherwise.
        /// </summary>
        string ApplicationKey { get; }

        /// <summary>
        /// Authentication method if it exists; null otherwise.
        /// </summary>
        string AuthenticationMethod { get; }

        /// <summary>
        /// Consumer name if it exists; null otherwise.
        /// </summary>
        string ConsumerName { get; }

        /// <summary>
        /// Data model namespace if it exists; null otherwise.
        /// </summary>
        string DataModelNamespace { get; }

        /// <summary>
        /// Data model namespace if it exists; null otherwise.
        /// </summary>
        string InfrastructureNamespace { get; }

        /// <summary>
        /// Delete on unregister flag if it exists; false otherwise.
        /// </summary>
        bool DeleteOnUnregister { get; }

        /// <summary>
        /// Environment type (BROKERED or DIRECT) if it exists; DIRECT otherwise.
        /// </summary>
        EnvironmentType EnvironmentType { get; }

        /// <summary>
        /// Environment URL if it exists; null otherwise.
        /// </summary>
        string EnvironmentUrl { get; }

        /// <summary>
        /// Instance ID if it exists; null otherwise.
        /// </summary>
        string InstanceId { get; }

        /// <summary>
        /// Number of objects returned in a "page" of results if it exists; 100 otherwise.
        /// </summary>
        int NavigationPageSize { get; }

        /// <summary>
        /// Shared secret if it exists; null otherwise.
        /// </summary>
        string SharedSecret { get; }

        /// <summary>
        /// Solution ID if it exists; null otherwise.
        /// </summary>
        string SolutionId { get; }

        /// <summary>
        /// Supported infrastructure version if it exists; null otherwise.
        /// </summary>
        string SupportedInfrastructureVersion { get; }

        /// <summary>
        /// User token if it exists; null otherwise.
        /// </summary>
        string UserToken { get; }

        /// <summary>
        /// The types of the functional services that are configured. Default is "any".
        /// </summary>
        string JobClasses { get; }

        /// <summary>
        /// How long in seconds to delay between starting each Functional Service thread. Default 10.
        /// </summary>
        int StartupDelay { get; }

        /// <summary>
        /// True if jobs are tightly coupled to the consumer that created them, false otherwise. Default true.
        /// </summary>
        bool JobBinding { get; }

        /// <summary>
        /// True if job timeouts are enabled, false otherwise. Default true.
        /// </summary>
        bool JobTimeoutEnabled { get; }

        /// <summary>
        /// How often to check for timedout jobs in seconds. Default 60.
        /// </summary>
        int JobTimeoutFrequency { get; }
    }
}
