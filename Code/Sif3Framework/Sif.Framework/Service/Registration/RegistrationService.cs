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

using Sif.Framework.Extensions;
using Sif.Framework.Model.Authentication;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Authentication;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Service.Sessions;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Xml;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Registration
{
    /// <summary>
    /// <see cref="IRegistrationService">IRegistrationService</see>
    /// </summary>
    internal class RegistrationService : IRegistrationService
    {
        private readonly slf4net.ILogger log =
            slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IAuthorisationTokenService authorisationTokenService;
        private readonly ISessionService sessionService;
        private readonly IFrameworkSettings settings;

        private AuthorisationToken authorisationToken;
        private string environmentUrl;
        private string sessionToken;

        /// <summary>
        /// <see cref="IRegistrationService.AuthorisationToken">AuthorisationToken</see>
        /// </summary>
        public AuthorisationToken AuthorisationToken => GetAuthorisationToken();

        /// <summary>
        /// <see cref="IRegistrationService.Registered">Registered</see>
        /// </summary>
        public bool Registered { get; private set; }

        /// <summary>
        /// The current environment that this RegistrationService has registered with.
        /// </summary>
        public Environment CurrentEnvironment { get; private set; }

        /// <summary>
        /// Get the current authorisation token to use.
        /// </summary>
        /// <returns>The current authorisation token to use.</returns>
        private AuthorisationToken GetAuthorisationToken()
        {
            if (!Registered)
            {
                return null;
            }

            if (authorisationTokenService is HmacShaAuthorisationTokenService)
            {
                string storedSessionToken = sessionService.RetrieveSessionToken(
                    CurrentEnvironment.ApplicationInfo.ApplicationKey,
                    CurrentEnvironment.SolutionId,
                    CurrentEnvironment.UserToken,
                    CurrentEnvironment.InstanceId);
                authorisationToken = authorisationTokenService.Generate(storedSessionToken, settings.SharedSecret);
            }

            if (log.IsDebugEnabled)
                log.Debug(
                    $"Authorisation token is {authorisationToken.Token} with a timestamp of {authorisationToken.Timestamp ?? "<null>"}.");

            return authorisationToken;
        }

        /// <summary>
        /// Parse the URL of the Environment infrastructure service from the XML.
        /// </summary>
        /// <param name="environmentXml">Serialised Environment object as XML.</param>
        /// <returns>URL of the Environment infrastructure service.</returns>
        private string TryParseEnvironmentUrl(string environmentXml)
        {
            string url = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(environmentXml))
                {
                    var xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
                    xmlNamespaceManager.AddNamespace("ns", "http://www.sifassociation.org/infrastructure/3.0.1");
                    var environmentDoc = new XmlDocument();
                    environmentDoc.LoadXml(environmentXml);
                    url = environmentDoc
                        .SelectSingleNode("//ns:infrastructureService[@name='environment']", xmlNamespaceManager)
                        ?.InnerText;

                    if (log.IsDebugEnabled) log.Debug($"Parsed environment URL is {url}.");
                }
            }
            catch (Exception)
            {
                // Return null if unable to parse for an environment URL.
            }

            return url;
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

            if (AuthenticationMethod.Basic.ToString()
                .Equals(settings.AuthenticationMethod, StringComparison.OrdinalIgnoreCase))
            {
                authorisationTokenService = new BasicAuthorisationTokenService();
            }
            else if (AuthenticationMethod.SIF_HMACSHA256.ToString()
                .Equals(settings.AuthenticationMethod, StringComparison.OrdinalIgnoreCase))
            {
                authorisationTokenService = new HmacShaAuthorisationTokenService();
            }
            else
            {
                authorisationTokenService = new BasicAuthorisationTokenService();
            }

            Registered = false;
        }

        /// <summary>
        /// <see cref="IRegistrationService.Register()">Register</see>
        /// </summary>
        public Environment Register()
        {
            Environment environment = EnvironmentUtils.LoadFromSettings(settings);
            return Register(ref environment);
        }

        /// <summary>
        /// <see cref="IRegistrationService.Register(ref Environment)">Register</see>
        /// </summary>
        public Environment Register(ref Environment environment)
        {
            if (Registered)
            {
                return CurrentEnvironment;
            }

            if (sessionService.HasSession(
                environment.ApplicationInfo.ApplicationKey,
                environment.SolutionId,
                environment.UserToken,
                environment.InstanceId))
            {
                if (log.IsDebugEnabled)
                    log.Debug("Session token already exists for this object service (Consumer/Provider).");

                string storedSessionToken = sessionService.RetrieveSessionToken(
                    environment.ApplicationInfo.ApplicationKey,
                    environment.SolutionId,
                    environment.UserToken,
                    environment.InstanceId);
                authorisationToken = authorisationTokenService.Generate(storedSessionToken, settings.SharedSecret);
                string storedEnvironmentUrl = sessionService.RetrieveEnvironmentUrl(
                    environment.ApplicationInfo.ApplicationKey,
                    environment.SolutionId,
                    environment.UserToken,
                    environment.InstanceId);
                string environmentBody = HttpUtils.GetRequest(
                    storedEnvironmentUrl,
                    authorisationToken,
                    settings.CompressPayload,
                    contentTypeOverride: settings.ContentType.ToDescription(),
                    acceptOverride: settings.Accept.ToDescription());

                if (log.IsDebugEnabled) log.Debug($"Environment response from GET request ...\n{environmentBody}");

                environmentType environmentTypeToDeserialise =
                    SerialiserFactory.GetSerialiser<environmentType>(settings.Accept).Deserialise(environmentBody);
                Environment environmentResponse =
                    MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeToDeserialise);

                sessionToken = environmentResponse.SessionToken;
                environmentUrl =
                    environmentResponse.InfrastructureServices[InfrastructureServiceNames.environment].Value;

                if (log.IsDebugEnabled) log.Debug($"Environment URL is {environmentUrl}.");

                if (!storedSessionToken.Equals(sessionToken) || !storedEnvironmentUrl.Equals(environmentUrl))
                {
                    authorisationToken = authorisationTokenService.Generate(sessionToken, settings.SharedSecret);
                    sessionService.RemoveSession(storedSessionToken);
                    sessionService.StoreSession(
                        environmentResponse.ApplicationInfo.ApplicationKey,
                        sessionToken,
                        environmentUrl,
                        environmentResponse.SolutionId,
                        environmentResponse.UserToken,
                        environmentResponse.InstanceId);
                }

                environment = environmentResponse;
            }
            else
            {
                if (log.IsDebugEnabled)
                    log.Debug("Session token does not exist for this object service (Consumer/Provider).");

                string environmentBody = null;

                try
                {
                    AuthorisationToken initialToken = authorisationTokenService.Generate(
                        environment.ApplicationInfo.ApplicationKey,
                        settings.SharedSecret);
                    environmentType environmentTypeToSerialise =
                        MapperFactory.CreateInstance<Environment, environmentType>(environment);
                    string body = SerialiserFactory.GetSerialiser<environmentType>(settings.ContentType)
                        .Serialise(environmentTypeToSerialise);
                    environmentBody = HttpUtils.PostRequest(
                        settings.EnvironmentUrl,
                        initialToken,
                        body,
                        settings.CompressPayload,
                        contentTypeOverride: settings.ContentType.ToDescription(),
                        acceptOverride: settings.Accept.ToDescription());

                    if (log.IsDebugEnabled) log.Debug($"Environment response from POST request ...\n{environmentBody}");

                    environmentType environmentTypeToDeserialise =
                        SerialiserFactory.GetSerialiser<environmentType>(settings.Accept).Deserialise(environmentBody);
                    Environment environmentResponse =
                        MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeToDeserialise);

                    sessionToken = environmentResponse.SessionToken;
                    environmentUrl =
                        environmentResponse.InfrastructureServices[InfrastructureServiceNames.environment].Value;

                    if (log.IsDebugEnabled) log.Debug($"Environment URL is {environmentUrl}.");

                    authorisationToken = authorisationTokenService.Generate(sessionToken, settings.SharedSecret);
                    sessionService.StoreSession(
                        environment.ApplicationInfo.ApplicationKey,
                        sessionToken,
                        environmentUrl,
                        environmentResponse.SolutionId,
                        environmentResponse.UserToken,
                        environmentResponse.InstanceId);
                    environment = environmentResponse;
                }
                catch (Exception e)
                {
                    if (environmentUrl != null)
                    {
                        HttpUtils.DeleteRequest(environmentUrl, authorisationToken, settings.CompressPayload);
                    }
                    else if (!string.IsNullOrWhiteSpace(TryParseEnvironmentUrl(environmentBody)))
                    {
                        HttpUtils.DeleteRequest(
                            TryParseEnvironmentUrl(environmentBody),
                            authorisationToken,
                            settings.CompressPayload);
                    }

                    throw new RegistrationException("Registration failed.", e);
                }
            }

            CurrentEnvironment = environment;
            Registered = true;
            return CurrentEnvironment;
        }

        /// <summary>
        /// <see cref="IRegistrationService.Unregister(bool?)">Unregister</see>
        /// </summary>
        public void Unregister(bool? deleteOnUnregister = null)
        {
            if (Registered)
            {
                if (deleteOnUnregister ?? settings.DeleteOnUnregister)
                {
                    HttpUtils.DeleteRequest(
                        environmentUrl,
                        authorisationToken,
                        settings.CompressPayload,
                        contentTypeOverride: settings.ContentType.ToDescription(),
                        acceptOverride: settings.Accept.ToDescription());
                    sessionService.RemoveSession(sessionToken);
                }

                Registered = false;
            }
        }
    }
}