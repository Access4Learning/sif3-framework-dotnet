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
using Sif.Framework.AspNetCore.Services.Authentication;
using Sif.Framework.EntityFrameworkCore.Data;
using Sif.Framework.EntityFrameworkCore.Persistence;
using Sif.Framework.Mappers;
using Sif.Framework.Persistence;
using Sif.Framework.Service.Authentication;
using Sif.Framework.Service.Infrastructure;
using Tardigrade.Framework.EntityFrameworkCore;
using Tardigrade.Framework.Persistence;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

const string DatabaseEngineKey = "database.engine";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string? databaseEngine = builder.Configuration[DatabaseEngineKey];

builder.Services.AddAutoMapper(typeof(InfrastructureProfile));
builder.Services.AddControllers().AddXmlSerializerFormatters();

if (string.Equals("LocalDB", databaseEngine, StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<SifFrameworkDbContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection.LocalDB")));
}
else if (string.Equals("SQLite", databaseEngine, StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<SifFrameworkDbContext>(
        options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection.SQLite")));
}
else
{
    builder.Services.AddDbContext<SifFrameworkDbContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddScoped<DbContext, SifFrameworkDbContext>();

builder.Services.AddScoped<IApplicationRegisterRepository, ApplicationRegisterRepository>();
builder.Services.AddScoped<IEnvironmentRegisterRepository, EnvironmentRegisterRepository>();
builder.Services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
builder.Services.AddScoped<IRepository<Environment, Guid>, Repository<Environment, Guid>>();

builder.Services.AddScoped<IApplicationRegisterService, ApplicationRegisterService>();
builder.Services.AddScoped<IAuthenticationService<IHeaderDictionary>, DirectAuthenticationService>();
builder.Services.AddScoped<IEnvironmentRegisterService, EnvironmentRegisterService>();
builder.Services.AddScoped<IEnvironmentService, EnvironmentService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();