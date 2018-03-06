/*
 * Copyright 2017 Systemic Pty Ltd
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
using Sif.Framework.Model.Authentication;
using System;

namespace Sif.Framework.Service.Authentication
{

    /// <summary>
    /// Unit tests for AuthorisationTokenService.
    /// </summary>
    [TestClass]
    public class AuthorisationTokenServiceTest
    {

        /// <summary>
        /// Use ClassInitialize to run code before running the first test in the class.
        /// </summary>
        /// <param name="testContext">Context information for the unit test.</param>
        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext)
        {
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run.
        /// </summary>
        [ClassCleanup()]
        public static void ClassCleanup()
        {
        }

        /// <summary>
        /// Use TestInitialize to run code before running each test.
        /// </summary>
        [TestInitialize()]
        public void TestInitialize()
        {
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run.
        /// </summary>
        [TestCleanup()]
        public void TestCleanup()
        {
        }

        /// <summary>
        /// Delegate method for retrieving a shared secret.
        /// </summary>
        /// <param name="token">Token associated with the shared secret.</param>
        /// <returns>Shared secret.</returns>
        string SharedSecret(string token)
        {
            return "guest";
        }

        /// <summary>
        /// Authentication test using a Basic authorisation token.
        /// </summary>
        [TestMethod]
        public void BasicAuthorisationTest()
        {
            IAuthorisationTokenService service = new BasicAuthorisationTokenService();
            AuthorisationToken authorisationToken = service.Generate("new", "guest");
            Console.WriteLine("Authorisation token is " + authorisationToken.Token + ".");
            Console.WriteLine("Generated UTC ISO 8601 date is " + authorisationToken.Timestamp + ".");
            GetSharedSecret sharedSecret = SharedSecret;
            string sessionToken;
            bool authorised = service.Verify(authorisationToken, sharedSecret, out sessionToken);
            Assert.AreEqual(sessionToken, "new");
            Assert.IsTrue(authorised);
        }

        /// <summary>
        /// Authentication test using a HMAC-SHA256 authorisation token.
        /// </summary>
        [TestMethod]
        public void HMACSHA256AuthorisationTest()
        {
            IAuthorisationTokenService service = new HmacShaAuthorisationTokenService();
            AuthorisationToken authorisationToken = service.Generate("new", "guest");
            Console.WriteLine("Authorisation token is " + authorisationToken.Token + ".");
            Console.WriteLine("Generated UTC ISO 8601 date is " + authorisationToken.Timestamp + ".");
            GetSharedSecret sharedSecret = SharedSecret;
            string sessionToken;
            bool authorised = service.Verify(authorisationToken, sharedSecret, out sessionToken);
            Assert.AreEqual(sessionToken, "new");
            Assert.IsTrue(authorised);
        }

    }

}
