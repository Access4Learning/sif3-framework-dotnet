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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sif.Framework.Demo.Consumer.Apps;
using Sif.Framework.Demo.Consumer.Utils;
using Sif.Framework.EntityFrameworkCore.Data;
using Sif.Framework.Model.Sessions;
using Sif.Framework.Service.Sessions;
using Sif.Framework.Settings;
using Tardigrade.Framework.EntityFrameworkCore;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Services;

const string DatabaseEngineKey = "demo.database.engine";

// Create a .NET Generic Host for this application.
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => services
        .AddTransient<DbContext>(_ =>
            new SessionDbContext(
                context.Configuration[DatabaseEngineKey] switch
                {
                    "LocalDB" => new DbContextOptionsBuilder<SessionDbContext>()
                        .UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection.LocalDB"))
                        .Options,
                    "SQLite" => new DbContextOptionsBuilder<SessionDbContext>()
                        .UseSqlite(context.Configuration.GetConnectionString("DefaultConnection.SQLite"))
                        .Options,
                    _ => throw new ArgumentOutOfRangeException()
                }))
        .AddTransient<IRepository<Session, Guid>, Repository<Session, Guid>>()
        .AddTransient<IObjectService<Session, Guid>, ObjectService<Session, Guid>>()
        .AddTransient<ISessionService, SessionService>())
    .Build();

try
{
    // Get required services.
    var config = host.Services.GetRequiredService<IConfiguration>();
    var dbContext = host.Services.GetRequiredService<DbContext>();
    var logger = host.Services.GetRequiredService<ILogger<StudentPersonalApp>>();
    var sessionService = host.Services.GetRequiredService<ISessionService>();

    // Check whether the Consumer database needs to be recreated.
    bool recreateDatabase =
        args.Length == 1 && string.Equals("recreateDb", args[0], StringComparison.OrdinalIgnoreCase);

    // Ensure the database is created if it does not already exist.
    var databaseManager = new DatabaseManager(dbContext, config);
    databaseManager.EnsureDatabaseCreated(recreateDatabase);

    // If the Consumer database needs to be recreated, then do not run the StudentPersonal Consumer.
    if (!recreateDatabase)
    {
        // Run CRUD operations against the StudentPersonal Consumer.
        var settings = new ConsumerSettings(config);
        var app = new StudentPersonalApp(logger);
        app.RunConsumer(settings, sessionService);
    }
}
catch (Exception e)
{
    Console.WriteLine($"Error: {e.Message}");
}

await host.RunAsync();