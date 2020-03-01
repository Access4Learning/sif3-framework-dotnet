/*
 * Copyright 2020 Systemic Pty Ltd
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

using System.IO;

namespace Sif.Framework.Service.Serialisation
{
    /// <summary>
    /// Interface for serialisers.
    /// </summary>
    public interface ISerialiser
    {
    }

    /// <summary>
    /// Interface of object serialisation operations.
    /// </summary>
    /// <typeparam name="T">Type of the object to serialise/deserialise.</typeparam>
    public interface ISerialiser<T> : ISerialiser
    {
        /// <summary>
        /// Deserializes the contents of the specified stream.
        /// </summary>
        /// <param name="stream">Stream that contains the content to deserialise.</param>
        /// <returns>Deserialised object if content exists; null otherwise.</returns>
        T Deserialise(Stream stream);

        /// <summary>
        /// Deserializes the contents of the specified string.
        /// </summary>
        /// <param name="str">String that contains the content to deserialise.</param>
        /// <returns>Deserialised object if content exists; null otherwise.</returns>
        T Deserialise(string str);

        /// <summary>
        /// Serializes the specified object and writes the contents to the specified stream.
        /// </summary>
        /// <param name="obj">Object to serialise.</param>
        /// <param name="stream">Stream to serialise the object to.</param>
        void Serialise(T obj, out Stream stream);

        /// <summary>
        /// Serializes the specified object to a string.
        /// </summary>
        /// <param name="obj">Object to serialise.</param>
        /// <returns>String representation of the serialised object.</returns>
        string Serialise(T obj);
    }
}