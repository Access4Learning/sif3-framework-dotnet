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
using Sif.Framework.AspNetCore.Extensions;
using Sif.Framework.AspNetCore.Formatters;
using Sif.Framework.AspNetCore.Middlewares;
using Sif.Framework.Demo.AspNetCore.Provider.Models;
using Sif.Framework.Demo.AspNetCore.Provider.Services;
using Sif.Framework.EntityFrameworkCore.Data;
using Sif.Framework.EntityFrameworkCore.Persistence;
using Sif.Framework.Model.Sessions;
using Sif.Framework.Model.Settings;
using Sif.Framework.Persistence;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Service.Providers;
using Sif.Framework.Service.Sessions;
using Sif.Framework.Settings;
using Sif.Specification.Infrastructure;
using Tardigrade.Framework.EntityFrameworkCore;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
    .AddControllers(options =>
    {
        options.InputFormatters.Add(new ArrayOfInputFormatter<SchoolInfo>(options));
        options.InputFormatters.Add(new ArrayOfInputFormatter<StudentPersonal>(options));
        options.InputFormatters.Add(new ArrayOfInputFormatter<StudentSchoolEnrollment>(options));
        options.InputFormatters.Add(new SifInputFormatter<deleteRequestType>(options));
        options.OutputFormatters.Add(new ArrayOfOutputFormatter<SchoolInfo>());
        options.OutputFormatters.Add(new ArrayOfOutputFormatter<StudentPersonal>());
        options.OutputFormatters.Add(new ArrayOfOutputFormatter<StudentSchoolEnrollment>());
    })
    .AddXmlSerializerFormatters();
builder.Services.AddDbContext<SifFrameworkDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddScoped<DbContext, SifFrameworkDbContext>();

builder.Services.AddScoped<IApplicationRegisterRepository, ApplicationRegisterRepository>();
builder.Services.AddScoped<IEnvironmentRegisterRepository, EnvironmentRegisterRepository>();
builder.Services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
builder.Services.AddScoped<IRepository<Session, Guid>, Repository<Session, Guid>>();

builder.Services.AddScoped<IApplicationRegisterService, ApplicationRegisterService>();
builder.Services.AddScoped<IBasicProviderService<SchoolInfo>, SchoolInfoService>();
builder.Services.AddScoped<IBasicProviderService<StudentPersonal>, StudentPersonalService>();
builder.Services.AddScoped<IBasicProviderService<StudentSchoolEnrollment>, StudentSchoolEnrollmentService>();
builder.Services.AddScoped<IEnvironmentRegisterService, EnvironmentRegisterService>();
builder.Services.AddScoped<IEnvironmentService, EnvironmentService>();
builder.Services.AddScoped<IObjectService<Session, Guid>, ObjectService<Session, Guid>>();
builder.Services.AddScoped<ISessionService, SessionService>();

builder.Services.AddScoped<IFrameworkSettings, ProviderSettings>();
builder.Services.AddScoped<MethodOverrideMiddleware>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

// The MethodOverrideMiddleware will not work unless app.UseRouting() is also called after.
// https://stackoverflow.com/questions/71686202/change-http-put-request-to-http-patch-request-using-iasyncactionfilter-and-rewri
app.UseMethodOverrideMiddleware();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();