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

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sif.Framework.Service.Serialisation
{

    /// <summary>
    /// 
    /// </summary>
    public static class SerialiserFactory
    {
        private static Dictionary<int, XmlSerializer> xmlSerializers = new Dictionary<int, XmlSerializer>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        private static int GenerateKey(Type[] types)
        {

            unchecked
            {
                int hashcode = 17;

                foreach (Type item in types)
                {
                    hashcode = hashcode * 31 + item.FullName.GetHashCode();
                }

                return hashcode;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootAttribute"></param>
        /// <returns></returns>
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
                XmlSerializer xmlSerializer;

                if (xmlSerializers.TryGetValue(serialiserKey, out xmlSerializer))
                {
                    serialiser = (ISerialiser<T>)xmlSerializer;
                }
                else
                {
                    serialiser = new XmlSerialiser<T>(rootAttribute);
                    xmlSerializers.Add(serialiserKey, (XmlSerializer)serialiser);
                }

            }

            return serialiser;
        }

    }

}
