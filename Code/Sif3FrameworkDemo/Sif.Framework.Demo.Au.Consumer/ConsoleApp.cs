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

namespace Sif.Framework.Demo.Au.Consumer
{
    internal abstract class ConsoleApp
    {
        protected enum SettingsSource
        { Database, File }

        protected static ISessionService GetSessionService(SettingsSource source)
        {
            ISessionService sessionService;

            switch (source)
            {
                case SettingsSource.Database:
                    DbContext dbContext = new SessionDbContext("name=FrameworkConfigDb");
                    IRepository<Session, Guid> repository = new Repository<Session, Guid>(dbContext);
                    IObjectService<Session, Guid> service = new ObjectService<Session, Guid>(repository);
                    sessionService = new SessionService(service);
                    break;

                case SettingsSource.File:
                    sessionService = SessionsManager.ConsumerSessionService;
                    break;

                default:
                    sessionService = SessionsManager.ConsumerSessionService;
                    break;
            }

            return sessionService;
        }

        protected static IFrameworkSettings GetSettings(SettingsSource source)
        {
            IFrameworkSettings settings;

            switch (source)
            {
                case SettingsSource.Database:
                    settings = new ConsumerSettings(
                        new ApplicationConfiguration(new AppSettingsConfigurationSource("name=FrameworkConfigDb")));
                    break;

                case SettingsSource.File:
                    settings = SettingsManager.ConsumerSettings;
                    break;

                default:
                    settings = SettingsManager.ConsumerSettings;
                    break;
            }

            return settings;
        }

        protected static SettingsSource SelectFrameworkConfigSource()
        {
            Console.WriteLine();
            Console.Write(
                "Would you like to read the application settings and session token from the SifFramework.config (F)ile or from the SifFrameworkConfig.db (D)atabase? Pressing enter defaults to (F)ile. - ");
            ConsoleKeyInfo info;

            do
            {
                info = Console.ReadKey();
            } while (info.Key != ConsoleKey.D && info.Key != ConsoleKey.F && info.Key != ConsoleKey.Enter);

            Console.WriteLine();
            Console.WriteLine();

            return info.Key == ConsoleKey.D ? SettingsSource.Database : SettingsSource.File;
        }
    }
}