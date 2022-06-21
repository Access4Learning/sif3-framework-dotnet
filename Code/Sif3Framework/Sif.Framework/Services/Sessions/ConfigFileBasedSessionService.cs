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

using Sif.Framework.Model.Settings;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Sif.Framework.Service.Sessions
{
    /// <summary>
    /// This class represents operations associated with sessions that are stored in the SifFramework.config custom
    /// configuration file.
    /// </summary>
    internal abstract class ConfigFileBasedSessionService : ISessionService
    {
        /// <summary>
        /// Reference to the SifFramework.config custom configuration file.
        /// </summary>
        protected Configuration Configuration { get; }

        /// <summary>
        /// Section of the configuration file containing the session information.
        /// </summary>
        protected abstract ISessionsSection SessionsSection { get; }

        /// <summary>
        /// Create an instance of this class based upon the SifFramework.config file.
        /// </summary>
        protected ConfigFileBasedSessionService()
        {
#if NETFULL
            string configurationFilePath =
                System.Web.Hosting.HostingEnvironment.MapPath("~/SifFramework.config") ?? "SifFramework.config";

#else
            string configurationFilePath = null;
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.HasFile && !string.IsNullOrWhiteSpace(config.FilePath))
            {
                configurationFilePath = $"{Path.GetDirectoryName(config.FilePath)}/SifFramework.config";
            }

            if (configurationFilePath == null)
            {
                configurationFilePath = "SifFramework.config";
            }
#endif

            var exeConfigurationFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configurationFilePath
            };

            Configuration =
                ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);

            if (!Configuration.HasFile)
            {
                string fullPath = Assembly.GetExecutingAssembly().Location;
                exeConfigurationFileMap.ExeConfigFilename = Path.GetDirectoryName(fullPath) + "\\SifFramework.config";
                Configuration = ConfigurationManager.OpenMappedExeConfiguration(
                    exeConfigurationFileMap,
                    ConfigurationUserLevel.None);
            }

            if (!Configuration.HasFile)
            {
                var message = $"Missing configuration file {configurationFilePath}.";
                throw new ConfigurationErrorsException(message);
            }
        }

        /// <summary>
        /// <see cref="ISessionService.HasSession(string, string, string, string)"/>
        /// </summary>
        public bool HasSession(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            return RetrieveSessionEntry(applicationKey, solutionId, userToken, instanceId) != null;
        }

        /// <summary>
        /// <see cref="ISessionService.HasSessionToken"/>
        /// </summary>
        public bool HasSessionToken(string sessionToken)
        {
            return SessionsSection.Sessions[sessionToken] != null;
        }

        /// <summary>
        /// <see cref="ISessionService.RemoveSession(string)"/>
        /// </summary>
        public void RemoveSession(string sessionToken)
        {
            if (HasSessionToken(sessionToken))
            {
                SessionsSection.Sessions.Remove(sessionToken);
                Configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(SifFrameworkSectionGroup.SectionGroupReference);
            }
        }

        /// <summary>
        /// <see cref="ISessionService.RetrieveEnvironmentUrl(string, string, string, string)"/>
        /// </summary>
        public string RetrieveEnvironmentUrl(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            string url = RetrieveSessionEntry(applicationKey, solutionId, userToken, instanceId)?.EnvironmentUrl;

            return (string.IsNullOrWhiteSpace(url) ? null : url);
        }

        /// <summary>
        /// <see cref="ISessionService.RetrieveQueueId(string, string, string, string)"/>
        /// </summary>
        public string RetrieveQueueId(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            string id = RetrieveSessionEntry(applicationKey, solutionId, userToken, instanceId)?.QueueId;

            return (string.IsNullOrWhiteSpace(id) ? null : id);
        }

        /// <summary>
        /// Retrieve the session entry that matches the specified parameters.
        /// </summary>
        /// <param name="applicationKey">Application key.</param>
        /// <param name="solutionId">Solution ID.</param>
        /// <param name="userToken">User token.</param>
        /// <param name="instanceId">Instance ID.</param>
        /// <returns>Session entry if found; null otherwise.</returns>
        private SessionElement RetrieveSessionEntry(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            SessionElement sessionElement = null;

            foreach (SessionElement session in SessionsSection.Sessions)
            {
                if (string.Equals(applicationKey, session.ApplicationKey) &&
                    (solutionId?.Equals(session.SolutionId) ?? string.IsNullOrWhiteSpace(session.SolutionId)) &&
                    (userToken?.Equals(session.UserToken) ?? string.IsNullOrWhiteSpace(session.UserToken)) &&
                    (instanceId?.Equals(session.InstanceId) ?? string.IsNullOrWhiteSpace(session.InstanceId)))
                {
                    sessionElement = session;
                    break;
                }
            }

            return sessionElement;
        }

        /// <summary>
        /// <see cref="ISessionService.RetrieveSessionToken(string, string, string, string)"/>
        /// </summary>
        public string RetrieveSessionToken(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            string sessionToken = RetrieveSessionEntry(applicationKey, solutionId, userToken, instanceId)?.SessionToken;

            return (string.IsNullOrWhiteSpace(sessionToken) ? null : sessionToken);
        }

        /// <summary>
        /// <see cref="ISessionService.RetrieveSubscriptionId(string, string, string, string)"/>
        /// </summary>
        public string RetrieveSubscriptionId(
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            string id = RetrieveSessionEntry(applicationKey, solutionId, userToken, instanceId)?.SubscriptionId;

            return (string.IsNullOrWhiteSpace(id) ? null : id);
        }

        /// <summary>
        /// <see cref="ISessionService.StoreSession(string, string, string, string, string, string)"/>
        /// </summary>
        public void StoreSession(
            string applicationKey,
            string sessionToken,
            string environmentUrl,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            if (HasSession(applicationKey, solutionId, userToken, instanceId))
            {
                string solutionIdText = solutionId == null ? "" : "[solutionId=" + solutionId + "]";
                string userTokenText = userToken == null ? "" : "[userToken=" + userToken + "]";
                string instanceIdText = instanceId == null ? "" : "[instanceId=" + instanceId + "]";
                var message =
                    $"Session with the following credentials already exists - [applicationKey={applicationKey}]{solutionIdText}{userTokenText}{instanceIdText}.";

                throw new ConfigurationErrorsException(message);
            }

            if (HasSessionToken(sessionToken))
            {
                var message = $"Session already exists with a session token of {sessionToken}.";

                throw new ConfigurationErrorsException(message);
            }

            var sessionElement =
                new SessionElement(applicationKey, sessionToken, environmentUrl, solutionId, userToken, instanceId);
            SessionsSection.Sessions.Add(sessionElement);
            Configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(SifFrameworkSectionGroup.SectionGroupReference);
        }

        /// <summary>
        /// <see cref="ISessionService.UpdateQueueId(string, string, string, string, string)"/>
        /// </summary>
        public void UpdateQueueId(
            string queueId,
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            SessionElement sessionElement = RetrieveSessionEntry(applicationKey, solutionId, userToken, instanceId);

            if (sessionElement != null)
            {
                sessionElement.QueueId = queueId;
                Configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(SifFrameworkSectionGroup.SectionGroupReference);
            }
        }

        /// <summary>
        /// <see cref="ISessionService.UpdateSubscriptionId(string, string, string, string, string)"/>
        /// </summary>
        public void UpdateSubscriptionId(
            string subscriptionId,
            string applicationKey,
            string solutionId = null,
            string userToken = null,
            string instanceId = null)
        {
            SessionElement sessionElement = RetrieveSessionEntry(applicationKey, solutionId, userToken, instanceId);

            if (sessionElement != null)
            {
                sessionElement.SubscriptionId = subscriptionId;
                Configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(SifFrameworkSectionGroup.SectionGroupReference);
            }
        }
    }
}