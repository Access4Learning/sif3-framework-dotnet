using Newtonsoft.Json;
using Sif.Framework.EntityFramework.Tests.SetUp;
using Sif.Framework.Model.Sessions;
using Sif.Framework.Service.Sessions;
using Sif.Framework.Shared;
using System;
using System.Linq;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Persistence;
using Xunit;
using Xunit.Abstractions;

namespace Sif.Framework.EntityFramework.Tests
{
    public class SessionTest : IClassFixture<UnitTestFixture>
    {
        private readonly ITestOutputHelper output;
        private readonly Session reference;
        private readonly IRepository<Session, Guid> repository;
        private readonly ISessionService service;

        public SessionTest(UnitTestFixture fixture, ITestOutputHelper output)
        {
            this.output = output;
            repository = fixture.Container.GetService<IRepository<Session, Guid>>();
            service = fixture.Container.GetService<ISessionService>();
            reference = fixture.Reference;
        }

        /// <summary>
        /// Create a copy of the original Session, but with a new Id.
        /// </summary>
        /// <param name="original">Session to copy.</param>
        /// <returns>Copy of the original Session.</returns>
        private static Session CopySession(Session original)
        {
            return new Session
            {
                ApplicationKey = original.ApplicationKey,
                EnvironmentUrl = original.EnvironmentUrl,
                Id = Guid.NewGuid(),
                QueueId = original.QueueId,
                SessionToken = original.SessionToken,
                SolutionId = original.SolutionId,
                SubscriptionId = original.SubscriptionId
            };
        }

        [Fact]
        public void Crud_NewObject_Success()
        {
            int originalCount = repository.Count();
            output.WriteLine($"Total number of sessions before executing CRUD operation is {originalCount}.");

            // Create.
            Session original = DataFactory.CreateSession();
            Session created = repository.Create(original);
            output.WriteLine($"Created new session:\n{JsonConvert.SerializeObject(created)}");
            Assert.Equal(original.Id, created.Id);

            // Retrieve single.
            Session retrieved = repository.Retrieve(created.Id);
            output.WriteLine($"Retrieved newly created session:\n{JsonConvert.SerializeObject(retrieved)}");
            Assert.Equal(created.Id, retrieved.Id);

            // Update.
            const string updatedSolutionId = "Sif3Framework2";
            retrieved.SolutionId = updatedSolutionId;
            repository.Update(retrieved);
            Session updated = repository.Retrieve(retrieved.Id);
            output.WriteLine(
                $"Updated the SolutionId of the newly created session to {updatedSolutionId}:\n{JsonConvert.SerializeObject(updated)}");
            Assert.Equal(retrieved.Id, updated.Id);
            Assert.Equal(updatedSolutionId, updated.SolutionId);

            // Delete.
            repository.Delete(created);
            Session deleted = repository.Retrieve(created.Id);
            output.WriteLine($"Successfully deleted session {created.Id} - {deleted == null}.");
            Assert.Null(deleted);
            int currentCount = repository.Count();
            output.WriteLine($"Total number of sessions after executing CRUD operation is {currentCount}.");
            Assert.Equal(originalCount, currentCount);
        }

        [Fact]
        public void HasSession_Exists_Success()
        {
            // Act.
            bool exists = service.HasSession(reference.ApplicationKey, reference.SolutionId);

            // Assert.
            Assert.True(exists);
        }

        [Theory]
        [InlineData("Sif3DemoConsumer")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework2")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework", "Blah")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework", "Blah", "De Blah")]
        public void HasSession_NotExists_Failure(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            // Act.
            bool exists = service.HasSession(applicationKey, solutionId, userToken, instanceId);

            // Assert.
            Assert.False(exists);
        }

        [Fact]
        public void HasSession_NullApplicationKey_ArgumentNullException()
        {
            // Assert.
            Assert.Throws<ArgumentNullException>(() => service.HasSession(applicationKey: null));
        }

