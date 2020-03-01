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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sif.Framework.Service.Serialisation;
using Sif.Specification.Infrastructure;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace Sif.Framework.Utils
{

    [TestClass]
    public class SerialisationUtilsTest
    {
        private readonly string environmentXmlFile = "Data files\\environment.xml";
        private readonly string environmentJsonFile = "Data files\\environment.json";

        [TestMethod]
        public void JsonEnvironment_Serialisation()
        {
            environmentType environmentType;

            using (FileStream stream = File.OpenRead(environmentJsonFile))
            {
                environmentType = SerialiserFactory.GetJsonSerialiser<environmentType>().Deserialise(stream);
            }

            Assert.AreEqual(environmentType.sessionToken, "2e5dd3ca282fc8ddb3d08dcacc407e8a", true, "Session token does not match.");

            string environmentText;

            using (StreamReader reader = new StreamReader(environmentJsonFile))
            {
                environmentText = reader.ReadToEnd();
            }

            string jsonString = SerialiserFactory.GetJsonSerialiser<environmentType>().Serialise(environmentType);
            //Assert.AreEqual(jsonString, environmentText.Trim(), true, "Environment deserialised does not match serialised version.");
            System.Console.WriteLine(jsonString);
        }

        [TestMethod]
        public void JsonEnvironments_Serialisation()
        {
            environmentType environmentType1;

            using (FileStream stream = File.OpenRead(environmentJsonFile))
            {
                environmentType1 = SerialiserFactory.GetJsonSerialiser<environmentType>().Deserialise(stream);
            }

            Assert.AreEqual(environmentType1.sessionToken, "2e5dd3ca282fc8ddb3d08dcacc407e8a", true, "Session token does not match.");

            environmentType environmentType2;

            using (FileStream stream = File.OpenRead(environmentJsonFile))
            {
                environmentType2 = SerialiserFactory.GetJsonSerialiser<environmentType>().Deserialise(stream);
            }

            Assert.AreEqual(environmentType2.sessionToken, "2e5dd3ca282fc8ddb3d08dcacc407e8a", true, "Session token does not match.");

            ICollection<environmentType> environmentTypes = new Collection<environmentType>
            {
                environmentType1,
                environmentType2
            };

            XmlRootAttribute xmlRootAttribute = new XmlRootAttribute("environments") { Namespace = SettingsManager.ConsumerSettings.DataModelNamespace, IsNullable = false };

            string jsonString = SerialiserFactory.GetJsonSerialiser<Collection<environmentType>>(xmlRootAttribute).Serialise((Collection<environmentType>)environmentTypes);
            System.Console.WriteLine(jsonString);

            environmentTypes = SerialiserFactory.GetJsonSerialiser<Collection<environmentType>>(xmlRootAttribute).Deserialise(jsonString);
            System.Console.WriteLine("Number deserialised is " + environmentTypes.Count);
        }

        [TestMethod]
        public void XmlEnvironment_Serialisation()
        {
            environmentType environmentType;

            using (FileStream xmlStream = File.OpenRead(environmentXmlFile))
            {
                environmentType = SerialiserFactory.GetXmlSerialiser<environmentType>().Deserialise(xmlStream);
            }

            Assert.AreEqual(environmentType.sessionToken, "2e5dd3ca282fc8ddb3d08dcacc407e8a", true, "Session token does not match.");

            string environmentText;

            using (StreamReader reader = new StreamReader(environmentXmlFile))
            {
                environmentText = reader.ReadToEnd();
            }

            string xmlString = SerialiserFactory.GetXmlSerialiser<environmentType>().Serialise(environmentType);
            Assert.AreEqual(xmlString, environmentText.Trim(), true, "Environment deserialised does not match serialised version.");
            System.Console.WriteLine(xmlString);
        }

        [TestMethod]
        public void XmlEnvironments_Serialisation()
        {
            environmentType environmentType1;

            using (FileStream xmlStream = File.OpenRead(environmentXmlFile))
            {
                environmentType1 = SerialiserFactory.GetXmlSerialiser<environmentType>().Deserialise(xmlStream);
            }

            Assert.AreEqual(environmentType1.sessionToken, "2e5dd3ca282fc8ddb3d08dcacc407e8a", true, "Session token does not match.");

            environmentType environmentType2;

            using (FileStream xmlStream = File.OpenRead(environmentXmlFile))
            {
                environmentType2 = SerialiserFactory.GetXmlSerialiser<environmentType>().Deserialise(xmlStream);
            }

            Assert.AreEqual(environmentType2.sessionToken, "2e5dd3ca282fc8ddb3d08dcacc407e8a", true, "Session token does not match.");

            ICollection<environmentType> environmentTypes = new Collection<environmentType>
            {
                environmentType1,
                environmentType2
            };

            XmlRootAttribute xmlRootAttribute = new XmlRootAttribute("environments") { Namespace = SettingsManager.ConsumerSettings.DataModelNamespace, IsNullable = false };

            string xmlString = SerialiserFactory.GetXmlSerialiser<Collection<environmentType>>(xmlRootAttribute).Serialise((Collection<environmentType>)environmentTypes);
            System.Console.WriteLine(xmlString);

            environmentTypes = SerialiserFactory.GetXmlSerialiser<Collection<environmentType>>(xmlRootAttribute).Deserialise(xmlString);
            System.Console.WriteLine("Number deserialised is " + environmentTypes.Count);
        }

    }

}
