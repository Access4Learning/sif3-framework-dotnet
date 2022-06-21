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

using Sif.Framework.Model.Requests;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sif.Framework.Service.Serialisation
{
    /// <summary>
    /// Factory of serialisers.
    /// </summary>
    public static class SerialiserFactory
    {
        private static readonly Dictionary<int, ISerialiser> jsonSerializers = new Dictionary<int, ISerialiser>();
        private static readonly Dictionary<int, ISerialiser> xmlSerializers = new Dictionary<int, ISerialiser>();

        /// <summary>
        /// Generate an index key for the serialiser collection cache.
        /// </summary>
        /// <param name="type">Type of the object associated with the serialiser.</param>
        /// <param name="rootAttribute">XML root attribute associated with the serialiser.</param>
        /// <returns>Index key.</returns>
        private static int GenerateKey(Type type, XmlRootAttribute rootAttribute)
        {
            unchecked
            {
                int hashcode = 17;
                hashcode = hashcode * 31 + type.FullName.GetHashCode();
                hashcode = hashcode * 31 + rootAttribute.GetHashCode();
                return hashcode;
            }
        }

        /// <summary>
        /// Retrieve an appropriate (XML to) JSON serialiser.
        /// </summary>
        /// <typeparam name="T">Type of the object associated with the serialiser.</typeparam>
        /// <param name="rootAttribute">XML root attribute associated with the serialiser (if specified).</param>
        /// <returns>JOSN serialiser.</returns>
        public static ISerialiser<T> GetJsonSerialiser<T>(XmlRootAttribute rootAttribute = null)
        {
            ISerialiser<T> serialiser;

            if (rootAttribute == null)
            {
                serialiser = new XmlToJsonSerialiser<T>();
            }
            else
            {
                int serialiserKey = GenerateKey(typeof(T), rootAttribute);

                if (jsonSerializers.TryGetValue(serialiserKey, out ISerialiser jsonSerializer))
                {
                    serialiser = (ISerialiser<T>)jsonSerializer;
                }
                else
                {
                    serialiser = new XmlToJsonSerialiser<T>(rootAttribute);
                    jsonSerializers.Add(serialiserKey, (XmlToJsonSerialiser<T>)serialiser);
                }
            }

            return serialiser;
        }

        /// <summary>
        /// Retrieve an appropriate JSON serialiser based on the specified accepted content type.
        /// </summary>
        /// <typeparam name="T">Type of the object associated with the serialiser.</typeparam>
        /// <param name="rootAttribute">XML root attribute associated with the serialiser (if specified).</param>
        /// <returns>An appropriate serialiser for the accepted content type.</returns>
        public static ISerialiser<T> GetSerialiser<T>(Accept accept, XmlRootAttribute rootAttribute = null)
        {
            ISerialiser<T> serialiser = null;

            switch (accept)
            {
                case Accept.JSON:
                    serialiser = GetJsonSerialiser<T>(rootAttribute);
                    break;

                case Accept.XML:
                    serialiser = GetXmlSerialiser<T>(rootAttribute);
                    break;
            }

            return serialiser;
        }

        /// <summary>
        /// Retrieve an appropriate JSON serialiser based on the specified content type.
        /// </summary>
        /// <typeparam name="T">Type of the object associated with the serialiser.</typeparam>
        /// <param name="rootAttribute">XML root attribute associated with the serialiser (if specified).</param>
        /// <returns>An appropriate serialiser for the content type.</returns>
        public static ISerialiser<T> GetSerialiser<T>(ContentType contentType, XmlRootAttribute rootAttribute = null)
        {
            ISerialiser<T> serialiser = null;

            switch (contentType)
            {
                case ContentType.JSON:
                    serialiser = GetJsonSerialiser<T>(rootAttribute);
                    break;

                case ContentType.XML:
                    serialiser = GetXmlSerialiser<T>(rootAttribute);
                    break;
            }

            return serialiser;
        }

        /// <summary>
        /// Retrieve an appropriate XML serialiser.
        /// </summary>
        /// <typeparam name="T">Type of the object associated with the serialiser.</typeparam>
        /// <param name="rootAttribute">XML root attribute associated with the serialiser (if specified).</param>
        /// <returns>XML serialiser.</returns>
        public static ISerialiser<T> GetXmlSerialiser<T>(XmlRootAttribute rootAttribute = null)
        {
            ISerialiser<T> serialiser;

            if (rootAttribute == null)
            {
                serialiser = new XmlSerialiser<T>();
            }
            else
            {
                int serialiserKey = GenerateKey(typeof(T), rootAttribute);

                if (xmlSerializers.TryGetValue(serialiserKey, out ISerialiser xmlSerializer))
                {
                    serialiser = (ISerialiser<T>)xmlSerializer;
                }
                else
                {
                    serialiser = new XmlSerialiser<T>(rootAttribute);
                    xmlSerializers.Add(serialiserKey, (XmlSerialiser<T>)serialiser);
                }
            }

            return serialiser;
        }
    }
}