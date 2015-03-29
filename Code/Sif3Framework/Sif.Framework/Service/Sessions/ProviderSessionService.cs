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

using Sif.Framework.Model.Settings;
using System;
using System.Configuration;

namespace Sif.Framework.Service.Sessions
{

    /// <summary>
    /// This class represents operations associated with Provider sessions that are stored in the SifFramework.config
    /// custom configuration file.
    /// </summary>
    class ProviderSessionService : ConfigFileBasedSessionService
    {

        /// <summary>
        /// <see cref="Sif.Framework.Service.Sessions.ConfigFileBasedSessionService.SessionsSection"/>
        /// </summary>
        protected override ISessionsSection SessionsSection
        {

            get
            {
                SifFrameworkSectionGroup sifFrameworkSectionGroup = (SifFrameworkSectionGroup)Configuration.GetSectionGroup(SifFrameworkSectionGroup.SectionGroupReference);

                if (sifFrameworkSectionGroup == null)
                {
                    string message = String.Format("The <sectionGroup name=\"{0}\" ... /> element is missing from the configuration file {1}.", SifFrameworkSectionGroup.SectionGroupReference, Configuration.FilePath);
                    throw new ConfigurationErrorsException(message);
                }

                ISessionsSection SessionsSection = sifFrameworkSectionGroup.ProviderSettings;

                if (SessionsSection == null)
                {
                    string message = String.Format("The <section name=\"{0}\" ... /> element is missing from the configuration file {1}.", ConsumerSection.SectionReference, Configuration.FilePath);
                    throw new ConfigurationErrorsException(message);
                }

                return SessionsSection;
            }

        }

        /// <summary>
        /// <see cref="Sif.Framework.Service.Sessions.ConfigFileBasedSessionService.ConfigFileBasedSessionService()"/>
        /// </summary>
        public ProviderSessionService()
            : base()
        {

        }

    }

}
