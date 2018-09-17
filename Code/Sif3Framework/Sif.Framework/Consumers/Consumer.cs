/*
 * Copyright 2018 Systemic Pty Ltd
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
using Sif.Framework.Model.Responses;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Sif.Framework.Consumers
{
    /// <summary>
    /// This class defines a Consumer of SIF data model objects.
    /// </summary>
    /// <typeparam name="TSingle">Type that defines a single object entity.</typeparam>
    /// <typeparam name="TMultiple">Type that defines a multiple objects entity.</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the SIF data model object.</typeparam>
    public class Consumer<TSingle, TMultiple, TPrimaryKey> : IConsumer<TSingle, TMultiple, TPrimaryKey> where TSingle : ISifRefId<TPrimaryKey>
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Model.Infrastructure.Environment environmentTemplate;

        /// <summary>
        /// Consumer environment.
        /// </summary>
        protected Model.Infrastructure.Environment EnvironmentTemplate
        {
            get
            {
                return environmentTemplate;
            }
        }

        /// <summary>
        /// Service for Consumer registration.
        /// </summary>
        protected IRegistrationService RegistrationService { get; private set; }

        /// <summary>
        /// Name of the SIF data model that the Consumer is based on, e.g. SchoolInfo, StudentPersonal, etc.
        /// </summary>
        protected virtual string TypeName
        {
            get
            {
                return typeof(TSingle).Name;
            }
        }

        /// <summary>
        /// Create a Consumer instance based upon the Environment passed.
        /// </summary>
        /// <param name="environment">Environment object.</param>
        public Consumer(Model.Infrastructure.Environment environment)
        {
            environmentTemplate = EnvironmentUtils.MergeWithSettings(environment, SettingsManager.ConsumerSettings);
            RegistrationService = new RegistrationService(SettingsManager.ConsumerSettings, SessionsManager.ConsumerSessionService);
        }

        /// <summary>
        /// Create a Consumer instance identified by the parameters passed.
        /// </summary>
        /// <param name="applicationKey">Application key.</param>
        /// <param name="instanceId">Instance ID.</param>
        /// <param name="userToken">User token.</param>
        /// <param name="solutionId">Solution ID.</param>
        public Consumer(string applicationKey, string instanceId = null, string userToken = null, string solutionId = null)
        {
            Model.Infrastructure.Environment environment = new Model.Infrastructure.Environment(applicationKey, instanceId, userToken, solutionId);
            environmentTemplate = EnvironmentUtils.MergeWithSettings(environment, SettingsManager.ConsumerSettings);
            RegistrationService = new RegistrationService(SettingsManager.ConsumerSettings, SessionsManager.ConsumerSessionService);
        }

        /// <summary>
        /// Generate a query parameter string based upon the message parameters provided. If not empty, the returned
        /// query parameter string will be prefixed with ?.
        /// </summary>
        /// <param name="messageParameters">Message parameters.</param>
        /// <returns>Query parameter string if message parameters exist; an empty string otherwise.</returns>
        private string GenerateQueryParameterString(params RequestParameter[] messageParameters)
        {
            string queryParameterString = string.Empty;

            if (messageParameters != null)
            {
                IEnumerable<string> queryParameters = messageParameters
                    .Where(m => m?.Type == ConveyanceType.QueryParameter)
                    .Select(m => $"{m.Name}={m.Value}");
                queryParameterString = string.Join("&", queryParameters);
                queryParameterString = (string.IsNullOrWhiteSpace(queryParameterString) ? string.Empty : $"?{queryParameterString}");
            }

            return queryParameterString;
        }

        /// <summary>
        /// Serialise a single object entity.
        /// </summary>
        /// <param name="obj">Payload of a single object.</param>
        /// <returns>XML string representation of the single object.</returns>
        protected virtual string SerialiseSingle(TSingle obj)
        {
            return SerialiserFactory.GetXmlSerialiser<TSingle>().Serialise(obj);
        }

        /// <summary>
        /// Serialise an entity of multiple objects.
        /// </summary>
        /// <param name="obj">Payload of multiple objects.</param>
        /// <returns>XML string representation of the multiple objects.</returns>
        protected virtual string SerialiseMultiple(TMultiple obj)
        {
            return SerialiserFactory.GetXmlSerialiser<TMultiple>().Serialise(obj);
        }

        /// <summary>
        /// Deserialise a single object entity.
        /// </summary>
        /// <param name="payload">Payload of a single object.</param>
        /// <returns>Entity representing the single object.</returns>
        protected virtual TSingle DeserialiseSingle(string payload)
        {
            return SerialiserFactory.GetXmlSerialiser<TSingle>().Deserialise(payload);
        }

        /// <summary>
        /// Deserialise an entity of multiple objects.
        /// </summary>
        /// <param name="payload">Payload of multiple objects.</param>
        /// <returns>Entity representing the multiple objects.</returns>
        protected virtual TMultiple DeserialiseMultiple(string payload)
        {
            return SerialiserFactory.GetXmlSerialiser<TMultiple>().Deserialise(payload);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Register()">Register</see>
        /// </summary>
        public void Register()
        {
            RegistrationService.Register(ref environmentTemplate);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Unregister(bool?)">Unregister</see>
        /// </summary>
        public void Unregister(bool? deleteOnUnregister = null)
        {
            RegistrationService.Unregister(deleteOnUnregister);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.GetChangesSinceMarker(string, string, RequestParameter[])">GetChangesSinceMarker</see>
        /// </summary>
        public virtual string GetChangesSinceMarker(
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = new StringBuilder(EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate))
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters)).ToString();
            WebHeaderCollection responseHeaders = HttpUtils.HeadRequest(url, RegistrationService.AuthorisationToken);

            return responseHeaders[ResponseParameterType.changesSinceMarker.ToDescription()];
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Create(TSingle, bool?, string, string, RequestParameter[])">Create</see>
        /// </summary>
        public virtual TSingle Create(
            TSingle obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = new StringBuilder(EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate))
                .Append($"/{TypeName}s")
                .Append($"/{TypeName}")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters)).ToString();
            string body = SerialiseSingle(obj);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body, mustUseAdvisory: mustUseAdvisory);
            if (log.IsDebugEnabled) log.Debug("XML from POST request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseSingle(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Create(TMultiple, bool?, string, string, RequestParameter[])">Create</see>
        /// </summary>
        public virtual MultipleCreateResponse Create(
            TMultiple obj,
            bool? mustUseAdvisory = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = new StringBuilder(EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate))
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters)).ToString();
            string body = SerialiseMultiple(obj);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body, mustUseAdvisory: mustUseAdvisory);
            if (log.IsDebugEnabled) log.Debug("XML from POST request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            createResponseType createResponseType = SerialiserFactory.GetXmlSerialiser<createResponseType>().Deserialise(xml);
            MultipleCreateResponse createResponse = MapperFactory.CreateInstance<createResponseType, MultipleCreateResponse>(createResponseType);

            return createResponse;
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Query(TPrimaryKey, string, string, RequestParameter[])">Query</see>
        /// </summary>
        public virtual TSingle Query(
            TPrimaryKey refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            TSingle obj = default(TSingle);

            try
            {
                string url = new StringBuilder(EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate))
                    .Append($"/{TypeName}s")
                    .Append($"/{refId}")
                    .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                    .Append(GenerateQueryParameterString(requestParameters)).ToString();
                string xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken);
                if (log.IsDebugEnabled) log.Debug("XML from GET request ...");
                if (log.IsDebugEnabled) log.Debug(xml);
                obj = DeserialiseSingle(xml);
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
            catch (Exception)
            {
                throw;
            }

            return obj;
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Query(uint?, uint?, string, string, RequestParameter[])">Query</see>
        /// </summary>
        public virtual TMultiple Query(
            uint? navigationPage = null,
            uint? navigationPageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = new StringBuilder(EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate))
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters)).ToString();
            string xml;

            if (navigationPage.HasValue && navigationPageSize.HasValue)
            {
                xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken, navigationPage: (int)navigationPage, navigationPageSize: (int)navigationPageSize);
            }
            else
            {
                xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken);
            }

            return DeserialiseMultiple(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.QueryByExample(TSingle, uint?, uint?, string, string, RequestParameter[])">QueryByExample</see>
        /// </summary>
        public virtual TMultiple QueryByExample(
            TSingle obj,
            uint? navigationPage = null,
            uint? navigationPageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = new StringBuilder(EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate))
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters)).ToString();
            string body = SerialiseSingle(obj);
            // TODO: Update PostRequest to accept paging parameters.
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body, methodOverride: "GET");
            if (log.IsDebugEnabled) log.Debug("XML from POST (Query by Example) request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseMultiple(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.QueryByServicePath(IEnumerable{EqualCondition}, uint?, uint?, string, string, RequestParameter[])">QueryByServicePath</see>
        /// </summary>
        public virtual TMultiple QueryByServicePath(
            IEnumerable<EqualCondition> conditions,
            uint? navigationPage = null,
            uint? navigationPageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            StringBuilder servicePath = new StringBuilder();

            if (conditions != null)
            {
                foreach (EqualCondition condition in conditions)
                {
                    servicePath.Append("/" + condition.Left + "/" + condition.Right);
                }
            }

            string url = new StringBuilder(EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate))
                .Append(servicePath)
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters)).ToString();
            if (log.IsDebugEnabled) log.Debug("Service Path URL is " + url);
            string xml;

            if (navigationPage.HasValue && navigationPageSize.HasValue)
            {
                xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken, ServiceType.SERVICEPATH, navigationPage: (int)navigationPage, navigationPageSize: (int)navigationPageSize);
            }
            else
            {
                xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken, ServiceType.SERVICEPATH);
            }

            return DeserialiseMultiple(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.QueryChangesSince(string, out string, uint?, uint?, string, string, RequestParameter[])">QueryChangesSince</see>
        /// </summary>
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
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            RequestParameter[] messageParameters = (requestParameters ?? (new RequestParameter[0]));
            messageParameters = string.IsNullOrWhiteSpace(changesSinceMarker)
                ? messageParameters
                : messageParameters.Concat(new RequestParameter[1] { new ChangesSinceQueryParameter(changesSinceMarker) }).ToArray();
            string url = new StringBuilder(EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate))
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(messageParameters)).ToString();
            WebHeaderCollection responseHeaders;
            string xml;

            if (navigationPage.HasValue && navigationPageSize.HasValue)
            {
                xml = HttpUtils.GetRequestAndHeaders(url, RegistrationService.AuthorisationToken, out responseHeaders, navigationPage: (int)navigationPage, navigationPageSize: (int)navigationPageSize);
            }
            else
            {
                xml = HttpUtils.GetRequestAndHeaders(url, RegistrationService.AuthorisationToken, out responseHeaders);
            }

            nextChangesSinceMarker = responseHeaders[ResponseParameterType.changesSinceMarker.ToDescription()];

            return DeserialiseMultiple(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.QueryChangesSince(string, out string, uint?, uint?, string, string, RequestParameter[])">QueryChangesSince</see>
        /// </summary>
        public TMultiple DynamicQuery(
            string whereClause,
            uint? navigationPage = null,
            uint? navigationPageSize = null,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            RequestParameter[] messageParameters = (requestParameters ?? (new RequestParameter[0]));
            messageParameters = string.IsNullOrWhiteSpace(whereClause)
                ? messageParameters
                : messageParameters.Concat(new RequestParameter[1] { new DynamicQueryParameter(whereClause) }).ToArray();
            string url = new StringBuilder(EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate))
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(messageParameters)).ToString();
            string xml;

            if (navigationPage.HasValue && navigationPageSize.HasValue)
            {
                xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken, navigationPage: (int)navigationPage, navigationPageSize: (int)navigationPageSize);
            }
            else
            {
                xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken);
            }

            return DeserialiseMultiple(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Update(TSingle, string, string, RequestParameter[])">Update</see>
        /// </summary>
        public virtual void Update(
            TSingle obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = new StringBuilder(EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate))
                .Append($"/{TypeName}s")
                .Append($"/{obj.RefId}")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters)).ToString();
            string body = SerialiseSingle(obj);
            string xml = HttpUtils.PutRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from PUT request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKeyPK}.Update(TMultiple, string, string, RequestParameter[])">Update</see>
        /// </summary>
        public virtual MultipleUpdateResponse Update(
            TMultiple obj,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = new StringBuilder(EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate))
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters)).ToString();
            string body = SerialiseMultiple(obj);
            string xml = HttpUtils.PutRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from PUT request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            updateResponseType updateResponseType = SerialiserFactory.GetXmlSerialiser<updateResponseType>().Deserialise(xml);
            MultipleUpdateResponse updateResponse = MapperFactory.CreateInstance<updateResponseType, MultipleUpdateResponse>(updateResponseType);

            return updateResponse;
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Delete(TPrimaryKey, string, string, RequestParameter[])">Delete</see>
        /// </summary>
        public virtual void Delete(
            TPrimaryKey refId,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = new StringBuilder(EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate))
                .Append($"/{TypeName}s")
                .Append($"/{refId}")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters)).ToString();
            string xml = HttpUtils.DeleteRequest(url, RegistrationService.AuthorisationToken);
            if (log.IsDebugEnabled) log.Debug("XML from DELETE request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Delete(IEnumerable{TPrimaryKey}, string, string, RequestParameter[])">Delete</see>
        /// </summary>
        public virtual MultipleDeleteResponse Delete(
            IEnumerable<TPrimaryKey> refIds,
            string zoneId = null,
            string contextId = null,
            params RequestParameter[] requestParameters)
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            List<deleteIdType> deleteIds = new List<deleteIdType>();

            foreach (TPrimaryKey id in refIds)
            {
                deleteIdType deleteId = new deleteIdType { id = id.ToString() };
                deleteIds.Add(deleteId);
            }

            deleteRequestType request = new deleteRequestType { deletes = deleteIds.ToArray() };
            string url = new StringBuilder(EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate))
                .Append($"/{TypeName}s")
                .Append(HttpUtils.MatrixParameters(zoneId, contextId))
                .Append(GenerateQueryParameterString(requestParameters)).ToString();
            string body = SerialiserFactory.GetXmlSerialiser<deleteRequestType>().Serialise(request);
            string xml = HttpUtils.PutRequest(url, RegistrationService.AuthorisationToken, body, methodOverride: "DELETE");
            if (log.IsDebugEnabled) log.Debug("XML from PUT (DELETE) request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            deleteResponseType updateResponseType = SerialiserFactory.GetXmlSerialiser<deleteResponseType>().Deserialise(xml);
            MultipleDeleteResponse updateResponse = MapperFactory.CreateInstance<deleteResponseType, MultipleDeleteResponse>(updateResponseType);

            return updateResponse;
        }
    }
}