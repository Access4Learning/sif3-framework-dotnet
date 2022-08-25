﻿/*
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
using Sif.Framework.Models.Sessions;

namespace Sif.Framework.EntityFrameworkCore.Data;

/// <summary>
/// Entity Framework Core database context for use with session data.
/// </summary>
public class SessionDbContext : DbContext
{
    /// <inheritdoc cref="DbContext"/>
    public SessionDbContext(DbContextOptions<SessionDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Sessions.
    /// </summary>
    public virtual DbSet<Session> Sessions => Set<Session>();
}