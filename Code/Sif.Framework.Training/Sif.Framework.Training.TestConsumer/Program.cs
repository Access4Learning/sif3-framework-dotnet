using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sif.Framework.EntityFrameworkCore.Data;
using Sif.Framework.Model.Sessions;
using Sif.Framework.Service.Sessions;
using Sif.Framework.Settings;
using Sif.Framework.Training.TestConsumer.Consumers;
using Sif.Framework.Training.TestConsumer.Models;
using Tardigrade.Framework.EntityFrameworkCore;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Services;

StudentPersonalConsumer? consumer = null;

try
{
    using IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) => services
            .AddTransient<DbContext>(_ =>
                new SessionDbContext(
                    new DbContextOptionsBuilder<SessionDbContext>()
                        .UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"))
                        .Options))
            .AddTransient<IRepository<Session, Guid>, Repository<Session, Guid>>()
            .AddTransient<IObjectService<Session, Guid>, ObjectService<Session, Guid>>()
            .AddTransient<ISessionService, SessionService>())
        .Build();

    var config = host.Services.GetRequiredService<IConfiguration>();
    var dbContext = host.Services.GetRequiredService<DbContext>();
    var sessionService = host.Services.GetRequiredService<ISessionService>();

    dbContext.Database.EnsureCreated();

    var settings = new ConsumerSettings(config);
    consumer = new StudentPersonalConsumer(
        settings.ApplicationKey,
        settings.InstanceId,
        settings.UserToken,
        settings.SolutionId,
        settings,
        sessionService);
    consumer.Register();
    IEnumerable<StudentPersonal> students = consumer.Query();
    Console.WriteLine($"Retrieved {students.Count()} students.");

    await host.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine($"Error: {e.Message}");
}
finally
{
    consumer?.Unregister();
}