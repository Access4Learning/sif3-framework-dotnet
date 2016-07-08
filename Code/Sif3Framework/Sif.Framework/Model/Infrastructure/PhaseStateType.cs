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

namespace Sif.Framework.Model.Infrastructure
{
    /// <summary>
    /// Enumeration of permitted phase states defined in the SIF 3.2 specification
    /// </summary>
    public enum PhaseStateType
    {
        /// <summary>
        /// This phase is not applicable in the current context.
        /// </summary>
        NOTAPPLICABLE,

        /// <summary>
        /// The phase has not been started yet.
        /// </summary>
        NOTSTARTED,

        /// <summary>
        /// The phase is waiting.
        /// </summary>
        PENDING,

        /// <summary>
        /// The phase has been safely skipped.
        /// </summary>
        SKIPPED,

        /// <summary>
        /// The phase is currently working, awaiting input, etc.
        /// </summary>
        INPROGRESS,

        /// <summary>
        /// The phase has completed successfully.
        /// </summary>
        COMPLETED,

        /// <summary>
        /// The phase has failed, steps may be taken to recover it.
        /// </summary>
        FAILED
    }

}
