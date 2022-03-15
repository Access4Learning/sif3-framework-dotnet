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

using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Persistence;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Utils;
using Sif.Specification.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using Job = Sif.Framework.Model.Infrastructure.Job;

namespace Sif.Framework.Service.Functional
{
    /// <summary>
    /// The abstract Functional Service implementation that a functional services would normally extend.
    /// </summary>
    public abstract class FunctionalService : SifService<jobType, Job>, IFunctionalService
    {
        private static readonly slf4net.ILogger Log =
            slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly IGenericRepository<SifObjectBinding, long> bindings;

        /// <summary>
        /// The dictionary of phases this service contains, with the actions each can perform
        /// </summary>
        protected IDictionary<string, IPhaseActions> PhaseActions;

        /// <summary>
        /// Basic constructor
        /// </summary>
        protected FunctionalService(
            IGenericRepository<Job, Guid> jobRepository,
            IGenericRepository<SifObjectBinding, long> objectBindingRepository)
            : base(jobRepository)
        {
            PhaseActions = new Dictionary<string, IPhaseActions>();
            bindings = objectBindingRepository;
        }

        //protected FunctionalService()
        //    : base(new GenericRepository<Job, Guid>(EnvironmentProviderSessionFactory.Instance))
        //{
        //    PhaseActions = new Dictionary<string, IPhaseActions>();
        //    bindings = new GenericRepository<SifObjectBinding, long>(EnvironmentProviderSessionFactory.Instance);
        //}

        /// <summary>
        /// Method that must be extended to add phases to a given job when it has been created.
        /// </summary>
        protected abstract void Configure(Job job);

        /// <summary>
        /// <see cref="IFunctionalService.GetServiceName"/>
        /// </summary>
        /// <returns></returns>
        public abstract string GetServiceName();

        /// <summary>
        /// <see cref="IFunctionalService.Startup"/>
        /// </summary>
        public virtual void Startup()
        {
            Log.Info("Starting thread for " + GetServiceName() + " service.");
        }

        /// <summary>
        /// <see cref="IFunctionalService.Shutdown"/>
        /// </summary>
        public virtual void Shutdown()
        {
            Log.Info("Shutting down thread for " + GetServiceName() + " service.");
        }

        /// <summary>
        /// <see cref="IFunctionalService.Bind(Guid, string)"/>
        /// </summary>
        public virtual void Bind(Guid objectId, string ownerId)
        {
            long id = bindings.Save(new SifObjectBinding
            {
                OwnerId = ownerId,
                RefId = objectId
            });

            Log.Info("Bound object " + objectId + " with session token " + ownerId + ". ID = " + id);
        }

        /// <summary>
        /// <see cref="IFunctionalService.Unbind(string)"/>
        /// </summary>
        public virtual void Unbind(string ownerId)
        {
            bindings.Delete(bindings.Retrieve(new SifObjectBinding
            {
                OwnerId = ownerId
            }));

            Log.Info("Unbound all objects from the session token " + ownerId);
        }

        /// <summary>
        /// <see cref="IFunctionalService.Unbind(Guid)"/>
        /// </summary>
        public virtual void Unbind(Guid objectId)
        {
            bindings.Delete(bindings.Retrieve(new SifObjectBinding
            {
                RefId = objectId
            }));

            Log.Info("Unbound object " + objectId + " from its session token");
        }

        /// <summary>
        /// <see cref="IFunctionalService.GetBinding(Guid)"/>
        /// </summary>
        public virtual string GetBinding(Guid objectId)
        {
            ICollection<SifObjectBinding> candidates = bindings.Retrieve(new SifObjectBinding
            {
                RefId = objectId
            });

            if (candidates == null || candidates.Count == 0)
            {
                Log.Debug("Could not find any bindings for the object " + objectId + ".");

                return null;
            }

            string sessionToken = candidates.FirstOrDefault()?.OwnerId;
            Log.Info("Binding for object " + objectId + " is session token " + sessionToken + ".");

            return sessionToken;
        }

        /// <summary>
        /// <see cref="IFunctionalService.IsBound(Guid, string)"/>
        /// </summary>
        public virtual bool IsBound(Guid objectId, string ownerId)
        {
            ICollection<SifObjectBinding> candidates = bindings.Retrieve(new SifObjectBinding
            {
                OwnerId = ownerId,
                RefId = objectId
            });

            bool bound = candidates != null && candidates.Count > 0;

            if (bound)
            {
                Log.Info("Found binding for object " + objectId + " with session token " + ownerId + ".");
            }
            else
            {
                Log.Debug("Could not find binding for object " + objectId + " with session token " + ownerId + ".");
            }

            return bound;
        }

