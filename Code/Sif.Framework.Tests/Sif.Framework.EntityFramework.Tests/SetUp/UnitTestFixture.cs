using Sif.Framework.Model.Sessions;
using System;
using Tardigrade.Framework.Patterns.DependencyInjection;
using Tardigrade.Framework.Persistence;

namespace Sif.Framework.EntityFramework.Tests.SetUp
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/50921675/dependency-injection-in-xunit-project">Dependency injection in XUnit project</a>
    /// </summary>
    public class UnitTestFixture : IDisposable
    {
        private readonly IRepository<Session, Guid> repository;

        public IServiceContainer Container { get; }

        public Session Reference { get; }

        public UnitTestFixture()
        {
            Container = new UnitTestServiceContainer();

            // Create a reference session for testing.
            Reference = new Session
            {
                ApplicationKey = "Sif3DemoConsumer",
                EnvironmentUrl = "http://localhost:62921/api/environments/bca76787-48ae-4532-b851-fd7099a4098b",
                Id = Guid.NewGuid(),
                QueueId = Guid.NewGuid().ToString(),
                SessionToken = "U2lmM0RlbW9Db25zdW1lcjo6OlNpZjNGcmFtZXdvcms=",
                SolutionId = "Sif3Framework",
                SubscriptionId = Guid.NewGuid().ToString()
            };

            repository = Container.GetService<IRepository<Session, Guid>>();
            _ = repository.Create(Reference);
        }

        public void Dispose()
        {
            // Delete the reference user.
            repository.Delete(Reference);
        }
    }
}