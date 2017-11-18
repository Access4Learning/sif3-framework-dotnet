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
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sif.Framework.Consumers
{

    /// <summary>
    /// This class defines a Consumer of SIF Events for data model objects.
    /// </summary>
    /// <typeparam name="TSingle">Type that defines a single object entity.</typeparam>
    /// <typeparam name="TMultiple">Type that defines a multiple objects entity.</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the SIF data model object.</typeparam>
    public class EventConsumer<TSingle, TMultiple, TPrimaryKey> where TSingle : ISifRefId<TPrimaryKey>
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private CancellationTokenSource cancellationTokenSource;
        private Model.Infrastructure.Environment environmentTemplate;
        private Task task;

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
        public EventConsumer(Model.Infrastructure.Environment environment)
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
        public EventConsumer(string applicationKey, string instanceId = null, string userToken = null, string solutionId = null)
        {
            Model.Infrastructure.Environment environment = new Model.Infrastructure.Environment(applicationKey, instanceId, userToken, solutionId);
            environmentTemplate = EnvironmentUtils.MergeWithSettings(environment, SettingsManager.ConsumerSettings);
            RegistrationService = new RegistrationService(SettingsManager.ConsumerSettings, SessionsManager.ConsumerSessionService);
        }

        /// <summary>
        /// Create a Queue that will be used by the Consumer.
        /// </summary>
        /// <param name="queue">Information related to the Queue.</param>
        /// <returns>Instance of the created Queue.</returns>
        private queueType CreateQueue(queueType queue)
        {
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate, connector: InfrastructureServiceNames.queues);
            string body = SerialiseQueue(queue);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug($"Response from POST {url} request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseQueue(xml);
        }

        /// <summary>
        /// Create a Subscription that will be associated to the Consumer.
        /// </summary>
        /// <param name="subscription">Information related to the Subscription.</param>
        /// <returns>Instance of the created Subscription.</returns>
        private subscriptionType CreateSubscription(subscriptionType subscription)
        {
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate, connector: InfrastructureServiceNames.subscriptions);
            string body = SerialiseSubscription(subscription);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug($"Response from POST {url} request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseSubscription(xml);
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

        private void ProcessEvents(CancellationToken cancellationToken)
        {

            while (true)
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                // Check the message queue.
                if (log.IsDebugEnabled) log.Debug("Checking the Queue!");

                Thread.Sleep(TimeSpan.FromSeconds(10));
            }

        }

        /// <summary>
        /// Serialise a queueType object.
        /// </summary>
        /// <param name="queue">queueType object.</param>
        /// <returns>XML string representation of the queueType object.</returns>
        public virtual string SerialiseQueue(queueType queue)
        {
            return SerialiserFactory.GetXmlSerialiser<queueType>().Serialise(queue);
        }

        /// <summary>
        /// Serialise a subscriptionType object.
        /// </summary>
        /// <param name="subscription">subscriptionType object.</param>
        /// <returns>XML string representation of the subscriptionType object.</returns>
        public virtual string SerialiseSubscription(subscriptionType subscription)
        {
            return SerialiserFactory.GetXmlSerialiser<subscriptionType>().Serialise(subscription);
        }

        public void Start()
        {

            if (log.IsDebugEnabled) log.Debug($"Started Consumer to wait for SIF Events of type {TypeName}.");

            RegistrationService.Register(ref environmentTemplate);

            queueType queue = new queueType();
            Queue = CreateQueue(queue);

            subscriptionType subscription = new subscriptionType()
            {
                queueId = Queue.id,
                serviceName = $"{TypeName}s",
                serviceType = ServiceType.OBJECT.ToDescription()
            };

            Subscription = CreateSubscription(subscription);

            // Manage SIF Events using background tasks.
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            task = Task.Factory.StartNew(
                () => ProcessEvents(cancellationToken),
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        public void Stop(bool? deleteOnUnregister = null)
        {
            cancellationTokenSource.Cancel();

            try
            {
                task.Wait();
            }
            catch (AggregateException e)
            {
                if (log.IsErrorEnabled) log.Error(e, $"Error occurred stopping the Event Consumer for {TypeName} - {e.GetBaseException().Message}.");
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error(e, $"Error occurred stopping the Event Consumer for {TypeName} - {e.GetBaseException().Message}.");
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }

            RegistrationService.Unregister(deleteOnUnregister);

            if (log.IsDebugEnabled) log.Debug($"Stopped Consumer that was waiting for SIF Events of type {TypeName}.");

        }

    }

}
