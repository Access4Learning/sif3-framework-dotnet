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
using Sif.Framework.Model.DataModels;
using Sif.Framework.Model.Events;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Parameters;
using Sif.Framework.Model.Requests;
using Sif.Framework.Model.Responses;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Net;
using System.Runtime.Serialization;
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
    public abstract class EventConsumer<TSingle, TMultiple, TPrimaryKey>
        : IEventConsumer where TSingle : ISifRefId<TPrimaryKey>
    {
        private readonly slf4net.ILogger log =
            slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private CancellationTokenSource cancellationTokenSource;
        private Model.Infrastructure.Environment _environment;
        private Task task;

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
        protected Model.Infrastructure.Environment Environment
        {
            get => _environment;
            private set => _environment = value;
        }

        /// <summary>
        /// Queue associated with Consumer SIF Events.
        /// </summary>
        protected queueType Queue { get; private set; }

        /// <summary>
        /// Service for Consumer registration.
        /// </summary>
        protected IRegistrationService RegistrationService { get; }

        /// <summary>
        /// Subscription associated with Consumer SIF Events.
        /// </summary>
        protected subscriptionType Subscription { get; private set; }

        /// <summary>
        /// Name of the SIF data model that the Consumer is based on, e.g. SchoolInfo, StudentPersonal, etc.
        /// </summary>
        protected virtual string TypeName => typeof(TSingle).Name;

        /// <summary>
        /// Create a Consumer instance based upon the Environment passed.
        /// </summary>
        /// <param name="environment">Environment object.</param>
        /// <param name="settings">Consumer settings. If null, Consumer settings will be read from the SifFramework.config file.</param>
        protected EventConsumer(Model.Infrastructure.Environment environment, IFrameworkSettings settings = null)
        {
            ConsumerSettings = settings ?? SettingsManager.ConsumerSettings;

            Environment = EnvironmentUtils.MergeWithSettings(environment, ConsumerSettings);
            RegistrationService = new RegistrationService(ConsumerSettings, SessionsManager.ConsumerSessionService);
        }

        /// <summary>
        /// Create a Consumer instance identified by the parameters passed.
        /// </summary>
        /// <param name="applicationKey">Application key.</param>
        /// <param name="instanceId">Instance ID.</param>
        /// <param name="userToken">User token.</param>
        /// <param name="solutionId">Solution ID.</param>
        /// <param name="settings">Consumer settings. If null, Consumer settings will be read from the SifFramework.config file.</param>
        protected EventConsumer(
            string applicationKey,
            string instanceId = null,
            string userToken = null,
            string solutionId = null,
            IFrameworkSettings settings = null)
            : this(new Model.Infrastructure.Environment(applicationKey, instanceId, userToken, solutionId), settings)
        {
        }

        /// <summary>
        /// Create a Queue that will be used by the Consumer.
        /// </summary>
        /// <param name="queue">Queue to create.</param>
        /// <returns>Instance of the created Queue.</returns>
        private queueType CreateQueue(queueType queue)
        {
            var url =
                $"{EnvironmentUtils.ParseServiceUrl(Environment, ServiceType.UTILITY, InfrastructureServiceNames.queues)}";
            string requestBody = SerialiseQueue(queue);
            string responseBody = HttpUtils.PostRequest(
                url,
                RegistrationService.AuthorisationToken,
                requestBody,
                ConsumerSettings.CompressPayload,
                contentTypeOverride: ContentType.ToDescription(),
                acceptOverride: Accept.ToDescription());

            if (log.IsDebugEnabled) log.Debug($"Response from POST {url} request ...");
            if (log.IsDebugEnabled) log.Debug(responseBody);

            return DeserialiseQueue(responseBody);
        }

        /// <summary>
        /// Create a Subscription that will be associated to the Consumer.
        /// </summary>
        /// <param name="subscription">Subscription to create.</param>
        /// <returns>Instance of the created Subscription.</returns>
        private subscriptionType CreateSubscription(subscriptionType subscription)
        {
            var url =
                $"{EnvironmentUtils.ParseServiceUrl(Environment, ServiceType.UTILITY, InfrastructureServiceNames.subscriptions)}";
            string requestBody = SerialiseSubscription(subscription);
            string responseBody = HttpUtils.PostRequest(
                url,
                RegistrationService.AuthorisationToken,
                requestBody,
                ConsumerSettings.CompressPayload,
                contentTypeOverride: ContentType.ToDescription(),
                acceptOverride: Accept.ToDescription());

            if (log.IsDebugEnabled) log.Debug($"Response from POST {url} request ...");
            if (log.IsDebugEnabled) log.Debug(responseBody);

            return DeserialiseSubscription(responseBody);
        }

        /// <summary>
        /// Deserialise an entity of multiple objects.
        /// </summary>
        /// <param name="payload">Payload of multiple objects.</param>
        /// <returns>Entity representing the multiple objects.</returns>
        /// <exception cref="SerializationException">Error deserialising the payload of multiple objects.</exception>
        protected virtual TMultiple DeserialiseMultiple(string payload)
        {
            TMultiple obj;

            try
            {
                var xmlRootAttribute = new XmlRootAttribute(TypeName + "s")
                    { Namespace = ConsumerSettings.DataModelNamespace, IsNullable = false };
                obj = SerialiserFactory.GetSerialiser<TMultiple>(Accept, xmlRootAttribute).Deserialise(payload);
            }
            catch (Exception e)
            {
                throw new SerializationException(
                    $"Error deserialising the following payload of multiple objects:\n{payload}", e);
            }

            return obj;
        }

        /// <summary>
        /// Deserialise a string representation of a queueType object.
        /// </summary>
        /// <param name="content">String representation of a queueType object.</param>
        /// <returns>queueType object.</returns>
        private queueType DeserialiseQueue(string content)
        {
            return SerialiserFactory.GetSerialiser<queueType>(Accept).Deserialise(content);
        }

        /// <summary>
        /// Deserialise a string representation of a subscriptionType object.
        /// </summary>
        /// <param name="content">String representation of a subscriptionType object.</param>
        /// <returns>subscriptionType object.</returns>
        private subscriptionType DeserialiseSubscription(string content)
        {
            return SerialiserFactory.GetSerialiser<subscriptionType>(Accept).Deserialise(content);
        }

        /// <summary>
        /// Handler to be called on a create event.
        /// </summary>
        /// <param name="obj">Collection of SIF data model objects associated with the create event.</param>
        /// <param name="zoneId">Zone associated with the create event.</param>
        /// <param name="contextId">Zone context.</param>
        public abstract void OnCreateEvent(TMultiple obj, string zoneId = null, string contextId = null);

        /// <summary>
        /// Handler to be called on a delete event.
        /// </summary>
        /// <param name="obj">Collection of SIF data model objects associated with the delete event.</param>
        /// <param name="zoneId">Zone associated with the delete event.</param>
        /// <param name="contextId">Zone context.</param>
        public abstract void OnDeleteEvent(TMultiple obj, string zoneId = null, string contextId = null);

        /// <summary>
        /// Handler to be called on a error event.
        /// </summary>
        /// <param name="error">The error associated with the error event.</param>
        /// <param name="zoneId">Zone associated with the error event.</param>
        /// <param name="contextId">Zone context.</param>
        public abstract void OnErrorEvent(ResponseError error, string zoneId = null, string contextId = null);

        /// <summary>
        /// Handler to be called on a update event.
        /// </summary>
        /// <param name="obj">Collection of SIF data model objects associated with the update event.</param>
        /// <param name="partialUpdate">True if the objects associated with the update event only contained updated fields; false if the objects contain all fields (regardless of if they were changed).</param>
        /// <param name="zoneId">Zone associated with the update event.</param>
        /// <param name="contextId">Zone context.</param>
        public abstract void OnUpdateEvent(
            TMultiple obj,
            bool partialUpdate,
            string zoneId = null,
            string contextId = null);

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

                var getEvents = true;
                TimeSpan waitTime = TimeSpan.FromSeconds(ConsumerSettings.EventProcessingWaitTime);
                var url =
                    $"{EnvironmentUtils.ParseServiceUrl(Environment, ServiceType.UTILITY, InfrastructureServiceNames.queues)}/{Queue.id}/messages";
                string deleteMessageId = null;

                // Read from the message queue until no more messages are found.
                do
                {
                    try
                    {
                        string deleteMessageIdMatrixParameter =
                            (deleteMessageId == null ? "" : $";deleteMessageId={deleteMessageId.Trim()}");

                        if (log.IsDebugEnabled)
                            log.Debug(
                                $"Making a request for an event message from {url}{deleteMessageIdMatrixParameter}.");

                        string responseBody = HttpUtils.GetRequestAndHeaders(
                            $"{url}{deleteMessageIdMatrixParameter}",
                            RegistrationService.AuthorisationToken,
                            ConsumerSettings.CompressPayload,
                            out WebHeaderCollection responseHeaders,
                            contentTypeOverride: ContentType.ToDescription(),
                            acceptOverride: Accept.ToDescription(),
                            deleteMessageId: deleteMessageId);
                        string contextId = responseHeaders?[EventParameterType.contextId.ToDescription()];
                        deleteMessageId = responseHeaders?[EventParameterType.messageId.ToDescription()];
                        string minWaitTimeValue = responseHeaders?[EventParameterType.minWaitTime.ToDescription()];
                        string zoneId = responseHeaders?[EventParameterType.zoneId.ToDescription()];

                        if (!string.IsNullOrWhiteSpace(minWaitTimeValue))
                        {
                            if (double.TryParse(minWaitTimeValue, out double minWaitTime) &&
                                (TimeSpan.FromSeconds(minWaitTime) > waitTime))
                            {
                                waitTime = TimeSpan.FromSeconds(minWaitTime);
                            }
                        }

                        // Call the appropriate event handler for messages read.
                        if (!string.IsNullOrWhiteSpace(responseBody))
                        {
                            try
                            {
                                TMultiple obj = DeserialiseMultiple(responseBody);
                                string eventAction = responseHeaders?[EventParameterType.eventAction.ToDescription()];

                                if (EventAction.CREATE.ToDescription().Equals(eventAction))
                                {
                                    if (log.IsDebugEnabled) log.Debug("Received create event message.");

                                    OnCreateEvent(obj, zoneId, contextId);
                                }
                                else if (EventAction.DELETE.ToDescription().Equals(eventAction))
                                {
                                    if (log.IsDebugEnabled) log.Debug("Received delete event message.");

                                    OnDeleteEvent(obj, zoneId, contextId);
                                }
                                else if ("UPDATE".Equals(eventAction))
                                {
                                    string replacement =
                                        responseHeaders?[EventParameterType.Replacement.ToDescription()];

                                    if ("FULL".Equals(replacement))
                                    {
                                        if (log.IsDebugEnabled) log.Debug("Received update (full) event message.");

                                        OnUpdateEvent(obj, false, zoneId, contextId);
                                    }
                                    else if ("PARTIAL".Equals(replacement))
                                    {
                                        if (log.IsDebugEnabled) log.Debug("Received update (partial) event message.");

                                        OnUpdateEvent(obj, true, zoneId, contextId);
                                    }
                                    else
                                    {
                                        if (log.IsDebugEnabled) log.Debug("Received update (partial) event message.");

                                        OnUpdateEvent(obj, true, zoneId, contextId);
                                    }
                                }
                                else
                                {
                                    BaseException eventException = new EventException(
                                        $"Event action {eventAction} not recognised for message received from {url}.");

                                    if (log.IsWarnEnabled) log.Warn(eventException.Message);

                                    var error = new ResponseError
                                    {
                                        Id = eventException.ExceptionReference,
                                        Code = 500,
                                        Message = eventException.Message,
                                        Description = responseBody,
                                        Scope = TypeName
                                    };

                                    OnErrorEvent(error);
                                }
                            }
                            catch (SerializationException e)
                            {
                                BaseException eventException = new EventException(
                                    $"Event message received from {url} could not be processed due to the following error:\n{e.GetBaseException().Message}.",
                                    e);

                                if (log.IsWarnEnabled) log.Warn(e.Message);

                                var error = new ResponseError
                                {
                                    Id = eventException.ExceptionReference,
                                    Code = 500,
                                    Message = e.Message,
                                    Description = responseBody,
                                    Scope = TypeName
                                };

                                OnErrorEvent(error);
                            }
                        }
                        else
                        {
                            if (log.IsDebugEnabled) log.Debug("No event messages.");

                            getEvents = false;
                        }
                    }
                    catch (Exception e)
                    {
                        var errorMessage =
                            $"Error processing event messages due to the following error:\n{e.GetBaseException().Message}.";

                        if (log.IsErrorEnabled) log.Error($"{errorMessage}\n{e.StackTrace}");

                        getEvents = false;
                    }
                } while (getEvents);

                if (log.IsDebugEnabled) log.Debug($"Wait time is {waitTime.Seconds} seconds.");

                // Wait an appropriate amount of time before reading from the message queue again.
                Thread.Sleep(waitTime);
            }
        }

        /// <summary>
        /// Retrieves a Queue that will be used by the Consumer using the queue id.
        /// </summary>
        /// <param name="queueId">The Queue identifier.</param>
        /// <returns>Instance of the Queue if id is valid and queue is found, null otherwise.</returns>
        private queueType RetrieveQueue(string queueId)
        {
            var url =
                $"{EnvironmentUtils.ParseServiceUrl(Environment, ServiceType.UTILITY, InfrastructureServiceNames.queues)}/{queueId}";
            string responseBody = HttpUtils.GetRequest(
                url,
                RegistrationService.AuthorisationToken,
                ConsumerSettings.CompressPayload,
                contentTypeOverride: ContentType.ToDescription(),
                acceptOverride: Accept.ToDescription());

            if (log.IsDebugEnabled) log.Debug($"Response from GET {url} request ...");
            if (log.IsDebugEnabled) log.Debug(responseBody);

            return DeserialiseQueue(responseBody);
        }

        /// <summary>
        /// Serialise a queueType object.
        /// </summary>
        /// <param name="queue">queueType object.</param>
        /// <returns>String representation of the queueType object.</returns>
        private string SerialiseQueue(queueType queue)
        {
            return SerialiserFactory.GetSerialiser<queueType>(ContentType).Serialise(queue);
        }

        /// <summary>
        /// Serialise a subscriptionType object.
        /// </summary>
        /// <param name="subscription">subscriptionType object.</param>
        /// <returns>String representation of the subscriptionType object.</returns>
        private string SerialiseSubscription(subscriptionType subscription)
        {
            return SerialiserFactory.GetSerialiser<subscriptionType>(ContentType).Serialise(subscription);
        }

        /// <summary>
        /// <see cref="IEventConsumer.Start(string, string)">Start</see>
        /// </summary>
        public void Start(string zoneId = null, string contextId = null)
        {
            if (log.IsDebugEnabled) log.Debug($"Started Consumer to wait for SIF Events of type {TypeName}.");

            try
            {
                // Register the Event Consumer with the SIF Broker.
                RegistrationService.Register(ref _environment);

                // Retrieve the Subscription identifier (if exist).
                string subscriptionId = SessionsManager.ConsumerSessionService.RetrieveSubscriptionId(
                    Environment.ApplicationInfo.ApplicationKey,
                    Environment.SolutionId,
                    Environment.UserToken,
                    Environment.InstanceId);

                // If the Subscription identifier does NOT exist, create a Subscription and associated Queue.
                if (string.IsNullOrWhiteSpace(subscriptionId))
                {
                    var queueNameSuffix = DateTime.UtcNow.ToString("yyMMddHHmmssfff");

                    // For the SIF Broker, the name property is a mandatory.
                    var queue = new queueType
                    {
                        name = $"{TypeName}-{queueNameSuffix}"
                    };

                    Queue = CreateQueue(queue);

                    var subscription = new subscriptionType()
                    {
                        contextId = contextId,
                        queueId = Queue.id,
                        serviceName = $"{TypeName}s",
                        serviceType = ServiceType.OBJECT.ToDescription(),
                        zoneId = zoneId
                    };

                    Subscription = CreateSubscription(subscription);

                    // Store Queue and Subscription identifiers.
                    SessionsManager.ConsumerSessionService.UpdateQueueId(
                        Queue.id,
                        Environment.ApplicationInfo.ApplicationKey,
                        Environment.SolutionId,
                        Environment.UserToken,
                        Environment.InstanceId);

                    SessionsManager.ConsumerSessionService.UpdateSubscriptionId(
                        Subscription.id,
                        Environment.ApplicationInfo.ApplicationKey,
                        Environment.SolutionId,
                        Environment.UserToken,
                        Environment.InstanceId);
                }
                // If the Subscription identifier does exist, retrieve the Queue.
                else
                {
                    try
                    {
                        string queueId = SessionsManager.ConsumerSessionService.RetrieveQueueId(
                            Environment.ApplicationInfo.ApplicationKey,
                            Environment.SolutionId,
                            Environment.UserToken,
                            Environment.InstanceId);

                        Queue = RetrieveQueue(queueId);
                    }
                    catch (Exception e)
                    {
                        var errorMessage =
                            $"Could not retrieve Queue details due to the following error:\n{e.GetBaseException().Message}.";

                        if (log.IsErrorEnabled) log.Error($"{errorMessage}\n{e.StackTrace}");

                        throw;
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
            catch (RegistrationException e)
            {
                var errorMessage =
                    $"Error registering the Event Consumer:\n{e.GetBaseException().Message}.\n{e.StackTrace}";

                if (log.IsErrorEnabled) log.Error(e, errorMessage);

                throw;
            }
            catch (Exception e)
            {
                var errorMessage =
                    $"Error starting the Event Consumer:\n{e.GetBaseException().Message}.\n{e.StackTrace}";

                if (log.IsErrorEnabled) log.Error(e, errorMessage);

                throw;
            }
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
                if (log.IsErrorEnabled)
                    log.Error(e,
                        $"Error occurred stopping the Event Consumer for {TypeName} - {e.GetBaseException().Message}.");
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error(e,
                        $"Error occurred stopping the Event Consumer for {TypeName} - {e.GetBaseException().Message}.");
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