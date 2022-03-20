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
using Sif.Framework.Model.DataModels;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Query;
using Sif.Framework.Model.Requests;
using Sif.Framework.Model.Responses;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Service.Sessions;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Consumers
{
    /// <summary>
    /// This class defines a Consumer of SIF data model objects.
    /// </summary>
    /// <typeparam name="TSingle">Type that defines a single object entity.</typeparam>
    /// <typeparam name="TMultiple">Type that defines a multiple objects entity.</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the SIF data model object.</typeparam>
    public class Consumer<TSingle, TMultiple, TPrimaryKey> : IConsumer<TSingle, TMultiple, TPrimaryKey>
        where TSingle : ISifRefId<TPrimaryKey>
    {
        private readonly slf4net.ILogger _log =
            slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private Environment _environmentTemplate;

        /// <summary>
        /// Accepted content type (XML or JSON) for a message payload.
        /// </summary>
        protected Accept Accept => ConsumerSettings.Accept;

        /// <summary>
        /// Application settings associated with the Consumer.
        /// </summary>
        protected IFrameworkSettings ConsumerSettings { get; }

        /// <summary>
        /// Content type (XML or JSON) of the message payload.
        /// </summary>
        protected ContentType ContentType => ConsumerSettings.ContentType;

        /// <summary>
        /// Consumer environment.
        /// </summary>
        protected Environment EnvironmentTemplate => _environmentTemplate;

        /// <summary>
        /// Service for Consumer registration.
        /// </summary>
        protected IRegistrationService RegistrationService { get; }

        /// <summary>
        /// Name of the SIF data model that the Consumer is based on, e.g. SchoolInfo, StudentPersonal, etc.
        /// </summary>
        protected virtual string TypeName => typeof(TSingle).Name;

        /// <summary>
        /// Create a Consumer instance based upon the Environment passed.
        /// </summary>
        /// <param name="environment">Environment object.</param>
        /// <param name="settings">Consumer settings. If null, Consumer settings will be read from the SifFramework.config file.</param>
        /// <param name="sessionService">Consumer session service. If null, the Consumer session will be stored in the SifFramework.config file.</param>
        protected Consumer(
            Environment environment,
            IFrameworkSettings settings = null,
            ISessionService sessionService = null)
        {
            ConsumerSettings = settings ?? SettingsManager.ConsumerSettings;
            _environmentTemplate = EnvironmentUtils.MergeWithSettings(environment, ConsumerSettings);
            RegistrationService =
                new RegistrationService(ConsumerSettings, sessionService ?? SessionsManager.ConsumerSessionService);
        }

        /// <summary>
        /// Create a Consumer instance identified by the parameters passed.
        /// </summary>
        /// <param name="applicationKey">Application key.</param>
        /// <param name="instanceId">Instance ID.</param>
        /// <param name="userToken">User token.</param>
        /// <param name="solutionId">Solution ID.</param>
        /// <param name="settings">Consumer settings. If null, Consumer settings will be read from the SifFramework.config file.</param>
        /// <param name="sessionService">Consumer session service. If null, the Consumer session will be stored in the SifFramework.config file.</param>
        public Consumer(
            string applicationKey,
            string instanceId = null,
            string userToken = null,
            string solutionId = null,
            IFrameworkSettings settings = null,
            ISessionService sessionService = null)
            : this(new Environment(applicationKey, instanceId, userToken, solutionId), settings, sessionService)
        {
        }

        /// <summary>
        /// Generate a query parameter string based upon the message parameters provided. If not empty, the returned
        /// query parameter string will be prefixed with ?.
        /// </summary>
        /// <param name="messageParameters">Message parameters.</param>
        /// <returns>Query parameter string if message parameters exist; an empty string otherwise.</returns>
        private static string GenerateQueryParameterString(params RequestParameter[] messageParameters)
        {
            if (messageParameters == null) return string.Empty;

            IEnumerable<string> queryParameters = messageParameters
                .Where(m => m?.Type == ConveyanceType.QueryParameter)
                .Select(m => $"{m.Name}={m.Value}");
            string queryParameterString = string.Join("&", queryParameters);
            queryParameterString =
                string.IsNullOrWhiteSpace(queryParameterString) ? string.Empty : $"?{queryParameterString}";

            return queryParameterString;
        }

        /// <summary>
        /// Serialise a single object entity.
        /// </summary>
        /// <param name="obj">Payload of a single object.</param>
        /// <returns>String representation of the single object.</returns>
        protected virtual string SerialiseSingle(TSingle obj)
        {
            return SerialiserFactory.GetSerialiser<TSingle>(ContentType).Serialise(obj);
        }

        /// <summary>
        /// Serialise an entity of multiple objects.
        /// </summary>
        /// <param name="obj">Payload of multiple objects.</param>
        /// <returns>String representation of the multiple objects.</returns>
        protected virtual string SerialiseMultiple(TMultiple obj)
        {
            return SerialiserFactory.GetSerialiser<TMultiple>(ContentType).Serialise(obj);
        }

        /// <summary>
        /// Deserialise a single object entity.
        /// </summary>
        /// <param name="payload">Payload of a single object.</param>
        /// <returns>Entity representing the single object.</returns>
        protected virtual TSingle DeserialiseSingle(string payload)
        {
            return SerialiserFactory.GetSerialiser<TSingle>(Accept).Deserialise(payload);
        }

        /// <summary>
        /// Deserialise an entity of multiple objects.
        /// </summary>
        /// <param name="payload">Payload of multiple objects.</param>
        /// <returns>Entity representing the multiple objects.</returns>
        protected virtual TMultiple DeserialiseMultiple(string payload)
        {
            return SerialiserFactory.GetSerialiser<TMultiple>(Accept).Deserialise(payload);
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Register()" />
        public void Register()
        {
            RegistrationService.Register(ref _environmentTemplate);
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Unregister(bool?)" />
        public void Unregister(bool? deleteOnUnregister = null)
        {
            RegistrationService.Unregister(deleteOnUnregister);
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.GetChangesSinceMarker(string, string, RequestParameter[])" />
        public virtual string GetChangesSinceMarker(
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
                throw new InvalidOperationException("Consumer has not been registered.");

            var url = new StringBuilder(EnvironmentTemplate.ParseServiceUrl())
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters))
                .ToString();
            WebHeaderCollection responseHeaders =
                HttpUtils.HeadRequest(url, RegistrationService.AuthorisationToken, ConsumerSettings.CompressPayload);

            return responseHeaders[ResponseParameterType.changesSinceMarker.ToDescription()];
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Create(TSingle, bool?, string, string, RequestParameter[])" />
        public virtual TSingle Create(
            TSingle obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
                throw new InvalidOperationException("Consumer has not been registered.");

            var url = new StringBuilder(EnvironmentTemplate.ParseServiceUrl())
                .Append($"/{TypeName}s")
                .Append($"/{TypeName}")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters))
                .ToString();
            string requestBody = SerialiseSingle(obj);
            string responseBody = HttpUtils.PostRequest(
                url,
                RegistrationService.AuthorisationToken,
                requestBody,
                ConsumerSettings.CompressPayload,
                contentTypeOverride: ContentType.ToDescription(),
                acceptOverride: Accept.ToDescription(),
                mustUseAdvisory: mustUseAdvisory);

            if (_log.IsDebugEnabled) _log.Debug("Response from POST request ...");
            if (_log.IsDebugEnabled) _log.Debug(responseBody);

            return DeserialiseSingle(responseBody);
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Create(TMultiple, bool?, string, string, RequestParameter[])" />
        public virtual MultipleCreateResponse Create(
            TMultiple obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
                throw new InvalidOperationException("Consumer has not been registered.");

            var url = new StringBuilder(EnvironmentTemplate.ParseServiceUrl())
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters))
                .ToString();
            string requestBody = SerialiseMultiple(obj);
            string responseBody = HttpUtils.PostRequest(
                url,
                RegistrationService.AuthorisationToken,
                requestBody,
                ConsumerSettings.CompressPayload,
                contentTypeOverride: ContentType.ToDescription(),
                acceptOverride: Accept.ToDescription(),
                mustUseAdvisory: mustUseAdvisory);

            if (_log.IsDebugEnabled) _log.Debug("Response from POST request ...");
            if (_log.IsDebugEnabled) _log.Debug(responseBody);

            createResponseType createResponseType =
                SerialiserFactory.GetSerialiser<createResponseType>(Accept).Deserialise(responseBody);
            MultipleCreateResponse createResponse =
                MapperFactory.CreateInstance<createResponseType, MultipleCreateResponse>(createResponseType);

            return createResponse;
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Query(TPrimaryKey, string, string, RequestParameter[])" />
        public virtual TSingle Query(
            TPrimaryKey refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
                throw new InvalidOperationException("Consumer has not been registered.");

            TSingle obj = default;

            try
            {
                var url = new StringBuilder(EnvironmentTemplate.ParseServiceUrl())
                    .Append($"/{TypeName}s")
                    .Append($"/{refId}")
                    .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                    .Append(GenerateQueryParameterString(requestParameters))
                    .ToString();
                string responseBody = HttpUtils.GetRequest(
                    url,
                    RegistrationService.AuthorisationToken,
                    ConsumerSettings.CompressPayload,
                    contentTypeOverride: ContentType.ToDescription(),
                    acceptOverride: Accept.ToDescription());

                if (_log.IsDebugEnabled) _log.Debug("Response from GET request ...");
                if (_log.IsDebugEnabled) _log.Debug(responseBody);

                obj = DeserialiseSingle(responseBody);
            }
            catch (WebException ex)
            {
                if (WebExceptionStatus.ProtocolError.Equals(ex.Status) && ex.Response != null)
                {
                    HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;

                    if (!HttpStatusCode.NotFound.Equals(statusCode))
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }

            return obj;
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Query(uint?, uint?, string, string, RequestParameter[])" />
        public virtual TMultiple Query(
            uint? navigationPage = null,
            uint? navigationPageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
                throw new InvalidOperationException("Consumer has not been registered.");

            var url = new StringBuilder(EnvironmentTemplate.ParseServiceUrl())
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters))
                .ToString();
            string responseBody;

            if (navigationPage.HasValue && navigationPageSize.HasValue)
            {
                responseBody = HttpUtils.GetRequest(
                    url,
                    RegistrationService.AuthorisationToken,
                    ConsumerSettings.CompressPayload,
                    navigationPage: (int)navigationPage,
                    navigationPageSize: (int)navigationPageSize,
                    contentTypeOverride: ContentType.ToDescription(),
                    acceptOverride: Accept.ToDescription());
            }
            else
            {
                responseBody = HttpUtils.GetRequest(
                    url,
                    RegistrationService.AuthorisationToken,
                    ConsumerSettings.CompressPayload,
                    contentTypeOverride: ContentType.ToDescription(),
                    acceptOverride: Accept.ToDescription());
            }

            return DeserialiseMultiple(responseBody);
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.QueryByExample(TSingle, uint?, uint?, string, string, RequestParameter[])" />
        public virtual TMultiple QueryByExample(
            TSingle obj,
            uint? navigationPage = null,
            uint? navigationPageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
                throw new InvalidOperationException("Consumer has not been registered.");

            var url = new StringBuilder(EnvironmentTemplate.ParseServiceUrl())
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters))
                .ToString();
            string requestBody = SerialiseSingle(obj);
            // TODO: Update PostRequest to accept paging parameters.
            string responseBody = HttpUtils.PostRequest(
                url,
                RegistrationService.AuthorisationToken,
                requestBody,
                ConsumerSettings.CompressPayload,
                methodOverride: "GET",
                contentTypeOverride: ContentType.ToDescription(),
                acceptOverride: Accept.ToDescription());

            if (_log.IsDebugEnabled) _log.Debug("Response from POST (Query by Example) request ...");
            if (_log.IsDebugEnabled) _log.Debug(responseBody);

            return DeserialiseMultiple(responseBody);
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.QueryByServicePath(IEnumerable{EqualCondition}, uint?, uint?, string, string, RequestParameter[])" />
        public virtual TMultiple QueryByServicePath(
            IEnumerable<EqualCondition> conditions,
            uint? navigationPage = null,
            uint? navigationPageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
                throw new InvalidOperationException("Consumer has not been registered.");

            var servicePath = new StringBuilder();

            if (conditions != null)
            {
                foreach (EqualCondition condition in conditions)
                {
                    servicePath.Append("/" + condition.Left + "/" + condition.Right);
                }
            }

            var url = new StringBuilder(EnvironmentTemplate.ParseServiceUrl())
                .Append(servicePath)
                .Append($"/{TypeName}s").Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters))
                .ToString();

            if (_log.IsDebugEnabled) _log.Debug($"Service Path URL is {url}.");

            string responseBody;

            if (navigationPage.HasValue && navigationPageSize.HasValue)
            {
                responseBody = HttpUtils.GetRequest(
                    url,
                    RegistrationService.AuthorisationToken,
                    ConsumerSettings.CompressPayload,
                    ServiceType.SERVICEPATH,
                    (int)navigationPage,
                    (int)navigationPageSize,
                    ContentType.ToDescription(),
                    Accept.ToDescription());
            }
            else
            {
                responseBody = HttpUtils.GetRequest(
                    url,
                    RegistrationService.AuthorisationToken,
                    ConsumerSettings.CompressPayload,
                    ServiceType.SERVICEPATH,
                    contentTypeOverride: ContentType.ToDescription(),
                    acceptOverride: Accept.ToDescription());
            }

            return DeserialiseMultiple(responseBody);
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.QueryChangesSince(string, out string, uint?, uint?, string, string, RequestParameter[])" />
        public virtual TMultiple QueryChangesSince(
            string changesSinceMarker,
            out string nextChangesSinceMarker,
            uint? navigationPage = null,
            uint? navigationPageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
                throw new InvalidOperationException("Consumer has not been registered.");

            RequestParameter[] messageParameters = requestParameters ?? Array.Empty<RequestParameter>();
            messageParameters = string.IsNullOrWhiteSpace(changesSinceMarker)
                ? messageParameters
                : messageParameters
                    .Concat(new RequestParameter[] { new ChangesSinceQueryParameter(changesSinceMarker) })
                    .ToArray();
            var url = new StringBuilder(EnvironmentTemplate.ParseServiceUrl())
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(messageParameters))
                .ToString();
            WebHeaderCollection responseHeaders;
            string responseBody;

            if (navigationPage.HasValue && navigationPageSize.HasValue)
            {
                responseBody = HttpUtils.GetRequestAndHeaders(
                    url,
                    RegistrationService.AuthorisationToken,
                    ConsumerSettings.CompressPayload,
                    out responseHeaders,
                    navigationPage: (int)navigationPage,
                    navigationPageSize: (int)navigationPageSize,
                    contentTypeOverride: ContentType.ToDescription(),
                    acceptOverride: Accept.ToDescription());
            }
            else
            {
                responseBody = HttpUtils.GetRequestAndHeaders(
                    url,
                    RegistrationService.AuthorisationToken,
                    ConsumerSettings.CompressPayload,
                    out responseHeaders,
                    contentTypeOverride: ContentType.ToDescription(),
                    acceptOverride: Accept.ToDescription());
            }

            nextChangesSinceMarker = responseHeaders[ResponseParameterType.changesSinceMarker.ToDescription()];

            return DeserialiseMultiple(responseBody);
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.QueryChangesSince(string, out string, uint?, uint?, string, string, RequestParameter[])" />
        public TMultiple DynamicQuery(
            string whereClause,
            uint? navigationPage = null,
            uint? navigationPageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
                throw new InvalidOperationException("Consumer has not been registered.");

            RequestParameter[] messageParameters = requestParameters ?? Array.Empty<RequestParameter>();
            messageParameters = string.IsNullOrWhiteSpace(whereClause)
                ? messageParameters
                : messageParameters.Concat(new RequestParameter[] { new DynamicQueryParameter(whereClause) }).ToArray();
            var url = new StringBuilder(EnvironmentTemplate.ParseServiceUrl())
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(messageParameters))
                .ToString();
            string responseBody;

            if (navigationPage.HasValue && navigationPageSize.HasValue)
            {
                responseBody = HttpUtils.GetRequest(
                    url,
                    RegistrationService.AuthorisationToken,
                    ConsumerSettings.CompressPayload,
                    navigationPage: (int)navigationPage,
                    navigationPageSize: (int)navigationPageSize,
                    contentTypeOverride: ContentType.ToDescription(),
                    acceptOverride: Accept.ToDescription());
            }
            else
            {
                responseBody = HttpUtils.GetRequest(
                    url,
                    RegistrationService.AuthorisationToken,
                    ConsumerSettings.CompressPayload,
                    contentTypeOverride: ContentType.ToDescription(),
                    acceptOverride: Accept.ToDescription());
            }

            return DeserialiseMultiple(responseBody);
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Update(TSingle, string, string, RequestParameter[])" />
        public virtual void Update(
            TSingle obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
                throw new InvalidOperationException("Consumer has not been registered.");

            var url = new StringBuilder(EnvironmentTemplate.ParseServiceUrl())
                .Append($"/{TypeName}s")
                .Append($"/{obj.RefId}")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters))
                .ToString();
            string requestBody = SerialiseSingle(obj);
            string responseBody = HttpUtils.PutRequest(
                url,
                RegistrationService.AuthorisationToken,
                requestBody,
                ConsumerSettings.CompressPayload,
                contentTypeOverride: ContentType.ToDescription(),
                acceptOverride: Accept.ToDescription());

            if (_log.IsDebugEnabled) _log.Debug("Response from PUT request ...");
            if (_log.IsDebugEnabled) _log.Debug(responseBody);
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKeyPK}.Update(TMultiple, string, string, RequestParameter[])" />
        public virtual MultipleUpdateResponse Update(
            TMultiple obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
                throw new InvalidOperationException("Consumer has not been registered.");

            var url = new StringBuilder(EnvironmentTemplate.ParseServiceUrl())
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters))
                .ToString();
            string requestBody = SerialiseMultiple(obj);
            string responseBody = HttpUtils.PutRequest(
                url,
                RegistrationService.AuthorisationToken,
                requestBody,
                ConsumerSettings.CompressPayload,
                contentTypeOverride: ContentType.ToDescription(),
                acceptOverride: Accept.ToDescription());

            if (_log.IsDebugEnabled) _log.Debug("Response from PUT request ...");
            if (_log.IsDebugEnabled) _log.Debug(responseBody);

            updateResponseType updateResponseType =
                SerialiserFactory.GetSerialiser<updateResponseType>(Accept).Deserialise(responseBody);
            MultipleUpdateResponse updateResponse =
                MapperFactory.CreateInstance<updateResponseType, MultipleUpdateResponse>(updateResponseType);

            return updateResponse;
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Delete(TPrimaryKey, string, string, RequestParameter[])" />
        public virtual void Delete(
            TPrimaryKey refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
                throw new InvalidOperationException("Consumer has not been registered.");

            var url = new StringBuilder(EnvironmentTemplate.ParseServiceUrl())
                .Append($"/{TypeName}s")
                .Append($"/{refId}")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters))
                .ToString();
            string responseBody = HttpUtils.DeleteRequest(
                url,
                RegistrationService.AuthorisationToken,
                ConsumerSettings.CompressPayload,
                contentTypeOverride: ContentType.ToDescription(),
                acceptOverride: Accept.ToDescription());

            if (_log.IsDebugEnabled) _log.Debug("Response from DELETE request ...");
            if (_log.IsDebugEnabled) _log.Debug(responseBody);
        }

        /// <inheritdoc cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Delete(IEnumerable{TPrimaryKey}, string, string, RequestParameter[])" />
        public virtual MultipleDeleteResponse Delete(
            IEnumerable<TPrimaryKey> refIds,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
                throw new InvalidOperationException("Consumer has not been registered.");

            var request = new deleteRequestType
            {
                deletes = refIds.Select(id => new deleteIdType { id = id.ToString() }).ToArray()
            };

            var url = new StringBuilder(EnvironmentTemplate.ParseServiceUrl())
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters))
                .ToString();
            string requestBody = SerialiserFactory.GetSerialiser<deleteRequestType>(ContentType).Serialise(request);
            string responseBody = HttpUtils.PutRequest(
                url,
                RegistrationService.AuthorisationToken,
                requestBody,
                ConsumerSettings.CompressPayload,
                methodOverride: "DELETE",
                contentTypeOverride: ContentType.ToDescription(),
                acceptOverride: Accept.ToDescription());

            if (_log.IsDebugEnabled) _log.Debug("Response from PUT (DELETE) request ...");
            if (_log.IsDebugEnabled) _log.Debug(responseBody);

            deleteResponseType updateResponseType =
                SerialiserFactory.GetSerialiser<deleteResponseType>(Accept).Deserialise(responseBody);
            MultipleDeleteResponse updateResponse =
                MapperFactory.CreateInstance<deleteResponseType, MultipleDeleteResponse>(updateResponseType);

            return updateResponse;
        }
    }
}