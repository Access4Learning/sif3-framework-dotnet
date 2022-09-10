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

using System;
using System.Collections.Generic;
using System.Linq;
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.Models.Infrastructure
{
    /// <summary>
    /// The following Phase elements/properties are mandatory according to the SIF specification:
    /// /phase/name
    /// /phase/state
    /// /phase/required
    /// /phase/rights
    /// TODO Currently not supported in the ASP.NET Core version of the SIF Framework.
    /// </summary>
    public class Phase : IHasUniqueIdentifier<long>
    {
        /// <summary>
        /// The ID of the phase, required for persistence in Hibernate
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        /// The name of the Phase unique within the context of the owning job.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// A collection of states for the this phase, the last of which is the current phase state.
        /// </summary>
        public virtual IList<PhaseState> States { get; set; }

        /// <summary>
        /// Whether or not this phase is required for the job to complete successfully.
        /// Note that it is up to the logic contained in the IPhaseAction implementations that decide how to use this
        /// property.
        /// </summary>
        public virtual bool Required { get; set; }

        /// <summary>
        /// Access rights given to the consumer for this phase.
        /// </summary>
        public virtual ICollection<Right> Rights { get; set; }

        /// <summary>
        /// Access rights given to the consumer for the states in this phase.
        /// </summary>
        public virtual ICollection<Right> StatesRights { get; set; }

        /// <summary>
        /// Basic constructor that sets logical default values
        /// </summary>
        public Phase()
        {
            Name = "unnamed";
            States = new List<PhaseState>();
            Rights = new List<Right>();
            StatesRights = new List<Right>();
        }

        /// <summary>
        /// Constructor that takes a phase name and if the phase is required or not.
        /// </summary>
        /// <param name="name">Name of the Phase.</param>
        /// <param name="required">If the Phase is required or not. Default is false.</param>
        public Phase(string name, bool required = false) : this()
        {
            if (string.IsNullOrWhiteSpace(name) || name.Contains(" "))
            {
                throw new ArgumentNullException(
                    nameof(name),
                    "Phases must have a name, which cannot contain any spaces");
            }

            Name = name;
            Required = required;
        }

        /// <summary>
        /// Constructor that takes a phase name, if it's required, a set of rights information, and initial state
        /// information.
        /// </summary>
        /// <param name="phaseName">The name of the phase</param>
        /// <param name="required">If this phase is required to complete for the job to complete</param>
        /// <param name="rights">Access rights for the phase</param>
        /// <param name="stateRights">Access rights for the states on this phase</param>
        /// <param name="phaseState">The initial state of the phase</param>
        /// <param name="stateDescription">The initial state description for the phase</param>
        public Phase(
            string phaseName,
            bool required,
            ICollection<Right> rights = null,
            ICollection<Right> stateRights = null,
            PhaseStateType phaseState = PhaseStateType.NOTAPPLICABLE,
            string stateDescription = "Not applicable")
            : this(phaseName, required)
        {
            if (rights != null)
            {
                Rights = rights;
            }

            if (stateRights != null)
            {
                StatesRights = stateRights;
            }

            UpdateState(phaseState, stateDescription);
        }

        /// <summary>
        /// Changes the state of the phase
        /// </summary>
        /// <param name="type">The state to set</param>
        /// <param name="description">The optional description to set</param>
        /// <returns>The current state of the phase</returns>
        public virtual PhaseState UpdateState(PhaseStateType type, string description = null)
        {
            PhaseState current = GetCurrentState();

            if (current != null && current.Type == type)
            {
                current.LastModified = DateTime.UtcNow;
                current.Description = description;
                return current;
            }

            var newState = new PhaseState(type, description);
            States.Add(newState);

            return newState;
        }

        /// <summary>
        /// Get the most recent/current/last state of this phase.
        /// </summary>
        /// <returns>The last state in the list of states for this phase.</returns>
        public virtual PhaseState GetCurrentState()
        {
            return States.LastOrDefault();
        }
    }
}