/*
 * Copyright 2014 Systemic Pty Ltd
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

using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Persistence;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Serialization;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Consumer
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="PK"></typeparam>
    public class GenericConsumer<T, PK> : IGenericConsumer<T, PK> where T : IPersistable<PK>, new()
    {
        private string authorisationToken;
        private Environment consumerEnvironment;
        private Environment environmentTemplate;
        private bool registered = false;
        private string serviceUrl;

        /// <summary>
        /// 
        /// </summary>
        protected virtual string TypeUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        private Environment BuildEnvironmentTemplate(Environment environment)
        {
            string applicationKey = ConfigurationManager.AppSettings["consumer.environment.template.applicationKey"];
            string authenticationMethod = ConfigurationManager.AppSettings["consumer.environment.template.authenticationMethod"];
            string consumerName = ConfigurationManager.AppSettings["consumer.environment.template.consumerName"];
            string dataModelNamespace = ConfigurationManager.AppSettings["consumer.environment.template.DataModelNamespace"];
            string supportedInfrastructureVersion = ConfigurationManager.AppSettings["consumer.environment.template.supportedInfrastructureVersion"];

            Environment environmentTemplate;

            if (environment == null)
            {
                environmentTemplate = new Environment();
            }
            else
            {
                environmentTemplate = environment;
            }

            if (environmentTemplate.ApplicationInfo == null)
            {
                environmentTemplate.ApplicationInfo = new ApplicationInfo();
            }

            if (String.IsNullOrWhiteSpace(environmentTemplate.ApplicationInfo.ApplicationKey) && applicationKey != null)
            {
                environmentTemplate.ApplicationInfo.ApplicationKey = applicationKey;
            }

            if (String.IsNullOrWhiteSpace(environmentTemplate.ApplicationInfo.ApplicationKey))
            {
                throw new System.ArgumentException("An applicationKey must either be provided or defined in the Consumer Environment template.", "applicationKey");
            }

            if (String.IsNullOrWhiteSpace(environmentTemplate.AuthenticationMethod) && authenticationMethod != null)
            {
                environmentTemplate.AuthenticationMethod = authenticationMethod;
            }

            if (String.IsNullOrWhiteSpace(environmentTemplate.ConsumerName) && consumerName != null)
            {
                environmentTemplate.ConsumerName = consumerName;
            }

            if (String.IsNullOrWhiteSpace(environmentTemplate.ApplicationInfo.DataModelNamespace) && dataModelNamespace != null)
            {
                environmentTemplate.ApplicationInfo.DataModelNamespace = dataModelNamespace;
            }

            if (String.IsNullOrWhiteSpace(environmentTemplate.ApplicationInfo.SupportedInfrastructureVersion) && supportedInfrastructureVersion != null)
            {
                environmentTemplate.ApplicationInfo.SupportedInfrastructureVersion = supportedInfrastructureVersion;
            }

            return environmentTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationKey"></param>
        /// <param name="instanceId"></param>
        /// <param name="userToken"></param>
        public GenericConsumer(string applicationKey, string instanceId = null, string userToken = null)
        {
            Environment environment = new Environment();

            if (!String.IsNullOrWhiteSpace(applicationKey))
            {
                environment.ApplicationInfo = new ApplicationInfo();
                environment.ApplicationInfo.ApplicationKey = applicationKey;
            }

            if (!String.IsNullOrWhiteSpace(instanceId))
            {
                environment.InstanceId = instanceId;
            }

            if (!String.IsNullOrWhiteSpace(userToken))
            {
                environment.UserToken = userToken;
            }

            environmentTemplate = BuildEnvironmentTemplate(environment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="environment"></param>
        public GenericConsumer(Environment environment)
        {
            environmentTemplate = BuildEnvironmentTemplate(environment);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Register()
        {

            if (!registered)
            {
                string environmentUrl = ConfigurationManager.AppSettings["consumer.environment.url"];
                string sharedSecret = ConfigurationManager.AppSettings["consumer.environment.sharedSecret"];

                string initialToken = AuthenticationUtils.GenerateBasicAuthorisationToken(environmentTemplate.ApplicationInfo.ApplicationKey, sharedSecret);
                environmentType environmentTypeToSerialise = MapperFactory.CreateInstance<Environment, environmentType>(environmentTemplate);
                string body = SerialiserFactory.GetXmlSerialiser<environmentType>().Serialise(environmentTypeToSerialise);
                string xml = HttpUtils.PostRequest(environmentUrl, initialToken, body);
                Console.WriteLine("Environment XML from POST request");
                Console.WriteLine(xml);

                environmentType environmentTypeToDeserialise = SerialiserFactory.GetXmlSerialiser<environmentType>().Deserialise(xml);
                consumerEnvironment = MapperFactory.CreateInstance<environmentType, Environment>(environmentTypeToDeserialise);
                Console.WriteLine("Environment URL is " + consumerEnvironment.InfrastructureServices[InfrastructureServiceNames.environment].Value + "/" + consumerEnvironment.Id);
                authorisationToken = AuthenticationUtils.GenerateBasicAuthorisationToken(consumerEnvironment.SessionToken, sharedSecret);

                TypeUrl = (new T()).GetType().Name;
                serviceUrl = consumerEnvironment.InfrastructureServices[InfrastructureServiceNames.requestsConnector].Value + "/" + TypeUrl + "s";
                Console.WriteLine("requestsConnector service URL is " + serviceUrl);

                registered = true;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void Unregister()
        {

            if (registered)
            {
                bool deleteOnUnregister;

                if (Boolean.TryParse(ConfigurationManager.AppSettings["consumer.environment.deleteOnUnregister"], out deleteOnUnregister))
                {

                    if (deleteOnUnregister)
                    {
                        string xml = HttpUtils.DeleteRequest(consumerEnvironment.InfrastructureServices[InfrastructureServiceNames.environment].Value + "/" + consumerEnvironment.Id, authorisationToken);
                        Console.WriteLine("Environment XML from DELETE request");
                        Console.WriteLine(xml);
                    }

                }

                registered = false;
            }

        }

        /// <summary>
        /// POST /StudentPersonals/StudentPersonal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual PK Create(T obj)
        {

            if (!registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string body = SerialiserFactory.GetXmlSerialiser<T>().Serialise(obj);
            string xml = HttpUtils.PostRequest(serviceUrl + "/" + TypeUrl, authorisationToken, body);
            Console.WriteLine("XML from POST request");
            Console.WriteLine(xml);
            return default(PK);
        }

        /// <summary>
        /// POST /StudentPersonals
        /// </summary>
        /// <param name="objs"></param>
        public virtual void Create(IEnumerable<T> objs)
        {

            if (!registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string body = SerialiserFactory.GetXmlSerialiser<List<T>>(new XmlRootAttribute(TypeUrl + "s")).Serialise((List<T>)objs);
            string xml = HttpUtils.PostRequest(serviceUrl, authorisationToken, body);
            Console.WriteLine("XML from POST request");
            Console.WriteLine(xml);
        }

        /// <summary>
        /// GET /StudentPersonals/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T Retrieve(PK id)
        {

            if (!registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string xml = HttpUtils.GetRequest(serviceUrl + "/" + id, authorisationToken);
            Console.WriteLine("XML from GET request");
            Console.WriteLine(xml);
            return SerialiserFactory.GetXmlSerialiser<T>().Deserialise(xml);
        }

        /// <summary>
        /// GET /StudentPersonals
        /// </summary>
        /// <returns></returns>
        public virtual ICollection<T> Retrieve()
        {

            if (!registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string xml = HttpUtils.GetRequest(serviceUrl, authorisationToken);
            Console.WriteLine("XML from GET request");
            Console.WriteLine(xml);
            return SerialiserFactory.GetXmlSerialiser<List<T>>(new XmlRootAttribute(TypeUrl + "s")).Deserialise(xml);
        }

        /// <summary>
        /// PUT /StudentPersonals/{id}
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Update(T obj)
        {

            if (!registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string body = SerialiserFactory.GetXmlSerialiser<T>().Serialise(obj);
            string xml = HttpUtils.PutRequest(serviceUrl + "/" + obj.Id, authorisationToken, body);
            Console.WriteLine("XML from PUT request");
            Console.WriteLine(xml);
        }

        /// <summary>
        /// PUT /StudentPersonals
        /// </summary>
        /// <param name="objs"></param>
        public virtual void Update(IEnumerable<T> objs)
        {

            if (!registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string body = SerialiserFactory.GetXmlSerialiser<List<T>>(new XmlRootAttribute(TypeUrl + "s")).Serialise((List<T>)objs);
            string xml = HttpUtils.PutRequest(serviceUrl, authorisationToken, body);
            Console.WriteLine("XML from PUT request");
            Console.WriteLine(xml);
        }

        /// <summary>
        /// DELETE /StudentPersonals/{id}
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(PK id)
        {

            if (!registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }

            string xml = HttpUtils.DeleteRequest(serviceUrl + "/" + id, authorisationToken);
            Console.WriteLine("XML from DELETE request");
            Console.WriteLine(xml);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objs"></param>
        public virtual void Delete(IEnumerable<T> objs)
        {
            throw new NotImplementedException();
        }

    }

}
