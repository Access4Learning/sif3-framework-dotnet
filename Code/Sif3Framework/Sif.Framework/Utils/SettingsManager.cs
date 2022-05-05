﻿/*
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

using Sif.Framework.Model.Settings;
using System;

namespace Sif.Framework.Utils
{
    /// <summary>
    /// Factory class to ensure that the appropriate settings are used.
    /// </summary>
    [Obsolete("Deprecating the use of the SifFramework.config file.")]
    public static class SettingsManager
    {
        private static ConsumerSettings _consumerSettings;
        private static ProviderSettings _providerSettings;

        /// <summary>
        /// Singleton instance of the Consumer settings.
        /// </summary>
        public static IFrameworkSettings ConsumerSettings =>
            _consumerSettings ?? (_consumerSettings = new ConsumerSettings());

        /// <summary>
        /// Singleton instance of the Provider settings.
        /// </summary>
        public static IFrameworkSettings ProviderSettings =>
            _providerSettings ?? (_providerSettings = new ProviderSettings());
    }
}