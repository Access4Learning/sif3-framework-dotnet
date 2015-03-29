﻿/*
 * Copyright 2015 Systemic Pty Ltd
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
using System;

namespace Sif.Framework.Utils
{

    /// <summary>
    /// Unit tests for SIF Framework settings.
    /// </summary>
    [TestClass]
    public class SettingsManagerTest
    {

        [TestMethod]
        public void TestRetrieveAppSettings()
        {
            IFrameworkSettings settings = SettingsManager.ConsumerSettings;
            Assert.AreEqual(settings.ApplicationKey, "Sif3DemoApp");
            Assert.AreEqual(settings.AuthenticationMethod, "Basic");
            Assert.AreEqual(settings.ConsumerName, "DemoConsumer");
            Assert.AreEqual(settings.DataModelNamespace, "http://www.sifassociation.org/au/datamodel/1.3");
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
            string sessionToken = Guid.NewGuid().ToString();
            ISessionService sessionService = SessionsManager.ConsumerSessionService;
            Assert.IsFalse(sessionService.HasSession(sessionToken: "NonExistantSessionToken"));
            Assert.IsFalse(sessionService.HasSession(applicationKey: "Sif3DemoApp"));
            sessionService.StoreSession("Sif3DemoApp", sessionToken, "http://www.news.com.au");
            Assert.IsTrue(sessionService.HasSession(sessionToken: sessionToken));
            Assert.IsTrue(sessionService.HasSession(applicationKey: "Sif3DemoApp"));
            Assert.AreEqual(sessionService.RetrieveSessionToken("Sif3DemoApp"), sessionToken);
            sessionService.RemoveSession(sessionToken);
            Assert.IsFalse(sessionService.HasSession(sessionToken: sessionToken));
            Assert.IsFalse(sessionService.HasSession(applicationKey: "Sif3DemoApp"));
        }

    }

}
