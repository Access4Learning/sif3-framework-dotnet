﻿/*
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

using System;
using System.Collections.Generic;
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.Models.Infrastructure
{
    /// <summary>
    /// The following Environment elements/properties are mandatory according to the SIF specification:
    /// /environment[@type]
    /// /environment/authenticationMethod
    /// /environment/consumerName
    /// /environment/applicationInfo/applicationKey
    /// /environment/applicationInfo/supportedInfrastructureVersion
    /// /environment/applicationInfo/supportedDataModel
    /// /environment/applicationInfo/supportedDataModelVersion
    /// </summary>
    public class Environment : IHasUniqueIdentifier<Guid>
    {
        public virtual ApplicationInfo ApplicationInfo { get; set; }

        /// <summary>
        /// Defines the way in which the applicationKey can be used to enforce security.
        /// </summary>
        public virtual string AuthenticationMethod { get; set; }

        /// <summary>
        /// A descriptive name for the application that will be readily identifiable to Zone Administrators if it
        /// becomes a Registered Consumer.
        /// </summary>
        public virtual string ConsumerName { get; set; }

        /// <summary>
        /// The default zone used by Consumer (and Provider?) service requests when no Zone is provided with the
        /// request.
        /// </summary>
        public virtual Zone DefaultZone { get; set; }

        /// <summary>
        /// The ID of the Environment as managed by the Environment Provider.
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// There must be an InfrastructureService element present for each defined Infrastructure Service. The value
        /// of each InfrastructureService Property value sub-element defines the URL location of that Infrastructure
        /// Service.
        /// </summary>
        public virtual ICollection<InfrastructureService> InfrastructureServices { get; set; }

        public virtual string InstanceId { get; set; }

        public virtual ICollection<ProvisionedZone> ProvisionedZones { get; set; }

        /// <summary>
        /// The ID associated with an instance of the Environment.
        /// </summary>
        public virtual string SessionToken { get; set; }

        /// <summary>
        /// The solution the Application would like to participate in. This is optional only, is advisory, and may be
        /// ignored by the Administrator. If processed it may be reflected in the URLs of the infrastructure services
        /// which are provided in the consumerEnvironment.
        /// </summary>
        public virtual string SolutionId { get; set; }

        /// <summary>
        /// Defines whether the connection to the Environment is DIRECT or BROKERED.
        /// </summary>
        public virtual EnvironmentType? Type { get; set; }

        public virtual string UserToken { get; set; }

        public Environment()
        {
        }

        public Environment(
            string applicationKey,
            string instanceId = null,
            string userToken = null,
            string solutionId = null)
        {
            if (!string.IsNullOrWhiteSpace(applicationKey))
            {
                ApplicationInfo = new ApplicationInfo
                {
                    ApplicationKey = applicationKey
                };
            }

            if (!string.IsNullOrWhiteSpace(instanceId))
            {
                InstanceId = instanceId;
            }

            if (!string.IsNullOrWhiteSpace(userToken))
            {
                UserToken = userToken;
            }

            if (!string.IsNullOrWhiteSpace(solutionId))
            {
                SolutionId = solutionId;
            }
        }
    }
}