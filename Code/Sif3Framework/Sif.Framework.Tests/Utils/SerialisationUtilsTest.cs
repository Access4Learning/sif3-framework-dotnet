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

using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private string environmentXmlFile = "Data files\\environment.xml";

        [TestMethod]
        public void environmentType_Serialisation()
        {
            environmentType environmentType;

            using (FileStream xmlStream = File.OpenRead(environmentXmlFile))
            {
                environmentType = SerialisationUtils.XmlDeserialise<environmentType>(xmlStream);
            }

            Assert.AreEqual(environmentType.sessionToken, "2e5dd3ca282fc8ddb3d08dcacc407e8a", true, "Session token does not match.");

            string environmentText;

            using (StreamReader reader = new StreamReader(environmentXmlFile))
            {
                environmentText = reader.ReadToEnd();
            }

            string xmlString;
            SerialisationUtils.XmlSerialise<environmentType>(environmentType, out xmlString);
            Assert.AreEqual(xmlString, environmentText.Trim(), true, "Environment deserialised does not match serialised version.");
            System.Console.WriteLine(xmlString);
        }

        [TestMethod]
        public void environmentTypes_Serialisation()
        {
            environmentType environmentType1;

            using (FileStream xmlStream = File.OpenRead(environmentXmlFile))
            {
                environmentType1 = SerialisationUtils.XmlDeserialise<environmentType>(xmlStream);
            }

            Assert.AreEqual(environmentType1.sessionToken, "2e5dd3ca282fc8ddb3d08dcacc407e8a", true, "Session token does not match.");

            environmentType environmentType2;

            using (FileStream xmlStream = File.OpenRead(environmentXmlFile))
            {
                environmentType2 = SerialisationUtils.XmlDeserialise<environmentType>(xmlStream);
            }

            Assert.AreEqual(environmentType2.sessionToken, "2e5dd3ca282fc8ddb3d08dcacc407e8a", true, "Session token does not match.");

            ICollection<environmentType> environmentTypes = new Collection<environmentType>
            {
                environmentType1,
                environmentType2
            };

            string xmlString;
            SerialisationUtils.XmlSerialise<environmentType>(environmentTypes, new XmlRootAttribute("environments"), out xmlString);
            System.Console.WriteLine(xmlString);

            environmentTypes = SerialisationUtils.XmlDeserialise<environmentType>(xmlString, new XmlRootAttribute("environments"));
            System.Console.WriteLine("Number deserialised is " + environmentTypes.Count);
        }

    }

}
