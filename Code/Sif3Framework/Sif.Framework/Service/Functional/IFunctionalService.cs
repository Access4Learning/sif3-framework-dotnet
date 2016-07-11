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
using Sif.Specification.Infrastructure;
using System;

namespace Sif.Framework.Service.Functional
{
    /// <summary>
    /// Interface that Functional services must implement
    /// </summary>
    public interface IFunctionalService : ISifService<jobType, Job>
    {
        /// <summary>
        /// Get the defined name of this service. Must be plural form, for example for a job named "Payload" this method should return "Payloads". Another example, for a job named "ISBSubmission" this should return "ISBSubmissions".
        /// </summary>
        string GetServiceName();

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

        /// <summary>
        /// Returns the name of supported jobs (the service name without the last character)
        /// </summary>
        /// <returns>The name of supported jobs</returns>
        string AcceptJob();

        /// <summary>
        /// Method that is run once to set up a thread for this service in the FunctionalServiceProviderFactory.
        /// </summary>
        void Startup();

        /// <summary>
        /// Method that is run once to abort this service's thread in the FunctionalServiceProviderFactory.
        /// </summary>
        void Shutdown();
        
        /// <summary>
        /// Extends the timeout of the specified job the by given duration.
        /// </summary>
        /// <param name="job">The job whose duration is to be extended.</param>
        /// <param name="duration">The TimeSpan to increase the duration by.</param>
        void ExtendJobTimeout(Job job, TimeSpan duration);

        /// <summary>
        /// Method that is run at specified intervals to timeout jobs that belong to this service.
        /// </summary>
        void JobTimeout();

        /// <summary>
        /// Binds a (job) object's refid to a session token
        /// </summary>
        /// <param name="ownerId">The session token the (job) object was created in</param>
        /// <param name="objectId">The refid of the created object</param>
        void Bind(Guid objectId, string ownerId);

        /// <summary>
        /// Unbinds all (job) object refid from this session token
        /// </summary>
        /// <param name="ownerId">The session token to unbind all objects from</param>
        void Unbind(string ownerId);

        /// <summary>
        /// Unbinds the (job) object's refid from its associated session token
        /// </summary>
        /// <param name="objectId">The refid of the object to unbind from its session token</param>
        void Unbind(Guid objectId);

        /// <summary>
        /// Checks if the given (job) object id is bound, and if it is returns the session token.
        /// </summary>
        /// <param name="objectId">The refid of the object</param>
        /// <returns>The session token this object id is bound to, null if not found</returns>
        string GetBinding(Guid objectId);

        /// <summary>
        /// Returns true if the (job) object refid is associated with the session token.
        /// </summary>
        /// <param name="ownerId">The session token to look for</param>
        /// <param name="objectId">The refid of the (job) object to look for</param>
        /// <returns>See summary</returns>
        Boolean IsBound(Guid objectId, string ownerId);
    }
}
