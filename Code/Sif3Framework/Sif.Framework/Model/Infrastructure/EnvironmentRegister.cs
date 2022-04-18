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

using System.Collections.Generic;
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.Model.Infrastructure
{
    public class EnvironmentRegister : IHasUniqueIdentifier<long>
    {
        public virtual string ApplicationKey { get; set; }

        public virtual Zone DefaultZone { get; set; }

        public virtual long Id { get; set; }

        public virtual ICollection<InfrastructureService> InfrastructureServices { get; set; }

        public virtual string InstanceId { get; set; }

        public virtual IDictionary<string, ProvisionedZone> ProvisionedZones { get; set; }

        public virtual string SolutionId { get; set; }

        public virtual string UserToken { get; set; }
    }
}