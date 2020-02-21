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

using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Sif.Framework.Service.Serialisation
{
    /// <summary>
    /// A serialiser that serialises an object to XML and then JSON, and deserialises JSON to XML and
    /// then an object.
    /// </summary>
    /// <typeparam name="T">Type of the object to serialise/deserialise.</typeparam>
    internal class XmlToJsonSerialiser<T> : ISerialiser<T>
    {
        private readonly XmlSerialiser<T> xmlSerialiser;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        public XmlToJsonSerialiser()
        {
            xmlSerialiser = new XmlSerialiser<T>();
        }

        /// <summary>
        /// Create an instance of this class, specifying the root element of the serialised object (using an XML root
        /// attribute).
        /// </summary>
        /// <param name="root">XML root attribute.</param>
        public XmlToJsonSerialiser(XmlRootAttribute root)
        {
            xmlSerialiser = new XmlSerialiser<T>(root);
        }

        /// <summary>
        /// <see cref="ISerialiser{T}.Deserialise(Stream)"/>
        /// </summary>
        public T Deserialise(Stream stream)
        {
            T obj = default(T);

            if (stream != null)
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    string json = streamReader.ReadToEnd();

                    // If there is JSON data, deserialise into an object.
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        // Convert the JSON string into an XML document.
                        XmlDocument xmlDocument = JsonConvert.DeserializeXmlNode(json);

                        using (XmlReader xmlReader = new XmlNodeReader(xmlDocument))
                        {
                            // Deserialise the XML document into an object.
                            obj = (T)xmlSerialiser.Deserialize(xmlReader);
                        }
                    }
                }
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
                    string xml;

                    using (StringWriter stringWriter = new StringWriter())
                    {
                        using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
                        {
                            // Serialise the object into an XML string.
                            xmlSerialiser.Serialize(xmlWriter, obj);
                            xml = stringWriter.ToString();
                        }
                    }

                    // Parse the XML string and remove empty elements (defined by the "xsi:nil" element).
                    XElement xElement = XElement.Parse(xml);
                    xElement
                        .Descendants()
                        .Where(x =>
                            string.IsNullOrEmpty(x.Value) &&
                            x.Attributes().Where(y => y.Name.LocalName == "nil" && y.Value == "true").Count() > 0)
                        .Remove();
                    xml = xElement.ToString();

                    // Convert the XML document into a JSON string.
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(xml);
                    string json = JsonConvert.SerializeXmlNode(xmlDocument);

                    // Write the JSON string to the stream.
                    byte[] buf = System.Text.Encoding.Default.GetBytes(json);
                    stream.Write(buf, 0, buf.Length);
                    stream.Flush();
                }
            }
        }
    }
}