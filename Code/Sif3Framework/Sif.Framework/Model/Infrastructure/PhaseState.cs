/*
 * Crown Copyright © Department for Education (UK) 2016
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

using Sif.Framework.Model.Persistence;
using System;

namespace Sif.Framework.Model.Infrastructure
{
    /// <summary>
    /// Object representing the state of a phase
    /// </summary>
    public class PhaseState : IPersistable<Guid>
    {
        /// <summary>
        /// The RefId of this object
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// The type of the phase state
        /// </summary>
        public virtual PhaseStateType Type { get; set; }

        /// <summary>
        /// When this phase state was created
        /// </summary>
        public virtual DateTime? Created { get; set; }

        /// <summary>
        /// When this phase state was last modified
        /// </summary>
        public virtual DateTime? LastModified { get; set; }

        /// <summary>
        /// The description of this phase state
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Basic constructor that sets logical defaults of this phase state
        /// </summary>
        public PhaseState() {
            Type = PhaseStateType.NOTSTARTED;
            Created = DateTime.UtcNow;
            LastModified = Created;
        }

        /// <summary>
        /// Constructor that takes a phase type and optional description arguments
        /// </summary>
        public PhaseState(PhaseStateType type, string description = null) : this()
        {
            Type = type;
            Description = description;
        }
    }
}
