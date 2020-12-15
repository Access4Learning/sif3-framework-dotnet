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

using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Sessions;
using System;
using System.Configuration;

namespace Sif.Framework.Utils
{
    /// <summary>
    /// Factory class to ensure that the appropriate registration service is provided.
    /// </summary>
    public static class RegistrationManager
    {
        /// <summary>
        /// New instance of the Consumer registration service.
        /// </summary>
        [Obsolete]
        public static IRegistrationService ConsumerRegistrationService =>
            new RegistrationService(SettingsManager.ConsumerSettings, SessionsManager.ConsumerSessionService);

        /// <summary>
        /// New instance of the Provider registration service.
        /// </summary>
        [Obsolete("Replaced by GetProviderRegistrationService(IFrameworkSettings, ISessionService).", true)]
        public static IRegistrationService ProviderRegistrationService
        {
            get
            {
                IRegistrationService registrationService;

                switch (SettingsManager.ProviderSettings.EnvironmentType)
                {
                    case EnvironmentType.BROKERED:
                        registrationService = new RegistrationService(SettingsManager.ProviderSettings,
                            SessionsManager.ProviderSessionService);
                        break;

                    case EnvironmentType.DIRECT:
                        registrationService = new NoRegistrationService();
                        break;

                    default:
                        throw new ConfigurationErrorsException(
                            "The Environment Type (BROKERED or DIRECT) is not defined.");
                }

                return registrationService;
            }
        }

        /// <summary>
        /// New instance of the Provider registration service.
        /// </summary>
        /// <param name="settings">Application settings associated with the Provider.</param>
        /// <param name="sessionService">Service associated with Provider sessions.</param>
        public static IRegistrationService GetProviderRegistrationService(
            IFrameworkSettings settings,
            ISessionService sessionService)
        {
            IRegistrationService registrationService;

            switch (settings.EnvironmentType)
            {
                case EnvironmentType.BROKERED:
                    registrationService = new RegistrationService(settings, sessionService);
                    break;

                case EnvironmentType.DIRECT:
                    registrationService = new NoRegistrationService();
                    break;

                default:
                    throw new ConfigurationErrorsException("Environment Type (BROKERED or DIRECT) is not defined.");
            }

            return registrationService;
        }
    }
}