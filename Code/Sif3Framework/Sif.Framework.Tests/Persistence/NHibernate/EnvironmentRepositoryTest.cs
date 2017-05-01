/*
 * Copyright 2017 Systemic Pty Ltd
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
using System.Collections.Generic;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Persistence.NHibernate
{

    /// <summary>
    /// Unit test for EnvironmentRepository.
    /// </summary>
    [TestClass]
    public class EnvironmentRepositoryTest
    {
        private IEnvironmentRepository environmentRepository;

        /// <summary>
        /// Use ClassInitialize to run code before running the first test in the class.
        /// </summary>
        /// <param name="testContext">Context information for the unit test.</param>
        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext)
        {
            DataFactory.CreateDatabase();
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run.
        /// </summary>
        [ClassCleanup()]
        public static void ClassCleanup()
        {
        }

        /// <summary>
        /// Use TestInitialize to run code before running each test.
        /// </summary>
        [TestInitialize()]
        public void TestInitialize()
        {
            environmentRepository = new EnvironmentRepository();
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run.
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        /// <summary>
        /// Save a new Environment and then retreieve it.
        /// </summary>
        [TestMethod]
        public void SaveAndRetrieve()
        {

            // Save a new Environment and then retrieve it using it's identifier.
            Environment saved = DataFactory.CreateEnvironmentRequest();
            Guid environmentId = environmentRepository.Save(saved);
            Environment retrieved = environmentRepository.Retrieve(environmentId);

            // Assert that the retrieved Environment matches the saved Environment.
            Assert.AreEqual(saved.Type, retrieved.Type);
            Assert.AreEqual(saved.AuthenticationMethod, retrieved.AuthenticationMethod);
            Assert.AreEqual(saved.ConsumerName, retrieved.ConsumerName);
            Assert.AreEqual(saved.ApplicationInfo.ApplicationKey, retrieved.ApplicationInfo.ApplicationKey);
            Assert.AreEqual(saved.ApplicationInfo.SupportedInfrastructureVersion, retrieved.ApplicationInfo.SupportedInfrastructureVersion);
            Assert.AreEqual(saved.ApplicationInfo.DataModelNamespace, retrieved.ApplicationInfo.DataModelNamespace);
        }

        /// <summary>
        /// Save a new Environment and then retreieve it using an example Environment instance.
        /// </summary>
        [TestMethod]
        public void SaveAndRetrieveByExample()
        {

            // Save a new Environment.
            Environment saved = DataFactory.CreateEnvironmentRequest();
            environmentRepository.Save(saved);

            // Create an example Environment instance for use in the retrieve call.
            ApplicationInfo applicationInfo = new ApplicationInfo { ApplicationKey = "UnitTesting" };
            Environment example = new Environment
            {
                ApplicationInfo = applicationInfo,
                InstanceId = saved.InstanceId,
                SessionToken = saved.SessionToken,
                SolutionId = saved.SolutionId,
                UserToken = saved.UserToken
            };

            // Retrieve Environments based on the example Environment instance.
            IEnumerable<Environment> environments = environmentRepository.Retrieve(example);

            // Assert that the retrieved Environments match properties of the example Environment instance.
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

        /// <summary>
        /// Save a new Environment and then retreieve it using its session token.
        /// </summary>
        [TestMethod]
        public void SaveAndRetrieveBySessionToken()
        {

            // Save a new Environment and then retrieve it based on it's session token.
            Environment saved = DataFactory.CreateEnvironmentRequest();
            environmentRepository.Save(saved);
            Environment retrieved = environmentRepository.RetrieveBySessionToken(saved.SessionToken);

            // Assert that the retrieved Environment matches the saved Environment.
            Assert.AreEqual(saved.ApplicationInfo.ApplicationKey, retrieved.ApplicationInfo.ApplicationKey);
            Assert.AreEqual(saved.InstanceId, retrieved.InstanceId);
            Assert.AreEqual(saved.SessionToken, retrieved.SessionToken);
            Assert.AreEqual(saved.Id, retrieved.Id);
            Assert.AreEqual(saved.SolutionId, retrieved.SolutionId);
            Assert.AreEqual(saved.UserToken, retrieved.UserToken);
        }

    }

}
