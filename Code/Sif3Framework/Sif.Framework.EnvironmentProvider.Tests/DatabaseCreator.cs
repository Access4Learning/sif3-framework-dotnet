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
using Sif.Framework.Model.Infrastructure;

namespace Sif.Framework.Persistence.NHibernate
{

    [TestClass]
    public class DatabaseCreator
    {

        private static void PopulateEnvironmentRegister()
        {
            EnvironmentRegister environmentRegister = DataFactory.CreateEnvironmentRegister();
            environmentRegister.ConsumerSecret = "SecretDem0";
            (new EnvironmentRegisterRepository()).Save(environmentRegister);
        }

        private static void PopulateEnvironmentRequest()
        {
            Environment environment = DataFactory.CreateEnvironmentRequest();
            (new EnvironmentRepository()).Save(environment);
        }

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            DataFactory.CreateDatabase();
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup() { }

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() { }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() { }

        [TestMethod]
        public void PopulateDatabase()
        {
            PopulateEnvironmentRegister();
            PopulateEnvironmentRequest();
        }

    }

}
