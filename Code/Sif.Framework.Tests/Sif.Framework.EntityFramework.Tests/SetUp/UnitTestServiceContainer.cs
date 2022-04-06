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

using Microsoft.Extensions.DependencyInjection;
using Sif.Framework.EntityFramework.Data;
using Sif.Framework.Model.Sessions;
using Sif.Framework.Service.Sessions;
using System;
using System.Data.Entity;
using Tardigrade.Framework.EntityFramework;
using Tardigrade.Framework.Patterns.DependencyInjection;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Services;

namespace Sif.Framework.EntityFramework.Tests.SetUp
{
    internal class UnitTestServiceContainer : MicrosoftServiceContainer
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // Inject business services.
            services.AddScoped<DbContext>(_ => new SessionDbContext("name=SettingsDb"));
            services.AddScoped<IRepository<Session, Guid>, Repository<Session, Guid>>();
            services.AddScoped<IObjectService<Session, Guid>, ObjectService<Session, Guid>>();
            services.AddScoped<ISessionService, SessionService>();
        }
    }
}