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
using System.Text;
using System.Xml.Serialization;

namespace Sif.Framework.Service.Serialisation
{
    /// <summary>
    /// Wrapper for the Microsoft XmlSerializer.
    /// <see cref="XmlSerializer"/>
    /// </summary>
    /// <typeparam name="T">Type of the object being serialised/deserialised.</typeparam>
    public class XmlSerialiser<T> : XmlSerializer, ISerialiser<T>
    {
        /// <summary>
        /// <see cref="XmlSerializer(System.Type)"/>
        /// </summary>
        public XmlSerialiser() : base(typeof(T))
        {
        }

        /// <summary>
        /// <see cref="XmlSerializer(System.Type, XmlRootAttribute)"/>
        /// </summary>
        public XmlSerialiser(XmlRootAttribute root) : base(typeof(T), root)
        {
        }

        /// <summary>
        /// <see cref="ISerialiser{T}.Deserialise(Stream)"/>
        /// </summary>
        public T Deserialise(Stream stream)
        {
            T obj = default(T);

            if (stream != null)
            {
                obj = (T)Deserialize(stream);
            }

            return obj;
        }

        /// <summary>
        /// <see cref="ISerialiser{T}.Deserialise(string)"/>
        /// </summary>
        public T Deserialise(string str)
        {
            T obj = default(T);

            if (!string.IsNullOrWhiteSpace(str))
            {
                using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
                {
                    obj = Deserialise(stream);
                }
            }

            return obj;
        }

        /// <summary>
        /// <see cref="ISerialiser{T}.Serialise(T, out Stream)"/>
        /// </summary>
        public void Serialise(T obj, out Stream stream)
        {
            stream = new MemoryStream();

            if (obj != null)
            {
                using (Stream serialiseStream = new MemoryStream())
                {
                    Serialize(serialiseStream, obj);
                    serialiseStream.Position = 0;
                    serialiseStream.CopyTo(stream);
                    stream.Position = 0;
                }
            }
        }

        /// <summary>
        /// <see cref="ISerialiser{T}.Serialise(T)"/>
        /// </summary>
        public string Serialise(T obj)
        {
            string str = null;

            if (obj != null)
            {
                Serialise(obj, out Stream stream);
                stream.Position = 0;

                using (StreamReader reader = new StreamReader(stream))
                {
                    str = reader.ReadToEnd();
                }
            }

            return str;
        }
    }
}