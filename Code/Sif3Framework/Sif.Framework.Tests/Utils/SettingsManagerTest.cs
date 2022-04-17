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
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Sessions;
using Sif.Framework.Utils;
using System;

namespace Sif.Framework.Tests.Utils
{
    /// <summary>
    /// Unit tests for SIF Framework settings.
    /// </summary>
    [TestClass]
    public class SettingsManagerTest
    {
        private const string ApplicationKey = "Sif3DemoApp";

        [TestMethod]
        public void TestRetrieveAppSettings()
        {
            IFrameworkSettings settings = SettingsManager.ConsumerSettings;
            Assert.AreEqual(settings.ApplicationKey, ApplicationKey);
            Assert.AreEqual(settings.AuthenticationMethod, "Basic");
            Assert.AreEqual(settings.ConsumerName, "DemoConsumer");
            Assert.AreEqual(settings.DataModelNamespace, "http://www.sifassociation.org/au/datamodel/1.4");
            Assert.IsTrue(settings.DeleteOnUnregister);
            Assert.AreEqual(settings.EnvironmentUrl, "http://localhost:62921/api/environments/environment");
            Assert.IsNull(settings.InstanceId);
            Assert.AreEqual(settings.SharedSecret, "SecretDem0");
            Assert.IsNull(settings.SolutionId);
            Assert.AreEqual(settings.SupportedInfrastructureVersion, "3.0.1");
            Assert.IsNull(settings.UserToken);
        }

        [TestMethod]
        public void TestSessionTokens()
        {
            var queueId = Guid.NewGuid().ToString();
            var sessionToken = Guid.NewGuid().ToString();
            ISessionService sessionService = SessionsManager.ConsumerSessionService;

            // Check session.
            Assert.IsFalse(sessionService.HasSessionToken("NonExistentSessionToken"));
            Assert.IsFalse(sessionService.HasSession(ApplicationKey));

            // Create session.
            sessionService.StoreSession(ApplicationKey, sessionToken, "http://www.news.com.au");
            Assert.IsTrue(sessionService.HasSessionToken(sessionToken));
            Assert.IsTrue(sessionService.HasSession(ApplicationKey));
            Assert.AreEqual(sessionService.RetrieveSessionToken(ApplicationKey), sessionToken);

            // Update session with a Queue ID.
            Assert.IsNull(sessionService.RetrieveQueueId(ApplicationKey));
            sessionService.UpdateQueueId(queueId, ApplicationKey);
            Assert.AreEqual(queueId, sessionService.RetrieveQueueId(ApplicationKey));

            // Delete session.
            sessionService.RemoveSession(sessionToken);
            Assert.IsFalse(sessionService.HasSessionToken(sessionToken));
            Assert.IsFalse(sessionService.HasSession(ApplicationKey));
        }
    }
}