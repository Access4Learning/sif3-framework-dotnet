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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sif.Framework.Service.Functional
{
    /// <summary>
    /// The interface used for all actions that a phase may perform.
    /// </summary>
    public interface IPhaseActions
    {
        /// <summary>
        /// Method to encapsulate a Create operation for a phase
        /// </summary>
        /// <param name="job">The Job on which the phase exists</param>
        /// <param name="phase">The phase being interacted with</param>
        /// <param name="body">The payload (string) being sent to the phase</param>
        /// <param name="contentType">A mime type that indicates what format the body is serialized as</param>
        /// <param name="accept">A mime type that indicates what format the response should be serialized as</param>
        /// <returns>A (possibly null) string to be sent back to the consumer</returns>
        /// <exception cref="Sif.Framework.Model.Exceptions.CreateException" />
        string Create(Job job, Phase phase, string body = null, string contentType = null, string accept = null);

        /// <summary>
        /// Method to encapsulate a Retrieve operation for a phase
        /// </summary>
        /// <param name="job">The Job on which the phase exists</param>
        /// <param name="phase">The phase being interacted with</param>
        /// <param name="body">The payload (string) being sent to the phase</param>
        /// <param name="contentType">A mime type that indicates what format the body is serialized as</param>
        /// <param name="accept">A mime type that indicates what format the response should be serialized as</param>
        /// <returns>A (possibly null) string to be sent back to the consumer</returns>
        /// <exception cref="Sif.Framework.Model.Exceptions.NotFoundException" />
        string Retrieve(Job job, Phase phase, string body = null, string contentType = null, string accept = null);

        /// <summary>
        /// Method to encapsulate a Update operation for a phase
        /// </summary>
        /// <param name="job">The Job on which the phase exists</param>
        /// <param name="phase">The phase being interacted with</param>
        /// <param name="body">The payload (string) being sent to the phase</param>
        /// <param name="contentType">A mime type that indicates what format the body is serialized as</param>
        /// <param name="accept">A mime type that indicates what format the response should be serialized as</param>
        /// <returns>A (possibly null) string to be sent back to the consumer</returns>
        /// <exception cref="Sif.Framework.Model.Exceptions.UpdateException" />
        string Update(Job job, Phase phase, string body = null, string contentType = null, string accept = null);

        /// <summary>
        /// Method to encapsulate a Delete operation for a phase
        /// </summary>
        /// <param name="job">The Job on which the phase exists</param>
        /// <param name="phase">The phase being interacted with</param>
        /// <param name="body">The payload (string) being sent to the phase</param>
        /// <param name="contentType">A mime type that indicates what format the body is serialized as</param>
        /// <param name="accept">A mime type that indicates what format the response should be serialized as</param>
        /// <returns>A (possibly null) string to be sent back to the consumer</returns>
        /// <exception cref="Sif.Framework.Model.Exceptions.DeleteException" />
        string Delete(Job job, Phase phase, string body = null, string contentType = null, string accept = null);
    }
}
