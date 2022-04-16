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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sif.Framework.EntityFrameworkCore.Tests.Data;
using Sif.Framework.EntityFrameworkCore.Tests.Models.Infrastructure;
using System.Reflection;
using Tardigrade.Framework.EntityFrameworkCore;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Testing;

namespace Sif.Framework.EntityFrameworkCore.Tests.Fixtures;

/// <inheritdoc />
public class EntityFrameworkCoreClassFixture : UnitTestClassFixture
{
    // Flag indicating if the current instance is already disposed.
    private bool _disposed;

    protected override Assembly EntryAssembly => Assembly.GetExecutingAssembly();

    /// <inheritdoc />
    protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // For self-hosted unit testing, services.AddDbContext() did not provide an accessible DbContext.
        string connectionString = context.Configuration.GetConnectionString("DefaultConnection");
        DbContextOptions<SifFrameworkDbContext> options =
            new DbContextOptionsBuilder<SifFrameworkDbContext>().UseSqlite(connectionString).Options;
        services.AddTransient<DbContext>(_ => new SifFrameworkDbContext(options));

        // Inject business services.
        services.AddTransient<IRepository<ApplicationInfo, long>, Repository<ApplicationInfo, long>>();
        services.AddTransient<IRepository<InfrastructureService, long>, Repository<InfrastructureService, long>>();
        services.AddTransient<IRepository<ProductIdentity, long>, Repository<ProductIdentity, long>>();
        services.AddTransient<IRepository<Property, long>, Repository<Property, long>>();
        services.AddTransient<IRepository<ProvisionedZone, long>, Repository<ProvisionedZone, long>>();
        services.AddTransient<IRepository<Right, long>, Repository<Right, long>>();
        services.AddTransient<IRepository<Models.Infrastructure.Service, long>, Repository<Models.Infrastructure.Service, long>>();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            // Dispose managed state (managed objects).
        }

        // Free unmanaged resources (unmanaged objects) and override finalizer.
        // Set large fields to null.

        _disposed = true;

        base.Dispose(disposing);
    }
}