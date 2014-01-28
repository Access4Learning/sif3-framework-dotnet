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
using Sif.Framework.Infrastructure;
using Sif.Framework.Model.Infrastructure;
using System.IO;

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

    }

}
