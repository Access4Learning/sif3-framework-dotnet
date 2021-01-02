using Microsoft.Extensions.DependencyInjection;
using Sif.Framework.EntityFramework.Data;
using Sif.Framework.EntityFramework.Services.Sessions;
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