        /// <see cref="ISifService{UI, DB}.Create(UI, string, string)"/>
        public override Guid Create(jobType item, string zoneId = null, string contextId = null)
        {
            Job job = MapperFactory.CreateInstance<jobType, Job>(item);

            if (StringUtils.NotEmpty(job.Id) && Repository.Exists(job.Id))
            {
                throw new CreateException("Create failed, an object with the id " + job.Id + "already exists.");
            }

            CheckJob(job);
            Configure(job);

            return Repository.Save(job);
        }

        /// <see cref="ISifService{UI, DB}.Create(IEnumerable{UI}, string, string)"/>
        public override void Create(IEnumerable<jobType> items, string zoneId = null, string contextId = null)
        {
            ICollection<Job> jobs = MapperFactory.CreateInstances<jobType, Job>(items);

            foreach (Job job in jobs)
            {
                if (StringUtils.NotEmpty(job.Id) && Repository.Exists(job.Id))
                {
                    throw new CreateException("Create failed, an object with the id " + job.Id + "already exists.");
                }

                CheckJob(job);
                Configure(job);
            }

            Repository.Save(jobs);
        }

        /// <see cref="ISifService{UI, DB}.Update(UI, string, string)"/>
        public override void Update(jobType item, string zoneId = null, string contextId = null)
        {
            throw new RejectedException();
        }

        /// <see cref="ISifService{UI, DB}.Update(IEnumerable{UI}, string, string)"/>
        public override void Update(IEnumerable<jobType> items, string zoneId = null, string contextId = null)
        {
            throw new RejectedException();
        }

        /// <see cref="ISifService{UI, DB}.Retrieve(Guid, string, string)"/>
        public override jobType Retrieve(Guid id, string zoneId = null, string contextId = null)
        {
            Job job = Repository.Retrieve(id);
            AcceptJob(job);

            return MapperFactory.CreateInstance<Job, jobType>(job);
        }

        /// <see cref="ISifService{UI, DB}.Retrieve(UI, string, string)"/>
        public override ICollection<jobType> Retrieve(jobType item, string zoneId = null, string contextId = null)
        {
            Job repoItem = MapperFactory.CreateInstance<jobType, Job>(item);
            IList<Job> repoItems = (from Job job in Repository.Retrieve(repoItem)
                                    where AcceptJob(job)
                                    select job).ToList();

            return MapperFactory.CreateInstances<Job, jobType>(repoItems);
        }

        /// <see cref="ISifService{UI, DB}.Retrieve(string, string)"/>
        public override ICollection<jobType> Retrieve(string zoneId = null, string contextId = null)
        {
            IList<Job> repoItems = (from Job job in Repository.Retrieve()
                                    where AcceptJob(job)
                                    select job).ToList();

            return MapperFactory.CreateInstances<Job, jobType>(repoItems);
        }

        /// <see cref="ISifService{UI, DB}.Delete(Guid, string, string)"/>
        public override void Delete(Guid id, string zoneId = null, string contextId = null)
        {
            try
            {
                Job job = Repository.Retrieve(id);
                CheckJob(job);
                JobShutdown(job);
                Repository.Delete(id);
            }
            catch (Exception e)
            {
                throw new DeleteException("Unable to delete job with ID " + id + " due to: " + e.Message, e);
            }
        }

        /// <see cref="ISifService{UI, DB}.Delete(UI, string, string)"/>
        public override void Delete(jobType item, string zoneId = null, string contextId = null)
        {
            try
            {
                Job job = Repository.Retrieve(Guid.Parse(item.id));
                CheckJob(job);
                JobShutdown(job);
                Repository.Delete(job);
            }
            catch (Exception e)
            {
                throw new DeleteException("Unable to delete job with ID " + item.id, e);
            }
        }

