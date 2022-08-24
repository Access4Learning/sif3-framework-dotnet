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

using FluentAssertions;
using Sif.Framework.EntityFrameworkCore.Tests.Fixtures;
using Sif.Framework.Persistence;
using System;
using System.Text.Json;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Persistence;
using Xunit;
using Xunit.Abstractions;
using Environment = Sif.Framework.Models.Infrastructure.Environment;

namespace Sif.Framework.EntityFrameworkCore.Tests;

/// <inheritdoc />
public class EnvironmentRepositoryTest : IClassFixture<EntityFrameworkCoreClassFixture>
{
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly ITestOutputHelper _output;

    private TEntity CreateTest<TEntity, TKey>(IRepository<TEntity, TKey> repository, TEntity toCreate)
        where TEntity : IHasUniqueIdentifier<TKey>
    {
        TEntity created = repository.Create(toCreate);
        _output.WriteLine($"Created {nameof(TEntity)}...\n{JsonSerializer.Serialize(created)}");
        created.Should().BeEquivalentTo(toCreate, options => options.Excluding(entity => entity.Id));

        return created;
    }

    private void DeleteTest<TEntity, TKey>(IRepository<TEntity, TKey> repository, TEntity toDelete)
        where TEntity : IHasUniqueIdentifier<TKey>
    {
        repository.Delete(toDelete);
        bool exists = repository.Exists(toDelete.Id);
        Assert.False(exists);
        _output.WriteLine($"Deleted {nameof(TEntity)}...\n{JsonSerializer.Serialize(toDelete)}");
    }

    private TEntity RetrieveTest<TEntity, TKey>(IReadOnlyRepository<TEntity, TKey> repository, TEntity toRetrieve)
        where TEntity : IHasUniqueIdentifier<TKey>
    {
        TEntity retrieved = repository.Retrieve(toRetrieve.Id);
        _output.WriteLine($"Retrieved {nameof(TEntity)}...\n{JsonSerializer.Serialize(retrieved)}");
        retrieved.Should().BeEquivalentTo(toRetrieve);

        return retrieved;
    }

    private TEntity UpdateTest<TEntity, TKey>(IRepository<TEntity, TKey> repository, TEntity toUpdate)
        where TEntity : IHasUniqueIdentifier<TKey>
    {
        repository.Update(toUpdate);
        TEntity updated = repository.Retrieve(toUpdate.Id);
        _output.WriteLine($"Updated {nameof(TEntity)}...\n{JsonSerializer.Serialize(updated)}");
        updated.Should().BeEquivalentTo(toUpdate);

        return updated;
    }

    /// <summary>
    /// Create an instance of this test.
    /// </summary>
    /// <param name="fixture">Class fixture.</param>
    /// <param name="output">Logger.</param>
    /// <exception cref="ArgumentNullException">Parameters are null.</exception>
    public EnvironmentRepositoryTest(EntityFrameworkCoreClassFixture fixture, ITestOutputHelper output)
    {
        if (fixture == null) throw new ArgumentNullException(nameof(fixture));

        _output = output ?? throw new ArgumentNullException(nameof(output));

        _environmentRepository =
            fixture.GetService<IEnvironmentRepository>() ??
            throw new InvalidOperationException("IEnvironmentRepository service not found.");
    }

    [Fact]
    public void Crud_Environment_Success()
    {
        var repository = (IRepository<Environment, Guid>)_environmentRepository;

        // Create.
        Environment created = CreateTest(repository, DataFactory.GetEnvironment());

        // Retrieve.
        Environment retrieved = RetrieveTest(repository, created);

        // Retrieve by session token.
        Environment retrievedBySessionToken = _environmentRepository.RetrieveBySessionToken(created.SessionToken);
        retrievedBySessionToken.Should().BeEquivalentTo(created);

        // Update.
        retrieved.InstanceId = "device-007";
        Environment updated = UpdateTest(repository, retrieved);

        // Delete.
        DeleteTest(repository, updated);
    }
}