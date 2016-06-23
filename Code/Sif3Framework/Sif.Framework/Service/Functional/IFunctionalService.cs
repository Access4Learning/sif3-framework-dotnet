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

using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Persistence;
using Sif.Framework.Service.Providers;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Service.Functional
{
    /// <summary>
    /// Interface that Functional services must implement
    /// </summary>
    public interface IFunctionalService : ISifService<jobType, Job>
    {
        /// <summary>
        /// Handles a create message being sent to, and response from, the named phase on the job with the given RefId.
        /// </summary>
        string CreateToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null);

        /// <summary>
        /// Handles a Retrieve message being sent to, and response from, the named phase on the job with the given RefId.
        /// </summary>
        string RetrieveToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null);

        /// <summary>
        /// Handles a Update message being sent to, and response from, the named phase on the job with the given RefId.
        /// </summary>
        string UpdateToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null);

        /// <summary>
        /// Handles a Delete message being sent to, and response from, the named phase on the job with the given RefId.
        /// </summary>
        string DeleteToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null);

        /// <summary>
        /// Handles creating a state on the named phase on the job with the given RefId.
        /// </summary>
        stateType CreateToState(Guid id, string phaseName, stateType item = null, string zone = null, string context = null);

        /// <summary>
        /// Checks if the job has the right name to be acceptable for this service.
        /// </summary>
        /// <param name="job">The job to check</param>
        /// <returns>true if the job's name conforms to the expected format for this service</returns>
        Boolean AcceptJob(Job job);

        /// <summary>
        /// Checks if a job's name is acceptable for this service.
        /// </summary>
        /// <param name="jobName">The job name to check</param>
        /// <returns>true if the name conforms to the expected format for this service</returns>
        Boolean AcceptJob(string jobName);

        /// <summary>
        /// Checks if a service name and a job's name is acceptable for this service.
        /// </summary>
        /// <param name="serviceName">The service name to check</param>
        /// <param name="jobName">The job name to check</param>
        /// <returns>true if the names conforms to the expected format for this service</returns>
        Boolean AcceptJob(string serviceName, string jobName);
    }
}
