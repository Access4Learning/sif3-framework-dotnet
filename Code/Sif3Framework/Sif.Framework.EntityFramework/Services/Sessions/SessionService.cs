/*
 * Copyright 2020 Systemic Pty Ltd
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

using Sif.Framework.Model.Sessions;
using Sif.Framework.Service.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Services;

namespace Sif.Framework.EntityFramework.Services.Sessions
{
    /// <inheritdoc cref="ISessionService"/>
    public class SessionService : ISessionService
    {
        private readonly IObjectService<Session, Guid> service;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="service">Object service associated with this facade service.</param>
        public SessionService(IObjectService<Session, Guid> service)
        {
            this.service = service;
        }

        /// <inheritdoc cref="ISessionService.HasSession(string,string,string,string)"/>
        public bool HasSession(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            if (applicationKey == null) throw new ArgumentNullException(nameof(applicationKey));

            return service
                .Retrieve(s =>
                    s.ApplicationKey == applicationKey &&
                    s.SolutionId == solutionId &&
                    s.InstanceId == instanceId &&
                    s.UserToken == userToken)
                .Any();
        }

        /// <inheritdoc cref="ISessionService.HasSession(string)"/>
        public bool HasSession(string sessionToken)
        {
            if (sessionToken == null) throw new ArgumentNullException(nameof(sessionToken));

            return service.Retrieve(s => s.SessionToken == sessionToken).Any();
        }

        /// <inheritdoc cref="ISessionService.RemoveSession(string)"/>
        public void RemoveSession(string sessionToken)
        {
            if (sessionToken == null) throw new ArgumentNullException(nameof(sessionToken));

            if (!HasSession(sessionToken))
            {
                throw new NotFoundException($"Session with token of {sessionToken} does not exist.");
            }

            try
            {
                Session session = service.Retrieve(s => s.SessionToken == sessionToken).Single();
                service.Delete(session);
            }
            catch (InvalidOperationException e)
            {
                throw new DuplicateFoundException($"Multiple sessions with the token {sessionToken} found.", e);
            }
        }

        /// <summary>
        /// Retrieve a session based upon the passed criteria parameters.
        /// </summary>
        /// <param name="applicationKey">Application key.</param>
        /// <param name="solutionId">Solution ID.</param>
        /// <param name="userToken">User token.</param>
        /// <param name="instanceId">Instance ID.</param>
        /// <exception cref="ArgumentNullException">applicationKey is null.</exception>
        /// <exception cref="DuplicateFoundException">Multiple session entries exist for the passed criteria parameters.</exception>
        /// <exception cref="NotFoundException">No session entry exists for the passed criteria parameters.</exception>
        /// <returns>The matched session entry.</returns>
        public Session Retrieve(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            if (applicationKey == null) throw new ArgumentNullException(nameof(applicationKey));

            IEnumerable<Session> sessions = service
                .Retrieve(s =>
                    s.ApplicationKey == applicationKey &&
                    s.SolutionId == solutionId &&
                    s.InstanceId == instanceId &&
                    s.UserToken == userToken)
                .ToList();

            if (!sessions.Any())
            {
                throw new NotFoundException(
                    $"Session does not exist for [applicationKey:{applicationKey}|solutionId:{solutionId}|userToken:{userToken}|instanceId:{instanceId}].");
            }

            if (sessions.Count() > 1)
            {
                throw new DuplicateFoundException(
                    $"Multiple sessions found for [applicationKey:{applicationKey}|solutionId:{solutionId}|userToken:{userToken}|instanceId:{instanceId}].");
            }

            return sessions.First();
        }

        /// <summary>
        /// Retrieve the session property (based upon the defined property selector) of a session entry that matches
        /// the passed criteria parameters.
        /// </summary>
        /// <param name="selector">Property selector.</param>
        /// <param name="applicationKey">Application key.</param>
        /// <param name="solutionId">Solution ID.</param>
        /// <param name="userToken">User token.</param>
        /// <param name="instanceId">Instance ID.</param>
        /// <returns>Value of the property for the matched session entry; null if no match found.</returns>
        /// <exception cref="ArgumentNullException">selector and/or applicationKey are null.</exception>
        /// <exception cref="DuplicateFoundException">Multiple session entries exist for the passed criteria parameters.</exception>
        private string RetrieveBySelector(
            Func<Session, string> selector,
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            if (applicationKey == null) throw new ArgumentNullException(nameof(applicationKey));

            IEnumerable<string> sessionValues = service
                .Retrieve(s =>
                    s.ApplicationKey == applicationKey &&
                    s.SolutionId == solutionId &&
                    s.InstanceId == instanceId &&
                    s.UserToken == userToken)
                .Select(selector)
                .ToList();

            if (sessionValues.Count() > 1)
            {
                throw new DuplicateFoundException(
                    $"Multiple sessions found for [applicationKey:{applicationKey}|solutionId:{solutionId}|userToken:{userToken}|instanceId:{instanceId}].");
            }

            return sessionValues.FirstOrDefault();
        }

        /// <inheritdoc cref="ISessionService.RetrieveEnvironmentUrl(string,string,string,string)"/>
        public string RetrieveEnvironmentUrl(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            return RetrieveBySelector(s => s.EnvironmentUrl, applicationKey, solutionId, userToken, instanceId);
        }

        /// <inheritdoc cref="ISessionService.RetrieveQueueId(string,string,string,string)"/>
        public string RetrieveQueueId(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            return RetrieveBySelector(s => s.QueueId, applicationKey, solutionId, userToken, instanceId);
        }

        /// <inheritdoc cref="ISessionService.RetrieveSessionToken(string,string,string,string)"/>
        public string RetrieveSessionToken(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            return RetrieveBySelector(s => s.SessionToken, applicationKey, solutionId, userToken, instanceId);
        }

        /// <inheritdoc cref="ISessionService.RetrieveSubscriptionId(string,string,string,string)"/>
        public string RetrieveSubscriptionId(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            return RetrieveBySelector(s => s.SubscriptionId, applicationKey, solutionId, userToken, instanceId);
        }

        /// <inheritdoc cref="ISessionService.StoreSession(string,string,string,string,string,string)"/>
        public void StoreSession(
            string applicationKey,
            string sessionToken,
            string environmentUrl,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            if (applicationKey == null) throw new ArgumentNullException(nameof(applicationKey));

            if (sessionToken == null) throw new ArgumentNullException(nameof(sessionToken));

            if (environmentUrl == null) throw new ArgumentNullException(nameof(environmentUrl));

            if (HasSession(applicationKey, solutionId, userToken, instanceId))
                throw new AlreadyExistsException(
                    $"Session already exists for [applicationKey:{applicationKey}|solutionId:{solutionId}|userToken:{userToken}|instanceId:{instanceId}].");

            if (HasSession(sessionToken: sessionToken))
                throw new AlreadyExistsException($"Session already exists for session token {sessionToken}.");

            var session = new Session
            {
                ApplicationKey = applicationKey,
                EnvironmentUrl = environmentUrl,
                Id = Guid.NewGuid(),
                InstanceId = instanceId,
                SessionToken = sessionToken,
                SolutionId = solutionId,
                UserToken = userToken
            };

            _ = service.Create(session);
        }

        /// <inheritdoc cref="ISessionService.UpdateQueueId(string,string,string,string,string)"/>
        public void UpdateQueueId(
            string queueId,
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            if (queueId == null) throw new ArgumentNullException(nameof(queueId));

            Session session = Retrieve(applicationKey, solutionId, userToken, instanceId);
            session.QueueId = queueId;
            service.Update(session);
        }

        /// <inheritdoc cref="ISessionService.UpdateSubscriptionId(string,string,string,string,string)"/>
        public void UpdateSubscriptionId(
            string subscriptionId,
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            if (subscriptionId == null) throw new ArgumentNullException(nameof(subscriptionId));

            Session session = Retrieve(applicationKey, solutionId, userToken, instanceId);
            session.SubscriptionId = subscriptionId;
            service.Update(session);
        }
    }
}