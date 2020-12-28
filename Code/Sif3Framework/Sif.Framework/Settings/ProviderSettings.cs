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

namespace Sif.Framework.Settings
{
    /// <summary>
    /// This class represents Provider application settings that are stored in a database.
    /// </summary>
    public class ProviderSettings : FrameworkSettings
    {
        /// <summary>
        /// Prefix used to indicate Provider specific application settings.
        /// </summary>
        protected override string SettingsPrefix => "provider.";

        /// <summary>
        /// Create an instance of this class based upon the configuration provided.
        /// </summary>
        /// <param name="configuration">Application configuration properties.</param>
        public ProviderSettings(IConfiguration configuration) : base(configuration)
        {
        }
    }
}