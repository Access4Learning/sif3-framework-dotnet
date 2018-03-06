/*
 * Copyright 2017 Systemic Pty Ltd
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

using Sif.Framework.Model.Events;

namespace Sif.Framework.Service.Providers
{

    /// <summary>
    /// This interface defines operations associated with SIF Events.
    /// </summary>
    /// <typeparam name="TMultiple">Type that defines a collection of SIF data model objects.</typeparam>
    public interface IEventService<TMultiple> 
    {

        /// <summary>
        /// Retrieve an iterator of SIF Events.
        /// </summary>
        /// <param name="zoneId">Zone associated with the event iterator.</param>
        /// <param name="contextId">Zone context.</param>
        /// <exception cref="Model.Exceptions.EventException">Error retrieving iterator of SIF Events.</exception>
        /// <returns>Iterator of SIF Events (should not be null).</returns>
        IEventIterator<TMultiple> GetEventIterator(string zoneId = null, string contextId = null);

    }

}
