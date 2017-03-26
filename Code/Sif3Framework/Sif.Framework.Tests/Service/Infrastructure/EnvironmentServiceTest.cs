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
using Sif.Framework.Persistence;
using Sif.Framework.Persistence.NHibernate;
using Sif.Specification.Infrastructure;
using System;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Infrastructure
{

    /// <summary>
    /// Unit test for EnvironmentService.
    /// </summary>
    [TestClass]
    public class EnvironmentServiceTest
    {
        private IEnvironmentRepository environmentRepository;
        private IEnvironmentService environmentService;

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
            environmentService = new EnvironmentService();
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run.
        /// </summary>
        [TestCleanup()]
        public void TestCleanup()
        {
        }

        /// <summary>
        /// Save a new Environment and then retreieve it.
        /// </summary>
        [TestMethod]
        public void Retrieve()
        {

            // Save a new Environment and then retrieve it using it's identifier.
            Environment saved = DataFactory.CreateEnvironmentRequest();
            Guid environmentId = environmentRepository.Save(saved);
            environmentType retrieved = environmentService.Retrieve(environmentId);

            // Assert that the retrieved Environment matches the saved Environment.
            Assert.AreEqual(saved.ApplicationInfo.AdapterProduct.IconURI, retrieved.applicationInfo.adapterProduct.iconURI);
            Assert.AreEqual(saved.ApplicationInfo.AdapterProduct.ProductName, retrieved.applicationInfo.adapterProduct.productName);
            Assert.AreEqual(saved.ApplicationInfo.AdapterProduct.ProductVersion, retrieved.applicationInfo.adapterProduct.productVersion);
            Assert.AreEqual(saved.ApplicationInfo.AdapterProduct.VendorName, retrieved.applicationInfo.adapterProduct.vendorName);
            Assert.AreEqual(saved.ApplicationInfo.ApplicationKey, retrieved.applicationInfo.applicationKey);
            Assert.AreEqual(saved.ApplicationInfo.ApplicationProduct.IconURI, retrieved.applicationInfo.applicationProduct.iconURI);
            Assert.AreEqual(saved.ApplicationInfo.ApplicationProduct.ProductName, retrieved.applicationInfo.applicationProduct.productName);
            Assert.AreEqual(saved.ApplicationInfo.ApplicationProduct.ProductVersion, retrieved.applicationInfo.applicationProduct.productVersion);
            Assert.AreEqual(saved.ApplicationInfo.ApplicationProduct.VendorName, retrieved.applicationInfo.applicationProduct.vendorName);
            Assert.AreEqual(saved.ApplicationInfo.DataModelNamespace, retrieved.applicationInfo.dataModelNamespace);
            Assert.AreEqual(saved.ApplicationInfo.SupportedInfrastructureVersion, retrieved.applicationInfo.supportedInfrastructureVersion);
            Assert.AreEqual(saved.ApplicationInfo.Transport, retrieved.applicationInfo.transport);
            Assert.AreEqual(saved.AuthenticationMethod, retrieved.authenticationMethod);
            Assert.AreEqual(saved.ConsumerName, retrieved.consumerName);
            Assert.AreEqual(saved.DefaultZone.Description, retrieved.defaultZone.description);
        }

    }

}
