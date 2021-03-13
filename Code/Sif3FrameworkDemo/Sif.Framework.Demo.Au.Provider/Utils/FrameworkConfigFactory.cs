/*
 * Copyright 2021 Systemic Pty Ltd
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

using Sif.Framework.Model.Settings;
using Sif.Framework.Settings;
using Sif.Framework.Utils;
using System;
using Tardigrade.Framework.Configurations;
using Tardigrade.Framework.EntityFramework.Configurations;

namespace Sif.Framework.Demo.Au.Provider.Utils
{
    internal static class FrameworkConfigFactory
    {
        public static IFrameworkSettings CreateSettings()
        {
            IFrameworkSettings settings;
            string settingsSource = System.Configuration.ConfigurationManager.AppSettings["demo.frameworkConfigSource"];

            if ("Database".Equals(settingsSource, StringComparison.InvariantCultureIgnoreCase))
            {
                settings = new ProviderSettings(
                    new ApplicationConfiguration(new AppSettingsConfigurationSource("name=FrameworkConfigDb")));
            }
            else if ("File".Equals(settingsSource, StringComparison.InvariantCultureIgnoreCase))
            {
                settings = SettingsManager.ProviderSettings;
            }
            else
            {
                settings = SettingsManager.ProviderSettings;
            }

            return settings;
        }
    }
}