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

using Sif.Framework.Persistence.NHibernate;
using System;
using Job = Sif.Framework.Model.Infrastructure.Job;
using System.Collections.Generic;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Exceptions;
using Sif.Specification.Infrastructure;
using Sif.Framework.Service.Mapper;
using Sif.Framework.Utils;
using log4net;
using System.Reflection;
using System.Linq;

namespace Sif.Framework.Service.Functional
{
    /// <summary>
    /// The abstract Functional Service implementation that a functional services would normally extend.
    /// </summary>
    public abstract class FunctionalService : SifService<jobType, Job>, IFunctionalService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private GenericRepository<SifObjectBinding, long> bindings;

        /// <summary>
        /// The disctionary of phases this service contains, with the actions each can perform
        /// </summary>
        protected IDictionary<string, IPhaseActions> phaseActions;

        /// <summary>
        /// Basic constructor
        /// </summary>
        public FunctionalService() : base(new GenericRepository<Job, Guid>(EnvironmentProviderSessionFactory.Instance))
        {
            phaseActions = new Dictionary<string, IPhaseActions>();
            bindings = new GenericRepository<SifObjectBinding, long>(EnvironmentProviderSessionFactory.Instance);
        }

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
            log.Info("Starting thread for " + GetServiceName() + " service.");
        }

        /// <summary>
        /// <see cref="IFunctionalService.Shutdown"/>
        /// </summary>
        public virtual void Shutdown()
        {
            log.Info("Shutting down thread for " + GetServiceName() + " service.");
        }

        /// <summary>
        /// <see cref="IFunctionalService.Bind(Guid, string)"/>
        /// </summary>
        public virtual void Bind(Guid objectId, string ownerId)
        {
            long id = bindings.Save(new SifObjectBinding()
            {
                OwnerId = ownerId,
                RefId = objectId
            });
            log.Info("Bound object " + objectId + " with session token " + ownerId + ". ID = " + id);
        }

        /// <summary>
        /// <see cref="IFunctionalService.Unbind(string)"/>
        /// </summary>
        public virtual void Unbind(string ownerId)
        {
            bindings.Delete(bindings.Retrieve(new SifObjectBinding()
            {
                OwnerId = ownerId
            }));
            log.Info("Unbound all objects from the session token " + ownerId);
        }

        /// <summary>
        /// <see cref="IFunctionalService.Unbind(Guid)"/>
        /// </summary>
        public virtual void Unbind(Guid objectId)
        {
            bindings.Delete(bindings.Retrieve(new SifObjectBinding()
            {
                RefId = objectId
            }));
            log.Info("Unbound object " + objectId + " from its session token");
        }

        /// <summary>
        /// <see cref="IFunctionalService.GetBinding(Guid)"/>
        /// </summary>
        public virtual string GetBinding(Guid objectId)
        {
            ICollection<SifObjectBinding> candidates = bindings.Retrieve(new SifObjectBinding()
            {
                RefId = objectId
            });
            if (candidates == null || candidates.Count == 0)
            {
                log.Debug("Could not find any bindings for the object " + objectId + ".");
                return null;
            }
            string sessionToken = candidates.FirstOrDefault().OwnerId;
            log.Info("Binding for object " + objectId + " is session token " + sessionToken + ".");
            return sessionToken;
        }

        /// <summary>
        /// <see cref="IFunctionalService.IsBound(Guid, string)"/>
        /// </summary>
        public virtual Boolean IsBound(Guid objectId, string ownerId)
        {
            ICollection<SifObjectBinding> candidates = bindings.Retrieve(new SifObjectBinding()
            {
                OwnerId = ownerId,
                RefId = objectId
            });
            Boolean bound = candidates != null && candidates.Count > 0;
            if (bound)
            {
                log.Info("Found binding for object " + objectId + " with session token " + ownerId + ".");
            }
            else
            {
                log.Debug("Cound not find binding for object " + objectId + " with session token " + ownerId + ".");
            }
            return bound;
        }

        /// <see cref="ISifService{UI, DB}.Create(UI, string, string)"/>
        public override Guid Create(jobType item, string zone = null, string context = null)
        {
            Job job = MapperFactory.CreateInstance<jobType, Job>(item);

            if (StringUtils.NotEmpty(job.Id) && repository.Exists(job.Id))
            {
                throw new CreateException("Create failed, an object with the id " + job.Id + "already exists.");
            }

            checkJob(job);
            Configure(job);
            return repository.Save(job);
        }

        /// <see cref="ISifService{UI, DB}.Create(IEnumerable{UI}, string, string)"/>
        public override void Create(IEnumerable<jobType> items, string zone = null, string context = null)
        {
            ICollection<Job> jobs = MapperFactory.CreateInstances<jobType, Job>(items);
            foreach (Job job in jobs)
            {
                if (StringUtils.NotEmpty(job.Id) && repository.Exists(job.Id))
                {
                    throw new CreateException("Create failed, an object with the id " + job.Id + "already exists.");
                }

                checkJob(job);
                Configure(job);
            }
            repository.Save(jobs);
        }

        /// <see cref="ISifService{UI, DB}.Update(UI, string, string)"/>
        public override void Update(jobType item, string zone = null, string context = null)
        {
            throw new RejectedException();
        }

        /// <see cref="ISifService{UI, DB}.Update(IEnumerable{UI}, string, string)"/>
        public override void Update(IEnumerable<jobType> items, string zone = null, string context = null)
        {
            throw new RejectedException();
        }

        /// <see cref="ISifService{UI, DB}.Retrieve(Guid, string, string)"/>
        public override jobType Retrieve(Guid id, string zone = null, string context = null)
        {
            Job job = repository.Retrieve(id);
            AcceptJob(job);
            return MapperFactory.CreateInstance<Job, jobType>(job);
        }

        /// <see cref="ISifService{UI, DB}.Retrieve(UI, string, string)"/>
        public override ICollection<jobType> Retrieve(jobType item, string zone = null, string context = null)
        {
            Job repoItem = MapperFactory.CreateInstance<jobType, Job>(item);
            IList<Job> repoItems = (from Job job in repository.Retrieve(repoItem)
                                    where AcceptJob(job)
                                    select job).ToList();
            return MapperFactory.CreateInstances<Job, jobType>(repoItems);
        }

        /// <see cref="ISifService{UI, DB}.Retrieve(string, string)"/>
        public override ICollection<jobType> Retrieve(string zone = null, string context = null)
        {
            IList<Job> repoItems = (from Job job in repository.Retrieve()
                                    where AcceptJob(job)
                                    select job).ToList();
            return MapperFactory.CreateInstances<Job, jobType>(repoItems);
        }

        /// <see cref="ISifService{UI, DB}.Delete(Guid, string, string)"/>
        public override void Delete(Guid id, string zone = null, string context = null)
        {
            try
            {
                Job job = repository.Retrieve(id);
                checkJob(job);
                JobShutdown(job);
                repository.Delete(id);
            }
            catch (Exception e)
            {
                throw new DeleteException("Unable to delete job with ID " + id + " due to: " + e.Message, e);
            }
        }

        /// <see cref="ISifService{UI, DB}.Delete(UI, string, string)"/>
        public override void Delete(jobType item, string zone = null, string context = null)
        {
            try
            {
                Job job = repository.Retrieve(Guid.Parse(item.id));
                checkJob(job);
                JobShutdown(job);
                repository.Delete(job);
            }
            catch (Exception e)
            {
                throw new DeleteException("Unable to delete job with ID " + item.id, e);
            }
        }

        /// <see cref="ISifService{UI, DB}.Delete(IEnumerable{UI}, string, string)"/>
        public override void Delete(IEnumerable<jobType> items, string zone = null, string context = null)
        {
            ICollection<Job> jobs = MapperFactory.CreateInstances<jobType, Job>(items);
            foreach (Job job in jobs)
            {
                try
                {
                    checkJob(job);
                    JobShutdown(job);
                }
                catch (Exception e)
                {
                    throw new DeleteException("Unable to shutdown job with ID " + job.Id + ", delete failed", e);
                }
            }
            try
            {
                repository.Delete(jobs);
            }
            catch (Exception e)
            {
                throw new DeleteException("Unable to delete jobs due to: " + e.Message, e);
            }
        }

        /// <see cref="IFunctionalService.CreateToPhase(Guid, string, string, string, string, string, string)"/>
        public virtual string CreateToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null)
        {
            Job job = repository.Retrieve(id);
            Phase phase = getPhase(job, phaseName);
            RightsUtils.CheckRight(phase.Rights, new Right(RightType.CREATE, RightValue.APPROVED));

            IPhaseActions action = getActions(phaseName);
            string result = action.Create(job, phase, body, contentType, accept);
            repository.Save(job);
            return result;
        }

        /// <see cref="IFunctionalService.RetrieveToPhase(Guid, string, string, string, string, string, string)"/>
        public virtual string RetrieveToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null)
        {
            Job job = repository.Retrieve(id);
            Phase phase = getPhase(job, phaseName);
            RightsUtils.CheckRight(phase.Rights, new Right(RightType.QUERY, RightValue.APPROVED));

            IPhaseActions action = getActions(phaseName);
            string result = action.Retrieve(job, phase, body, contentType, accept);
            repository.Save(job);
            return result;
        }

        /// <see cref="IFunctionalService.UpdateToPhase(Guid, string, string, string, string, string, string)"/>
        public virtual string UpdateToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null)
        {
            Job job = repository.Retrieve(id);
            Phase phase = getPhase(job, phaseName);
            RightsUtils.CheckRight(phase.Rights, new Right(RightType.UPDATE, RightValue.APPROVED));

            IPhaseActions action = getActions(phaseName);
            string result = action.Update(job, phase, body, contentType, accept);
            repository.Save(job);
            return result;
        }

        /// <see cref="IFunctionalService.DeleteToPhase(Guid, string, string, string, string, string, string)"/>
        public virtual string DeleteToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null)
        {
            Job job = repository.Retrieve(id);
            Phase phase = getPhase(job, phaseName);
            RightsUtils.CheckRight(phase.Rights, new Right(RightType.DELETE, RightValue.APPROVED));

            IPhaseActions action = getActions(phaseName);
            string result = action.Delete(job, phase, body, contentType, accept);
            repository.Save(job);
            return result;
        }

        /// <see cref="IFunctionalService.CreateToState(Guid, string, stateType, string, string)"/>
        public virtual stateType CreateToState(Guid id, string phaseName, stateType item = null, string zone = null, string context = null)
        {
            Job job = repository.Retrieve(id);
            Phase phase = getPhase(job, phaseName);
            RightsUtils.CheckRight(phase.StatesRights, new Right(RightType.CREATE, RightValue.APPROVED));

            PhaseState state = MapperFactory.CreateInstance<stateType, PhaseState>(item);

            job.UpdatePhaseState(phaseName, state.Type, state.Description);
            repository.Save(job);

            return MapperFactory.CreateInstance<PhaseState, stateType>(phase.GetCurrentState());
        }

        /// <summary>
        /// Override this method to perform actions when a job of this type is deleted. Should throw a exception if it cannot be deleted.
        /// </summary>
        /// <param name="job">The job instance to configure</param>
        protected virtual void JobShutdown(Job job)
        {
        }

        /// <see cref="IFunctionalService.AcceptJob(Job)"/>
        public virtual Boolean AcceptJob(Job job)
        {
            return AcceptJob(job.Name);
        }

        /// <see cref="IFunctionalService.AcceptJob(string)"/>
        public virtual Boolean AcceptJob(string jobName)
        {
            return AcceptJob(GetServiceName(), jobName);
        }

        /// <see cref="IFunctionalService.AcceptJob(string, string)"/>
        public virtual Boolean AcceptJob(string serviceName, string jobName)
        {
            if (StringUtils.IsEmpty(GetServiceName()) || StringUtils.IsEmpty(serviceName) || StringUtils.IsEmpty(jobName))
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

        private void checkJob(Job job)
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
            repository.Save(job);
        }

        /// <see cref="IFunctionalService.JobTimeout()"/>
        public virtual void JobTimeout()
        {
            log.Info("++++++++++++++++++++++++++++++ JobTimeout() called for service " + GetServiceName());
            IList<Job> jobs = (from Job job in repository.Retrieve()
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
                log.Info("No jobs have timed out.");
            } else
            {
                int count = 0;

                foreach (Job job in jobs)
                {
                    log.Info("Job " + job.Id + " has timed out, requesting its deletion.");
                    try
                    {
                        JobShutdown(job);
                    }
                    catch (Exception e)
                    {
                        log.Debug("Job " + job.Id + " has timed out, but could not be shut down. Will try again later.", e);
                        continue;
                    }
                    Unbind(job.Id);
                    repository.Delete(job.Id);
                    count += 1;
                }

                log.Info("Successfully timed out " + count + " / " + jobs.Count + " jobs.");
            }

            log.Info("++++++++++++++++++++++++++++++ Finished JobTimeout() for service " + GetServiceName());
        }

        /// <summary>
        /// Internal method to get a named phase from a job, throwing an appropriate exception if not found
        /// </summary>
        private Phase getPhase(Job job, string phaseName)
        {
            checkJob(job);
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
        private IPhaseActions getActions(string phaseName)
        {
            IPhaseActions actions = phaseActions[phaseName];
            if (actions == null)
            {
                throw new ArgumentException("Unknown phase actions");
            }
            return actions;
        }
    }
}
