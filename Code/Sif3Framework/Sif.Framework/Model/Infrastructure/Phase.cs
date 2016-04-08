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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Sif.Framework.Model.Infrastructure
{
    /// <summary>
    /// The following Phase elements/properties are mandatory according to the SIF specification:
    /// /phase/name
    /// /phase/state
    /// /phase/required
    /// /phase/rights
    /// </summary>
    public class Phase : IPersistable<long>
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// The name of the Phase unique within the context of the owning job.
        /// </summary>
        public virtual string Name { get; set; }
        
        /// <summary>
        /// The current state of the Phase.
        /// </summary>
        public virtual IList<State> States { get; set; }

        /// <summary>
        /// Whether or not this phase is required for the job to complete successfully.
        /// </summary>
        public virtual Boolean Required { get; set; }

        /// <summary>
        /// Access rights given to the consumer for this phase.
        /// </summary>
        public virtual IDictionary<string, Right> Rights { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Phase()
        {
            States = new List<State>();
            Rights = new Dictionary<string, Right>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Name of the Phase.</param>
        /// <param name="required">If the Phase is required or not. Default is false.</param>
        public Phase(string name, Boolean required = false) : this()
        {
            if (String.IsNullOrWhiteSpace(name) || name.Contains(" "))
            {
                throw new ArgumentNullException("name", "Phases must have a name, which cannot contain any spaces");
            }
            Name = name;
            Required = required;
        }

        public Phase(string phaseName, Boolean required, IDictionary<string, Right> rights = null, PhaseStateType state = PhaseStateType.NOTAPPLICABLE, string stateDescription = "Not applicable") : this(phaseName, required)
        {
            if (rights != null)
            {
                Rights = rights;
            }
            changeState(state, stateDescription);
        }

        public virtual State changeState(PhaseStateType type, string description = null)
        {
            State current = States.LastOrDefault();
            if(current != null && current.Type == type)
            {
                current.LastModified = DateTime.UtcNow;
                current.Description = description;
                return current;
            }

            State newState = new State(type, description);
            States.Add(newState);
            return newState;
        }

        public virtual State getCurrentState()
        {
            return States.LastOrDefault();
        }
    }
}
