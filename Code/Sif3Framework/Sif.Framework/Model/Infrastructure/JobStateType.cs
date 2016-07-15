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
    /// Enumeration of permitted job states defined in the SIF 3.2 specification
    /// </summary>
    public enum JobStateType
    {
        /// <summary>
        /// The job has not been started.
        /// </summary>
        NOTSTARTED,

        /// <summary>
        /// The job is currently performing some operation, awaiting input, etc.
        /// </summary>
        INPROGRESS,

        /// <summary>
        /// The job has finished and can be deleted.
        /// </summary>
        COMPLETED,

        /// <summary>
        /// The job has failed, steps may be taken to recover the job.
        /// </summary>
        FAILED
    }

}
