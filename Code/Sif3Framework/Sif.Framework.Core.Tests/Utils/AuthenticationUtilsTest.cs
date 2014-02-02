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
using System;

namespace Sif.Framework.Utils
{

    [TestClass]
    public class AuthenticationUtilsTest
    {

        string SharedSecret(string token)
        {
            return "guest";
        }

        [TestMethod]
        public void BasicAuthorisationTest()
        {
            string authorisationToken = AuthenticationUtils.GenerateBasicAuthorisationToken("new", "guest");
            Console.WriteLine("Authorisation token is " + authorisationToken + ".");
            AuthenticationUtils.GetSharedSecret sharedSecret = SharedSecret;
            string sessionToken;
            bool authorised = AuthenticationUtils.VerifyBasicAuthorisationToken(authorisationToken, sharedSecret, out sessionToken);
            Assert.AreEqual(sessionToken, "new");
            Assert.IsTrue(authorised);
        }

        [TestMethod]
        public void HMACSHA256AuthorisationTest()
        {
            string dateString;
            string authorisationToken = AuthenticationUtils.GenerateHMACSHA256AuthorisationToken("new", "guest", out dateString);
            Console.WriteLine("Authorisation token is " + authorisationToken + ".");
            Console.WriteLine("Generated UTC ISO 8601 date is " + dateString + ".");
            AuthenticationUtils.GetSharedSecret sharedSecret = SharedSecret;
            string sessionToken;
            bool authorised = AuthenticationUtils.VerifyHMACSHA256AuthorisationToken(authorisationToken, dateString, sharedSecret, out sessionToken);
            Assert.AreEqual(sessionToken, "new");
            Assert.IsTrue(authorised);
        }

    }

}
