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
using System.Linq;
using System.Xml;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Service.Registration
{
    /// <inheritdoc cref="IRegistrationService" />
    public class RegistrationService : IRegistrationService
    {
        private readonly slf4net.ILogger _log =
            slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly IAuthorisationTokenService _authorisationTokenService;
        private readonly ISessionService _sessionService;
        private readonly IFrameworkSettings _settings;

        private AuthorisationToken _authorisationToken;
        private string _environmentUrl;
        private string _sessionToken;

        /// <inheritdoc cref="IRegistrationService.AuthorisationToken" />
        public AuthorisationToken AuthorisationToken => GetAuthorisationToken();

        /// <inheritdoc cref="IRegistrationService.Registered" />
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

            if (_authorisationTokenService is HmacShaAuthorisationTokenService)
            {
                string storedSessionToken = _sessionService.RetrieveSessionToken(
                    CurrentEnvironment.ApplicationInfo.ApplicationKey,
                    CurrentEnvironment.SolutionId,
                    CurrentEnvironment.UserToken,
                    CurrentEnvironment.InstanceId);
                _authorisationToken = _authorisationTokenService.Generate(storedSessionToken, _settings.SharedSecret);
            }

            if (_log.IsDebugEnabled)
                _log.Debug(
                    $"Authorisation token is {_authorisationToken.Token} with a timestamp of {_authorisationToken.Timestamp ?? "<null>"}.");

            return _authorisationToken;
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

                    if (_log.IsDebugEnabled) _log.Debug($"Parsed environment URL is {url}.");
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
        /// <exception cref="ArgumentNullException">Either settings and/or sessionService are null.</exception>
        public RegistrationService(IFrameworkSettings settings, ISessionService sessionService)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));

            if (AuthenticationMethod.Basic.ToString()
                .Equals(settings.AuthenticationMethod, StringComparison.OrdinalIgnoreCase))
            {
                _authorisationTokenService = new BasicAuthorisationTokenService();
            }
            else if (AuthenticationMethod.SIF_HMACSHA256.ToString()
                .Equals(settings.AuthenticationMethod, StringComparison.OrdinalIgnoreCase))
            {
                _authorisationTokenService = new HmacShaAuthorisationTokenService();
            }
            else
            {
                _authorisationTokenService = new BasicAuthorisationTokenService();
            }

            Registered = false;
        }

        /// <inheritdoc cref="IRegistrationService.Register()" />
        public Environment Register()
        {
            Environment environment = EnvironmentUtils.LoadFromSettings(_settings);
            return Register(ref environment);
        }

        /// <inheritdoc cref="IRegistrationService.Register(ref Environment)" />
        public Environment Register(ref Environment environment)
        {
            if (Registered)
            {
                return CurrentEnvironment;
            }

            if (_sessionService.HasSession(
                environment.ApplicationInfo.ApplicationKey,
                environment.SolutionId,
                environment.UserToken,
                environment.InstanceId))
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("Session token already exists for this object service (Consumer/Provider).");

                string storedSessionToken = _sessionService.RetrieveSessionToken(
                    environment.ApplicationInfo.ApplicationKey,
                    environment.SolutionId,
                    environment.UserToken,
                    environment.InstanceId);
                _authorisationToken = _authorisationTokenService.Generate(storedSessionToken, _settings.SharedSecret);
                string storedEnvironmentUrl = _sessionService.RetrieveEnvironmentUrl(
                    environment.ApplicationInfo.ApplicationKey,
                    environment.SolutionId,
                    environment.UserToken,
                    environment.InstanceId);
                string environmentBody = HttpUtils.GetRequest(
                    storedEnvironmentUrl,
                    _authorisationToken,
                    _settings.CompressPayload,
                    contentTypeOverride: _settings.ContentType.ToDescription(),
                    acceptOverride: _settings.Accept.ToDescription());

                if (_log.IsDebugEnabled) _log.Debug($"Environment response from GET request ...\n{environmentBody}");

                environmentType environmentTypeToDeserialise =
                    SerialiserFactory.GetSerialiser<environmentType>(_settings.Accept).Deserialise(environmentBody);
                Environment environmentResponse =
                    MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeToDeserialise);

                _sessionToken = environmentResponse.SessionToken;
                _environmentUrl = environmentResponse.InfrastructureServices.FirstOrDefault(
                    i => i.Name == InfrastructureServiceNames.environment)?.Value;

                if (_log.IsDebugEnabled) _log.Debug($"Environment URL is {_environmentUrl}.");

                if (!storedSessionToken.Equals(_sessionToken) || !storedEnvironmentUrl.Equals(_environmentUrl))
                {
                    _authorisationToken = _authorisationTokenService.Generate(_sessionToken, _settings.SharedSecret);
                    _sessionService.RemoveSession(storedSessionToken);
                    _sessionService.StoreSession(
                        environmentResponse.ApplicationInfo.ApplicationKey,
                        _sessionToken,
                        _environmentUrl,
                        environmentResponse.SolutionId,
                        environmentResponse.UserToken,
                        environmentResponse.InstanceId);
                }

                environment = environmentResponse;
            }
            else
            {
                if (_log.IsDebugEnabled)
                    _log.Debug("Session token does not exist for this object service (Consumer/Provider).");

                string environmentBody = null;

                try
                {
                    AuthorisationToken initialToken = _authorisationTokenService.Generate(
                        environment.ApplicationInfo.ApplicationKey,
                        _settings.SharedSecret);
                    environmentType environmentTypeToSerialise =
                        MapperFactory.CreateInstance<Environment, environmentType>(environment);
                    string body = SerialiserFactory.GetSerialiser<environmentType>(_settings.ContentType)
                        .Serialise(environmentTypeToSerialise);
                    environmentBody = HttpUtils.PostRequest(
                        _settings.EnvironmentUrl,
                        initialToken,
                        body,
                        _settings.CompressPayload,
                        contentTypeOverride: _settings.ContentType.ToDescription(),
                        acceptOverride: _settings.Accept.ToDescription());

                    if (_log.IsDebugEnabled) _log.Debug($"Environment response from POST request ...\n{environmentBody}");

                    environmentType environmentTypeToDeserialise =
                        SerialiserFactory.GetSerialiser<environmentType>(_settings.Accept).Deserialise(environmentBody);
                    Environment environmentResponse =
                        MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeToDeserialise);

                    _sessionToken = environmentResponse.SessionToken;
                    _environmentUrl = environmentResponse.InfrastructureServices.FirstOrDefault(
                        i => i.Name == InfrastructureServiceNames.environment)?.Value;

                    if (_log.IsDebugEnabled) _log.Debug($"Environment URL is {_environmentUrl}.");

                    _authorisationToken = _authorisationTokenService.Generate(_sessionToken, _settings.SharedSecret);
                    _sessionService.StoreSession(
                        environment.ApplicationInfo.ApplicationKey,
                        _sessionToken,
                        _environmentUrl,
                        environmentResponse.SolutionId,
                        environmentResponse.UserToken,
                        environmentResponse.InstanceId);
                    environment = environmentResponse;
                }
                catch (Exception e)
                {
                    if (_environmentUrl != null)
                    {
                        HttpUtils.DeleteRequest(_environmentUrl, _authorisationToken, _settings.CompressPayload);
                    }
                    else if (!string.IsNullOrWhiteSpace(TryParseEnvironmentUrl(environmentBody)))
                    {
                        HttpUtils.DeleteRequest(
                            TryParseEnvironmentUrl(environmentBody),
                            _authorisationToken,
                            _settings.CompressPayload);
                    }

                    throw new RegistrationException("Registration failed.", e);
                }
            }

            CurrentEnvironment = environment;
            Registered = true;
            return CurrentEnvironment;
        }

        /// <inheritdoc cref="IRegistrationService.Unregister" />
        public void Unregister(bool? deleteOnUnregister = null)
        {
            if (!Registered) return;

            if (deleteOnUnregister ?? _settings.DeleteOnUnregister)
            {
                HttpUtils.DeleteRequest(
                    _environmentUrl,
                    _authorisationToken,
                    _settings.CompressPayload,
                    contentTypeOverride: _settings.ContentType.ToDescription(),
                    acceptOverride: _settings.Accept.ToDescription());
                _sessionService.RemoveSession(_sessionToken);
            }

            Registered = false;
        }
    }
}