        /// <see cref="ISifService{UI, DB}.Delete(IEnumerable{UI}, string, string)"/>
        public override void Delete(IEnumerable<jobType> items, string zoneId = null, string contextId = null)
        {
            ICollection<Job> jobs = MapperFactory.CreateInstances<jobType, Job>(items);

            foreach (Job job in jobs)
            {
                try
                {
                    CheckJob(job);
                    JobShutdown(job);
                }
                catch (Exception e)
                {
                    throw new DeleteException("Unable to shutdown job with ID " + job.Id + ", delete failed", e);
                }
            }

            try
            {
                Repository.Delete(jobs);
            }
            catch (Exception e)
            {
                throw new DeleteException("Unable to delete jobs due to: " + e.Message, e);
            }
        }

        /// <see cref="IFunctionalService.CreateToPhase(Guid, string, string, string, string, string, string)"/>
        public virtual string CreateToPhase(
            Guid id,
            string phaseName,
            string body = null,
            string zoneId = null,
            string contextId = null,
            string contentType = null,
            string accept = null)
        {
            Job job = Repository.Retrieve(id);
            Phase phase = GetPhase(job, phaseName);
            RightsUtils.CheckRight(phase.Rights, new Right(RightType.CREATE, RightValue.APPROVED));

            IPhaseActions action = GetActions(phaseName);
            string result = action.Create(job, phase, body, contentType, accept);
            Repository.Save(job);

            return result;
        }

        /// <see cref="IFunctionalService.RetrieveToPhase(Guid, string, string, string, string, string, string)"/>
        public virtual string RetrieveToPhase(
            Guid id,
            string phaseName,
            string body = null,
            string zoneId = null,
            string contextId = null,
            string contentType = null,
            string accept = null)
        {
            Job job = Repository.Retrieve(id);
            Phase phase = GetPhase(job, phaseName);
            RightsUtils.CheckRight(phase.Rights, new Right(RightType.QUERY, RightValue.APPROVED));

            IPhaseActions action = GetActions(phaseName);
            string result = action.Retrieve(job, phase, body, contentType, accept);
            Repository.Save(job);

            return result;
        }

        /// <see cref="IFunctionalService.UpdateToPhase(Guid, string, string, string, string, string, string)"/>
        public virtual string UpdateToPhase(
            Guid id,
            string phaseName,
            string body = null,
            string zoneId = null,
            string contextId = null,
            string contentType = null,
            string accept = null)
        {
            Job job = Repository.Retrieve(id);
            Phase phase = GetPhase(job, phaseName);
            RightsUtils.CheckRight(phase.Rights, new Right(RightType.UPDATE, RightValue.APPROVED));

            IPhaseActions action = GetActions(phaseName);
            string result = action.Update(job, phase, body, contentType, accept);
            Repository.Save(job);

            return result;
        }

        /// <see cref="IFunctionalService.DeleteToPhase(Guid, string, string, string, string, string, string)"/>
        public virtual string DeleteToPhase(
            Guid id,
            string phaseName,
            string body = null,
            string zoneId = null,
            string contextId = null,
            string contentType = null,
            string accept = null)
        {
            Job job = Repository.Retrieve(id);
            Phase phase = GetPhase(job, phaseName);
            RightsUtils.CheckRight(phase.Rights, new Right(RightType.DELETE, RightValue.APPROVED));

            IPhaseActions action = GetActions(phaseName);
            string result = action.Delete(job, phase, body, contentType, accept);
            Repository.Save(job);

            return result;
        }

        /// <see cref="IFunctionalService.CreateToState(Guid, string, stateType, string, string)"/>
        public virtual stateType CreateToState(
            Guid id,
            string phaseName,
            stateType item = null,
            string zoneId = null,
            string contextId = null)
        {
            Job job = Repository.Retrieve(id);
            Phase phase = GetPhase(job, phaseName);
            RightsUtils.CheckRight(phase.StatesRights, new Right(RightType.CREATE, RightValue.APPROVED));

            PhaseState state = MapperFactory.CreateInstance<stateType, PhaseState>(item);

            job.UpdatePhaseState(phaseName, state.Type, state.Description);
            Repository.Save(job);

            return MapperFactory.CreateInstance<PhaseState, stateType>(phase.GetCurrentState());
        }

        /// <summary>
        /// Override this method to perform actions when a job of this type is deleted. Should throw a exception if it
        /// cannot be deleted.
        /// </summary>
        /// <param name="job">The job instance to configure</param>
        protected virtual void JobShutdown(Job job)
        {
        }

        /// <see cref="IFunctionalService.AcceptJob(Job)"/>
        public virtual bool AcceptJob(Job job)
        {
            return AcceptJob(job.Name);
        }

