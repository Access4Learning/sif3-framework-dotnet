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

using Sif.Framework.Service.Sessions;

namespace Sif.Framework.Utils
{

    /// <summary>
    /// Factory class to ensure that the appropriate session service is provided.
    /// </summary>
    public static class SessionsManager
    {
        private static ConsumerSessionService consumerSessionService;
        private static ProviderSessionService providerSessionService;

        /// <summary>
        /// Singleton instance of the Consumer session service.
        /// </summary>
        public static ISessionService ConsumerSessionService
        {

            get
            {

                if (consumerSessionService == null)
                {
                    consumerSessionService = new ConsumerSessionService();
                }

                return consumerSessionService;
            }

        }

        /// <summary>
        /// Singleton instance of the Provider session service.
        /// </summary>
        public static ISessionService ProviderSessionService
        {

            get
            {

                if (providerSessionService == null)
                {
                    providerSessionService = new ProviderSessionService();
                }

                return providerSessionService;
            }

        }

    }

}
