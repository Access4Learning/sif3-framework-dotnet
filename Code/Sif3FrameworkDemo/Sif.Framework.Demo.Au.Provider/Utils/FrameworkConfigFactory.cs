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

using Sif.Framework.EntityFramework.Data;
using Sif.Framework.Models.Sessions;
using Sif.Framework.Models.Settings;
using Sif.Framework.Services.Sessions;
using Sif.Framework.Settings;
using Sif.Framework.Utils;
using System;
using System.Data.Entity;
using Tardigrade.Framework.Configurations;
using Tardigrade.Framework.EntityFramework;
using Tardigrade.Framework.EntityFramework.Configurations;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Services;

namespace Sif.Framework.Demo.Au.Provider.Utils
{
    internal static class FrameworkConfigFactory
    {
        public static ISessionService CreateSessionService()
        {
            ISessionService sessionService;
            string frameworkConfigSource =
                System.Configuration.ConfigurationManager.AppSettings["demo.frameworkConfigSource"];

            if ("Database".Equals(frameworkConfigSource, StringComparison.InvariantCultureIgnoreCase))
            {
                DbContext dbContext = new SessionDbContext("name=FrameworkConfigDb");
                IRepository<Session, Guid> repository = new Repository<Session, Guid>(dbContext);
                IObjectService<Session, Guid> service = new ObjectService<Session, Guid>(repository);
                sessionService = new SessionService(service);
            }
            else if ("File".Equals(frameworkConfigSource, StringComparison.InvariantCultureIgnoreCase))
            {
                sessionService = SessionsManager.ProviderSessionService;
            }
            else
            {
                sessionService = SessionsManager.ProviderSessionService;
            }

            return sessionService;
        }

        public static IFrameworkSettings CreateSettings()
        {
            IFrameworkSettings settings;
            string frameworkConfigSource =
                System.Configuration.ConfigurationManager.AppSettings["demo.frameworkConfigSource"];

            if ("Database".Equals(frameworkConfigSource, StringComparison.InvariantCultureIgnoreCase))
            {
                settings = new ProviderSettings(
                    new ApplicationConfiguration(new AppSettingsConfigurationSource("name=FrameworkConfigDb")));
            }
            else if ("File".Equals(frameworkConfigSource, StringComparison.InvariantCultureIgnoreCase))
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