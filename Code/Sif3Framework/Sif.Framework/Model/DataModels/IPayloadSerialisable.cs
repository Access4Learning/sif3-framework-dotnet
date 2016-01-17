/*
 * Copyright 2016 Systemic Pty Ltd
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

namespace Sif.Framework.Model.DataModels
{

    /// <summary>
    /// This interface defines serialisation operations for SIF message payloads.
    /// </summary>
    /// <typeparam name="TSingle">Type that defines a single object entity.</typeparam>
    /// <typeparam name="TMultiple">Type that defines a multiple objects entity.</typeparam>
    public interface IPayloadSerialisable<TSingle, TMultiple>
    {

        /// <summary>
        /// Serialise a single object entity.
        /// </summary>
        /// <param name="obj">Payload of a single object.</param>
        /// <returns>XML string representation of the single object.</returns>
        string SerialiseSingle(TSingle obj);

        /// <summary>
        /// Serialise an entity of multiple objects.
        /// </summary>
        /// <param name="obj">Payload of multiple objects.</param>
        /// <returns>XML string representation of the multiple objects.</returns>
        string SerialiseMultiple(TMultiple obj);

        /// <summary>
        /// Deserialise a single object entity.
        /// </summary>
        /// <param name="payload">Payload of a single object.</param>
        /// <returns>Entity representing the single object.</returns>
        TSingle DeserialiseSingle(string payload);

        /// <summary>
        /// Deserialise an entity of multiple objects.
        /// </summary>
        /// <param name="payload">Payload of multiple objects.</param>
        /// <returns>Entity representing the multiple objects.</returns>
        TMultiple DeserialiseMultiple(string payload);

    }

}
