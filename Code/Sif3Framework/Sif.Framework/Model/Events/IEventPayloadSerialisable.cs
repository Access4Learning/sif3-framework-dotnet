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

namespace Sif.Framework.Model.Events
{

    /// <summary>
    /// This interface defines serialisation operations for SIF Event payloads.
    /// </summary>
    /// <typeparam name="TMultiple">Type that defines a SIF Events entity.</typeparam>
    public interface IEventPayloadSerialisable<TMultiple>
    {

        /// <summary>
        /// Serialise a SIF Events entity.
        /// </summary>
        /// <param name="obj">Payload of SIF Events.</param>
        /// <returns>XML string representation of the SIF Events.</returns>
        string SerialiseEvents(TMultiple obj);

    }

}
