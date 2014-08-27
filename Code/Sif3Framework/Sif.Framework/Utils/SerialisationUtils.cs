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

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Sif.Framework.Utils
{

    /// <summary>
    /// 
    /// </summary>
    public class SerialisationUtils
    {
        private static Dictionary<int, XmlSerializer> serializers = new Dictionary<int, XmlSerializer>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        private static int GenerateKey(System.Type[] types)
        {

            unchecked
            {
                int hashcode = 17;

                foreach (System.Type item in types)
                {
                    hashcode = hashcode * 31 + item.FullName.GetHashCode();
                }

                return hashcode;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="derivedTypes"></param>
        /// <returns></returns>
        private static XmlSerializer GetSerialiserInstance<T>(System.Type[] derivedTypes = null)
        {
            XmlSerializer serialiser;

            if (derivedTypes == null)
            {
                serialiser = new XmlSerializer(typeof(T));
            }
            else
            {
                int serialiserKey = GenerateKey(derivedTypes);

                if (!serializers.TryGetValue(serialiserKey, out serialiser))
                {
                    serialiser = new XmlSerializer(typeof(T), derivedTypes);
                    serializers.Add(serialiserKey, serialiser);
                }

            }

            return serialiser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootAttribute"></param>
        /// <param name="derivedTypes"></param>
        /// <returns></returns>
        private static XmlSerializer GetSerialiserInstance<T>(XmlRootAttribute rootAttribute, System.Type[] derivedTypes = null)
        {
            XmlSerializer serialiser;

            if (derivedTypes == null)
            {
                serialiser = new XmlSerializer(typeof(List<T>), rootAttribute);
            }
            else
            {
                int serialiserKey = GenerateKey(derivedTypes) + rootAttribute.GetHashCode();

                if (!serializers.TryGetValue(serialiserKey, out serialiser))
                {
                    serialiser = new XmlSerializer(typeof(List<T>), null, derivedTypes, rootAttribute, null);
                    serializers.Add(serialiserKey, serialiser);
                }

            }

            return serialiser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlStream"></param>
        /// <param name="derivedTypes"></param>
        /// <returns></returns>
        public static T XmlDeserialise<T>(Stream xmlStream, System.Type[] derivedTypes = null)
        {

            if (xmlStream == null)
            {
                throw new System.ArgumentNullException("xmlStream");
            }

            T deserialisedObject = (T)GetSerialiserInstance<T>(derivedTypes).Deserialize(xmlStream);

            return deserialisedObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <param name="derivedTypes"></param>
        /// <returns></returns>
        public static T XmlDeserialise<T>(string xmlString, System.Type[] derivedTypes = null)
        {
            T deserialisedObject;

            using (Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {
                deserialisedObject = XmlDeserialise<T>(xmlStream, derivedTypes);
            }

            return deserialisedObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlStream"></param>
        /// <param name="rootAttribute"></param>
        /// <param name="derivedTypes"></param>
        /// <returns></returns>
        public static ICollection<T> XmlDeserialise<T>(Stream xmlStream, XmlRootAttribute rootAttribute, System.Type[] derivedTypes = null)
        {

            if (xmlStream == null)
            {
                throw new System.ArgumentNullException("xmlStream");
            }

            ICollection<T> deserialisedObjects = (ICollection<T>)GetSerialiserInstance<T>(rootAttribute, derivedTypes).Deserialize(xmlStream);

            return deserialisedObjects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <param name="rootAttribute"></param>
        /// <param name="derivedTypes"></param>
        /// <returns></returns>
        public static ICollection<T> XmlDeserialise<T>(string xmlString, XmlRootAttribute rootAttribute, System.Type[] derivedTypes = null)
        {
            ICollection<T> deserialisedObjects;

            using (Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {
                deserialisedObjects = XmlDeserialise<T>(xmlStream, rootAttribute, derivedTypes);
            }

            return deserialisedObjects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serialiseObject"></param>
        /// <param name="xmlStream"></param>
        /// <param name="derivedTypes"></param>
        public static void XmlSerialise<T>(T serialiseObject, out Stream xmlStream, System.Type[] derivedTypes = null)
        {

            if (serialiseObject == null)
            {
                throw new System.ArgumentNullException("serialiseObject");
            }

            xmlStream = new MemoryStream();

            using (Stream serialiseStream = new MemoryStream())
            {
                GetSerialiserInstance<T>(derivedTypes).Serialize(serialiseStream, serialiseObject);
                serialiseStream.Position = 0;
                serialiseStream.CopyTo(xmlStream);
                xmlStream.Position = 0;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serialiseObject"></param>
        /// <param name="xmlString"></param>
        /// <param name="derivedTypes"></param>
        public static void XmlSerialise<T>(T serialiseObject, out string xmlString, System.Type[] derivedTypes = null)
        {
            Stream xmlStream;
            XmlSerialise<T>(serialiseObject, out xmlStream, derivedTypes);
            xmlStream.Position = 0;

            using (StreamReader reader = new StreamReader(xmlStream))
            {
                xmlString = reader.ReadToEnd();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serialiseObjects"></param>
        /// <param name="rootAttribute"></param>
        /// <param name="xmlStream"></param>
        /// <param name="derivedTypes"></param>
        public static void XmlSerialise<T>(IEnumerable<T> serialiseObjects, XmlRootAttribute rootAttribute, out Stream xmlStream, System.Type[] derivedTypes = null)
        {

            if (serialiseObjects == null)
            {
                throw new System.ArgumentNullException("serialiseObjects");
            }

            xmlStream = new MemoryStream();

            using (Stream serialiseStream = new MemoryStream())
            {
                GetSerialiserInstance<T>(rootAttribute, derivedTypes).Serialize(serialiseStream, serialiseObjects);
                serialiseStream.Position = 0;
                serialiseStream.CopyTo(xmlStream);
                xmlStream.Position = 0;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serialiseObjects"></param>
        /// <param name="rootAttribute"></param>
        /// <param name="xmlString"></param>
        /// <param name="derivedTypes"></param>
        public static void XmlSerialise<T>(IEnumerable<T> serialiseObjects, XmlRootAttribute rootAttribute, out string xmlString, System.Type[] derivedTypes = null)
        {
            Stream xmlStream;
            XmlSerialise<T>(serialiseObjects, rootAttribute, out xmlStream, derivedTypes);
            xmlStream.Position = 0;

            using (StreamReader reader = new StreamReader(xmlStream))
            {
                xmlString = reader.ReadToEnd();
            }

        }

    }

}
