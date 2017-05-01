﻿/*
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

using Sif.Framework.Model.Settings;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web.Hosting;

namespace Sif.Framework.Service.Sessions
{

    /// <summary>
    /// This class represents operations associated with sessions that are stored in the SifFramework.config custom
    /// configuration file.
    /// </summary>
    abstract class ConfigFileBasedSessionService : ISessionService
    {

        /// <summary>
        /// Reference to the SifFramework.config custom configuration file.
        /// </summary>
        protected Configuration Configuration { get; private set; }

        /// <summary>
        /// Section of the configuration file containing the session information.
        /// </summary>
        protected abstract ISessionsSection SessionsSection { get; }

        /// <summary>
        /// Create an instance of this class based upon the SifFramework.config file.
        /// </summary>
        public ConfigFileBasedSessionService()
        {
            string configurationFilePath = HostingEnvironment.MapPath("~/SifFramework.config");

            if (configurationFilePath == null)
            {
                configurationFilePath = "SifFramework.config";
            }

            ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();
            exeConfigurationFileMap.ExeConfigFilename = configurationFilePath;
            Configuration = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);

            if (!Configuration.HasFile)
            {
                string fullPath = Assembly.GetExecutingAssembly().Location;
                exeConfigurationFileMap.ExeConfigFilename = Path.GetDirectoryName(fullPath) + "\\SifFramework.config";
                Configuration = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
            }

            if (!Configuration.HasFile)
            {
                string message = $"Missing configuration file {configurationFilePath}.";
                throw new ConfigurationErrorsException(message);
            }

        }

        /// <summary>
        /// <see cref="ISessionService.HasSession(string, string, string, string)"/>
        /// </summary>
        public bool HasSession(string applicationKey, string solutionId = null, string userToken = null, string instanceId = null)
        {
            return (RetrieveSessionToken(applicationKey, solutionId, userToken, instanceId) == null ? false : true);
        }

        /// <summary>
        /// <see cref="ISessionService.HasSession(string)"/>
        /// </summary>
        public bool HasSession(string sessionToken)
        {
            return (SessionsSection.Sessions[sessionToken] == null ? false : true);
        }

        /// <summary>
        /// <see cref="ISessionService.RemoveSession(string)"/>
        /// </summary>
        public void RemoveSession(string sessionToken)
        {

            if (HasSession(sessionToken))
            {
                SessionsSection.Sessions.Remove(sessionToken);
                Configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(SifFrameworkSectionGroup.SectionGroupReference);
            }

        }

        /// <summary>
        /// <see cref="ISessionService.RetrieveEnvironmentUrl(string, string, string, string)"/>
        /// </summary>
        public string RetrieveEnvironmentUrl(string applicationKey, string solutionId = null, string userToken = null, string instanceId = null)
        {
            string environmentUrl = null;

            foreach (SessionElement session in SessionsSection.Sessions)
            {

                if (string.Equals(applicationKey, session.ApplicationKey) &&
                    (solutionId == null ? string.IsNullOrWhiteSpace(session.SolutionId) : solutionId.Equals(session.SolutionId)) &&
                    (userToken == null ? string.IsNullOrWhiteSpace(session.UserToken) : userToken.Equals(session.UserToken)) &&
                    (instanceId == null ? string.IsNullOrWhiteSpace(session.InstanceId) : instanceId.Equals(session.InstanceId)))
                {
                    environmentUrl = session.EnvironmentUrl;
                    break;
                }

            }

            return environmentUrl;
        }

        /// <summary>
        /// <see cref="ISessionService.RetrieveSessionToken(string, string, string, string)"/>
        /// </summary>
        public string RetrieveSessionToken(string applicationKey, string solutionId = null, string userToken = null, string instanceId = null)
        {
            string sessionToken = null;

            foreach (SessionElement session in SessionsSection.Sessions)
            {

                if (string.Equals(applicationKey, session.ApplicationKey) &&
                    (solutionId == null ? string.IsNullOrWhiteSpace(session.SolutionId) : solutionId.Equals(session.SolutionId)) &&
                    (userToken == null ? string.IsNullOrWhiteSpace(session.UserToken) : userToken.Equals(session.UserToken)) &&
                    (instanceId == null ? string.IsNullOrWhiteSpace(session.InstanceId) : instanceId.Equals(session.InstanceId)))
                {
                    sessionToken = session.SessionToken;
                    break;
                }

            }

            return sessionToken;
        }

        /// <summary>
        /// <see cref="ISessionService.StoreSession(string, string, string, string, string, string)"/>
        /// </summary>
        public void StoreSession(string applicationKey, string sessionToken, string environmentUrl, string solutionId = null, string userToken = null, string instanceId = null)
        {

            if (HasSession(applicationKey, solutionId, userToken, instanceId))
            {
                string message = string.Format("Session with the following credentials already exists - [applicationKey={0}]{1}{2}{3}.",
                    applicationKey,
                    (solutionId == null ? "" : "[solutionId=" + solutionId + "]"),
                    (userToken == null ? "" : "[userToken=" + userToken + "]"),
                    (instanceId == null ? "" : "[instanceId=" + instanceId + "]"));
                throw new ConfigurationErrorsException(message);
            }

            if (HasSession(sessionToken))
            {
                string message = string.Format("Session already exists with a session token of {0}.", sessionToken);
                throw new ConfigurationErrorsException(message);
            }

            SessionElement sessionElement = new SessionElement(applicationKey, sessionToken, environmentUrl, solutionId, userToken, instanceId);
            SessionsSection.Sessions.Add(sessionElement);
            Configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(SifFrameworkSectionGroup.SectionGroupReference);
        }

    }

}
