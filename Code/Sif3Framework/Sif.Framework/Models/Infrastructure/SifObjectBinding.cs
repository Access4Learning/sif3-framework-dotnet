/*
 * Crown Copyright © Department for Education (UK) 2016
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

using Sif.Framework.Models.DataModels;
using System;
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.Models.Infrastructure
{
    /// <summary>
    /// TODO Currently not supported in the ASP.NET Core version of the SIF Framework.
    /// </summary>
    public class SifObjectBinding : IHasUniqueIdentifier<long>, ISifRefId<Guid>
    {
        /// <summary>
        /// Internal identifier used by hibernate. Do not use/alter this.
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        /// The ref id of the object to be bound
        /// </summary>
        public virtual Guid RefId { get; set; }

        /// <summary>
        /// The id of the owner of the object (application key, source id, etc.)
        /// </summary>
        public virtual string OwnerId { get; set; }
    }
}