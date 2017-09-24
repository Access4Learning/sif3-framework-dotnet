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

namespace Sif.Framework.Consumers
{

    /// <summary>
    /// This interface defines Consumer operations used to support SIF Events.
    /// </summary>
    /// <typeparam name="TMultiple">Type that defines a collection of SIF data model objects.</typeparam>
    public interface ISupportsEvents<TMultiple>
    {

        /// <summary>
        /// Handler to be called on a create event.
        /// </summary>
        /// <param name="objs">Collection of SIF data model objects associated with the create event.</param>
        /// <param name="zoneId">Zone associated with the create event.</param>
        /// <param name="contextId">Zone context.</param>
        void OnCreateEvent(TMultiple objs, string zoneId = null, string contextId = null);

        /// <summary>
        /// Handler to be called on a delete event.
        /// </summary>
        /// <param name="objs">Collection of SIF data model objects associated with the delete event.</param>
        /// <param name="zoneId">Zone associated with the delete event.</param>
        /// <param name="contextId">Zone context.</param>
        void OnDeleteEvent(TMultiple objs, string zoneId = null, string contextId = null);

        /// <summary>
        /// Handler to be called on a error event.
        /// </summary>
        /// <param name="objs">Collection of SIF data model objects associated with the error event.</param>
        /// <param name="zoneId">Zone associated with the error event.</param>
        /// <param name="contextId">Zone context.</param>
        void OnErrorEvent(TMultiple objs, string zoneId = null, string contextId = null);

        /// <summary>
        /// Handler to be called on a update event.
        /// </summary>
        /// <param name="objs">Collection of SIF data model objects associated with the update event.</param>
        /// <param name="zoneId">Zone associated with the update event.</param>
        /// <param name="contextId">Zone context.</param>
        void OnUpdateEvent(TMultiple objs, string zoneId = null, string contextId = null);

    }

}
