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

using Sif.Framework.Models.Sessions;
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