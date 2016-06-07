/*
 * Crown Copyright © Department for Education (UK) 2016
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
using Sif.Framework.Model.Infrastructure;
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
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Consumers
{
    /// <summary>
    /// The base class for all Functional Service consumers
    /// </summary>
    public abstract class BasicJobConsumer
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Environment environmentTemplate;
        private RegistrationService registrationService;


        /// <summary>
        /// Consumer environment.
        /// </summary>
        protected Environment EnvironmentTemplate
        {
            get { return environmentTemplate; }
        }

        /// <summary>
        /// Service for Consumer registration.
        /// </summary>
        protected RegistrationService RegistrationService
        {
            get { return registrationService; }
        }

        /// <summary>
        /// Name of the Functional Service that the Consumer is based on
        /// </summary>
        public abstract string TypeName { get; }

        /// <summary>
        /// Create a Consumer instance based upon the Environment passed.
        /// </summary>
        /// <param name="environment">Environment object.</param>
        public BasicJobConsumer(Environment environment)
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
        /// <param name="solutionId">Solution ID.</param>
        public BasicJobConsumer(string applicationKey, string instanceId = null, string userToken = null, string solutionId = null): this(new Environment(applicationKey, instanceId, userToken, solutionId))
        {
        }

        /// <summary>
        /// Convenience method to check if the Consumer is registered, throwing a standardised invalid operation exception if not.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        protected virtual void checkRegistered()
        {
            if (!RegistrationService.Registered)
            {
                throw new InvalidOperationException("Consumer has not registered.");
            }
        }

        /// <summary>
        /// Serialise a single job entity.
        /// </summary>
        /// <param name="obj">Payload of a single job.</param>
        /// <returns>XML string representation of the single job.</returns>
        public virtual string SerialiseSingle(Job obj)
        {
            jobType data = MapperFactory.CreateInstance<Job, jobType>(obj);
            return SerialiserFactory.GetXmlSerialiser<jobType>().Serialise(data);
        }

        /// <summary>
        /// Serialise an entity of multiple jobs.
        /// </summary>
        /// <param name="obj">Payload of multiple jobs.</param>
        /// <returns>XML string representation of the multiple jobs.</returns>
        public virtual string SerialiseMultiple(IEnumerable<Job> obj)
        {
            List<jobType> data = MapperFactory.CreateInstances<Job, jobType>(obj).ToList();
            return SerialiserFactory.GetXmlSerialiser<List<jobType>>().Serialise(data);
        }

        /// <summary>
        /// Deserialise a single job entity.
        /// </summary>
        /// <param name="payload">Payload of a single job.</param>
        /// <returns>Entity representing the single job.</returns>
        public virtual Job DeserialiseSingle(string payload)
        {
            jobType data = SerialiserFactory.GetXmlSerialiser<jobType>().Deserialise(payload);
            return MapperFactory.CreateInstance<jobType, Job>(data);
        }

        /// <summary>
        /// Deserialise an entity of multiple jobs.
        /// </summary>
        /// <param name="payload">Payload of multiple jobs.</param>
        /// <returns>Entity representing multiple jobs.</returns>
        public virtual List<Job> DeserialiseMultiple(string payload)
        {
            List<jobType> data = SerialiserFactory.GetXmlSerialiser<List<jobType>>().Deserialise(payload);
            return MapperFactory.CreateInstances<jobType, Job>(data).ToList();
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
        /// Create a single Job with the defaults provided, and persist it to the data store
        /// </summary>
        /// <param name="obj">Job object with defaults to use when creating the Job</param>
        /// <param name="zone">The zone in which to create the Job</param>
        /// <param name="context">The context in which to create the Job</param>
        /// <returns>The created Job object</returns>
        public virtual Job Create(Job obj, string zone = null, string context = null)
        {
            checkRegistered();
            
            configureJob(obj);

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + TypeName + HttpUtils.MatrixParameters(zone, context);
            string body = SerialiseSingle(obj);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from POST request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseSingle(xml);
        }

        /// <summary>
        /// Create a multiple Jobs with the defaults provided, and persist it to the data store
        /// </summary>
        /// <param name="objs">Job objects with defaults to use when creating the Jobs</param>
        /// <param name="zone">The zone in which to create the Jobs</param>
        /// <param name="context">The context in which to create the Jobs</param>
        /// <returns>The created Job objects</returns>
        public virtual MultipleCreateResponse Create(List<Job> objs, string zone = null, string context = null)
        {
            checkRegistered();

            if (objs == null || objs.Count == 0)
            {
                throw new ArgumentException("List of job objects cannot be null or empty");
            }

            foreach (Job obj in objs)
            {
                configureJob(obj);
            }

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + HttpUtils.MatrixParameters(zone, context);
            string body = SerialiseMultiple(objs);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from POST request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            createResponseType createResponseType = SerialiserFactory.GetXmlSerialiser<createResponseType>().Deserialise(xml);
            MultipleCreateResponse createResponse = MapperFactory.CreateInstance<createResponseType, MultipleCreateResponse>(createResponseType);

            return createResponse;
        }

        /// <summary>
        /// Get a single Job by its RefId
        /// </summary>
        /// <param name="id">The RefId of the Job to fetch</param>
        /// <param name="zone">The zone in which to operate</param>
        /// <param name="context">The context in which to operate</param>
        /// <returns>The Job object</returns>
        public virtual Job Query(Guid id, string zone = null, string context = null)
        {
            checkRegistered();

            try
            {
                string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + id + HttpUtils.MatrixParameters(zone, context);
                string xml = HttpUtils.GetRequest(url, RegistrationService.AuthorisationToken);
                if (log.IsDebugEnabled) log.Debug("XML from GET request ...");
                if (log.IsDebugEnabled) log.Debug(xml);
                return DeserialiseSingle(xml);
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

            return null;
        }

        /// <summary>
        /// Get a all Jobs
        /// </summary>
        /// <param name="navigationPage">The page to fetch</param>
        /// <param name="navigationPageSize">The number of items to fetch per page</param>
        /// <param name="zone">The zone in which to operate</param>
        /// <param name="context">The context in which to operate</param>
        /// <returns>A page of Job objects</returns>
        public virtual List<Job> Query(uint? navigationPage = null, uint? navigationPageSize = null, string zone = null, string context = null)
        {
            checkRegistered();

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + HttpUtils.MatrixParameters(zone, context);
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
        /// Get a all Jobs that match the example provided.
        /// </summary>
        /// <param name="obj">The example object to match against</param>
        /// <param name="navigationPage">The page to fetch</param>
        /// <param name="navigationPageSize">The number of items to fetch per page</param>
        /// <param name="zone">The zone in which to operate</param>
        /// <param name="context">The context in which to operate</param>
        /// <returns>A page of Job objects</returns>
        public virtual List<Job> QueryByExample(Job obj, uint? navigationPage = null, uint? navigationPageSize = null, string zone = null, string context = null)
        {
            checkRegistered();

            configureJob(obj);

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + HttpUtils.MatrixParameters(zone, context);
            string body = SerialiseSingle(obj);
            // TODO: Update PostRequest to accept paging parameters.
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body, "GET");
            if (log.IsDebugEnabled) log.Debug("XML from POST (Query by Example) request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseMultiple(xml);
        }


        /// <summary>
        /// Update single job object is not supported for Functional Services. Throws a HttpResponseException with Forbidden status code.
        /// </summary>
        /// <param name="obj">Job object to update</param>
        /// <param name="zone">The zone in which to update the Job</param>
        /// <param name="context">The context in which to update the Job</param>
        public virtual void Update(Job obj, string zone = null, string context = null)
        {
            checkRegistered();

            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Update multiple job objects is not supported for Functional Services. Throws a HttpResponseException with Forbidden status code.
        /// </summary>
        /// <param name="objs">Job objects to update</param>
        /// <param name="zone">The zone in which to update the Jobs</param>
        /// <param name="context">The context in which to update the Jobs</param>
        public virtual MultipleUpdateResponse Update(List<Job> objs, string zone = null, string context = null)
        {
            checkRegistered();

            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Delete a Job object by its RefId
        /// </summary>
        /// <param name="id">The RefId of the Job to delete</param>
        /// <param name="zone">The zone in which to delete the Job</param>
        /// <param name="context">The context in which to delete the Job</param>
        public virtual void Delete(Guid id, string zone = null, string context = null)
        {
            checkRegistered();

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + id + HttpUtils.MatrixParameters(zone, context);
            string xml = HttpUtils.DeleteRequest(url, RegistrationService.AuthorisationToken);
            if (log.IsDebugEnabled) log.Debug("XML from DELETE request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
        }

        /// <summary>
        /// Delete a series of Job objects by their RefIds
        /// </summary>
        /// <param name="ids">The RefIds of the Jobs to delete</param>
        /// <param name="zone">The zone in which to delete the Jobs</param>
        /// <param name="context">The context in which to delete the Jobs</param>
        /// <returns>A response</returns>
        public virtual MultipleDeleteResponse Delete(IEnumerable<string> ids, string zone = null, string context = null)
        {
            checkRegistered();

            List<deleteIdType> deleteIds = new List<deleteIdType>();

            foreach (string id in ids)
            {
                deleteIdType deleteId = new deleteIdType { id = id.ToString() };
                deleteIds.Add(deleteId);
            }

            deleteRequestType request = new deleteRequestType { deletes = deleteIds.ToArray() };
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + HttpUtils.MatrixParameters(zone, context);
            string body = SerialiserFactory.GetXmlSerialiser<deleteRequestType>().Serialise(request);
            string xml = HttpUtils.PutRequest(url, RegistrationService.AuthorisationToken, body, "DELETE");
            if (log.IsDebugEnabled) log.Debug("XML from PUT (DELETE) request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            deleteResponseType updateResponseType = SerialiserFactory.GetXmlSerialiser<deleteResponseType>().Deserialise(xml);
            MultipleDeleteResponse updateResponse = MapperFactory.CreateInstance<deleteResponseType, MultipleDeleteResponse>(updateResponseType);

            return updateResponse;
        }

        /// <summary>
        /// Send a create operation to a specified phase on the specified job.
        /// </summary>
        /// <param name="job">The Job on which to execute the phase</param>
        /// <param name="phaseName">The name of the phase</param>
        /// <param name="body">The payload to send to the phase</param>
        /// <param name="zone">The zone in which to operate</param>
        /// <param name="context">The context in which to operate</param>
        /// <param name="contentTypeOverride">The mime type of the data to be sent</param>
        /// <param name="acceptOverride">The expected mime type of the result</param>
        /// <returns>A string, possibly containing a serialized object, returned from the functional service</returns>
        public virtual string CreateToPhase(Job job, string phaseName, string body = null, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            configureJob(job);

            return CreateToPhase(job.Id, phaseName, body, zone, context, contentTypeOverride, acceptOverride);
        }

        /// <summary>
        /// Send a create operation to a specified phase on the specified job.
        /// </summary>
        /// <param name="id">The RefId of the Job on which to execute the phase</param>
        /// <param name="phaseName">The name of the phase</param>
        /// <param name="body">The payload to send to the phase</param>
        /// <param name="zone">The zone in which to operate</param>
        /// <param name="context">The context in which to operate</param>
        /// <param name="contentTypeOverride">The mime type of the data to be sent</param>
        /// <param name="acceptOverride">The expected mime type of the result</param>
        /// <returns>A string, possibly containing a serialized object, returned from the functional service</returns>
        public virtual string CreateToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            checkRegistered();
            string response = null;
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + id + "/phase/" + phaseName + HttpUtils.MatrixParameters(zone, context);
            response = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body, contentTypeOverride: contentTypeOverride, acceptOverride: acceptOverride);
            if (log.IsDebugEnabled) log.Debug("String from CREATE request to phase ...");
            if (log.IsDebugEnabled) log.Debug(response);
            return response;
        }

        /// <summary>
        /// Send a retrieve operation to a specified phase on the specified job.
        /// </summary>
        /// <param name="job">The Job on which to execute the phase</param>
        /// <param name="phaseName">The name of the phase</param>
        /// <param name="body">The payload to send to the phase</param>
        /// <param name="zone">The zone in which to operate</param>
        /// <param name="context">The context in which to operate</param>
        /// <param name="contentTypeOverride">The mime type of the data to be sent</param>
        /// <param name="acceptOverride">The expected mime type of the result</param>
        /// <returns>A string, possibly containing a serialized object, returned from the functional service</returns>
        public virtual string RetrieveToPhase(Job job, string phaseName, string body = null, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            configureJob(job);

            return RetrieveToPhase(job.Id, phaseName, body, zone, context, contentTypeOverride, acceptOverride);
        }

        /// <summary>
        /// Send a retrieve operation to a specified phase on the specified job.
        /// </summary>
        /// <param name="id">The RefId of the Job on which to execute the phase</param>
        /// <param name="phaseName">The name of the phase</param>
        /// <param name="body">The payload to send to the phase</param>
        /// <param name="zone">The zone in which to operate</param>
        /// <param name="context">The context in which to operate</param>
        /// <param name="contentTypeOverride">The mime type of the data to be sent</param>
        /// <param name="acceptOverride">The expected mime type of the result</param>
        /// <returns>A string, possibly containing a serialized object, returned from the functional service</returns>
        public virtual string RetrieveToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            checkRegistered();
            string response = null;
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + id + "/phase/" + phaseName + HttpUtils.MatrixParameters(zone, context);
            response = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body, "GET", contentTypeOverride, acceptOverride);
            if (log.IsDebugEnabled) log.Debug("String from GET request to phase ...");
            if (log.IsDebugEnabled) log.Debug(response);
            return response;
        }

        /// <summary>
        /// Send a update operation to a specified phase on the specified job.
        /// </summary>
        /// <param name="job">The Job on which to execute the phase</param>
        /// <param name="phaseName">The name of the phase</param>
        /// <param name="body">The payload to send to the phase</param>
        /// <param name="zone">The zone in which to operate</param>
        /// <param name="context">The context in which to operate</param>
        /// <param name="contentTypeOverride">The mime type of the data to be sent</param>
        /// <param name="acceptOverride">The expected mime type of the result</param>
        /// <returns>A string, possibly containing a serialized object, returned from the functional service</returns>
        public virtual string UpdateToPhase(Job obj, string phaseName, string body, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            configureJob(obj);

            return UpdateToPhase(obj.Id, phaseName, body, zone, context, contentTypeOverride, acceptOverride);
        }

        /// <summary>
        /// Send a update operation to a specified phase on the specified job.
        /// </summary>
        /// <param name="id">The RefId of the Job on which to execute the phase</param>
        /// <param name="phaseName">The name of the phase</param>
        /// <param name="body">The payload to send to the phase</param>
        /// <param name="zone">The zone in which to operate</param>
        /// <param name="context">The context in which to operate</param>
        /// <param name="contentTypeOverride">The mime type of the data to be sent</param>
        /// <param name="acceptOverride">The expected mime type of the result</param>
        /// <returns>A string, possibly containing a serialized object, returned from the functional service</returns>
        public virtual string UpdateToPhase(Guid id, string phaseName, string body, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            checkRegistered();
            string response = null;
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + id + "/phase/" + phaseName + HttpUtils.MatrixParameters(zone, context);
            response = HttpUtils.PutRequest(url, RegistrationService.AuthorisationToken, body, contentTypeOverride: contentTypeOverride, acceptOverride: acceptOverride);
            if (log.IsDebugEnabled) log.Debug("String from PUT request to phase ...");
            if (log.IsDebugEnabled) log.Debug(response);
            return response;
        }

        /// <summary>
        /// Send a delete operation to a specified phase on the specified job.
        /// </summary>
        /// <param name="job">The Job on which to execute the phase</param>
        /// <param name="phaseName">The name of the phase</param>
        /// <param name="body">The payload to send to the phase</param>
        /// <param name="zone">The zone in which to operate</param>
        /// <param name="context">The context in which to operate</param>
        /// <param name="contentTypeOverride">The mime type of the data to be sent</param>
        /// <param name="acceptOverride">The expected mime type of the result</param>
        /// <returns>A string, possibly containing a serialized object, returned from the functional service</returns>
        public virtual string DeleteToPhase(Job obj, string phaseName, string body, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            configureJob(obj);

            return DeleteToPhase(obj.Id, phaseName, body, zone, context, contentTypeOverride, acceptOverride);
        }

        /// <summary>
        /// Send a delete operation to a specified phase on the specified job.
        /// </summary>
        /// <param name="id">The RefId of the Job on which to execute the phase</param>
        /// <param name="phaseName">The name of the phase</param>
        /// <param name="body">The payload to send to the phase</param>
        /// <param name="zone">The zone in which to operate</param>
        /// <param name="context">The context in which to operate</param>
        /// <param name="contentTypeOverride">The mime type of the data to be sent</param>
        /// <param name="acceptOverride">The expected mime type of the result</param>
        /// <returns>A string, possibly containing a serialized object, returned from the functional service</returns>
        public virtual string DeleteToPhase(Guid id, string phaseName, string body, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            checkRegistered();
            string response = null;
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/" + TypeName + "s" + "/" + id + "/phase/" + phaseName + HttpUtils.MatrixParameters(zone, context);
            response = HttpUtils.DeleteRequest(url, RegistrationService.AuthorisationToken, body, contentTypeOverride: contentTypeOverride, acceptOverride: acceptOverride);
            if (log.IsDebugEnabled) log.Debug("String from DELETE request to phase ...");
            if (log.IsDebugEnabled) log.Debug(response);
            return response;
        }

        private void configureJob(Job job)
        {
            if (job == null)
            {
                throw new ArgumentException("Job cannot be null.");
            }

            if (StringUtils.NotEmpty(job.Name) && log.IsDebugEnabled) log.Debug("Changing job name from '" + job.Name + "'to '" + TypeName + "'");
            job.Name = TypeName;
        }
    }
}
