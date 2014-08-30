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
using System;
using System.Collections;
using System.Collections.Generic;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Persistence.NHibernate
{

    [TestClass]
    public class EnvironmentRepositoryTest
    {

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
        public void SaveAndRetrieve()
        {
            Environment saved = DataFactory.CreateEnvironmentRequest();
            Guid environmentId = (new EnvironmentRepository()).Save(saved);
            Environment retrieved = (new EnvironmentRepository()).Retrieve(environmentId);
            Assert.AreEqual(saved.Type, retrieved.Type);
            Assert.AreEqual(saved.AuthenticationMethod, retrieved.AuthenticationMethod);
            Assert.AreEqual(saved.ConsumerName, retrieved.ConsumerName);
            Assert.AreEqual(saved.ApplicationInfo.ApplicationKey, retrieved.ApplicationInfo.ApplicationKey);
            Assert.AreEqual(saved.ApplicationInfo.SupportedInfrastructureVersion, retrieved.ApplicationInfo.SupportedInfrastructureVersion);
            Assert.AreEqual(saved.ApplicationInfo.DataModelNamespace, retrieved.ApplicationInfo.DataModelNamespace);
        }

        [TestMethod]
        public void SaveAndRetrieveByExample()
        {
            Environment saved = DataFactory.CreateEnvironmentRequest();
            Guid environmentId = (new EnvironmentRepository()).Save(saved);
            ApplicationInfo applicationInfo = new ApplicationInfo
            {
                ApplicationKey = "UnitTesting",            };
            Environment example = new Environment
            {
                ApplicationInfo = applicationInfo,
                InstanceId = "ThisInstance01",
                SessionToken = "2e5dd3ca282fc8ddb3d08dcacc407e8a",
                SolutionId = "auTestSolution",
                UserToken = "UserToken01"
            };
            IEnumerable<Environment> environments = (new EnvironmentRepository()).Retrieve(example);

            foreach (Environment retrieved in environments)
            {
                Assert.AreEqual(saved.ApplicationInfo.ApplicationKey, retrieved.ApplicationInfo.ApplicationKey);
                Assert.AreEqual(saved.InstanceId, retrieved.InstanceId);
                Assert.AreEqual(saved.SessionToken, retrieved.SessionToken);
                Assert.AreEqual(saved.Id, retrieved.Id);
                Assert.AreEqual(saved.SolutionId, retrieved.SolutionId);
                Assert.AreEqual(saved.UserToken, retrieved.UserToken);
            }

        }

        [TestMethod]
        public void SaveAndRetrieveBySessionToken()
        {
            Environment saved = DataFactory.CreateEnvironmentRequest();
            Guid environmentId = (new EnvironmentRepository()).Save(saved);
            ApplicationInfo applicationInfo = new ApplicationInfo
            {
                ApplicationKey = "UnitTesting",
            };
            Environment example = new Environment
            {
                ApplicationInfo = applicationInfo,
                InstanceId = "ThisInstance01",
                SessionToken = "2e5dd3ca282fc8ddb3d08dcacc407e8a",
                SolutionId = "auTestSolution",
                UserToken = "UserToken01"
            };
            Environment retrieved = (new EnvironmentRepository()).RetrieveBySessionToken("2e5dd3ca282fc8ddb3d08dcacc407e8a");
            Assert.AreEqual(saved.ApplicationInfo.ApplicationKey, retrieved.ApplicationInfo.ApplicationKey);
            Assert.AreEqual(saved.InstanceId, retrieved.InstanceId);
            Assert.AreEqual(saved.SessionToken, retrieved.SessionToken);
            Assert.AreEqual(saved.Id, retrieved.Id);
            Assert.AreEqual(saved.SolutionId, retrieved.SolutionId);
            Assert.AreEqual(saved.UserToken, retrieved.UserToken);
        }

    }

}
