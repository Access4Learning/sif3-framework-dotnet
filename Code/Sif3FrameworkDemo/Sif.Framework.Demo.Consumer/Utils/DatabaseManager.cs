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
using Tardigrade.Framework.EntityFrameworkCore.Extensions;

namespace Sif.Framework.Demo.Consumer.Utils;

public class DatabaseManager
{
    private const string DatabaseEngineKey = "demo.database.engine";

    private readonly IConfiguration _config;
    private readonly DbContext _dbContext;

    public DatabaseManager(DbContext dbContext, IConfiguration config)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public void EnsureDatabaseCreated(bool forceCreateNew = false)
    {
        // Reset the database.
        if (forceCreateNew)
        {
            _dbContext.Database.EnsureDeleted();
        }

        _dbContext.Database.EnsureCreated();

        // Get the current project's directory to store the create script.
        DirectoryInfo? binDirectory =
            Directory.GetParent(Directory.GetCurrentDirectory())?.Parent ??
            Directory.GetParent(Directory.GetCurrentDirectory());
        DirectoryInfo? projectDirectory = binDirectory?.Parent ?? binDirectory;

        // Create and store SQL script for the test database.
        string scriptFilename = $"{projectDirectory}\\Scripts\\CreateScript-{_config[DatabaseEngineKey]}.sql";
        _dbContext.GenerateCreateScript(scriptFilename, true);
    }
}