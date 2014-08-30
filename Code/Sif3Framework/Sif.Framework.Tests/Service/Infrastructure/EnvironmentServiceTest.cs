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
using Sif.Framework.Persistence.NHibernate;
using Sif.Specification.Infrastructure;

namespace Sif.Framework.Service.Infrastructure
{

    [TestClass]
    public class EnvironmentServiceTest
    {

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            DataFactory.CreateDatabase();
        }

        [TestMethod]
        public void Retrieve()
        {
            Environment saved = DataFactory.CreateEnvironmentRequest();
            (new EnvironmentRepository()).Save(saved);
            environmentType retrieved = (new EnvironmentService()).Retrieve(saved.Id);
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
