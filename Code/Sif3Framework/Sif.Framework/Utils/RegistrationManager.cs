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
using Sif.Framework.Service.Registration;
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
        public static IRegistrationService ConsumerRegistrationService
        {

            get
            {
                return new RegistrationService(SettingsManager.ConsumerSettings, SessionsManager.ConsumerSessionService);
            }

        }

        /// <summary>
        /// New instance of the Consumer registration service.
        /// </summary>
        public static IRegistrationService ProviderRegistrationService
        {

            get
            {
                IRegistrationService registrationService = null;

                if (EnvironmentType.BROKERED.Equals(SettingsManager.ProviderSettings.EnvironmentType))
                {
                    registrationService = new RegistrationService(SettingsManager.ProviderSettings, SessionsManager.ProviderSessionService);
                }
                else if (EnvironmentType.DIRECT.Equals(SettingsManager.ProviderSettings.EnvironmentType))
                {
                    registrationService = new NoRegistrationService();
                }
                else
                {
                    throw new ConfigurationErrorsException("The Environment Type (BROKERED or DIRECT) is not defined.");
                }

                return registrationService;
            }

        }

    }

}
