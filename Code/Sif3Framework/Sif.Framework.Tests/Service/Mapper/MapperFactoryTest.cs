/*
 * Copyright 2022 Systemic Pty Ltd
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
using Sif.Framework.Model.Responses;
using Sif.Framework.Service.Mapper;
using Sif.Specification.Infrastructure;
using System.Collections.Generic;

namespace Sif.Framework.Tests.Service.Mapper
{
    /// <summary>
    /// Unit test for the MapperFactory.
    /// </summary>
    [TestClass]
    public class MapperFactoryTest
    {
        /// <summary>
        /// Mapper test between Environment and environmentType.
        /// </summary>
        [TestMethod]
        public void EnvironmentRequestMapperTest()
        {
            Environment source = DataFactory.CreateEnvironmentRequest();
            environmentType destination = MapperFactory.CreateInstance<Environment, environmentType>(source);

            // Assert that the mapping was successful.
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.IconUri, destination.applicationInfo.adapterProduct.iconURI);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.ProductName, destination.applicationInfo.adapterProduct.productName);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.ProductVersion, destination.applicationInfo.adapterProduct.productVersion);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.VendorName, destination.applicationInfo.adapterProduct.vendorName);
            Assert.AreEqual(source.ApplicationInfo.ApplicationKey, destination.applicationInfo.applicationKey);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.IconUri, destination.applicationInfo.applicationProduct.iconURI);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.ProductName, destination.applicationInfo.applicationProduct.productName);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.ProductVersion, destination.applicationInfo.applicationProduct.productVersion);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.VendorName, destination.applicationInfo.applicationProduct.vendorName);
            Assert.AreEqual(source.ApplicationInfo.DataModelNamespace, destination.applicationInfo.dataModelNamespace);
            Assert.AreEqual(source.ApplicationInfo.SupportedInfrastructureVersion, destination.applicationInfo.supportedInfrastructureVersion);
            Assert.AreEqual(source.ApplicationInfo.Transport, destination.applicationInfo.transport);
            Assert.AreEqual(source.AuthenticationMethod, destination.authenticationMethod);
            Assert.AreEqual(source.ConsumerName, destination.consumerName);
            Assert.AreEqual(source.DefaultZone.Description, destination.defaultZone.description);
            Assert.AreEqual(source.DefaultZone.SifId, destination.defaultZone.id);
            Assert.AreEqual(source.InstanceId, destination.instanceId);
            Assert.AreEqual(source.SessionToken, destination.sessionToken);
            Assert.AreEqual(source.SolutionId, destination.solutionId);
            Assert.AreEqual(source.Type.ToString(), destination.type.ToString());
            Assert.AreEqual(source.UserToken, destination.userToken);
            Assert.AreEqual(source.Id.ToString(), destination.id);
        }

        /// <summary>
        /// Mapper test between Environment and environmentType.
        /// </summary>
        [TestMethod]
        public void EnvironmentResponseMapperTest()
        {
            Environment source = DataFactory.CreateEnvironmentResponse();
            environmentType destination = MapperFactory.CreateInstance<Environment, environmentType>(source);

            // Assert that the mapping was successful.
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.IconUri, destination.applicationInfo.adapterProduct.iconURI);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.ProductName, destination.applicationInfo.adapterProduct.productName);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.ProductVersion, destination.applicationInfo.adapterProduct.productVersion);
            Assert.AreEqual(source.ApplicationInfo.AdapterProduct.VendorName, destination.applicationInfo.adapterProduct.vendorName);
            Assert.AreEqual(source.ApplicationInfo.ApplicationKey, destination.applicationInfo.applicationKey);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.IconUri, destination.applicationInfo.applicationProduct.iconURI);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.ProductName, destination.applicationInfo.applicationProduct.productName);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.ProductVersion, destination.applicationInfo.applicationProduct.productVersion);
            Assert.AreEqual(source.ApplicationInfo.ApplicationProduct.VendorName, destination.applicationInfo.applicationProduct.vendorName);
            Assert.AreEqual(source.ApplicationInfo.DataModelNamespace, destination.applicationInfo.dataModelNamespace);
            Assert.AreEqual(source.ApplicationInfo.SupportedInfrastructureVersion, destination.applicationInfo.supportedInfrastructureVersion);
            Assert.AreEqual(source.ApplicationInfo.Transport, destination.applicationInfo.transport);
            Assert.AreEqual(source.AuthenticationMethod, destination.authenticationMethod);
            Assert.AreEqual(source.ConsumerName, destination.consumerName);
            Assert.AreEqual(source.DefaultZone.Description, destination.defaultZone.description);
            Assert.AreEqual(source.DefaultZone.SifId, destination.defaultZone.id);
            Assert.AreEqual(source.Id.ToString(), destination.id);
            var index = 0;

            foreach (InfrastructureService sourceProperty in source.InfrastructureServices.Values)
            {
                Assert.AreEqual(sourceProperty.Name.ToString(), destination.infrastructureServices[index].name.ToString());
                Assert.AreEqual(sourceProperty.Value, destination.infrastructureServices[index].Value);
                index++;
            }

            index = 0;

            foreach (ProvisionedZone sourceProvisionedZone in source.ProvisionedZones.Values)
            {
                Assert.AreEqual(sourceProvisionedZone.SifId, destination.provisionedZones[index].id);
                var sourceIndex = 0;

                foreach (Model.Infrastructure.Service sourceService in sourceProvisionedZone.Services)
                {
                    Assert.AreEqual(sourceService.ContextId, destination.provisionedZones[index].services[sourceIndex].contextId);
                    Assert.AreEqual(sourceService.Name, destination.provisionedZones[index].services[sourceIndex].name);
                    Assert.AreEqual(sourceService.Type, destination.provisionedZones[index].services[sourceIndex].type);
                    var rightIndex = 0;

                    foreach (Right sourceRight in sourceService.Rights)
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

        /// <summary>
        /// Mapper test between response types.
        /// </summary>
        [TestMethod]
        public void ExplicitResponseMapperTest()
        {
            // Error.
            var srcError = new ResponseError { Code = 123, Description = "Err desc", Id = "42", Message = "Error occurred", Scope = "request" };
            errorType destError = MapperFactory.CreateInstance<ResponseError, errorType>(srcError);

            // Create.
            var srcCreateStatus = new CreateStatus { AdvisoryId = "src456", Error = srcError, Id = "cr8", StatusCode = "200" };
            createType destCreateStatus = MapperFactory.CreateInstance<CreateStatus, createType>(srcCreateStatus);
            var srcCreateResponse = new MultipleCreateResponse { StatusRecords = new List<CreateStatus> { srcCreateStatus } };
            createResponseType destCreateResponse = MapperFactory.CreateInstance<MultipleCreateResponse, createResponseType>(srcCreateResponse);
            var index = 0;

            // Assert that the mapping was successful.
            foreach (CreateStatus record in srcCreateResponse.StatusRecords)
            {
                Assert.AreEqual(record.AdvisoryId, destCreateResponse.creates[index].advisoryId);
                Assert.AreEqual(record.Error.Code, destCreateResponse.creates[index].error.code);
                Assert.AreEqual(record.Error.Description, destCreateResponse.creates[index].error.description);
                Assert.AreEqual(record.Error.Id, destCreateResponse.creates[index].error.id);
                Assert.AreEqual(record.Error.Message, destCreateResponse.creates[index].error.message);
                Assert.AreEqual(record.Error.Scope, destCreateResponse.creates[index].error.scope);
                Assert.AreEqual(record.Id, destCreateResponse.creates[index].id);
                Assert.AreEqual(record.StatusCode, destCreateResponse.creates[index].statusCode);
                index++;
            }

            // Delete.
            var srcDeleteStatus = new DeleteStatus { Error = srcError, Id = "del8", StatusCode = "300" };
            deleteStatus destDeleteStatus = MapperFactory.CreateInstance<DeleteStatus, deleteStatus>(srcDeleteStatus);
            var srcDeleteResponse = new MultipleDeleteResponse { StatusRecords = new List<DeleteStatus> { srcDeleteStatus } };
            deleteResponseType destDeleteResponse = MapperFactory.CreateInstance<MultipleDeleteResponse, deleteResponseType>(srcDeleteResponse);
            index = 0;

            // Assert that the mapping was successful.
            foreach (DeleteStatus record in srcDeleteResponse.StatusRecords)
            {
                Assert.AreEqual(record.Error.Code, destDeleteResponse.deletes[index].error.code);
                Assert.AreEqual(record.Error.Description, destDeleteResponse.deletes[index].error.description);
                Assert.AreEqual(record.Error.Id, destDeleteResponse.deletes[index].error.id);
                Assert.AreEqual(record.Error.Message, destDeleteResponse.deletes[index].error.message);
                Assert.AreEqual(record.Error.Scope, destDeleteResponse.deletes[index].error.scope);
                Assert.AreEqual(record.Id, destDeleteResponse.deletes[index].id);
                Assert.AreEqual(record.StatusCode, destDeleteResponse.deletes[index].statusCode);
                index++;
            }

            // Update.
            var srcUpdateStatus = new UpdateStatus { Error = srcError, Id = "up8", StatusCode = "400" };
            updateType destUpdateStatus = MapperFactory.CreateInstance<UpdateStatus, updateType>(srcUpdateStatus);
            var srcUpdateResponse = new MultipleUpdateResponse { StatusRecords = new List<UpdateStatus> { srcUpdateStatus } };
            updateResponseType destUpdateResponse = MapperFactory.CreateInstance<MultipleUpdateResponse, updateResponseType>(srcUpdateResponse);
            index = 0;

            // Assert that the mapping was successful.
            foreach (UpdateStatus record in srcUpdateResponse.StatusRecords)
            {
                Assert.AreEqual(record.Error.Code, destUpdateResponse.updates[index].error.code);
                Assert.AreEqual(record.Error.Description, destUpdateResponse.updates[index].error.description);
                Assert.AreEqual(record.Error.Id, destUpdateResponse.updates[index].error.id);
                Assert.AreEqual(record.Error.Message, destUpdateResponse.updates[index].error.message);
                Assert.AreEqual(record.Error.Scope, destUpdateResponse.updates[index].error.scope);
                Assert.AreEqual(record.Id, destUpdateResponse.updates[index].id);
                Assert.AreEqual(record.StatusCode, destUpdateResponse.updates[index].statusCode);
                index++;
            }
        }
    }
}