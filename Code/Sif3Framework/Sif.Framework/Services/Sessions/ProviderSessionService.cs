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

using Sif.Framework.Models.Settings;
using System.Configuration;

namespace Sif.Framework.Services.Sessions
{
    /// <summary>
    /// This class represents operations associated with Provider sessions that are stored in the SifFramework.config
    /// custom configuration file.
    /// </summary>
    public class ProviderSessionService : ConfigFileBasedSessionService
    {
        /// <summary>
        /// <see cref="ConfigFileBasedSessionService.SessionsSection"/>
        /// </summary>
        protected override ISessionsSection SessionsSection
        {
            get
            {
                var sifFrameworkSectionGroup = (SifFrameworkSectionGroup)Configuration.GetSectionGroup(SifFrameworkSectionGroup.SectionGroupReference);

                if (sifFrameworkSectionGroup == null)
                {
                    var message =
                        $"The <sectionGroup name=\"{SifFrameworkSectionGroup.SectionGroupReference}\" ... /> element is missing from the configuration file {Configuration.FilePath}.";
                    throw new ConfigurationErrorsException(message);
                }

                ISessionsSection SessionsSection = sifFrameworkSectionGroup.ProviderSettings;

                if (SessionsSection == null)
                {
                    var message =
                        $"The <section name=\"{ConsumerSection.SectionReference}\" ... /> element is missing from the configuration file {Configuration.FilePath}.";
                    throw new ConfigurationErrorsException(message);
                }

                return SessionsSection;
            }
        }
    }
}