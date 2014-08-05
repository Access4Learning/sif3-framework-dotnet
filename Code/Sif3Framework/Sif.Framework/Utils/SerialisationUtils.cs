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

    public class SerialisationUtils
    {
        private static Dictionary<int, XmlSerializer> serializers = new Dictionary<int, XmlSerializer>();

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

        public static T XmlDeserialise<T>(Stream xmlStream, System.Type[] derivedTypes = null)
        {

            if (xmlStream == null)
            {
                throw new System.ArgumentNullException("xmlStream");
            }

            T deserialisedObject = (T)GetSerialiserInstance<T>(derivedTypes).Deserialize(xmlStream);

            return deserialisedObject;
        }

        public static T XmlDeserialise<T>(string xmlString, System.Type[] derivedTypes = null)
        {
            T deserialisedObject;

            using (Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {
                deserialisedObject = XmlDeserialise<T>(xmlStream, derivedTypes);
            }

            return deserialisedObject;
        }

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

    }

}
