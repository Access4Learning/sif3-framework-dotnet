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

using Sif.Framework.EntityFrameworkCore.Tests.Fixtures;
using Sif.Framework.EntityFrameworkCore.Tests.Models.Infrastructure;
using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sif.Framework.Service.Mapper;
using Sif.Specification.Infrastructure;
using Tardigrade.Framework.EntityFrameworkCore.Extensions;
using Tardigrade.Framework.Persistence;
using Xunit;
using Xunit.Abstractions;
using Environment = Sif.Framework.EntityFrameworkCore.Tests.Models.Infrastructure.Environment;

namespace Sif.Framework.EntityFrameworkCore.Tests;

/// <inheritdoc />
public class RepositoryTest : IClassFixture<EntityFrameworkCoreClassFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly IRepository<Right, long> _rightRepository;
    private readonly IRepository<Models.Infrastructure.Service, long> _serviceRepository;

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
    public void Crud_Right_Success()
    {
        const string typeName = nameof(Right);

        // Create.
        Right original = DataFactory.Right;
        Right created = _rightRepository.Create(original);
        _output.WriteLine($"Created new {typeName}...\n{JsonSerializer.Serialize(created)}");
        Assert.Equal(original.Type, created.Type);
        Assert.Equal(original.Value, created.Value);

        // Retrieve.
        Right retrieved = _rightRepository.Retrieve(created.Id);
        _output.WriteLine($"Retrieved newly created {typeName}...\n{JsonSerializer.Serialize(retrieved)}");
        Assert.Equal(created.Id, retrieved.Id);

        // Update.
        retrieved.Value = "REJECTED";
        _rightRepository.Update(retrieved);
        Right updated = _rightRepository.Retrieve(retrieved.Id);
        _output.WriteLine($"Updated created {typeName}...\n{JsonSerializer.Serialize(updated)}");
        Assert.Equal(retrieved.Id, updated.Id);
        Assert.Equal("REJECTED", updated.Value);

        // Delete.
        _rightRepository.Delete(created);
        bool exists = _rightRepository.Exists(created.Id);
        Assert.False(exists);
        _output.WriteLine($"Successfully deleted {typeName} {created.Id}.");
    }

    [Fact]
    public void Crud_Service_Success()
    {
        const string typeName = nameof(Models.Infrastructure.Service);

        // Create.
        Models.Infrastructure.Service original = DataFactory.Service;
        Models.Infrastructure.Service created = _serviceRepository.Create(original);
        _output.WriteLine($"Created new {typeName}...\n{JsonSerializer.Serialize(created)}");
        created.Should().BeEquivalentTo(original, options => options.Excluding(service => service.Id));
        Assert.Equal(original.ContextId, created.ContextId);
        Assert.Equal(original.Name, created.Name);
        Assert.Equal(original.Type, created.Type);

        // Retrieve.
        Models.Infrastructure.Service retrieved = _serviceRepository.Retrieve(created.Id);
        _output.WriteLine($"Retrieved newly created {typeName}...\n{JsonSerializer.Serialize(retrieved)}");
        Assert.Equal(created.Id, retrieved.Id);
        created.Should().BeEquivalentTo(original);

        // Update.
        retrieved.Type = "SERVICEPATH";
        _serviceRepository.Update(retrieved);
        Models.Infrastructure.Service updated = _serviceRepository.Retrieve(retrieved.Id);
        _output.WriteLine($"Updated created Ser{typeName}vice...\n{JsonSerializer.Serialize(updated)}");
        Assert.Equal(retrieved.Id, updated.Id);
        Assert.Equal("SERVICEPATH", updated.Type);

        // Delete.
        _serviceRepository.Delete(created);
        bool exists = _serviceRepository.Exists(created.Id);
        Assert.False(exists);
        _output.WriteLine($"Successfully deleted {typeName} {created.Id}.");
    }
}