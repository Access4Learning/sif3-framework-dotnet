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

using Sif.Framework.Models.Infrastructure;
using Sif.Framework.Models.Requests;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Sif.Framework.Models.Settings
{
    /// <summary>
    /// This class represents settings that are stored in the SifFramework.config custom configuration file.
    /// </summary>
    [Obsolete("Deprecating the use of the SifFramework.config file.")]
    internal abstract class ConfigFileBasedFrameworkSettings : IFrameworkSettings
    {
        private readonly Configuration configuration;

        /// <summary>
        /// Retrieve the setting (boolean) value associated with the key.
        /// </summary>
        /// <param name="key">Key (identifier) of the setting.</param>
        /// <param name="defaultValue">Value returned if key not found.</param>
        /// <returns>Setting (boolean) value associated with the key if found; default value otherwise.</returns>
        protected bool GetBooleanSetting(string key, bool defaultValue)
        {
            KeyValueConfigurationElement setting = configuration.AppSettings.Settings[key];

            if (setting == null) return defaultValue;

            bool value;

            try
            {
                value = bool.Parse(setting.Value);
            }
            catch (FormatException)
            {
                var message =
                    $"The valid values for the {setting.Key} setting are \"true\" or \"false\". The value \"{setting.Value}\" is not valid.";

                throw new ConfigurationErrorsException(message);
            }

            return value;
        }

        /// <summary>
        /// Retrieve the setting (int) value associated with the key.
        /// </summary>
        /// <param name="key">Key (identifier) of the setting.</param>
        /// <param name="defaultValue">Value returned if key not found.</param>
        /// <returns>Setting (int) value associated with the key if found; default value otherwise.</returns>
        protected int GetIntegerSetting(string key, int defaultValue)
        {
            KeyValueConfigurationElement setting = configuration.AppSettings.Settings[key];

            if (setting == null) return defaultValue;

            int value;

            try
            {
                value = int.Parse(setting.Value);
            }
            catch (FormatException)
            {
                var message = $"The value \"{setting.Value}\" is not a valid integer for the {setting.Key} setting.";

                throw new ConfigurationErrorsException(message);
            }

            return value;
        }

        /// <summary>
        /// Retrieve the setting (string) value associated with the key.
        /// </summary>
        /// <param name="key">Key (identifier) of the setting.</param>
        /// <param name="defaultValue">Value returned if key not found.</param>
        /// <returns>Setting (string) value associated with the key if found; default value otherwise.</returns>
        protected string GetStringSetting(string key, string defaultValue)
        {
            KeyValueConfigurationElement setting = configuration.AppSettings.Settings[key];

            return setting != null ? setting.Value : defaultValue;
        }

        /// <summary>
        /// Retrieve the setting (string) value associated with the key.
        /// </summary>
        /// <param name="key">Key (identifier) of the setting.</param>
        /// <returns>Setting (string) value associated with the key.</returns>
        protected string GetStringSetting(string key)
        {
            KeyValueConfigurationElement setting = configuration.AppSettings.Settings[key];

            return setting?.Value;
        }

        /// <summary>
        /// Prefix associated will all settings.
        /// </summary>
        protected abstract string SettingsPrefix { get; }

        /// <summary>
        /// Create an instance of this class based upon the values stored in the SifFramework.config custom
        /// configuration file.
        /// </summary>
        protected ConfigFileBasedFrameworkSettings()
        {
            string configurationFilePath = null;
#if NETFULL
            configurationFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/SifFramework.config");
#else
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.HasFile && !string.IsNullOrWhiteSpace(config.FilePath))
            {
                configurationFilePath = $"{Path.GetDirectoryName(config.FilePath)}/SifFramework.config";
            }
#endif

            configurationFilePath = configurationFilePath ?? "SifFramework.config";

            var exeConfigurationFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configurationFilePath
            };

            configuration =
                ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);

            if (!configuration.HasFile)
            {
                string fullPath = Assembly.GetExecutingAssembly().Location;
                exeConfigurationFileMap.ExeConfigFilename = Path.GetDirectoryName(fullPath) + "\\SifFramework.config";
                configuration = ConfigurationManager.OpenMappedExeConfiguration(
                    exeConfigurationFileMap,
                    ConfigurationUserLevel.None);
            }

            if (!configuration.HasFile)
            {
                var message = $"Missing configuration file {configurationFilePath}.";

                throw new ConfigurationErrorsException(message);
            }
        }

        /// <summary>
        /// <see cref="IFrameworkSettings.Accept"/>
        /// </summary>
        public Accept Accept
        {
            get
            {
                var settingKey = $"{SettingsPrefix}.payload.accept";
                string settingValue = GetStringSetting(settingKey, "XML");

                if (Accept.XML.ToString().Equals(settingValue.ToUpper()))
                {
                    return Accept.XML;
                }

                if (ContentType.JSON.ToString().Equals(settingValue.ToUpper()))
                {
                    return Accept.JSON;
                }

                var message =
                    $"The valid values for the {settingKey} setting are \"XML\" or \"JSON\". The value \"{settingValue}\" is not valid.";

                throw new ConfigurationErrorsException(message);
            }
        }

        /// <summary>
        /// <see cref="IFrameworkSettings.ApplicationKey"/>
        /// </summary>
        public string ApplicationKey => GetStringSetting($"{SettingsPrefix}.environment.template.applicationKey");

        /// <summary>
        /// <see cref="IFrameworkSettings.AuthenticationMethod"/>
        /// </summary>
        public string AuthenticationMethod =>
            GetStringSetting($"{SettingsPrefix}.environment.template.authenticationMethod");

        /// <summary>
        /// <see cref="IFrameworkSettings.CompressPayload"/>
        /// </summary>
        public bool CompressPayload => GetBooleanSetting($"{SettingsPrefix}.payload.compress", false);

        /// <summary>
        /// <see cref="IFrameworkSettings.ConsumerName"/>
        /// </summary>
        public string ConsumerName => GetStringSetting($"{SettingsPrefix}.environment.template.consumerName");

        /// <summary>
        /// <see cref="IFrameworkSettings.ContentType"/>
        /// </summary>
        public ContentType ContentType
        {
            get
            {
                var settingKey = $"{SettingsPrefix}.payload.contentType";
                string settingValue = GetStringSetting(settingKey, "XML");

                if (ContentType.XML.ToString().Equals(settingValue.ToUpper()))
                {
                    return ContentType.XML;
                }

                if (ContentType.JSON.ToString().Equals(settingValue.ToUpper()))
                {
                    return ContentType.JSON;
                }

                var message =
                    $"The valid values for the {settingKey} setting are \"XML\" or \"JSON\". The value \"{settingValue}\" is not valid.";

                throw new ConfigurationErrorsException(message);
            }
        }

        /// <summary>
        /// <see cref="IFrameworkSettings.DataModelNamespace"/>
        /// </summary>
        public string DataModelNamespace =>
            GetStringSetting($"{SettingsPrefix}.environment.template.dataModelNamespace");

        /// <summary>
        /// <see cref="IFrameworkSettings.InfrastructureNamespace"/>
        /// </summary>
        public string InfrastructureNamespace =>
            $"http://www.sifassociation.org/infrastructure/{SupportedInfrastructureVersion}";

        /// <summary>
        /// <see cref="IFrameworkSettings.DeleteOnUnregister"/>
        /// </summary>
        public bool DeleteOnUnregister => GetBooleanSetting($"{SettingsPrefix}.environment.deleteOnUnregister", false);

        /// <summary>
        /// <see cref="IFrameworkSettings.EnvironmentType"/>
        /// </summary>
        public EnvironmentType EnvironmentType
        {
            get
            {
                var settingKey = $"{SettingsPrefix}.environmentType";
                string settingValue = GetStringSetting(settingKey, "DIRECT");

                if (EnvironmentType.BROKERED.ToString().Equals(settingValue))
                {
                    return EnvironmentType.BROKERED;
                }

                if (EnvironmentType.DIRECT.ToString().Equals(settingValue))
                {
                    return EnvironmentType.DIRECT;
                }

                var message =
                    $"The valid values for the {settingKey} setting are \"BROKERED\" or \"DIRECT\". The value \"{settingValue}\" is not valid.";

                throw new ConfigurationErrorsException(message);
            }
        }

        /// <summary>
        /// <see cref="IFrameworkSettings.EnvironmentUrl"/>
        /// </summary>
        public string EnvironmentUrl => GetStringSetting($"{SettingsPrefix}.environment.url");

        /// <summary>
        /// <see cref="IFrameworkSettings.EventProcessingWaitTime"/>
        /// </summary>
        public int EventProcessingWaitTime => GetIntegerSetting($"{SettingsPrefix}.events.minWaitTimeSeconds", 60);

        /// <summary>
        /// <see cref="IFrameworkSettings.InstanceId"/>
        /// </summary>
        public string InstanceId => GetStringSetting($"{SettingsPrefix}.environment.template.instanceId");

        /// <summary>
        /// <see cref="IFrameworkSettings.NavigationPageSize"/>
        /// </summary>
        public int NavigationPageSize => GetIntegerSetting($"{SettingsPrefix}.paging.navigationPageSize", 100);

        /// <summary>
        /// <see cref="IFrameworkSettings.SharedSecret"/>
        /// </summary>
        public string SharedSecret => GetStringSetting($"{SettingsPrefix}.environment.sharedSecret");

        /// <summary>
        /// <see cref="IFrameworkSettings.SolutionId"/>
        /// </summary>
        public string SolutionId => GetStringSetting($"{SettingsPrefix}.environment.template.solutionId");

        /// <summary>
        /// <see cref="IFrameworkSettings.SupportedInfrastructureVersion"/>
        /// </summary>
        public string SupportedInfrastructureVersion =>
            GetStringSetting($"{SettingsPrefix}.environment.template.supportedInfrastructureVersion");

        /// <summary>
        /// <see cref="IFrameworkSettings.UserToken"/>
        /// </summary>
        public string UserToken => GetStringSetting($"{SettingsPrefix}.environment.template.userToken");

        /// <summary>
        /// <see cref="IFrameworkSettings.JobClasses"/>
        /// </summary>
        public string JobClasses => GetStringSetting($"{SettingsPrefix}.job.classes", "any");

        /// <summary>
        /// How long in seconds to delay between starting each Functional Service thread. Default 10.
        /// </summary>
        public int StartupDelay => GetIntegerSetting($"{SettingsPrefix}.startup.delay", 10);

        /// <summary>
        /// True if job objects should be bound to the consumer that created them, false otherwise. Default true.
        /// </summary>
        public bool JobBinding => GetBooleanSetting($"{SettingsPrefix}.job.binding", true);

        /// <summary>
        /// True if job timeouts are enabled, false otherwise. Default true.
        /// </summary>
        public bool JobTimeoutEnabled => GetBooleanSetting($"{SettingsPrefix}.job.timeout.enabled", true);

        /// <summary>
        /// How often to check for timed out jobs in seconds. Default 60.
        /// </summary>
        public int JobTimeoutFrequency => GetIntegerSetting($"{SettingsPrefix}.job.timeout.frequency", 60);
    }
}