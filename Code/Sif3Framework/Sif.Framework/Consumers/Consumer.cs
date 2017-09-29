/*
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

using Sif.Framework.Extensions;
using Sif.Framework.Model.DataModels;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Query;
using Sif.Framework.Model.Responses;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
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
    public class Consumer<TSingle, TMultiple, TPrimaryKey> : IConsumer<TSingle, TMultiple, TPrimaryKey> where TSingle : ISifRefId<TPrimaryKey>
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Environment environmentTemplate;

        /// <summary>
        /// Consumer environment.
        /// </summary>
        protected Environment EnvironmentTemplate
        {

            get
            {
                return environmentTemplate;
            }

        }

        /// <summary>
        /// Queue associated with Consumer SIF Events.
        /// </summary>
        protected queueType Queue { get; private set; }

        /// <summary>
        /// Service for Consumer registration.
        /// </summary>
        protected IRegistrationService RegistrationService { get; private set; }

        /// <summary>
        /// Subscription associated with Consumer SIF Events.
        /// </summary>
        protected subscriptionType Subscription { get; private set; }

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
        public Consumer(Environment environment)
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
            Environment environment = new Environment(applicationKey, instanceId, userToken, solutionId);
            environmentTemplate = EnvironmentUtils.MergeWithSettings(environment, SettingsManager.ConsumerSettings);
            RegistrationService = new RegistrationService(SettingsManager.ConsumerSettings, SessionsManager.ConsumerSessionService);
        }

        /// <summary>
        /// <see cref="IPayloadSerialisable{TSingle,TMultiple}.SerialiseSingle(TSingle)">SerialiseSingle</see>
        /// </summary>
        public virtual string SerialiseSingle(TSingle obj)
        {
            return SerialiserFactory.GetXmlSerialiser<TSingle>().Serialise(obj);
        }

        /// <summary>
        /// <see cref="IPayloadSerialisable{TSingle,TMultiple}.SerialiseMultiple(TMultiple)">SerialiseMultiple</see>
        /// </summary>
        public virtual string SerialiseMultiple(TMultiple obj)
        {
            return SerialiserFactory.GetXmlSerialiser<TMultiple>().Serialise(obj);
        }

        /// <summary>
        /// Serialise a queueType object.
        /// </summary>
        /// <param name="obj">queueType object.</param>
        /// <returns>XML string representation of the queueType object.</returns>
        public virtual string SerialiseQueue(queueType obj)
        {
            return SerialiserFactory.GetXmlSerialiser<queueType>().Serialise(obj);
        }

        /// <summary>
        /// Serialise a subscriptionType object.
        /// </summary>
        /// <param name="obj">subscriptionType object.</param>
        /// <returns>XML string representation of the subscriptionType object.</returns>
        public virtual string SerialiseSubscription(subscriptionType obj)
        {
            return SerialiserFactory.GetXmlSerialiser<subscriptionType>().Serialise(obj);
        }

        /// <summary>
        /// <see cref="IPayloadSerialisable{TSingle,TMultiple}.DeserialiseSingle(string)">DeserialiseSingle</see>
        /// </summary>
        public virtual TSingle DeserialiseSingle(string payload)
        {
            return SerialiserFactory.GetXmlSerialiser<TSingle>().Deserialise(payload);
        }

        /// <summary>
        /// <see cref="IPayloadSerialisable{TSingle,TMultiple}.DeserialiseMultiple(string)">DeserialiseMultiple</see>
        /// </summary>
        public virtual TMultiple DeserialiseMultiple(string payload)
        {
            return SerialiserFactory.GetXmlSerialiser<TMultiple>().Deserialise(payload);
        }

        /// <summary>
        /// Deserialise an XML string representation of a queueType object.
        /// </summary>
        /// <param name="xml">XML string representation of a queueType object.</param>
        /// <returns>queueType object.</returns>
        private queueType DeserialiseQueue(string xml)
        {
            return SerialiserFactory.GetXmlSerialiser<queueType>().Deserialise(xml);
        }

        /// <summary>
        /// Deserialise an XML string representation of a subscriptionType object.
        /// </summary>
        /// <param name="xml">XML string representation of a subscriptionType object.</param>
        /// <returns>subscriptionType object.</returns>
        private subscriptionType DeserialiseSubscription(string xml)
        {
            return SerialiserFactory.GetXmlSerialiser<subscriptionType>().Deserialise(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Register()">Register</see>
        /// </summary>
        public void Register()
        {
            RegistrationService.Register(ref environmentTemplate);
            queueType newQueue = new queueType();
            Queue = CreateQueue(newQueue);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Unregister(bool?)">Unregister</see>
        /// </summary>
        public void Unregister(bool? deleteOnUnregister = null)
        {
            RegistrationService.Unregister(deleteOnUnregister);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.GetChangesSinceMarker(string, string)">GetChangesSinceMarker</see>
        /// </summary>
        public string GetChangesSinceMarker(string zoneId = null, string contextId = null)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + HttpUtils.MatrixParameters(zoneId, contextId);
            WebHeaderCollection responseHeaders = HttpUtils.HeadRequest(url, RegistrationService.AuthorisationToken);

            return responseHeaders[HttpUtils.RequestHeader.changesSinceMarker.ToDescription()];
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Create(TSingle, bool?, string, string)">Create</see>
        /// </summary>
        public virtual TSingle Create(TSingle obj, bool? mustUseAdvisory = null, string zoneId = null, string contextId = null)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + TypeName + HttpUtils.MatrixParameters(zoneId, contextId);
            string body = SerialiseSingle(obj);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body, mustUseAdvisory: mustUseAdvisory);
            if (log.IsDebugEnabled) log.Debug("XML from POST request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseSingle(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Create(TMultiple, bool?, string, string)">Create</see>
        /// </summary>
        public virtual MultipleCreateResponse Create(TMultiple obj, bool? mustUseAdvisory = null, string zoneId = null, string contextId = null)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + HttpUtils.MatrixParameters(zoneId, contextId);
            string body = SerialiseMultiple(obj);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body, mustUseAdvisory: mustUseAdvisory);
            if (log.IsDebugEnabled) log.Debug("XML from POST request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            createResponseType createResponseType = SerialiserFactory.GetXmlSerialiser<createResponseType>().Deserialise(xml);
            MultipleCreateResponse createResponse = MapperFactory.CreateInstance<createResponseType, MultipleCreateResponse>(createResponseType);

            return createResponse;
        }

        private queueType CreateQueue(queueType obj, string zoneId = null, string contextId = null)
        {
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate, connector: InfrastructureServiceNames.queues);
            string body = SerialiseQueue(obj);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug($"Response from POST {url} request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseQueue(xml);
        }

        private subscriptionType CreateSubscription(subscriptionType obj, string zoneId = null, string contextId = null)
        {
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate, connector: InfrastructureServiceNames.queues);
            string body = SerialiseSubscription(obj);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug($"Response from POST {url} request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseSubscription(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Query(TPrimaryKey, string, string)">Query</see>
        /// </summary>
        public virtual TSingle Query(TPrimaryKey refId, string zoneId = null, string contextId = null)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            TSingle obj = default(TSingle);

            try
            {
                string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + refId + HttpUtils.MatrixParameters(zoneId, contextId);
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
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Query(uint?, uint?, string, string)">Query</see>
        /// </summary>
        public virtual TMultiple Query(uint? navigationPage = null, uint? navigationPageSize = null, string zoneId = null, string contextId = null)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + HttpUtils.MatrixParameters(zoneId, contextId);
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
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.QueryByExample(TSingle, uint?, uint?, string, string)">QueryByExample</see>
        /// </summary>
        public virtual TMultiple QueryByExample(TSingle obj, uint? navigationPage = null, uint? navigationPageSize = null, string zoneId = null, string contextId = null)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + HttpUtils.MatrixParameters(zoneId, contextId);
            string body = SerialiseSingle(obj);
            // TODO: Update PostRequest to accept paging parameters.
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body, methodOverride: "GET");
            if (log.IsDebugEnabled) log.Debug("XML from POST (Query by Example) request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseMultiple(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.QueryByServicePath(IEnumerable{EqualCondition}, uint?, uint?, string, string)">QueryByServicePath</see>
        /// </summary>
        public virtual TMultiple QueryByServicePath(IEnumerable<EqualCondition> conditions, uint? navigationPage = null, uint? navigationPageSize = null, string zoneId = null, string contextId = null)
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

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + servicePath + "/" + TypeName + "s" + HttpUtils.MatrixParameters(zoneId, contextId);
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
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.QueryChangesSince(string, out string, uint?, uint?, string, string)">QueryChangesSince</see>
        /// </summary>
        public TMultiple QueryChangesSince(string changesSinceMarker, out string nextChangesSinceMarker, uint? navigationPage = null, uint? navigationPageSize = null, string zoneId = null, string contextId = null)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string changesSinceParameter = (changesSinceMarker == null ? string.Empty : "?changesSinceMarker=" + changesSinceMarker);
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + HttpUtils.MatrixParameters(zoneId, contextId) + changesSinceParameter;
            WebHeaderCollection responseHeaders;
            string xml;

            if (navigationPage.HasValue && navigationPageSize.HasValue)
            {
                xml = HttpUtils.GetRequestAndHeaders(url, RegistrationService.AuthorisationToken, out responseHeaders, (int)navigationPage, (int)navigationPageSize);
            }
            else
            {
                xml = HttpUtils.GetRequestAndHeaders(url, RegistrationService.AuthorisationToken, out responseHeaders);
            }

            nextChangesSinceMarker = responseHeaders[HttpUtils.RequestHeader.changesSinceMarker.ToDescription()];

            return DeserialiseMultiple(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Update(TSingle, string, string)">Update</see>
        /// </summary>
        public virtual void Update(TSingle obj, string zoneId = null, string contextId = null)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + obj.RefId + HttpUtils.MatrixParameters(zoneId, contextId);
            string body = SerialiseSingle(obj);
            string xml = HttpUtils.PutRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from PUT request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKeyPK}.Update(TMultiple, string, string)">Update</see>
        /// </summary>
        public virtual MultipleUpdateResponse Update(TMultiple obj, string zoneId = null, string contextId = null)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + HttpUtils.MatrixParameters(zoneId, contextId);
            string body = SerialiseMultiple(obj);
            string xml = HttpUtils.PutRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from PUT request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            updateResponseType updateResponseType = SerialiserFactory.GetXmlSerialiser<updateResponseType>().Deserialise(xml);
            MultipleUpdateResponse updateResponse = MapperFactory.CreateInstance<updateResponseType, MultipleUpdateResponse>(updateResponseType);

            return updateResponse;
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Delete(TPrimaryKey, string, string)">Delete</see>
        /// </summary>
        public virtual void Delete(TPrimaryKey refId, string zoneId = null, string contextId = null)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + refId + HttpUtils.MatrixParameters(zoneId, contextId);
            string xml = HttpUtils.DeleteRequest(url, RegistrationService.AuthorisationToken);
            if (log.IsDebugEnabled) log.Debug("XML from DELETE request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Delete(IEnumerable{TPrimaryKey}, string, string)">Delete</see>
        /// </summary>
        public virtual MultipleDeleteResponse Delete(IEnumerable<TPrimaryKey> refIds, string zoneId = null, string contextId = null)
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
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + HttpUtils.MatrixParameters(zoneId, contextId);
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
