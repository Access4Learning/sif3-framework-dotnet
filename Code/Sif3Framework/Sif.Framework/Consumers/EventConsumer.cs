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
using Sif.Framework.Model.Events;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sif.Framework.Consumers
{

    /// <summary>
    /// This class defines a Consumer of SIF Events for data model objects.
    /// </summary>
    /// <typeparam name="TSingle">Type that defines a single object entity.</typeparam>
    /// <typeparam name="TMultiple">Type that defines a multiple objects entity.</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the SIF data model object.</typeparam>
    public abstract class EventConsumer<TSingle, TMultiple, TPrimaryKey> : IEventConsumer where TSingle : ISifRefId<TPrimaryKey>
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private CancellationTokenSource cancellationTokenSource;
        private Model.Infrastructure.Environment environment;
        private Task task;

        /// <summary>
        /// Consumer environment.
        /// </summary>
        protected Model.Infrastructure.Environment Environment
        {

            get { return environment; }

            private set { environment = value; }

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
            Environment = EnvironmentUtils.MergeWithSettings(environment, SettingsManager.ConsumerSettings);
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
            Environment = EnvironmentUtils.MergeWithSettings(environment, SettingsManager.ConsumerSettings);
            RegistrationService = new RegistrationService(SettingsManager.ConsumerSettings, SessionsManager.ConsumerSessionService);
        }

        /// <summary>
        /// Create a Queue that will be used by the Consumer.
        /// </summary>
        /// <param name="queue">Information related to the Queue.</param>
        /// <returns>Instance of the created Queue.</returns>
        private queueType CreateQueue(queueType queue)
        {
            string url = $"{EnvironmentUtils.ParseServiceUrl(Environment, ServiceType.UTILITY, InfrastructureServiceNames.queues)}/queue";
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
            string url = $"{EnvironmentUtils.ParseServiceUrl(Environment, ServiceType.UTILITY, InfrastructureServiceNames.subscriptions)}/subscription";
            string body = SerialiseSubscription(subscription);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug($"Response from POST {url} request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseSubscription(xml);
        }

        /// <summary>
        /// Deserialise an entity of multiple objects.
        /// </summary>
        /// <param name="payload">Payload of multiple objects.</param>
        /// <returns>Entity representing the multiple objects.</returns>
        protected virtual TMultiple DeserialiseMultiple(string payload)
        {
            XmlRootAttribute xmlRootAttribute = new XmlRootAttribute(TypeName + "s") { Namespace = SettingsManager.ConsumerSettings.DataModelNamespace, IsNullable = false };
            return SerialiserFactory.GetXmlSerialiser<TMultiple>(xmlRootAttribute).Deserialise(payload);
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
        /// Handler to be called on a create event.
        /// </summary>
        /// <param name="objs">Collection of SIF data model objects associated with the create event.</param>
        /// <param name="zoneId">Zone associated with the create event.</param>
        /// <param name="contextId">Zone context.</param>
        public abstract void OnCreateEvent(TMultiple objs, string zoneId = null, string contextId = null);

        /// <summary>
        /// Handler to be called on a delete event.
        /// </summary>
        /// <param name="objs">Collection of SIF data model objects associated with the delete event.</param>
        /// <param name="zoneId">Zone associated with the delete event.</param>
        /// <param name="contextId">Zone context.</param>
        public abstract void OnDeleteEvent(TMultiple objs, string zoneId = null, string contextId = null);

        /// <summary>
        /// Handler to be called on a error event.
        /// </summary>
        /// <param name="errorMessage">The error message associated with the error event.</param>
        /// <param name="zoneId">Zone associated with the error event.</param>
        /// <param name="contextId">Zone context.</param>
        public abstract void OnErrorEvent(string errorMessage, string zoneId = null, string contextId = null);

        /// <summary>
        /// Handler to be called on a update event.
        /// </summary>
        /// <param name="objs">Collection of SIF data model objects associated with the update event.</param>
        /// <param name="zoneId">Zone associated with the update event.</param>
        /// <param name="contextId">Zone context.</param>
        public abstract void OnUpdateEvent(TMultiple objs, string zoneId = null, string contextId = null);

        /// <summary>
        /// Periodically process SIF Events.
        /// </summary>
        /// <param name="cancellationToken">Notification that processing should be cancelled.</param>
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

                System.Net.WebHeaderCollection responseHeaders;

                string url = $"{EnvironmentUtils.ParseServiceUrl(Environment, ServiceType.UTILITY, InfrastructureServiceNames.queues)}/{Queue.id}/messages";
                string xml = HttpUtils.GetRequestAndHeaders(url, RegistrationService.AuthorisationToken, out responseHeaders);

                // if there's content available, keep reading the batches, 
                // otherwise sleep and give time to the server recompose itself
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    TMultiple objects = default(TMultiple);
                    try
                    {
                        // deserializing collection
                        objects = DeserialiseMultiple(xml);

                        string messageType = responseHeaders?[HttpUtils.RequestHeader.eventAction.ToDescription()];
                        if (!string.IsNullOrWhiteSpace(messageType))
                        {
                            if (messageType.Equals(EventAction.CREATE.ToDescription()))
                            {
                                if (log.IsDebugEnabled) log.Debug("CREATE Message received.");
                                OnCreateEvent(objects);
                            }
                            else if (messageType.Equals(EventAction.DELETE.ToDescription()))
                            {
                                if (log.IsDebugEnabled) log.Debug("DELETE Message received.");
                                OnDeleteEvent(objects);
                            }
                            else if (messageType.Equals(EventAction.UPDATE_FULL.ToDescription()) || messageType.Equals(EventAction.CREATE.ToDescription()))
                            {
                                if (log.IsDebugEnabled) log.Debug("UPDATE Message received.");
                                OnUpdateEvent(objects);
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string message = $"Could not notify consumers. {ex.Message}.";
                        if (log.IsDebugEnabled) log.Debug($"{message}\n{ex.StackTrace}");
                        OnErrorEvent(message);
                    }
                }
                else
                {
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }

        }

        /// <summary>
        /// Retrieves a Queue that will be used by the Consumer using the queue id.
        /// </summary>
        /// <param name="queueId">The Queue identifier.</param>
        /// <returns>Instance of the Queue if id is valid and queue is found, null otherwise.</returns>
        private queueType RetrieveQueue(string queueId)
        {
            string url = $"{EnvironmentUtils.ParseServiceUrl(Environment, ServiceType.UTILITY, InfrastructureServiceNames.queues)}/{queueId}";
            string xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken);
            if (log.IsDebugEnabled) log.Debug($"Response from GET {url} request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseQueue(xml);
        }

        /// <summary>
        /// Retrieves a Queue that will be used by the Consumer using the subscription id.
        /// </summary>
        /// <param name="subscriptionId">The subscription's identifier.</param>
        /// <returns>Instance of the Subscription if id is valid and subscription is found, null otherwise.</returns>
        private subscriptionType RetrieveSubscription(string subscriptionId)
        {
            string url = $"{EnvironmentUtils.ParseServiceUrl(Environment, ServiceType.UTILITY, InfrastructureServiceNames.subscriptions)}/{subscriptionId}";
            string xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken);
            if (log.IsDebugEnabled) log.Debug($"Response from GET {url} request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseSubscription(xml);
        }

        /// <summary>
        /// Serialise an entity of multiple objects.
        /// </summary>
        /// <param name="obj">Payload of multiple objects.</param>
        /// <returns>XML string representation of the multiple objects.</returns>
        protected virtual string SerialiseMultiple(TMultiple obj)
        {
            XmlRootAttribute xmlRootAttribute = new XmlRootAttribute(TypeName + "s") { Namespace = SettingsManager.ConsumerSettings.DataModelNamespace, IsNullable = false };
            return SerialiserFactory.GetXmlSerialiser<TMultiple>(xmlRootAttribute).Serialise(obj);
        }

        /// <summary>
        /// Serialise a queueType object.
        /// </summary>
        /// <param name="queue">queueType object.</param>
        /// <returns>XML string representation of the queueType object.</returns>
        private string SerialiseQueue(queueType queue)
        {
            return SerialiserFactory.GetXmlSerialiser<queueType>().Serialise(queue);
        }

        /// <summary>
        /// Serialise a subscriptionType object.
        /// </summary>
        /// <param name="subscription">subscriptionType object.</param>
        /// <returns>XML string representation of the subscriptionType object.</returns>
        private string SerialiseSubscription(subscriptionType subscription)
        {
            return SerialiserFactory.GetXmlSerialiser<subscriptionType>().Serialise(subscription);
        }

        /// <summary>
        /// <see cref="IEventConsumer.Start()">Start</see>
        /// </summary>
        public void Start()
        {
            if (log.IsDebugEnabled) log.Debug($"Started Consumer to wait for SIF Events of type {TypeName}.");

            RegistrationService.Register(ref environment);

            string subscriptionId = SessionsManager.ConsumerSessionService.RetrieveSubscriptionId(
                Environment.ApplicationInfo.ApplicationKey,
                Environment.SolutionId,
                Environment.UserToken,
                Environment.InstanceId);

            if (subscriptionId == null)
            {
                queueType queue = new queueType();
                Queue = CreateQueue(queue);

                subscriptionType subscription = new subscriptionType()
                {
                    queueId = Queue.id,
                    serviceName = $"{TypeName}s",
                    serviceType = ServiceType.OBJECT.ToDescription()
                };

                Subscription = CreateSubscription(subscription);

                // Attempting to save subscription and queue info
                SessionsManager.ConsumerSessionService.UpdateQueueId(Queue.id, Environment.ApplicationInfo.ApplicationKey,
                    Environment.SolutionId,
                    Environment.UserToken,
                    Environment.InstanceId);

                SessionsManager.ConsumerSessionService.UpdateSubscriptionId(Subscription.id, Environment.ApplicationInfo.ApplicationKey,
                    Environment.SolutionId,
                    Environment.UserToken,
                    Environment.InstanceId);
            }
            else
            {
                Subscription = RetrieveSubscription(subscriptionId);
                if (Subscription != null)
                {
                    Queue = RetrieveQueue(Subscription.queueId);
                }
            }

            // Manage SIF Events using background tasks.
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            task = Task.Factory.StartNew(
                () => ProcessEvents(cancellationToken),
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        /// <summary>
        /// <see cref="IEventConsumer.Stop(bool?)">Stop</see>
        /// </summary>
        public void Stop(bool? deleteOnStop = null)
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

            RegistrationService.Unregister(deleteOnStop);

            if (log.IsDebugEnabled) log.Debug($"Stopped Consumer that was waiting for SIF Events of type {TypeName}.");
        }

    }

}
