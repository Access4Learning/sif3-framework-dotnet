/*
 * Copyright 2018 Systemic Pty Ltd
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

namespace Sif.Framework.Consumers
{

    /// <summary>
    /// This interface defines the operations available for SIF Event Consumers of SIF data model objects.
    /// </summary>
    public interface IEventConsumer
    {

        /// <summary>
        /// Start the SIF Event Consumer.
        /// </summary>
        /// <param name="zoneId">Zone associated with the request.</param>
        /// <param name="contextId">Zone context.</param>
        void Start(string zoneId = null, string contextId = null);

        /// <summary>
        /// Stop the SIF Event Consumer.
        /// </summary>
        /// <param name="deleteOnStop">True to remove session data when stopping; false to leave session data.</param>
        void Stop(bool? deleteOnStop = null);

    }

}
