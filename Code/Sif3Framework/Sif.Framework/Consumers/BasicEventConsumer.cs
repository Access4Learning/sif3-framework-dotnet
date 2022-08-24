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

using Sif.Framework.Models.DataModels;
using Sif.Framework.Models.Infrastructure;
using Sif.Framework.Models.Settings;
using Sif.Framework.Services.Sessions;
using System.Collections.Generic;

namespace Sif.Framework.Consumers
{
    /// <summary>
    /// This is a convenience class for SIF Event Consumers of SIF data model objects whereby the primary key is of
    /// type System.String and the multiple objects entity is represented as a list of single objects.
    /// </summary>
    /// <typeparam name="T">Type that defines a SIF data model object.</typeparam>
    public abstract class BasicEventConsumer<T> : EventConsumer<T, List<T>, string> where T : ISifRefId<string>
    {
        /// <inheritdoc />
        protected BasicEventConsumer(
            Environment environment,
            IFrameworkSettings settings = null,
            ISessionService sessionService = null)
            : base(environment, settings, sessionService)
        {
        }

        /// <inheritdoc />
        protected BasicEventConsumer(
            string applicationKey,
            string instanceId = null,
            string userToken = null,
            string solutionId = null,
            IFrameworkSettings settings = null,
            ISessionService sessionService = null)
            : base(applicationKey, instanceId, userToken, solutionId, settings, sessionService)
        {
        }
    }
}