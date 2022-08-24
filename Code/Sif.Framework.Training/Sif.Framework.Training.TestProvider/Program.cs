using Microsoft.EntityFrameworkCore;
using Sif.Framework.AspNetCore.Extensions;
using Sif.Framework.AspNetCore.Formatters;
using Sif.Framework.AspNetCore.Middlewares;
using Sif.Framework.EntityFrameworkCore.Data;
using Sif.Framework.EntityFrameworkCore.Persistence;
using Sif.Framework.Models.Sessions;
using Sif.Framework.Models.Settings;
using Sif.Framework.Persistence;
using Sif.Framework.Services.Infrastructure;
using Sif.Framework.Services.Providers;
using Sif.Framework.Services.Sessions;
using Sif.Framework.Settings;
using Sif.Framework.Training.TestProvider.Models;
using Sif.Framework.Training.TestProvider.Services;
using Sif.Specification.Infrastructure;
using Tardigrade.Framework.EntityFrameworkCore;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers(options =>
    {
        options.InputFormatters.Add(new ArrayOfInputFormatter<StudentPersonal>(options));
        options.InputFormatters.Add(new SifInputFormatter<deleteRequestType>(options));
        options.OutputFormatters.Add(new ArrayOfOutputFormatter<StudentPersonal>());
    })
    .AddXmlSerializerFormatters();

builder.Services.AddDbContext<SifFrameworkDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<DbContext, SifFrameworkDbContext>();
builder.Services.AddScoped<IApplicationRegisterRepository, ApplicationRegisterRepository>();
builder.Services.AddScoped<IEnvironmentRegisterRepository, EnvironmentRegisterRepository>();
builder.Services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
builder.Services.AddScoped<IRepository<Session, Guid>, Repository<Session, Guid>>();
builder.Services.AddScoped<IApplicationRegisterService, ApplicationRegisterService>();
builder.Services.AddScoped<IBasicProviderService<StudentPersonal>, StudentPersonalService>();
builder.Services.AddScoped<IEnvironmentRegisterService, EnvironmentRegisterService>();
builder.Services.AddScoped<IEnvironmentService, EnvironmentService>();
builder.Services.AddScoped<IObjectService<Session, Guid>, ObjectService<Session, Guid>>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IFrameworkSettings, ProviderSettings>();
builder.Services.AddScoped<MethodOverrideMiddleware>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMethodOverrideMiddleware();
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();