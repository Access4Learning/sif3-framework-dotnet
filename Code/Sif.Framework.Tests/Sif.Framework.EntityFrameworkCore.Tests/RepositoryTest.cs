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
using Microsoft.EntityFrameworkCore;
using Sif.Framework.EntityFrameworkCore.Tests.Fixtures;
using Sif.Framework.EntityFrameworkCore.Tests.Models.Infrastructure;
using Sif.Specification.Infrastructure;
using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using Tardigrade.Framework.EntityFrameworkCore.Extensions;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Persistence;
using Xunit;
using Xunit.Abstractions;

namespace Sif.Framework.EntityFrameworkCore.Tests;

/// <inheritdoc />
public class RepositoryTest : IClassFixture<EntityFrameworkCoreClassFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly IRepository<ProvisionedZone, long> _provisionedZoneRepository;
    private readonly IRepository<Right, long> _rightRepository;
    private readonly IRepository<Models.Infrastructure.Service, long> _serviceRepository;

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
    public RepositoryTest(EntityFrameworkCoreClassFixture fixture, ITestOutputHelper output)
    {
        if (fixture == null) throw new ArgumentNullException(nameof(fixture));

        _output = output ?? throw new ArgumentNullException(nameof(output));

        _rightRepository = fixture.GetService<IRepository<Right, long>>();
        _serviceRepository = fixture.GetService<IRepository<Models.Infrastructure.Service, long>>();
        _provisionedZoneRepository = fixture.GetService<IRepository<ProvisionedZone, long>>();

        // Get the current project's directory to store the create script.
        DirectoryInfo? binDirectory =
            Directory.GetParent(Directory.GetCurrentDirectory())?.Parent ??
            Directory.GetParent(Directory.GetCurrentDirectory());
        DirectoryInfo? projectDirectory = binDirectory?.Parent ?? binDirectory;

        // Create and store SQL script for the test database.
        fixture.GetService<DbContext>().GenerateCreateScript($"{projectDirectory}\\Scripts\\DatabaseCreateScript.sql");

        var xmlSerializer = new XmlSerializer(typeof(environmentType));

        var streamReader = new StreamReader(@"xml\environment.xml");
        var environmentType = (environmentType)xmlSerializer.Deserialize(streamReader)!;
        streamReader.Close();

        //Environment environment = MapperFactory.CreateInstance<environmentType, Environment>(environmentType);
    }

    [Fact]
    public void Crud_ProvisionedZone_Success()
    {
        // Create.
        ProvisionedZone created = CreateTest(_provisionedZoneRepository, DataFactory.ProvisionedZone);

        // Retrieve.
        ProvisionedZone retrieved = RetrieveTest(_provisionedZoneRepository, created);

        // Update.
        retrieved.SifId = "Sif3DemoZone4";
        ProvisionedZone updated = UpdateTest(_provisionedZoneRepository, retrieved);

        // Delete.
        DeleteTest(_provisionedZoneRepository, updated);
    }

    [Fact]
    public void Crud_Right_Success()
    {
        // Create.
        Right created = CreateTest(_rightRepository, DataFactory.QueryRight);

        // Retrieve.
        Right retrieved = RetrieveTest(_rightRepository, created);

        // Update.
        retrieved.Value = "REJECTED";
        Right updated = UpdateTest(_rightRepository, retrieved);

        // Delete.
        DeleteTest(_rightRepository, updated);
    }

    [Fact]
    public void Crud_Service_Success()
    {
        // Create.
        Models.Infrastructure.Service created = CreateTest(_serviceRepository, DataFactory.StudentService);

        // Retrieve.
        Models.Infrastructure.Service retrieved = RetrieveTest(_serviceRepository, created);

        // Update.
        retrieved.Type = "SERVICEPATH";
        Models.Infrastructure.Service updated = UpdateTest(_serviceRepository, retrieved);

        // Delete.
        DeleteTest(_serviceRepository, updated);
    }
}