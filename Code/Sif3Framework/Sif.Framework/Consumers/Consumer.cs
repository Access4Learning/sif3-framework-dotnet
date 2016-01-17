/*
 * Copyright 2016 Systemic Pty Ltd
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
using Sif.Framework.Model.DataModels;
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
using System.Reflection;
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
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Environment environmentTemplate;
        private RegistrationService registrationService;

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
        /// Service for Consumer registration.
        /// </summary>
        protected RegistrationService RegistrationService
        {

            get
            {
                return registrationService;
            }

        }

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
            registrationService = new RegistrationService(SettingsManager.ConsumerSettings, SessionsManager.ConsumerSessionService);
        }

        /// <summary>
        /// Create a Consumer instance identified by the parameters passed.
        /// </summary>
        /// <param name="applicationKey">Application key.</param>
        /// <param name="instanceId">Instance ID.</param>
        /// <param name="userToken">User token.</param>
        /// <param name="solutionId">Olution ID.</param>
        public Consumer(string applicationKey, string instanceId = null, string userToken = null, string solutionId = null)
        {
            Environment environment = new Environment(applicationKey, instanceId, userToken, solutionId);
            environmentTemplate = EnvironmentUtils.MergeWithSettings(environment, SettingsManager.ConsumerSettings);
            registrationService = new RegistrationService(SettingsManager.ConsumerSettings, SessionsManager.ConsumerSessionService);
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
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Register()">Register</see>
        /// </summary>
        public void Register()
        {
            registrationService.Register(ref environmentTemplate);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Unregister()">Unregister</see>
        /// </summary>
        public void Unregister(bool? deleteOnUnregister = null)
        {
            registrationService.Unregister(deleteOnUnregister);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Create(TSingle)">Create</see>
        /// </summary>
        public virtual TSingle Create(TSingle obj)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + TypeName;
            string body = SerialiseSingle(obj);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from POST request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseSingle(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Create(TMultiple)">Create</see>
        /// </summary>
        public virtual MultipleCreateResponse Create(TMultiple obj)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s";
            string body = SerialiseMultiple(obj);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from POST request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            createResponseType createResponseType = SerialiserFactory.GetXmlSerialiser<createResponseType>().Deserialise(xml);
            MultipleCreateResponse createResponse = MapperFactory.CreateInstance<createResponseType, MultipleCreateResponse>(createResponseType);

            return createResponse;
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Query(TPrimaryKey)">Query</see>
        /// </summary>
        public virtual TSingle Query(TPrimaryKey refId)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            TSingle obj = default(TSingle);

            try
            {
                string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + refId;
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
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Query(uint?, uint?)">Query</see>
        /// </summary>
        public virtual TMultiple Query(uint? navigationPage = null, uint? navigationPageSize = null)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s";
            string xml;

            if (navigationPage.HasValue && navigationPageSize.HasValue)
            {
                xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken, (int)navigationPage, (int)navigationPageSize);
            }
            else
            {
                xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken);
            }

            return DeserialiseMultiple(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.QueryByExample(TSingle, uint?, uint?)">Query</see>
        /// </summary>
        public virtual TMultiple QueryByExample(TSingle obj, uint? navigationPage = null, uint? navigationPageSize = null)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s";
            string body = SerialiseSingle(obj);
            // TODO: Update PostRequest to accept paging parameters.
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body, "GET");
            if (log.IsDebugEnabled) log.Debug("XML from POST (Query by Example) request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseMultiple(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.QueryByServicePath(IEnumerable{EqualCondition}, uint?, uint?)">Query</see>
        /// </summary>
        public virtual TMultiple QueryByServicePath(IEnumerable<EqualCondition> conditions, uint? navigationPage = null, uint? navigationPageSize = null)
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

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + servicePath + "/" + TypeName + "s";
            if (log.IsDebugEnabled) log.Debug("Service Path URL is " + url);
            string xml;

            if (navigationPage.HasValue && navigationPageSize.HasValue)
            {
                xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken, (int)navigationPage, (int)navigationPageSize);
            }
            else
            {
                xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken);
            }

            return DeserialiseMultiple(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Update(TSingle)">Update</see>
        /// </summary>
        public virtual void Update(TSingle obj)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + obj.RefId;
            string body = SerialiseSingle(obj);
            string xml = HttpUtils.PutRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from PUT request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKeyPK}.Update(TMultiple)">Update</see>
        /// </summary>
        public virtual MultipleUpdateResponse Update(TMultiple obj)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s";
            string body = SerialiseMultiple(obj);
            string xml = HttpUtils.PutRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from PUT request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            updateResponseType updateResponseType = SerialiserFactory.GetXmlSerialiser<updateResponseType>().Deserialise(xml);
            MultipleUpdateResponse updateResponse = MapperFactory.CreateInstance<updateResponseType, MultipleUpdateResponse>(updateResponseType);

            return updateResponse;
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Delete(TPrimaryKey)">Delete</see>
        /// </summary>
        public virtual void Delete(TPrimaryKey refId)
        {

            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + refId;
            string xml = HttpUtils.DeleteRequest(url, RegistrationService.AuthorisationToken);
            if (log.IsDebugEnabled) log.Debug("XML from DELETE request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
        }

        /// <summary>
        /// <see cref="IConsumer{TSingle,TMultiple,TPrimaryKey}.Delete(IEnumerable{TPrimaryKey})">Delete</see>
        /// </summary>
        public virtual MultipleDeleteResponse Delete(IEnumerable<TPrimaryKey> refIds)
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
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s";
            string body = SerialiserFactory.GetXmlSerialiser<deleteRequestType>().Serialise(request);
            string xml = HttpUtils.PutRequest(url, RegistrationService.AuthorisationToken, body, "DELETE");
            if (log.IsDebugEnabled) log.Debug("XML from PUT (DELETE) request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            deleteResponseType updateResponseType = SerialiserFactory.GetXmlSerialiser<deleteResponseType>().Deserialise(xml);
            MultipleDeleteResponse updateResponse = MapperFactory.CreateInstance<deleteResponseType, MultipleDeleteResponse>(updateResponseType);

            return updateResponse;
        }

    }

}
