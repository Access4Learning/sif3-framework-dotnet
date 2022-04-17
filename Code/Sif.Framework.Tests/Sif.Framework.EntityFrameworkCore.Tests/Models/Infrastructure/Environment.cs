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

using System;
using System.Collections.Generic;
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.EntityFrameworkCore.Tests.Models.Infrastructure;

public class Environment : IHasUniqueIdentifier<Guid>
{
    public virtual ApplicationInfo? ApplicationInfo { get; set; }

    public virtual string? AuthenticationMethod { get; set; }

    public virtual string? ConsumerName { get; set; }

    public virtual Zone? DefaultZone { get; set; }

    public virtual Guid Id { get; set; }

    public virtual ICollection<InfrastructureService>? InfrastructureServices { get; set; }

    public virtual string? InstanceId { get; set; }

    public virtual ICollection<ProvisionedZone>? ProvisionedZones { get; set; }

    public virtual string? SessionToken { get; set; }

    public virtual string? SolutionId { get; set; }

    public virtual string? Type { get; set; }

    public virtual string? UserToken { get; set; }
}