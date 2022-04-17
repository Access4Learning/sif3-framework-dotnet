/*
 * Crown Copyright © Department for Education (UK) 2016
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
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
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
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Consumers
{
    /// <summary>
    /// The base class for all Functional Service consumers
    /// </summary>
    public class FunctionalServiceConsumer
    {
        private static readonly slf4net.ILogger Log =
            slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly RegistrationService registrationService;

        private Environment environmentTemplate;

        /// <summary>
        /// Application settings associated with the Consumer.
        /// </summary>
        protected IFrameworkSettings ConsumerSettings { get; }

        /// <summary>
        /// Consumer environment template
        /// </summary>
        protected Environment EnvironmentTemplate => environmentTemplate;

        /// <summary>
        /// Service for Consumer registration.
        /// </summary>
        protected IRegistrationService RegistrationService => registrationService;

        /// <summary>
        /// Create a Consumer instance based upon the Environment passed.
        /// </summary>
        /// <param name="environment">Environment object.</param>
        /// <param name="settings">Consumer settings. If null, Consumer settings will be read from the SifFramework.config file.</param>
        /// <param name="sessionService">Consumer session service. If null, the Consumer session will be stored in the SifFramework.config file.</param>
        protected FunctionalServiceConsumer(
            Environment environment,
            IFrameworkSettings settings = null,
            ISessionService sessionService = null)
        {
            ConsumerSettings = settings ?? SettingsManager.ConsumerSettings;
            environmentTemplate = EnvironmentUtils.MergeWithSettings(environment, ConsumerSettings);
            registrationService =
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
        public FunctionalServiceConsumer(
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
        /// Convenience method to check if the Consumer is registered, throwing a standardised invalid operation
        /// exception if not.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        protected virtual void CheckRegistered()
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not been registered.");
            }
        }

        /// <summary>
        /// Serialise a single entity.
        /// </summary>
        /// <param name="item">Payload of a single entity.</param>
        /// <returns>XML string representation of the single entity.</returns>
        public virtual string SerialiseSingle<TSource, TDestination>(TSource item)
        {
            TDestination data = MapperFactory.CreateInstance<TSource, TDestination>(item);

            return SerialiserFactory.GetXmlSerialiser<TDestination>().Serialise(data);
        }

        /// <summary>
        /// Serialise an entity of multiple entities.
        /// </summary>
        /// <param name="items">Payload of multiple entities.</param>
        /// <returns>XML string representation of the multiple entities.</returns>
        public virtual string SerialiseMultiple(IEnumerable<Job> items)
        {
            List<jobType> data = MapperFactory.CreateInstances<Job, jobType>(items).ToList();
            var collection = new jobCollectionType() { job = data.ToArray() };

            return SerialiserFactory.GetXmlSerialiser<jobCollectionType>().Serialise(collection);
        }

        /// <summary>
        /// Deserialise a single entity.
        /// </summary>
        /// <param name="payload">Payload of a single entity.</param>
        /// <returns>The deserialized single entity.</returns>
        public virtual TDestination DeserialiseSingle<TDestination, TSource>(string payload)
        {
            TSource data = SerialiserFactory.GetXmlSerialiser<TSource>().Deserialise(payload);

            return MapperFactory.CreateInstance<TSource, TDestination>(data);
        }

        /// <summary>
        /// Deserialise an entity of multiple jobs.
        /// </summary>
        /// <param name="payload">Payload of multiple jobs.</param>
        /// <returns>Entity representing multiple jobs.</returns>
        public virtual List<TDestination> DeserialiseMultiple<TDestination, TSource>(string payload)
        {
            List<TSource> data = SerialiserFactory.GetXmlSerialiser<List<TSource>>().Deserialise(payload);

            return MapperFactory.CreateInstances<TSource, TDestination>(data).ToList();
        }

        /// <summary>
        /// Register this Consumer.
        /// </summary>
        public void Register()
        {
            registrationService.Register(ref environmentTemplate);
        }

        /// <summary>
        /// Unregister this Consumer.
        /// </summary>
        /// <param name="deleteOnUnregister"></param>
        public void Unregister(bool? deleteOnUnregister = null)
        {
            registrationService.Unregister(deleteOnUnregister);
        }

        /// <summary>
        /// Gets the URL of the functional service for the specified job name.
        /// </summary>
        /// <param name="jobName">The jon name to build a URL for.</param>
        /// <returns>A string in the form {http/https}://{domain:port}/{servicesConnectorPath}/{jobName}s.</returns>
        protected virtual string GetUrlPrefix(string jobName)
        {
            return $"{EnvironmentTemplate.ParseServiceUrl(ServiceType.FUNCTIONAL)}/{jobName}s";
        }

        /// <summary>
        /// Create a single Job with the defaults provided, and persist it to the data store
        /// </summary>
        /// <param name="job">Job object with defaults to use when creating the Job</param>
        /// <param name="zoneId">The zone in which to create the Job</param>
        /// <param name="contextId">The context in which to create the Job</param>
        /// <returns>The created Job object</returns>
        public virtual Job Create(Job job, string zoneId = null, string contextId = null)
        {
            CheckRegistered();
            CheckJob(job, RightType.CREATE, zoneId);
            string url = GetUrlPrefix(job.Name) + "/" + job.Name + HttpUtils.MatrixParameters(zoneId, contextId);
            string body = SerialiseSingle<Job, jobType>(job);
            string xml = HttpUtils.PostRequest(
                url,
                RegistrationService.AuthorisationToken,
                body,
                ConsumerSettings.CompressPayload,
                ServiceType.FUNCTIONAL);

            if (Log.IsDebugEnabled) Log.Debug($"XML from POST request ...\n{xml}");

            return DeserialiseSingle<Job, jobType>(xml);
        }

        /// <summary>
        /// Create a multiple Jobs with the defaults provided, and persist it to the data store
        /// </summary>
        /// <param name="jobs">Job objects with defaults to use when creating the Jobs</param>
        /// <param name="zoneId">The zone in which to create the Jobs</param>
        /// <param name="contextId">The context in which to create the Jobs</param>
        /// <returns>A MultipleCreateResponse object</returns>
        public virtual MultipleCreateResponse Create(List<Job> jobs, string zoneId = null, string contextId = null)
        {
            CheckRegistered();
            string jobName = CheckJobs(jobs, RightType.CREATE, zoneId);
            string url = GetUrlPrefix(jobName) + HttpUtils.MatrixParameters(zoneId, contextId);
            string body = SerialiseMultiple(jobs);
            string xml = HttpUtils.PostRequest(
                url,
                RegistrationService.AuthorisationToken,
                body,
                ConsumerSettings.CompressPayload,
                ServiceType.FUNCTIONAL);

            if (Log.IsDebugEnabled) Log.Debug($"XML from POST request ...\n{xml}");

            createResponseType createResponseType =
                SerialiserFactory.GetXmlSerialiser<createResponseType>().Deserialise(xml);
            MultipleCreateResponse createResponse =
                MapperFactory.CreateInstance<createResponseType, MultipleCreateResponse>(createResponseType);

            return createResponse;
        }

        /// <summary>
        /// Convenience method that processes a MultipleCreateResponse message and fetches all successfully created
        /// jobs. It does this by issuing multiple individual query requests for any create status codes that start
        /// with a "2" (OK, Created, etc.).
        /// </summary>
        /// <param name="creates">A MultipleCreateResponse object to parse</param>
        /// <param name="jobName">The job name (singular) that the MultipleCreateResponse refers to</param>
        /// <param name="zoneId">The zone in which to fetch the Jobs</param>
        /// <param name="contextId">The context in which to fetch the Jobs</param>
        /// <returns>The created Job objects</returns>
        public virtual IList<Job> GetCreated(
            MultipleCreateResponse creates,
            string jobName,
            string zoneId = null,
            string contextId = null)
        {
            if (creates == null)
            {
                throw new ArgumentNullException(nameof(creates));
            }

            IList<Job> fetched = new List<Job>();
            IList<Job> toFetch = (from CreateStatus s in creates.StatusRecords
                                  where s.StatusCode.StartsWith("2")
                                  select new Job()
                                  {
                                      Id = Guid.Parse(s.Id),
                                      Name = jobName
                                  }).ToList();

            foreach (Job job in toFetch)
            {
                fetched.Add(Query(job, zoneId, contextId));
            }

            return fetched;
        }

        /// <summary>
        /// Get a single Job
        /// </summary>
        /// <param name="job">The job template of the Job to fetch, must have name and id properties populated.</param>
        /// <param name="zoneId">The zone in which to perform the request.</param>
        /// <param name="contextId">The context in which to perform the request.</param>
        /// <returns>The Job object</returns>
        public virtual Job Query(Job job, string zoneId = null, string contextId = null)
        {
            CheckRegistered();
            CheckJob(job, RightType.QUERY, zoneId);

            try
            {
                string url = GetUrlPrefix(job.Name) + "/" + job.Id + HttpUtils.MatrixParameters(zoneId, contextId);
                string xml = HttpUtils.GetRequest(
                    url,
                    RegistrationService.AuthorisationToken,
                    ConsumerSettings.CompressPayload,
                    ServiceType.FUNCTIONAL);

                if (Log.IsDebugEnabled) Log.Debug($"XML from GET request ...\n{xml}");

                return DeserialiseSingle<Job, jobType>(xml);
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

            return null;
        }

        /// <summary>
        /// Get a all Jobs
        /// </summary>
        /// <param name="jobName">The name of the job used to resolve the right functional service</param>
        /// <param name="navigationPage">The page to fetch</param>
        /// <param name="navigationPageSize">The number of items to fetch per page</param>
        /// <param name="zoneId">The zone in which to operate</param>
        /// <param name="contextId">The context in which to operate</param>
        /// <returns>A page of Job objects</returns>
        public virtual List<Job> Query(
            string jobName,
            uint? navigationPage = null,
            uint? navigationPageSize = null,
            string zoneId = null,
            string contextId = null)
        {
            CheckRegistered();
            CheckJob(new Job(jobName), RightType.QUERY, zoneId, true);
            string url = GetUrlPrefix(jobName) + HttpUtils.MatrixParameters(zoneId, contextId);
            string xml;

            if (navigationPage.HasValue && navigationPageSize.HasValue)
            {
                xml = HttpUtils.GetRequest(
                    url,
                    RegistrationService.AuthorisationToken,
                    ConsumerSettings.CompressPayload,
                    ServiceType.FUNCTIONAL,
                    (int)navigationPage,
                    (int)navigationPageSize);
            }
            else
            {
                xml = HttpUtils.GetRequest(
                    url,
                    RegistrationService.AuthorisationToken,
                    ConsumerSettings.CompressPayload,
                    ServiceType.FUNCTIONAL);
            }

            return DeserialiseMultiple<Job, jobType>(xml);
        }

        /// <summary>
        /// Get a all Jobs that match the example provided.
        /// </summary>
        /// <param name="job">The example object to match against</param>
        /// <param name="navigationPage">The page to fetch</param>
        /// <param name="navigationPageSize">The number of items to fetch per page</param>
        /// <param name="zoneId">The zone in which to operate</param>
        /// <param name="contextId">The context in which to operate</param>
        /// <returns>A page of Job objects</returns>
        public virtual List<Job> QueryByExample(
            Job job,
            uint? navigationPage = null,
            uint? navigationPageSize = null,
            string zoneId = null,
            string contextId = null)
        {
            CheckRegistered();
            CheckJob(job, RightType.QUERY, zoneId);
            string url = GetUrlPrefix(job.Name) + HttpUtils.MatrixParameters(zoneId, contextId);
            string body = SerialiseSingle<Job, jobType>(job);
            // TODO: Update PostRequest to accept paging parameters.
            string xml = HttpUtils.PostRequest(
                url,
                RegistrationService.AuthorisationToken,
                body,
                ConsumerSettings.CompressPayload,
                ServiceType.FUNCTIONAL,
                "GET");

            if (Log.IsDebugEnabled) Log.Debug($"XML from POST (Query by Example) request ...\n{xml}");

            return DeserialiseMultiple<Job, jobType>(xml);
        }

        /// <summary>
        /// Update single job object is not supported for Functional Services. Throws a HttpResponseException with
        /// Forbidden status code.
        /// </summary>
        /// <param name="job">Job object to update</param>
        /// <param name="zoneId">The zone in which to update the Job</param>
        /// <param name="contextId">The context in which to update the Job</param>
        public virtual void Update(Job job, string zoneId = null, string contextId = null)
        {
            CheckRegistered();
            CheckJob(job, RightType.UPDATE, zoneId);

            throw new UpdateException("HttpStatusCode.Forbidden");
        }

        /// <summary>
        /// Update multiple job objects is not supported for Functional Services. Throws a HttpResponseException with
        /// Forbidden status code.
        /// </summary>
        /// <param name="jobs">Job objects to update</param>
        /// <param name="zoneId">The zone in which to update the Jobs</param>
        /// <param name="contextId">The context in which to update the Jobs</param>
        public virtual MultipleUpdateResponse Update(List<Job> jobs, string zoneId = null, string contextId = null)
        {
            CheckRegistered();
            CheckJobs(jobs, RightType.UPDATE, zoneId);

            throw new UpdateException("HttpStatusCode.Forbidden");
        }

        /// <summary>
        /// Delete a Job
        /// </summary>
        /// <param name="job">The job template of the Job to delete, must have name and id populated.</param>
        /// <param name="zoneId">The zone in which to perform the request.</param>
        /// <param name="contextId">The context in which to perform the request.</param>
        public virtual void Delete(Job job, string zoneId = null, string contextId = null)
        {
            CheckRegistered();
            CheckJob(job, RightType.DELETE, zoneId);
            string url = GetUrlPrefix(job.Name) + "/" + job.Id + HttpUtils.MatrixParameters(zoneId, contextId);
            string xml = HttpUtils.DeleteRequest(
                url,
                RegistrationService.AuthorisationToken,
                ConsumerSettings.CompressPayload,
                ServiceType.FUNCTIONAL);

            if (Log.IsDebugEnabled) Log.Debug($"XML from DELETE request ...\n{xml}");
        }

        /// <summary>
        /// Delete a series of Job objects
        /// </summary>
        /// <param name="jobs">The job object templates of the Jobs to delete, each must have name and id populated. tHe name of all jobs must be the same.</param>
        /// <param name="zoneId">The zone in which to perform the request.</param>
        /// <param name="contextId">The context in which to perform the request.</param>
        /// <returns>A response</returns>
        public virtual MultipleDeleteResponse Delete(List<Job> jobs, string zoneId = null, string contextId = null)
        {
            CheckRegistered();
            string jobName = CheckJobs(jobs, RightType.DELETE, zoneId);
            var deleteIds = new List<deleteIdType>();

            foreach (Job job in jobs)
            {
                deleteIds.Add(new deleteIdType { id = job.Id.ToString() });
            }

            var request = new deleteRequestType { deletes = deleteIds.ToArray() };
            string url = GetUrlPrefix(jobName) + HttpUtils.MatrixParameters(zoneId, contextId);
            string body = SerialiserFactory.GetXmlSerialiser<deleteRequestType>().Serialise(request);
            string xml = HttpUtils.PutRequest(
                url,
                RegistrationService.AuthorisationToken,
                body,
                ConsumerSettings.CompressPayload,
                ServiceType.FUNCTIONAL,
                "DELETE");

            if (Log.IsDebugEnabled) Log.Debug($"XML from PUT (DELETE) request ...\n{xml}");

            deleteResponseType updateResponseType =
                SerialiserFactory.GetXmlSerialiser<deleteResponseType>().Deserialise(xml);
            MultipleDeleteResponse updateResponse =
                MapperFactory.CreateInstance<deleteResponseType, MultipleDeleteResponse>(updateResponseType);

            return updateResponse;
        }

        /// <summary>
        /// Send a create operation to a specified phase on the specified job.
        /// </summary>
        /// <param name="job">The Job on which to execute the phase</param>
        /// <param name="phaseName">The name of the phase</param>
        /// <param name="body">The payload to send to the phase</param>
        /// <param name="zoneId">The zone in which to operate</param>
        /// <param name="contextId">The context in which to operate</param>
        /// <param name="contentTypeOverride">The mime type of the data to be sent</param>
        /// <param name="acceptOverride">The expected mime type of the result</param>
        /// <returns>A string, possibly containing a serialized object, returned from the functional service</returns>
        public virtual string CreateToPhase(
            Job job,
            string phaseName,
            string body = null,
            string zoneId = null,
            string contextId = null,
            string contentTypeOverride = null,
            string acceptOverride = null)
        {
            CheckRegistered();
            CheckJob(job, zoneId);
            string url = GetUrlPrefix(job.Name) + "/" + job.Id + "/" + phaseName +
                         HttpUtils.MatrixParameters(zoneId, contextId);
            string response = HttpUtils.PostRequest(
                url,
                RegistrationService.AuthorisationToken,
                body,
                ConsumerSettings.CompressPayload,
                ServiceType.FUNCTIONAL,
                contentTypeOverride: contentTypeOverride,
                acceptOverride: acceptOverride);

            if (Log.IsDebugEnabled) Log.Debug($"String from CREATE request to phase ...\n{response}");

            return response;
        }

        /// <summary>
        /// Send a retrieve operation to a specified phase on the specified job.
        /// </summary>
        /// <param name="job">The Job on which to execute the phase</param>
        /// <param name="phaseName">The name of the phase</param>
        /// <param name="body">The payload to send to the phase</param>
        /// <param name="zoneId">The zone in which to operate</param>
        /// <param name="contextId">The context in which to operate</param>
        /// <param name="contentTypeOverride">The mime type of the data to be sent</param>
        /// <param name="acceptOverride">The expected mime type of the result</param>
        /// <returns>A string, possibly containing a serialized object, returned from the functional service</returns>
        public virtual string RetrieveToPhase(
            Job job,
            string phaseName,
            string body = null,
            string zoneId = null,
            string contextId = null,
            string contentTypeOverride = null,
            string acceptOverride = null)
        {
            CheckRegistered();
            CheckJob(job, zoneId);
            string url = GetUrlPrefix(job.Name) + "/" + job.Id + "/" + phaseName +
                         HttpUtils.MatrixParameters(zoneId, contextId);
            string response = HttpUtils.PostRequest(
                url,
                RegistrationService.AuthorisationToken,
                body,
                ConsumerSettings.CompressPayload,
                ServiceType.FUNCTIONAL,
                "GET",
                contentTypeOverride, acceptOverride);

            if (Log.IsDebugEnabled) Log.Debug($"String from GET request to phase ...\n{response}");

            return response;
        }

        /// <summary>
        /// Send a update operation to a specified phase on the specified job.
        /// </summary>
        /// <param name="job">The Job on which to execute the phase</param>
        /// <param name="phaseName">The name of the phase</param>
        /// <param name="body">The payload to send to the phase</param>
        /// <param name="zoneId">The zone in which to operate</param>
        /// <param name="contextId">The context in which to operate</param>
        /// <param name="contentTypeOverride">The mime type of the data to be sent</param>
        /// <param name="acceptOverride">The expected mime type of the result</param>
        /// <returns>A string, possibly containing a serialized object, returned from the functional service</returns>
        public virtual string UpdateToPhase(
            Job job,
            string phaseName,
            string body,
            string zoneId = null,
            string contextId = null,
            string contentTypeOverride = null,
            string acceptOverride = null)
        {
            CheckRegistered();
            CheckJob(job, zoneId);
            string url = GetUrlPrefix(job.Name) + "/" + job.Id + "/" + phaseName +
                         HttpUtils.MatrixParameters(zoneId, contextId);
            string response = HttpUtils.PutRequest(
                url,
                RegistrationService.AuthorisationToken,
                body,
                ConsumerSettings.CompressPayload,
                ServiceType.FUNCTIONAL,
                contentTypeOverride: contentTypeOverride,
                acceptOverride: acceptOverride);

            if (Log.IsDebugEnabled) Log.Debug($"String from PUT request to phase ...{response}");

            return response;
        }

        /// <summary>
        /// Send a delete operation to a specified phase on the specified job.
        /// </summary>
        /// <param name="job">The Job on which to execute the phase</param>
        /// <param name="phaseName">The name of the phase</param>
        /// <param name="body">The payload to send to the phase</param>
        /// <param name="zoneId">The zone in which to operate</param>
        /// <param name="contextId">The context in which to operate</param>
        /// <param name="contentTypeOverride">The mime type of the data to be sent</param>
        /// <param name="acceptOverride">The expected mime type of the result</param>
        /// <returns>A string, possibly containing a serialized object, returned from the functional service</returns>
        public virtual string DeleteToPhase(
            Job job,
            string phaseName,
            string body,
            string zoneId = null,
            string contextId = null,
            string contentTypeOverride = null,
            string acceptOverride = null)
        {
            CheckRegistered();
            CheckJob(job, zoneId);
            string url = GetUrlPrefix(job.Name) + "/" + job.Id + "/" + phaseName +
                         HttpUtils.MatrixParameters(zoneId, contextId);
            string response = HttpUtils.DeleteRequest(
                url,
                RegistrationService.AuthorisationToken,
                body,
                ConsumerSettings.CompressPayload,
                ServiceType.FUNCTIONAL,
                contentTypeOverride,
                acceptOverride);

            if (Log.IsDebugEnabled) Log.Debug($"String from DELETE request to phase ...\n{response}");

            return response;
        }

        /// <summary>
        /// Send a create operation to  the state of the specified phase on the specified job.
        /// </summary>
        /// <param name="job">The Job on which to operate</param>
        /// <param name="phaseName">The name of the phase whose state is to change</param>
        /// <param name="item">The PhaseState instance template</param>
        /// <param name="zoneId">The zone in which to perform the request.</param>
        /// <param name="contextId">The context in which to perform the request.</param>
        /// <returns>The current state of the phase.</returns>
        public virtual PhaseState CreateToState(
            Job job,
            string phaseName,
            PhaseState item,
            string zoneId = null,
            string contextId = null)
        {
            CheckRegistered();
            CheckJob(job, zoneId);
            string url = GetUrlPrefix(job.Name) + "/" + job.Id + "/" + phaseName + "/states/state" +
                         HttpUtils.MatrixParameters(zoneId, contextId);
            string body = SerialiseSingle<PhaseState, stateType>(item);
            string xml = HttpUtils.PostRequest(
                url,
                RegistrationService.AuthorisationToken,
                body,
                ConsumerSettings.CompressPayload,
                ServiceType.FUNCTIONAL);

            if (Log.IsDebugEnabled) Log.Debug($"Guid from CREATE request to state on phase ...\n{xml}");

            return DeserialiseSingle<PhaseState, stateType>(xml);
        }

        private Model.Infrastructure.Service CheckJob(Job job, string zoneId = null)
        {
            if (job == null)
            {
                throw new ArgumentException("Job cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(job.Name))
            {
                throw new ArgumentException("Job name must be specified.");
            }

            Model.Infrastructure.Service service = ZoneUtils.GetService(
                registrationService.CurrentEnvironment.GetTargetZone(zoneId),
                job.Name + "s",
                ServiceType.FUNCTIONAL);

            if (service == null)
            {
                throw new ArgumentException(
                    $"A FUNCTIONAL service with the name {job.Name}s cannot be found in the current environment");
            }

            return service;
        }

        private void CheckJob(Job job, RightType right, string zoneId = null, bool ignoreId = false)
        {
            Model.Infrastructure.Service service = CheckJob(job, zoneId);

            if (!ignoreId && !right.Equals(RightType.CREATE) && job.Id == null)
            {
                throw new ArgumentException("Job must have an Id for any non-creation operation");
            }

            string rightValue = service.Rights.FirstOrDefault(r => r.Type == right.ToString())?.Value;

            if (rightValue == null || rightValue.Equals(RightValue.REJECTED.ToString()))
            {
                throw new ArgumentException(
                    "The attempted operation is not permitted in the ACL of the current environment");
            }
        }

        private string CheckJobs(IList<Job> jobs, RightType right, string zoneId = null)
        {
            if (jobs == null || jobs.Count == 0)
            {
                throw new ArgumentException("List of job objects cannot be null or empty");
            }

            string name = null;

            foreach (Job job in jobs)
            {
                CheckJob(job, right, zoneId);

                if (string.IsNullOrWhiteSpace(name))
                {
                    name = job.Name;
                }

                if (name == null || !name.Equals(job.Name))
                {
                    throw new ArgumentException("All job objects must have the same name");
                }
            }
            return name;
        }
    }
}