        /// <see cref="IFunctionalService.AcceptJob(string)"/>
        public virtual bool AcceptJob(string jobName)
        {
            return AcceptJob(GetServiceName(), jobName);
        }

        /// <see cref="IFunctionalService.AcceptJob(string, string)"/>
        public virtual bool AcceptJob(string serviceName, string jobName)
        {
            if (StringUtils.IsEmpty(GetServiceName()) ||
                StringUtils.IsEmpty(serviceName) ||
                StringUtils.IsEmpty(jobName))
            {
                return false;
            }

            return GetServiceName().Equals(serviceName) && GetServiceName().Equals(jobName + "s");
        }

        /// <see cref="IFunctionalService.AcceptJob()"/>
        public virtual string AcceptJob()
        {
            return GetServiceName().Substring(0, GetServiceName().Length - 1);
        }

        private void CheckJob(Job job)
        {
            if (job == null)
            {
                throw new ArgumentException("Job cannot be null.");
            }

            if (StringUtils.IsEmpty(job.Name))
            {
                throw new ArgumentException("Unsupported operation, job name not supplied.");
            }

            if (!AcceptJob(job))
            {
                throw new ArgumentException("Unsupported job name '" + job.Name + "', expected " + AcceptJob() + ".");
            }
        }

        /// <see cref="IFunctionalService.ExtendJobTimeout(Job, TimeSpan)"/>
        public void ExtendJobTimeout(Job job, TimeSpan duration)
        {
            job.Timeout = job.Timeout.Add(duration);
            Repository.Save(job);
        }

        /// <see cref="IFunctionalService.JobTimeout()"/>
        public virtual void JobTimeout()
        {
            Log.Info("++++++++++++++++++++++++++++++ JobTimeout() called for service " + GetServiceName());

            IList<Job> jobs = (from Job job in Repository.Retrieve()
                               where AcceptJob(job)
                               && job.Timeout.TotalSeconds != 0
                               && DateTime.UtcNow.CompareTo((job.Created ?? DateTime.UtcNow).Add(job.Timeout)) > 0
                               select job).ToList();

            /*
            foreach(Job job in repository.Retrieve()) {
                log.Debug("Id: " + job.Id);
                log.Debug("Name: " + job.Name + "s | " + serviceName);
                log.Debug("Timeout: " + job.Timeout.ToString(@"dd\.hh\:mm\:ss"));
                log.Debug("Created: " + (job.Created ?? DateTime.UtcNow).ToString(@"dd\/MM\/yyyy HH:mm"));
                DateTime calculated = (job.Created ?? DateTime.UtcNow).Add(job.Timeout);
                log.Debug("Timeout calculated: " + calculated.ToString(@"dd\/MM\/yyyy HH:mm"));
                log.Debug("Current time: " + DateTime.UtcNow);
                log.Debug("Span Comparison: " + DateTime.UtcNow.CompareTo(calculated) + " | 0");
            }
            */

            if (jobs.Count == 0)
            {
                Log.Info("No jobs have timed out.");
            }
            else
            {
                var count = 0;

                foreach (Job job in jobs)
                {
                    Log.Info("Job " + job.Id + " has timed out, requesting its deletion.");

                    try
                    {
                        JobShutdown(job);
                    }
                    catch (Exception e)
                    {
                        Log.Debug($"Job {job.Id} has timed out, but could not be shut down. Will try again later.", e);

                        continue;
                    }

                    Unbind(job.Id);
                    Repository.Delete(job.Id);
                    count += 1;
                }

                Log.Info("Successfully timed out " + count + " / " + jobs.Count + " jobs.");
            }

            Log.Info("++++++++++++++++++++++++++++++ Finished JobTimeout() for service " + GetServiceName());
        }

        /// <summary>
        /// Internal method to get a named phase from a job, throwing an appropriate exception if not found
        /// </summary>
        private Phase GetPhase(Job job, string phaseName)
        {
            CheckJob(job);
            Phase phase = job.Phases[phaseName];

            if (phase == null)
            {
                throw new ArgumentException("Unknown phase name");
            }

            return phase;
        }

        /// <summary>
        /// Internal method to get the PhaseAction object for a named phase, throwing an appropriate exception if none are found.
        /// </summary>
        private IPhaseActions GetActions(string phaseName)
        {
            IPhaseActions actions = PhaseActions[phaseName];

            if (actions == null)
            {
                throw new ArgumentException("Unknown phase actions");
            }

            return actions;
        }
    }
}