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
using Sif.Framework.Model.Persistence;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Consumer
{

    /// <summary>
    /// <see cref="Sif.Framework.Consumer.IGenericConsumer{T,PK}">IGenericConsumer</see>
    /// </summary>
    public class GenericConsumer<T, PK> : IGenericConsumer<T, PK> where T : IPersistable<PK>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Environment environmentTemplate;
        private RegistrationService registrationService;

        /// <summary>
        /// Name of the SIF data type that the Consumer is based on, e.g. SchoolInfo, StudentPersonal, etc.
        /// </summary>
        protected virtual string TypeName
        {

            get
            {
                return typeof(T).Name;
            }

        }

        /// <summary>
        /// Create a Consumer instance identified by the parameters passed.
        /// </summary>
        /// <param name="applicationKey">Application key.</param>
        /// <param name="instanceId">Instance ID.</param>
        /// <param name="userToken">User token.</param>
        /// <param name="solutionId"></param>
        public GenericConsumer(string applicationKey, string instanceId = null, string userToken = null, string solutionId = null)
        {
            Environment environment = new Environment(applicationKey, instanceId, userToken, solutionId);
            environmentTemplate = EnvironmentUtils.MergeWithSettings(environment, SettingsManager.ConsumerSettings);
            registrationService = new RegistrationService(SettingsManager.ConsumerSettings, SessionsManager.ConsumerSessionService);
        }

        /// <summary>
        /// Create a Consumer instance based upon the Environment passed.
        /// </summary>
        /// <param name="environment">Environment object.</param>
        public GenericConsumer(Environment environment)
        {
            environmentTemplate = EnvironmentUtils.MergeWithSettings(environment, SettingsManager.ConsumerSettings);
            registrationService = new RegistrationService(SettingsManager.ConsumerSettings, SessionsManager.ConsumerSessionService);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Consumer.IGenericConsumer{T,PK}.Register()">Register</see>
        /// </summary>
        public void Register()
        {
            registrationService.Register(ref environmentTemplate);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Consumer.IGenericConsumer{T,PK}.Unregister()">Unregister</see>
        /// </summary>
        public void Unregister(bool? deleteOnUnregister = null)
        {
            registrationService.Unregister(deleteOnUnregister);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Consumer.IGenericConsumer{T,PK}.Create(T)">Create</see>
        /// </summary>
        public virtual PK Create(T obj)
        {

            if (!registrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(environmentTemplate) + "/" + TypeName + "s" + "/" + TypeName;
            string body = SerialiserFactory.GetXmlSerialiser<T>().Serialise(obj);
            string xml = HttpUtils.PostRequest(url, registrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from POST request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            return (SerialiserFactory.GetXmlSerialiser<T>().Deserialise(xml)).Id;
        }

        /// <summary>
        /// <see cref="Sif.Framework.Consumer.IGenericConsumer{T,PK}.Create(System.Collections.Generic.IEnumerable<T>)">Create</see>
        /// </summary>
        public virtual void Create(IEnumerable<T> objs)
        {

            if (!registrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(environmentTemplate) + "/" + TypeName + "s";
            string body = SerialiserFactory.GetXmlSerialiser<List<T>>(new XmlRootAttribute(TypeName + "s")).Serialise((List<T>)objs);
            string xml = HttpUtils.PostRequest(url, registrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from POST request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Consumer.IGenericConsumer{T,PK}.Retrieve(PK)">Retrieve</see>
        /// </summary>
        public virtual T Retrieve(PK id)
        {

            if (!registrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(environmentTemplate) + "/" + TypeName + "s" + "/" + id;
            string xml = HttpUtils.GetRequest(url, registrationService.AuthorisationToken);
            if (log.IsDebugEnabled) log.Debug("XML from GET request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            return SerialiserFactory.GetXmlSerialiser<T>().Deserialise(xml);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Consumer.IGenericConsumer{T,PK}.Retrieve()">Retrieve</see>
        /// </summary>
        public virtual ICollection<T> Retrieve()
        {

            if (!registrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(environmentTemplate) + "/" + TypeName + "s";
            List<T> result = new List<T>();
            int pageIndex = 0;
            int pageResultCount = 0;

            do
            {
                string xml = HttpUtils.GetRequest(url, registrationService.AuthorisationToken, pageIndex++, SettingsManager.ConsumerSettings.NavigationPageSize);
                if (log.IsDebugEnabled) log.Debug("XML from GET request (page " + (pageIndex - 1) + ") ...");
                if (log.IsDebugEnabled) log.Debug(xml);

                if (xml.Length > 0)
                {
                    ICollection<T> pageResult = SerialiserFactory.GetXmlSerialiser<List<T>>(new XmlRootAttribute(TypeName + "s")).Deserialise(xml);

                    if (pageResult == null || pageResult.Count == 0)
                    {
                        pageResultCount = 0;
                    }
                    else
                    {
                        pageResultCount = pageResult.Count;
                        result.AddRange(pageResult);
                    }

                }

            }
            while (pageResultCount == SettingsManager.ConsumerSettings.NavigationPageSize);

            return result;
        }

        /// <summary>
        /// <see cref="Sif.Framework.Consumer.IGenericConsumer{T,PK}.Retrieve(System.Int32, System.Int32)">Retrieve</see>
        /// </summary>
        public virtual ICollection<T> Retrieve(int navigationPage, int navigationPageSize)
        {

            if (!registrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(environmentTemplate) + "/" + TypeName + "s";
            string xml = HttpUtils.GetRequest(url, registrationService.AuthorisationToken, navigationPage, navigationPageSize);
            if (log.IsDebugEnabled) log.Debug("XML from GET request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            return SerialiserFactory.GetXmlSerialiser<List<T>>(new XmlRootAttribute(TypeName + "s")).Deserialise(xml);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Consumer.IGenericConsumer{T,PK}.Update(T)">Update</see>
        /// </summary>
        public virtual void Update(T obj)
        {

            if (!registrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(environmentTemplate) + "/" + TypeName + "s" + "/" + obj.Id;
            string body = SerialiserFactory.GetXmlSerialiser<T>().Serialise(obj);
            string xml = HttpUtils.PutRequest(url, registrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from PUT request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Consumer.IGenericConsumer{T,PK}.Update(System.Collections.Generic.IEnumerable<T>)">Update</see>
        /// </summary>
        public virtual void Update(IEnumerable<T> objs)
        {

            if (!registrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(environmentTemplate) + "/" + TypeName + "s";
            string body = SerialiserFactory.GetXmlSerialiser<List<T>>(new XmlRootAttribute(TypeName + "s")).Serialise((List<T>)objs);
            string xml = HttpUtils.PutRequest(url, registrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from PUT request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Consumer.IGenericConsumer{T,PK}.Delete(PK)">Delete</see>
        /// </summary>
        public virtual void Delete(PK id)
        {

            if (!registrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string url = EnvironmentUtils.ParseServiceUrl(environmentTemplate) + "/" + TypeName + "s" + "/" + id;
            string xml = HttpUtils.DeleteRequest(url, registrationService.AuthorisationToken);
            if (log.IsDebugEnabled) log.Debug("XML from DELETE request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
        }

        /// <summary>
        /// <see cref="Sif.Framework.Consumer.IGenericConsumer{T,PK}.Delete(System.Collections.Generic.IEnumerable<T>)">Delete</see>
        /// </summary>
        public virtual void Delete(IEnumerable<T> objs)
        {
            throw new NotImplementedException();
        }

    }

}
