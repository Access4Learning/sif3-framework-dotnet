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
using Sif.Framework.Demo.AspNetCore.Setup.Utils;
using Sif.Framework.EntityFrameworkCore.Data;
using Sif.Framework.EntityFrameworkCore.Persistence;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Persistence;
using Tardigrade.Framework.EntityFrameworkCore.Extensions;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => services
        .AddTransient<DbContext>(_ =>
            new SifFrameworkDbContext(
                new DbContextOptionsBuilder<SifFrameworkDbContext>()
                    .UseSqlite(context.Configuration.GetConnectionString("DefaultConnection"))
                    .Options))
        .AddTransient<IApplicationRegisterRepository, ApplicationRegisterRepository>())
    .Build();

// Determine locale to use.
var config = host.Services.GetRequiredService<IConfiguration>();
string prop = args.Length == 1 ? args[0] : config["demo.locale"];
bool localeValid = "AU".Equals(prop, StringComparison.OrdinalIgnoreCase) ||
                   "UK".Equals(prop, StringComparison.OrdinalIgnoreCase) ||
                   "US".Equals(prop, StringComparison.OrdinalIgnoreCase);
string? locale = localeValid ? prop.ToUpper() : null;

if (locale == null)
{
    Console.WriteLine(
        "To execute, setup requires a parameter which specifies locale, i.e. AU, UK or US.");
}
else
{
    // Reset the database.
    Console.WriteLine("Configuring the demonstration for the " + locale + " locale.");
    var dbContext = host.Services.GetRequiredService<DbContext>();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();

    // Get the current project's directory to store the create script.
    DirectoryInfo? binDirectory =
        Directory.GetParent(Directory.GetCurrentDirectory())?.Parent ??
        Directory.GetParent(Directory.GetCurrentDirectory());
    DirectoryInfo? projectDirectory = binDirectory?.Parent ?? binDirectory;

    // Create and store SQL script for the test database.
    dbContext.GenerateCreateScript($"{projectDirectory}\\Scripts\\DatabaseCreateScript.sql");

    // Generate seed application registers for the demonstration project.
    IList<ApplicationRegister> applicationRegisters =
        DataFactory.CreateApplicationRegisters(locale);
    var applicationRegisterRepository = host.Services.GetRequiredService<IApplicationRegisterRepository>();

    foreach (ApplicationRegister applicationRegister in applicationRegisters)
    {
        applicationRegisterRepository.Create(applicationRegister);
    }
}

await host.RunAsync();