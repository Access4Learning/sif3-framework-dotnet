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
using Sif.Specification.Infrastructure;

namespace Sif.Framework.Service.Mapper
{

    [TestClass]
    public class MapperFactoryTest
    {

        [TestMethod]
        public void EnvironmentRequestMapperTest()
        {
            Environment source = DataFactory.CreateEnvironmentRequest();
            environmentType destination = MapperFactory.CreateInstance<Environment, environmentType>(source);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.IconURI, destination.applicationInfo.adapterProduct.iconURI);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.ProductName, destination.applicationInfo.adapterProduct.productName);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.ProductVersion, destination.applicationInfo.adapterProduct.productVersion);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.VendorName, destination.applicationInfo.adapterProduct.vendorName);
            Assert.AreEqual(source.ApplicationInfo.ApplicationKey, destination.applicationInfo.applicationKey);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.IconURI, destination.applicationInfo.applicationProduct.iconURI);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.ProductName, destination.applicationInfo.applicationProduct.productName);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.ProductVersion, destination.applicationInfo.applicationProduct.productVersion);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.VendorName, destination.applicationInfo.applicationProduct.vendorName);
            Assert.AreEqual(source.ApplicationInfo.DataModelNamespace, destination.applicationInfo.dataModelNamespace);
            Assert.AreEqual(source.ApplicationInfo.SupportedInfrastructureVersion, destination.applicationInfo.supportedInfrastructureVersion);
            Assert.AreEqual(source.ApplicationInfo.Transport, destination.applicationInfo.transport);
            Assert.AreEqual(source.AuthenticationMethod, destination.authenticationMethod);
            Assert.AreEqual(source.ConsumerName, destination.consumerName);
            Assert.AreEqual(source.DefaultZone.Description, destination.defaultZone.description);
            Assert.AreEqual(source.DefaultZone.Id.ToString(), destination.defaultZone.id);
            Assert.AreEqual(source.InstanceId, destination.instanceId);
            Assert.AreEqual(source.SessionToken, destination.sessionToken);
            Assert.AreEqual(source.SolutionId, destination.solutionId);
            Assert.AreEqual(source.Type.ToString(), destination.type.ToString());
            Assert.AreEqual(source.UserToken, destination.userToken);
            Assert.AreEqual(source.Id.ToString(), destination.id);
        }

        [TestMethod]
        public void EnvironmentResponseMapperTest()
        {
            Environment source = DataFactory.CreateEnvironmentResponse();
            environmentType destination = MapperFactory.CreateInstance<Environment, environmentType>(source);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.IconURI, destination.applicationInfo.adapterProduct.iconURI);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.ProductName, destination.applicationInfo.adapterProduct.productName);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.ProductVersion, destination.applicationInfo.adapterProduct.productVersion);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.VendorName, destination.applicationInfo.adapterProduct.vendorName);
            Assert.AreEqual(source.ApplicationInfo.ApplicationKey, destination.applicationInfo.applicationKey);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.IconURI, destination.applicationInfo.applicationProduct.iconURI);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.ProductName, destination.applicationInfo.applicationProduct.productName);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.ProductVersion, destination.applicationInfo.applicationProduct.productVersion);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.VendorName, destination.applicationInfo.applicationProduct.vendorName);
            Assert.AreEqual(source.ApplicationInfo.DataModelNamespace, destination.applicationInfo.dataModelNamespace);
            Assert.AreEqual(source.ApplicationInfo.SupportedInfrastructureVersion, destination.applicationInfo.supportedInfrastructureVersion);
            Assert.AreEqual(source.ApplicationInfo.Transport, destination.applicationInfo.transport);
            Assert.AreEqual(source.AuthenticationMethod, destination.authenticationMethod);
            Assert.AreEqual(source.ConsumerName, destination.consumerName);
            Assert.AreEqual(source.DefaultZone.Description, destination.defaultZone.description);
            Assert.AreEqual(source.DefaultZone.Id.ToString(), destination.defaultZone.id);
            Assert.AreEqual(source.Id.ToString(), destination.id);
            int index = 0;

            foreach (InfrastructureService sourceProperty in source.InfrastructureServices.Values)
            {
                Assert.AreEqual(sourceProperty.Name.ToString(), destination.infrastructureServices[index].name.ToString());
                Assert.AreEqual(sourceProperty.Value, destination.infrastructureServices[index].Value);
                index++;
            }

            index = 0;

            foreach (ProvisionedZone sourceProvisionedZone in source.ProvisionedZones.Values)
            {
                Assert.AreEqual(sourceProvisionedZone.SifId , destination.provisionedZones[index].id);
                int sourceIndex = 0;

                foreach (Model.Infrastructure.Service sourceService in sourceProvisionedZone.Services)
                {
                    Assert.AreEqual(sourceService.ContextId, destination.provisionedZones[index].services[sourceIndex].contextId);
                    Assert.AreEqual(sourceService.Name, destination.provisionedZones[index].services[sourceIndex].name);
                    Assert.AreEqual(sourceService.Type, destination.provisionedZones[index].services[sourceIndex].type);
                    int rightIndex = 0;

                    foreach (Right sourceRight in sourceService.Rights.Values)
                    {
                        Assert.AreEqual(sourceRight.Type, destination.provisionedZones[index].services[sourceIndex].rights[rightIndex].type);
                        Assert.AreEqual(sourceRight.Value, destination.provisionedZones[index].services[sourceIndex].rights[rightIndex].Value);
                        rightIndex++;
                    }

                    sourceIndex++;
                }

                index++;
            }

        }

    }

}
