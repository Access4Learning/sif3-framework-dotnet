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

using Sif.Framework.Consumers;
using Sif.Framework.Demo.Au.Consumer.Models;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Responses;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Sessions;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Au.Consumer.Consumers
{
    /// <inheritdoc />
    internal class StudentPersonalEventConsumer : BasicEventConsumer<StudentPersonal>
    {
        private static readonly slf4net.ILogger Log =
            slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <inheritdoc />
        public StudentPersonalEventConsumer(
            Environment environment,
            IFrameworkSettings settings = null,
            ISessionService sessionService = null)
            : base(environment, settings, sessionService)
        {
        }

        /// <inheritdoc />
        public StudentPersonalEventConsumer(
            string applicationKey,
            string instanceId = null,
            string userToken = null,
            string solutionId = null,
            IFrameworkSettings settings = null,
            ISessionService sessionService = null)
            : base(applicationKey, instanceId, userToken, solutionId, settings, sessionService)
        {
        }

        /// <inheritdoc />
        public override void OnCreateEvent(
            List<StudentPersonal> students,
            string zoneId = null,
            string contextId = null)
        {
            if (Log.IsDebugEnabled) Log.Debug("*** OnCreateEvent handler called ...");
            if (Log.IsDebugEnabled) Log.Debug($"*** >>> Zone ID is {zoneId}.");
            if (Log.IsDebugEnabled) Log.Debug($"*** >>> Context ID is {contextId}.");

            foreach (StudentPersonal student in students)
            {
                if (Log.IsDebugEnabled)
                    Log.Debug(
                        $"*** >>> Student created is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
            }
        }

        /// <inheritdoc />
        public override void OnDeleteEvent(
            List<StudentPersonal> students,
            string zoneId = null,
            string contextId = null)
        {
            if (Log.IsDebugEnabled) Log.Debug("*** OnDeleteEvent handler called ...");
            if (Log.IsDebugEnabled) Log.Debug($"*** >>> Zone ID is {zoneId}.");
            if (Log.IsDebugEnabled) Log.Debug($"*** >>> Context ID is {contextId}.");

            foreach (StudentPersonal student in students)
            {
                if (Log.IsDebugEnabled)
                    Log.Debug(
                        $"*** >>> Student deleted is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
            }
        }

        /// <inheritdoc />
        public override void OnErrorEvent(ResponseError error, string zoneId = null, string contextId = null)
        {
            if (Log.IsDebugEnabled) Log.Debug("*** OnErrorEvent handler called ...");
            if (Log.IsDebugEnabled) Log.Debug($"*** >>> Zone ID is {zoneId}.");
            if (Log.IsDebugEnabled) Log.Debug($"*** >>> Context ID is {contextId}.");
            if (Log.IsDebugEnabled) Log.Debug($"*** >>> Error: {error.Message}.");
        }

        /// <inheritdoc />
        public override void OnUpdateEvent(
            List<StudentPersonal> students,
            bool partialUpdate,
            string zoneId = null,
            string contextId = null)
        {
            if (Log.IsDebugEnabled) Log.Debug("*** OnUpdateEvent handler called ...");
            if (Log.IsDebugEnabled) Log.Debug($"*** >>> Partial update is {partialUpdate}.");
            if (Log.IsDebugEnabled) Log.Debug($"*** >>> Zone ID is {zoneId}.");
            if (Log.IsDebugEnabled) Log.Debug($"*** >>> Context ID is {contextId}.");

            foreach (StudentPersonal student in students)
            {
                if (Log.IsDebugEnabled)
                    Log.Debug(
                        $"*** >>> Student updated is {student.PersonInfo.Name.GivenName} {student.PersonInfo.Name.FamilyName}.");
            }
        }
    }
}