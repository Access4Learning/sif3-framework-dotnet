/*
 * Copyright 2020 Systemic Pty Ltd
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

using Microsoft.Extensions.Configuration;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Requests;
using Sif.Framework.Model.Settings;
using Tardigrade.Framework.Extensions;

namespace Sif.Framework.Settings
{
    /// <summary>
    /// This class represents application settings that are stored in a database.
    /// </summary>
    public class FrameworkSettings : IFrameworkSettings
    {
        /// <summary>
        /// Application configuration properties.
        /// </summary>
        protected IConfiguration Configuration { get; }

        /// <inheritdoc/>
        public Accept Accept => Configuration.GetAsEnum<Accept>($"{SettingsPrefix}payload.accept") ?? Accept.XML;

        /// <inheritdoc/>
        public string ApplicationKey =>
            Configuration.GetAsString($"{SettingsPrefix}environment.template.applicationKey");

        /// <inheritdoc/>
        public string AuthenticationMethod =>
            Configuration.GetAsString($"{SettingsPrefix}environment.template.authenticationMethod");

        /// <inheritdoc/>
        public bool CompressPayload => Configuration.GetAsBoolean($"{SettingsPrefix}payload.compress") ?? false;

        /// <inheritdoc/>
        public string ConsumerName => Configuration.GetAsString($"{SettingsPrefix}environment.template.consumerName");

        /// <inheritdoc/>
        public ContentType ContentType =>
            Configuration.GetAsEnum<ContentType>($"{SettingsPrefix}payload.contentType") ?? ContentType.XML;

        /// <inheritdoc/>
        public string DataModelNamespace =>
            Configuration.GetAsString($"{SettingsPrefix}environment.template.dataModelNamespace");

        /// <inheritdoc/>
        public bool DeleteOnUnregister =>
            Configuration.GetAsBoolean($"{SettingsPrefix}environment.deleteOnUnregister") ?? false;

        /// <inheritdoc/>
        public EnvironmentType EnvironmentType =>
            Configuration.GetAsEnum<EnvironmentType>($"{SettingsPrefix}environmentType") ?? EnvironmentType.DIRECT;

        /// <inheritdoc/>
        public string EnvironmentUrl => Configuration.GetAsString($"{SettingsPrefix}environment.url");

        /// <inheritdoc/>
        public int EventProcessingWaitTime =>
            Configuration.GetAsInt($"{SettingsPrefix}events.minWaitTimeSeconds") ?? 60;

        /// <inheritdoc/>
        public string InfrastructureNamespace =>
            "http://www.sifassociation.org/infrastructure/" + SupportedInfrastructureVersion;

        /// <inheritdoc/>
        public string InstanceId => Configuration.GetAsString($"{SettingsPrefix}environment.template.instanceId");

        /// <inheritdoc/>
        public bool JobBinding => Configuration.GetAsBoolean($"{SettingsPrefix}job.binding") ?? true;

        /// <inheritdoc/>
        public string JobClasses => Configuration.GetAsString($"{SettingsPrefix}job.classes", "any");

        /// <inheritdoc/>
        public bool JobTimeoutEnabled => Configuration.GetAsBoolean($"{SettingsPrefix}job.timeout.enabled") ?? true;

        /// <inheritdoc/>
        public int JobTimeoutFrequency => Configuration.GetAsInt($"{SettingsPrefix}job.timeout.frequency") ?? 60;

        /// <inheritdoc/>
        public int NavigationPageSize => Configuration.GetAsInt($"{SettingsPrefix}paging.navigationPageSize") ?? 100;

        /// <inheritdoc/>
        public string SharedSecret => Configuration.GetAsString($"{SettingsPrefix}environment.sharedSecret");

        /// <inheritdoc/>
        public string SolutionId => Configuration.GetAsString($"{SettingsPrefix}environment.template.solutionId");

        /// <inheritdoc/>
        public int StartupDelay => Configuration.GetAsInt($"{SettingsPrefix}startup.delay") ?? 10;

        /// <inheritdoc/>
        public string SupportedInfrastructureVersion =>
            Configuration.GetAsString($"{SettingsPrefix}environment.template.supportedInfrastructureVersion");

        /// <inheritdoc/>
        public string UserToken => Configuration.GetAsString($"{SettingsPrefix}environment.template.userToken");

        /// <summary>
        /// Prefix associated will all setting names.
        /// </summary>
        protected virtual string SettingsPrefix => string.Empty;

        /// <summary>
        /// Create an instance of this class based upon the configuration provided.
        /// </summary>
        /// <param name="configuration">Application configuration properties.</param>
        protected FrameworkSettings(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}