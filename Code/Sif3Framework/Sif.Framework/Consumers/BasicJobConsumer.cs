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
using System.Reflection;
using System.Web.Http;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.Consumers
{
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
        protected abstract string TypeName { get; }

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

        public virtual Job Create(Job obj, string zone = null, string context = null)
        {
            checkRegistered();

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/services/" + TypeName + "s" + "/" + TypeName + HttpUtils.MatrixParameters(zone, context);
            string body = SerialiseSingle(obj);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from POST request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseSingle(xml);
        }

        public virtual MultipleCreateResponse Create(List<Job> obj, string zone = null, string context = null)
        {
            checkRegistered();

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/services/" + TypeName + "s" + HttpUtils.MatrixParameters(zone, context);
            string body = SerialiseMultiple(obj);
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body);
            if (log.IsDebugEnabled) log.Debug("XML from POST request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            createResponseType createResponseType = SerialiserFactory.GetXmlSerialiser<createResponseType>().Deserialise(xml);
            MultipleCreateResponse createResponse = MapperFactory.CreateInstance<createResponseType, MultipleCreateResponse>(createResponseType);

            return createResponse;
        }

        public virtual Job Query(Guid id, string zone = null, string context = null)
        {
            checkRegistered();

            try
            {
                string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/services/" + TypeName + "s" + "/" + id + HttpUtils.MatrixParameters(zone, context);
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

        public virtual List<Job> Query(uint? navigationPage = null, uint? navigationPageSize = null, string zone = null, string context = null)
        {
            checkRegistered();

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/services/" + TypeName + "s" + HttpUtils.MatrixParameters(zone, context);
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

        public virtual List<Job> QueryByExample(Job obj, uint? navigationPage = null, uint? navigationPageSize = null, string zone = null, string context = null)
        {
            checkRegistered();

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/services/" + TypeName + "s" + HttpUtils.MatrixParameters(zone, context);
            string body = SerialiseSingle(obj);
            // TODO: Update PostRequest to accept paging parameters.
            string xml = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body, "GET");
            if (log.IsDebugEnabled) log.Debug("XML from POST (Query by Example) request ...");
            if (log.IsDebugEnabled) log.Debug(xml);

            return DeserialiseMultiple(xml);
        }
        
        
        public virtual void Update(Job obj, string zone = null, string context = null)
        {
            checkRegistered();

            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public virtual MultipleUpdateResponse Update(List<Job> obj, string zone = null, string context = null)
        {
            checkRegistered();

            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public virtual void Delete(Guid id, string zone = null, string context = null)
        {
            checkRegistered();

            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/services/" + TypeName + "s" + "/" + id + HttpUtils.MatrixParameters(zone, context);
            string xml = HttpUtils.DeleteRequest(url, RegistrationService.AuthorisationToken);
            if (log.IsDebugEnabled) log.Debug("XML from DELETE request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
        }
        
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
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/services/" + TypeName + "s" + HttpUtils.MatrixParameters(zone, context);
            string body = SerialiserFactory.GetXmlSerialiser<deleteRequestType>().Serialise(request);
            string xml = HttpUtils.PutRequest(url, RegistrationService.AuthorisationToken, body, "DELETE");
            if (log.IsDebugEnabled) log.Debug("XML from PUT (DELETE) request ...");
            if (log.IsDebugEnabled) log.Debug(xml);
            deleteResponseType updateResponseType = SerialiserFactory.GetXmlSerialiser<deleteResponseType>().Deserialise(xml);
            MultipleDeleteResponse updateResponse = MapperFactory.CreateInstance<deleteResponseType, MultipleDeleteResponse>(updateResponseType);

            return updateResponse;
        }

        public virtual string CreateToPhase(Job job, string phaseName, string body = null, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            return CreateToPhase(job.Id, phaseName, body, zone, context, contentTypeOverride, acceptOverride);
        }

        public virtual string CreateToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            checkRegistered();
            string response = null;
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/services/" + TypeName + "s" + "/" + id + "/phase/" + phaseName + HttpUtils.MatrixParameters(zone, context);
            response = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body, contentTypeOverride: contentTypeOverride, acceptOverride: acceptOverride);
            if (log.IsDebugEnabled) log.Debug("String from CREATE request to phase ...");
            if (log.IsDebugEnabled) log.Debug(response);
            return response;
        }

        public virtual string RetrieveToPhase(Job job, string phaseName, string body = null, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            return RetrieveToPhase(job.Id, phaseName, body, zone, context, contentTypeOverride, acceptOverride);
        }

        public virtual string RetrieveToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            checkRegistered();
            string response = null;
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/services/" + TypeName + "s" + "/" + id + "/phase/" + phaseName + HttpUtils.MatrixParameters(zone, context);
            response = HttpUtils.PostRequest(url, RegistrationService.AuthorisationToken, body, "GET", contentTypeOverride, acceptOverride);
            if (log.IsDebugEnabled) log.Debug("String from GET request to phase ...");
            if (log.IsDebugEnabled) log.Debug(response);
            return response;
        }
        
        public virtual string UpdateToPhase(Job obj, string phaseName, string body, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            return UpdateToPhase(obj.Id, phaseName, body, zone, context, contentTypeOverride, acceptOverride);
        }

        public virtual string UpdateToPhase(Guid id, string phaseName, string body, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            checkRegistered();
            string response = null;
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/services/" + TypeName + "s" + "/" + id + "/phase/" + phaseName + HttpUtils.MatrixParameters(zone, context);
            response = HttpUtils.PutRequest(url, RegistrationService.AuthorisationToken, body, contentTypeOverride: contentTypeOverride, acceptOverride: acceptOverride);
            if (log.IsDebugEnabled) log.Debug("String from PUT request to phase ...");
            if (log.IsDebugEnabled) log.Debug(response);
            return response;
        }

        public virtual string DeleteToPhase(Job obj, string phaseName, string body, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            return DeleteToPhase(obj.Id, phaseName, body, zone, context, contentTypeOverride, acceptOverride);
        }

        public virtual string DeleteToPhase(Guid id, string phaseName, string body, string zone = null, string context = null, string contentTypeOverride = null, string acceptOverride = null)
        {
            checkRegistered();
            string response = null;
            string url = EnvironmentUtils.ParseServiceUrl(EnvironmentTemplate) + "/services/" + TypeName + "s" + "/" + id + "/phase/" + phaseName + HttpUtils.MatrixParameters(zone, context);
            response = HttpUtils.DeleteRequest(url, RegistrationService.AuthorisationToken, body, contentTypeOverride: contentTypeOverride, acceptOverride: acceptOverride);
            if (log.IsDebugEnabled) log.Debug("String from DELETE request to phase ...");
            if (log.IsDebugEnabled) log.Debug(response);
            return response;
        }
    }
}