        [Fact]
        public void HasSessionToken_Exists_Success()
        {
            // Act.
            bool exists = service.HasSession(sessionToken: reference.SessionToken);

            // Assert.
            Assert.True(exists);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Nonsense")]
        public void HasSessionToken_NotExists_Failure(string sessionToken)
        {
            // Act.
            bool exists = service.HasSession(sessionToken: sessionToken);

            // Assert.
            Assert.False(exists);
        }

        [Fact]
        public void HasSessionToken_NullApplicationKey_ArgumentNullException()
        {
            // Assert.
            Assert.Throws<ArgumentNullException>(() => service.HasSession(sessionToken: null));
        }

        [Fact]
        public void RemoveSession_MultipleSessions_DuplicateFoundException()
        {
            // Arrange.
            Session original = DataFactory.CreateSession();
            Session copy = CopySession(original);
            _ = repository.Create(original);
            _ = repository.Create(copy);

            // Assert.
            Assert.Throws<DuplicateFoundException>(() => service.RemoveSession(original.SessionToken));

            // Restore.
            repository.Delete(original);
            repository.Delete(copy);
        }

        [Fact]
        public void RemoveSession_NewSession_Success()
        {
            // Arrange.
            Session original = DataFactory.CreateSession();
            Session created = repository.Create(original);

            // Act.
            service.RemoveSession(created.SessionToken);

            // Assert.
            bool exists = repository.Exists(original.Id);
            Assert.False(exists);
        }

        [Fact]
        public void RemoveSession_NoSession_NotFoundException()
        {
            // Assert.
            Assert.Throws<NotFoundException>(() => service.RemoveSession("FakeSessionToken"));
        }

        [Fact]
        public void RemoveSession_NullApplicationKey_ArgumentNullException()
        {
            // Assert.
            Assert.Throws<ArgumentNullException>(() => service.RemoveSession(null));
        }

        [Fact]
        public void RetrieveEnvironmentUrl_Exists_Success()
        {
            // Act.
            string url = service.RetrieveEnvironmentUrl(
                reference.ApplicationKey,
                reference.SolutionId,
                reference.UserToken,
                reference.InstanceId);

            // Assert.
            Assert.Equal(reference.EnvironmentUrl, url);
        }

        [Fact]
        public void RetrieveEnvironmentUrl_MultipleSessions_DuplicateFoundException()
        {
            // Arrange.
            Session original = DataFactory.CreateSession();
            Session copy = CopySession(original);
            _ = repository.Create(original);
            _ = repository.Create(copy);

            // Assert.
            Assert.Throws<DuplicateFoundException>(
                () => service.RetrieveEnvironmentUrl(
                    original.ApplicationKey,
                    original.SolutionId,
                    original.UserToken,
                    original.InstanceId));

            // Restore.
            repository.Delete(original);
            repository.Delete(copy);
        }

        [Theory]
        [InlineData("Sif3DemoConsumer")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework2")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework", "Blah")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework", "Blah", "De Blah")]
        public void RetrieveEnvironmentUrl_NotExists_Failure(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            // Act.
            string url = service.RetrieveEnvironmentUrl(applicationKey, solutionId, userToken, instanceId);

            // Assert.
            Assert.Null(url);
        }

        [Fact]
        public void RetrieveEnvironmentUrl_NullApplicationKey_ArgumentNullException()
        {
            // Assert.
            Assert.Throws<ArgumentNullException>(() => service.RetrieveEnvironmentUrl(null));
        }

        [Fact]
        public void RetrieveQueueId_Exists_Success()
        {
            // Act.
            string queueId = service.RetrieveQueueId(
                reference.ApplicationKey,
                reference.SolutionId,
                reference.UserToken,
                reference.InstanceId);

            // Assert.
            Assert.Equal(reference.QueueId, queueId);
        }

        [Fact]
        public void RetrieveQueueId_MultipleSessions_DuplicateFoundException()
        {
            // Arrange.
            Session original = DataFactory.CreateSession();
            Session copy = CopySession(original);
            _ = repository.Create(original);
            _ = repository.Create(copy);

            // Assert.
            Assert.Throws<DuplicateFoundException>(
                () => service.RetrieveQueueId(
                    original.ApplicationKey,
                    original.SolutionId,
                    original.UserToken,
                    original.InstanceId));

            // Restore.
            repository.Delete(original);
            repository.Delete(copy);
        }

        [Theory]
        [InlineData("Sif3DemoConsumer")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework2")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework", "Blah")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework", "Blah", "De Blah")]
        public void RetrieveQueueId_NotExists_Failure(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            // Act.
            string queueId = service.RetrieveQueueId(applicationKey, solutionId, userToken, instanceId);

            // Assert.
            Assert.Null(queueId);
        }

        [Fact]
        public void RetrieveQueueId_NullApplicationKey_ArgumentNullException()
        {
            // Assert.
            Assert.Throws<ArgumentNullException>(() => service.RetrieveQueueId(null));
        }

        [Fact]
        public void RetrieveSessionToken_Exists_Success()
        {
            // Act.
            string sessionToken = service.RetrieveSessionToken(
                reference.ApplicationKey,
                reference.SolutionId,
                reference.UserToken,
                reference.InstanceId);

            // Assert.
            Assert.Equal(reference.SessionToken, sessionToken);
        }

        [Fact]
        public void RetrieveSessionToken_MultipleSessions_DuplicateFoundException()
        {
            // Arrange.
            Session original = DataFactory.CreateSession();
            Session copy = CopySession(original);
            _ = repository.Create(original);
            _ = repository.Create(copy);

            // Assert.
            Assert.Throws<DuplicateFoundException>(
                () => service.RetrieveSessionToken(
                    original.ApplicationKey,
                    original.SolutionId,
                    original.UserToken,
                    original.InstanceId));

            // Restore.
            repository.Delete(original);
            repository.Delete(copy);
        }

        [Theory]
        [InlineData("Sif3DemoConsumer")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework2")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework", "Blah")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework", "Blah", "De Blah")]
        public void RetrieveSessionToken_NotExists_Failure(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            // Act.
            string sessionToken = service.RetrieveSessionToken(applicationKey, solutionId, userToken, instanceId);

            // Assert.
            Assert.Null(sessionToken);
        }

        [Fact]
        public void RetrieveSessionToken_NullApplicationKey_ArgumentNullException()
        {
            // Assert.
            Assert.Throws<ArgumentNullException>(() => service.RetrieveSessionToken(null));
        }

        [Fact]
        public void RetrieveSubscriptionId_Exists_Success()
        {
            // Act.
            string subscriptionId = service.RetrieveSubscriptionId(
                reference.ApplicationKey,
                reference.SolutionId,
                reference.UserToken,
                reference.InstanceId);

            // Assert.
            Assert.Equal(reference.SubscriptionId, subscriptionId);
        }

        [Fact]
        public void RetrieveSubscriptionId_MultipleSessions_DuplicateFoundException()
        {
            // Arrange.
            Session original = DataFactory.CreateSession();
            Session copy = CopySession(original);
            _ = repository.Create(original);
            _ = repository.Create(copy);

            // Assert.
            Assert.Throws<DuplicateFoundException>(
                () => service.RetrieveSubscriptionId(
                    original.ApplicationKey,
                    original.SolutionId,
                    original.UserToken,
                    original.InstanceId));

            // Restore.
            repository.Delete(original);
            repository.Delete(copy);
        }

        [Theory]
        [InlineData("Sif3DemoConsumer")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework2")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework", "Blah")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework", "Blah", "De Blah")]
        public void RetrieveSubscriptionId_NotExists_Failure(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            // Act.
            string subscriptionId = service.RetrieveSubscriptionId(applicationKey, solutionId, userToken, instanceId);

            // Assert.
            Assert.Null(subscriptionId);
        }

        [Fact]
        public void RetrieveSubscriptionId_NullApplicationKey_ArgumentNullException()
        {
            // Assert.
            Assert.Throws<ArgumentNullException>(() => service.RetrieveSubscriptionId(null));
        }

        [Fact]
        public void StoreSession_Exists_AlreadyExistsException()
        {
            // Arrange.
            Session original = DataFactory.CreateSession();

            // Assert.
            Assert.Throws<AlreadyExistsException>(
                () => service.StoreSession(
                    reference.ApplicationKey,
                    original.SessionToken,
                    original.EnvironmentUrl,
                    reference.SolutionId,
                    reference.UserToken,
                    reference.InstanceId));
            Assert.Throws<AlreadyExistsException>(
                () => service.StoreSession(
                    original.ApplicationKey,
                    reference.SessionToken,
                    original.EnvironmentUrl,
                    original.SolutionId,
                    original.UserToken,
                    original.InstanceId));
        }

        [Fact]
        public void StoreSession_NewSession_Success()
        {
            // Arrange.
            Session original = DataFactory.CreateSession();

            // Act.
            service.StoreSession(
                original.ApplicationKey,
                original.SessionToken,
                original.EnvironmentUrl,
                original.SolutionId,
                original.UserToken,
                original.InstanceId);

            // Assert.
            Session retrieved = repository
                .Retrieve(s =>
                    s.ApplicationKey == original.ApplicationKey &&
                    s.EnvironmentUrl == original.EnvironmentUrl &&
                    s.InstanceId == original.InstanceId &&
                    s.SessionToken == original.SessionToken &&
                    s.SolutionId == original.SolutionId &&
                    s.UserToken == original.UserToken)
                .First();
            Assert.NotNull(retrieved);

            // Restore.
            repository.Delete(retrieved);
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("Sif3DemoConsumer", null, null)]
        [InlineData(null, "Sif3Framework", null)]
        [InlineData(null, null, "http://localhost:62921/api/environments/bca76787-48ae-4532-b851-fd7099a4098b")]
        [InlineData("Sif3DemoConsumer", "Sif3Framework", null)]
        [InlineData("Sif3DemoConsumer", null,
            "http://localhost:62921/api/environments/bca76787-48ae-4532-b851-fd7099a4098b")]
        [InlineData(null, "Sif3Framework",
            "http://localhost:62921/api/environments/bca76787-48ae-4532-b851-fd7099a4098b")]
        public void StoreSession_NullParameters_ArgumentNullException(
            string applicationKey,
            string sessionToken,
            string environmentUrl)
        {
            // Arrange.
            Session original = DataFactory.CreateSession();

            // Assert.
            Assert.Throws<ArgumentNullException>(
                () => service.StoreSession(
                    applicationKey,
                    sessionToken,
                    environmentUrl,
                    original.SolutionId,
                    original.UserToken,
                    original.InstanceId));
        }

        [Fact]
        public void UpdateQueueId_MultipleSessions_DuplicateFoundException()
        {
            // Arrange.
            Session original = DataFactory.CreateSession();
            Session copy = CopySession(original);
            _ = repository.Create(original);
            _ = repository.Create(copy);
            var newQueueId = Guid.NewGuid().ToString();

            // Assert.
            Assert.Throws<DuplicateFoundException>(
                () => service.UpdateQueueId(
                    newQueueId,
                    original.ApplicationKey,
                    original.SolutionId,
                    original.UserToken,
                    original.InstanceId));

            // Restore.
            repository.Delete(original);
            repository.Delete(copy);
        }

        [Fact]
        public void UpdateQueueId_NewSession_Success()
        {
            // Arrange.
            Session original = DataFactory.CreateSession();
            Session created = repository.Create(original);
            var newQueueId = Guid.NewGuid().ToString();

            // Act.
            service.UpdateQueueId(
                newQueueId,
                created.ApplicationKey,
                created.SolutionId,
                created.UserToken,
                created.InstanceId);

            // Assert.
            Session retrieved = repository.Retrieve(created.Id);
            Assert.Equal(newQueueId, retrieved.QueueId);

            // Restore.
            repository.Delete(created);
        }

        [Fact]
        public void UpdateQueueId_NoSession_NotFoundException()
        {
            // Assert.
            Assert.Throws<NotFoundException>(() => service.UpdateQueueId(Guid.NewGuid().ToString(), "FakeQueueId"));
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("bca76787-48ae-4532-b851-fd7099a4098b", null)]
        [InlineData(null, "Sif3DemoConsumer")]
        public void UpdateQueueId_NullApplicationKey_ArgumentNullException(string queueId, string applicationKey)
        {
            // Assert.
            Assert.Throws<ArgumentNullException>(() => service.UpdateQueueId(queueId, applicationKey));
        }

        [Fact]
        public void UpdateSubscriptionId_MultipleSessions_DuplicateFoundException()
        {
            // Arrange.
            Session original = DataFactory.CreateSession();
            Session copy = CopySession(original);
            _ = repository.Create(original);
            _ = repository.Create(copy);
            var newSubscriptionId = Guid.NewGuid().ToString();

            // Assert.
            Assert.Throws<DuplicateFoundException>(
                () => service.UpdateSubscriptionId(
                    newSubscriptionId,
                    original.ApplicationKey,
                    original.SolutionId,
                    original.UserToken,
                    original.InstanceId));

            // Restore.
            repository.Delete(original);
            repository.Delete(copy);
        }

        [Fact]
        public void UpdateSubscriptionId_NewSession_Success()
        {
            // Arrange.
            Session original = DataFactory.CreateSession();
            Session created = repository.Create(original);
            var newSubscriptionId = Guid.NewGuid().ToString();

            // Act.
            service.UpdateSubscriptionId(
                newSubscriptionId,
                created.ApplicationKey,
                created.SolutionId,
                created.UserToken,
                created.InstanceId);

            // Assert.
            Session retrieved = repository.Retrieve(created.Id);
            Assert.Equal(newSubscriptionId, retrieved.SubscriptionId);

            // Restore.
            repository.Delete(created);
        }

        [Fact]
        public void UpdateSubscriptionId_NoSession_NotFoundException()
        {
            // Assert.
            Assert.Throws<NotFoundException>(() =>
                service.UpdateSubscriptionId(Guid.NewGuid().ToString(), "FakeSubscriptionId"));
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("bca76787-48ae-4532-b851-fd7099a4098b", null)]
        [InlineData(null, "Sif3DemoConsumer")]
        public void UpdateSubscriptionId_NullApplicationKey_ArgumentNullException(string newSubscriptionId,
            string applicationKey)
        {
            // Assert.
            Assert.Throws<ArgumentNullException>(() => service.UpdateSubscriptionId(newSubscriptionId, applicationKey));
        }
    }
}