/*
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

using System.Configuration;

namespace Sif.Framework.Model.Settings
{

    /// <summary>
    /// Represents the "session" configuration element within a configuration file.
    /// </summary>
    class SessionElement : ConfigurationElement
    {
        public const string ElementReference = "session";

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        public SessionElement()
        {

        }

        /// <summary>
        /// Create an instance of this class based upon the passed parameters.
        /// </summary>
        /// <param name="applicationKey">Application key.</param>
        /// <param name="sessionToken">Session token.</param>
        /// <param name="environmentUrl">Environment URL.</param>
        /// <param name="solutionId">Solution ID.</param>
        /// <param name="userToken">User token.</param>
        /// <param name="instanceId">Instance ID.</param>
        public SessionElement(string applicationKey, string sessionToken, string environmentUrl, string solutionId = null, string userToken = null, string instanceId = null)
        {
            ApplicationKey = applicationKey;
            SessionToken = sessionToken;
            EnvironmentUrl = environmentUrl;
            SolutionId = solutionId;
            UserToken = userToken;
            InstanceId = instanceId;
        }

        /// <summary>
        /// Mandatory applicationKey attribute.
        /// </summary>
        [ConfigurationProperty("applicationKey", IsRequired = true)]
        public string ApplicationKey
        {

            get
            {
                return (string)this["applicationKey"];
            }

            set
            {
                this["applicationKey"] = value;
            }

        }

        /// <summary>
        /// Mandatory environmentUrl attribute.
        /// </summary>
        [ConfigurationProperty("environmentUrl", IsRequired = true)]
        public string EnvironmentUrl
        {

            get
            {
                return (string)this["environmentUrl"];
            }

            set
            {
                this["environmentUrl"] = value;
            }

        }

        /// <summary>
        /// Optional instanceId attribute.
        /// </summary>
        [ConfigurationProperty("instanceId")]
        public string InstanceId
        {

            get
            {
                return (string)this["instanceId"];
            }

            set
            {
                this["instanceId"] = value;
            }

        }

        /// <summary>
        /// Mandatory sessionToken attribute.
        /// </summary>
        [ConfigurationProperty("sessionToken", IsRequired = true)]
        public string SessionToken
        {

            get
            {
                return (string)this["sessionToken"];
            }

            set
            {
                this["sessionToken"] = value;
            }

        }

        /// <summary>
        /// Optional solutionId attribute.
        /// </summary>
        [ConfigurationProperty("solutionId")]
        public string SolutionId
        {

            get
            {
                return (string)this["solutionId"];
            }

            set
            {
                this["solutionId"] = value;
            }

        }

        /// <summary>
        /// Optional userToken attribute.
        /// </summary>
        [ConfigurationProperty("userToken")]
        public string UserToken
        {

            get
            {
                return (string)this["userToken"];
            }

            set
            {
                this["userToken"] = value;
            }

        }

    }

}
