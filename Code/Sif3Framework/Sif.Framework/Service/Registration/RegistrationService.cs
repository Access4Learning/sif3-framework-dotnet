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

using log4net;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Service.Sessions;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Reflection;
using System.Xml;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Registration
{

    /// <summary>
    /// <see cref="Sif.Framework.Service.Registration.IRegistrationService">IRegistrationService</see>
    /// </summary>
    public class RegistrationService : IRegistrationService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string environmentUrl;
        private ISessionService sessionService;
        private string sessionToken;
        private IFrameworkSettings settings;

        /// <summary>
        /// <see cref="Sif.Framework.Service.Registration.IRegistrationService.AuthorisationToken">AuthorisationToken</see>
        /// </summary>
        public string AuthorisationToken { get; private set; }

        /// <summary>
        /// <see cref="Sif.Framework.Service.Registration.IRegistrationService.Registered">Registered</see>
        /// </summary>
        public bool Registered { get; private set; }

        /// <summary>
        /// The current environment that this RegistrationService has registered with.
        /// </summary>
        public Environment CurrentEnvironment { get; private set; }

        /// <summary>
        /// Parse the URL of the Environment infrastructure service from the XML.
        /// </summary>
        /// <param name="environmentXml">Serialised Environment object as XML.</param>
        /// <returns>URL of the Environment infrastructure service.</returns>
        private string TryParseEnvironmentUrl(string environmentXml)
        {
            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            xmlNamespaceManager.AddNamespace("ns", "http://www.sifassociation.org/infrastructure/3.0.1");

            XmlDocument environmentDoc = new XmlDocument();
            environmentDoc.LoadXml(environmentXml);

            string environmentUrl = environmentDoc.SelectSingleNode("//ns:infrastructureService[@name='environment']", xmlNamespaceManager).InnerText;
            if (log.IsDebugEnabled) log.Debug("Parsed environment URL is " + environmentUrl + ".");

            return environmentUrl;

        }

        /// <summary>
        /// Create an instance using the appropriate settings and service.
        /// </summary>
        /// <param name="settings">Framework settings.</param>
        /// <param name="sessionService">Service used for managing sessions.</param>
        public RegistrationService(IFrameworkSettings settings, ISessionService sessionService)
        {
            this.settings = settings;
            this.sessionService = sessionService;
            Registered = false;
        }
        /// <summary>
        /// <see cref="Sif.Framework.Service.Registration.IRegistrationService.Register()">Register</see>
        /// </summary>
        public Environment Register()
        {
            Environment environment = EnvironmentUtils.LoadFromSettings(SettingsManager.ProviderSettings);
            return Register(ref environment);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Service.Registration.IRegistrationService.Register(Sif.Framework.Model.Infrastructure.Environment)">Register</see>
        /// </summary>
        public Environment Register(ref Environment environment)
        {
            if (Registered)
            {
                return CurrentEnvironment;
            }

            if (sessionService.HasSession(environment.ApplicationInfo.ApplicationKey, environment.SolutionId, environment.UserToken, environment.InstanceId))
            {
                if (log.IsDebugEnabled) log.Debug("Session token already exists for this object service (Consumer/Provider).");

                string storedSessionToken = sessionService.RetrieveSessionToken(environment.ApplicationInfo.ApplicationKey, environment.SolutionId, environment.UserToken, environment.InstanceId);
                AuthorisationToken = AuthenticationUtils.GenerateBasicAuthorisationToken(storedSessionToken, settings.SharedSecret);
                string storedEnvironmentUrl = sessionService.RetrieveEnvironmentUrl(environment.ApplicationInfo.ApplicationKey, environment.SolutionId, environment.UserToken, environment.InstanceId);
                string environmentXml = HttpUtils.GetRequest(storedEnvironmentUrl, AuthorisationToken);

                if (log.IsDebugEnabled) log.Debug("Environment XML from GET request ...");
                if (log.IsDebugEnabled) log.Debug(environmentXml);

                environmentType environmentTypeToDeserialise = SerialiserFactory.GetXmlSerialiser<environmentType>().Deserialise(environmentXml);
                Environment environmentResponse = MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeToDeserialise);

                sessionToken = environmentResponse.SessionToken;
                environmentUrl = environmentResponse.InfrastructureServices[InfrastructureServiceNames.environment].Value;

                if (log.IsDebugEnabled) log.Debug("Environment URL is " + environmentUrl + ".");

                if (!storedSessionToken.Equals(sessionToken) || !storedEnvironmentUrl.Equals(environmentUrl))
                {
                    AuthorisationToken = AuthenticationUtils.GenerateBasicAuthorisationToken(sessionToken, settings.SharedSecret);
                    sessionService.RemoveSession(storedSessionToken);
                    sessionService.StoreSession(environmentResponse.ApplicationInfo.ApplicationKey, sessionToken, environmentUrl, environmentResponse.SolutionId, environmentResponse.UserToken, environmentResponse.InstanceId);
                }

                environment = environmentResponse;
            }
            else
            {
                if (log.IsDebugEnabled) log.Debug("Session token does not exist for this object service (Consumer/Provider).");

                string initialToken = AuthenticationUtils.GenerateBasicAuthorisationToken(environment.ApplicationInfo.ApplicationKey, settings.SharedSecret);
                environmentType environmentTypeToSerialise = MapperFactory.CreateInstance<Environment, environmentType>(environment);
                string body = SerialiserFactory.GetXmlSerialiser<environmentType>().Serialise(environmentTypeToSerialise);
                string environmentXml = HttpUtils.PostRequest(settings.EnvironmentUrl, initialToken, body);

                if (log.IsDebugEnabled) log.Debug("Environment XML from POST request ...");
                if (log.IsDebugEnabled) log.Debug(environmentXml);

                try
                {
                    environmentType environmentTypeToDeserialise = SerialiserFactory.GetXmlSerialiser<environmentType>().Deserialise(environmentXml);
                    Environment environmentResponse = MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeToDeserialise);

                    sessionToken = environmentResponse.SessionToken;
                    environmentUrl = environmentResponse.InfrastructureServices[InfrastructureServiceNames.environment].Value;

                    if (log.IsDebugEnabled) log.Debug("Environment URL is " + environmentUrl + ".");

                    AuthorisationToken = AuthenticationUtils.GenerateBasicAuthorisationToken(sessionToken, settings.SharedSecret);
                    sessionService.StoreSession(environment.ApplicationInfo.ApplicationKey, sessionToken, environmentUrl, environmentResponse.SolutionId, environmentResponse.UserToken, environmentResponse.InstanceId);
                    environment = environmentResponse;
                }
                catch (Exception)
                {

                    if (environmentUrl != null)
                    {
                        HttpUtils.DeleteRequest(environmentUrl, AuthorisationToken);
                    }
                    else if (!String.IsNullOrWhiteSpace(TryParseEnvironmentUrl(environmentXml)))
                    {
                        HttpUtils.DeleteRequest(TryParseEnvironmentUrl(environmentXml), AuthorisationToken);
                    }

                    throw;
                }

            }

            CurrentEnvironment = environment;
            Registered = true;
            return CurrentEnvironment;
        }

        /// <summary>
        /// <see cref="Sif.Framework.Service.Registration.IRegistrationService.Unregister(bool?)">Unregister</see>
        /// </summary>
        public void Unregister(bool? deleteOnUnregister = null)
        {

            if (Registered)
            {

                if (deleteOnUnregister ?? settings.DeleteOnUnregister)
                {
                    string xml = HttpUtils.DeleteRequest(environmentUrl, AuthorisationToken);
                    sessionService.RemoveSession(sessionToken);
                }

                Registered = false;
            }

        }

    }

}
