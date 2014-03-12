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

using Sif.Framework.Infrastructure;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Service.Authentication;
using Sif.Framework.Service.Infrastructure;
using Sif.Framework.Utils;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Sif.Framework.Controller
{

    public abstract class BaseController : ApiController
    {

        string InitialSharedSecret(string applicationKey)
        {
            ApplicationRegister applicationRegister = (new ApplicationRegisterService()).RetrieveByApplicationKey(applicationKey);
            return (applicationRegister == null ? null : applicationRegister.SharedSecret);
        }

        string SharedSecret(string sessionToken)
        {
            environmentType environment = (new EnvironmentService()).RetrieveBySessionToken(sessionToken);
            ApplicationRegister applicationRegister = (new ApplicationRegisterService()).RetrieveByApplicationKey(environment.applicationInfo.applicationKey);
            return (applicationRegister == null ? null : applicationRegister.SharedSecret);
        }

        protected virtual bool VerifyAuthorisationHeader(AuthenticationHeaderValue header)
        {
            bool verified = false;
            string sessionTokenChecked = null;

            if ("Basic".Equals(header.Scheme))
            {
                AuthenticationUtils.GetSharedSecret sharedSecret = SharedSecret;
                verified = AuthenticationUtils.VerifyBasicAuthorisationToken(header.ToString(), sharedSecret, out sessionTokenChecked);
            }
            else if ("SIF_HMACSHA256".Equals(header.Scheme))
            {
                verified = true;
            }

            return verified;
        }

        protected virtual bool VerifyInitialAuthorisationHeader(AuthenticationHeaderValue header, out string sessionToken)
        {
            bool verified = false;
            string sessionTokenChecked = null;

            if ("Basic".Equals(header.Scheme))
            {
                AuthenticationUtils.GetSharedSecret sharedSecret = InitialSharedSecret;
                verified = AuthenticationUtils.VerifyBasicAuthorisationToken(header.ToString(), sharedSecret, out sessionTokenChecked);
            }
            else if ("SIF_HMACSHA256".Equals(header.Scheme))
            {
                verified = true;
            }

            sessionToken = sessionTokenChecked;

            return verified;
        }

    }

}
