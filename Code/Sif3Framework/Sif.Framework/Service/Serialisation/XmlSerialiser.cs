/*
 * Copyright 2014 Systemic Pty Ltd
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
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class XmlSerialiser<T> : XmlSerializer, ISerialiser<T>
    {

        /// <summary>
        /// 
        /// </summary>
        public XmlSerialiser()
            : base(typeof(T))
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public XmlSerialiser(XmlRootAttribute root)
            : base(typeof(T), root)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
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
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialise(T obj)
        {
            string str = null;

            if (obj != null)
            {
                Stream stream;
                Serialise(obj, out stream);
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
