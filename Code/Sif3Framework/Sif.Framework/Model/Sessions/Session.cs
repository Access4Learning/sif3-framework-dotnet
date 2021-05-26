/*
 * Copyright 2021 Systemic Pty Ltd
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
using System.ComponentModel.DataAnnotations;
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.Model.Sessions
{
    /// <summary>
    /// Session data associated with Consumer/Provider interaction with an Environment Provider or Broker.
    /// </summary>
    public class Session : IHasUniqueIdentifier<Guid>
    {
        /// <summary>
        /// Application key.
        /// </summary>
        [Required]
        public string ApplicationKey { get; set; }

        /// <summary>
        /// Environment URL.
        /// </summary>
        [Required]
        public string EnvironmentUrl { get; set; }

        /// <summary>
        /// Unique identifier for the session.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Instance identifier.
        /// </summary>
        public string InstanceId { get; set; }

        /// <summary>
        /// Queue identifier.
        /// </summary>
        public string QueueId { get; set; }

        /// <summary>
        /// Session token.
        /// </summary>
        [Required]
        public string SessionToken { get; set; }

        /// <summary>
        /// Solution identifier.
        /// </summary>
        public string SolutionId { get; set; }

        /// <summary>
        /// Subscription identifier.
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// User token.
        /// </summary>
        public string UserToken { get; set; }
    }
}