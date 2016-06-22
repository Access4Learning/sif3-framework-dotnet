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
using Sif.Framework.Model;
using System.Threading;
using Sif.Framework.Model.Settings;
using log4net;
using System.Reflection;
using System.Linq;
using System.Xml;

namespace Sif.Framework.Service.Functional
{
    public abstract class BasicFunctionalService : SifService<jobType, Job>, IFunctionalService 
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Timer timeoutTimer = null;

        public override ServiceType getServiceType()
        {
            return ServiceType.FUNCTIONAL;
        }

        /// <summary>
        /// The disctionary of phases this service contains, with the actions each can perform
        /// </summary>
        protected IDictionary<string, IPhaseActions> phaseActions;

        /// <summary>
        /// Basic constructor
        /// </summary>
        public BasicFunctionalService() : base(new GenericRepository<Job, Guid>(EnvironmentProviderSessionFactory.Instance))
        {
            phaseActions = new Dictionary<string, IPhaseActions>();
        }

        public override void Run()
        {
            base.Run();
            string serviceName = getServiceName();
            ProviderSettings settings = new ProviderSettings();

            // Only if we intend to timeout jobs will we start the job manager
            if (!settings.JobTimeoutEnabled)
            {
                log.Debug("Jobs under the service " + serviceName + " will not timeout.");
                return;
            }

            int frequencyInSec = settings.JobTimeoutFrequency;
            if (frequencyInSec == 0)
            {
                log.Debug("Intending to timeout jobs for " + serviceName + ", but job timeout currently turned off (frequency=0)");
                return;
            }

            int frequency = frequencyInSec * 1000;
            log.Info("Job timeout frequency = " + frequencyInSec + " secs. (" + frequency + ")");

            timeoutTimer = new Timer((o) =>
            {
                log.Debug("++++++++++++++++++++++++++++++ Starting job timout task for " + serviceName + ".");

                IList<Job> jobs = (from job in repository.Retrieve()
                       where (job.Name + "s").Equals(serviceName) &&
                       job.Timeout.TotalSeconds != 0 &&
                       DateTime.UtcNow.CompareTo((job.Created ?? DateTime.UtcNow).Add(job.Timeout)) > 0
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

                foreach (Job job in jobs)
                {
                    log.Debug("Job " + job.Id + " has timed out, requesting its deletion.");
                    Delete(job.Id);
                }

                log.Debug("++++++++++++++++++++++++++++++ Finished job timout task for " + serviceName + ".");
            }, null, 0, frequency);
        }

        public override void Finalise()
        {
            base.Finalise();
            if (timeoutTimer != null)
            {
                log.Debug("Shutdown job timeout timer for: " + getServiceName());
                timeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
                timeoutTimer.Dispose();
                timeoutTimer = null;
            }
        }

        /// <summary>
        /// Method that must be extended to add phases to a given job when it has been created.
        /// </summary>
        protected abstract void configure(Job job);
        
        public override Guid Create(jobType item, string zone = null, string context = null)
        {
            Job job = MapperFactory.CreateInstance<jobType, Job>(item);
            checkJob(job);
            configure(job);
            return repository.Save(job);
        }
        
        public override void Create(IEnumerable<jobType> items, string zone = null, string context = null)
        {
            ICollection<Job> jobs = MapperFactory.CreateInstances<jobType, Job>(items);
            foreach (Job job in jobs)
            {
                checkJob(job);
                configure(job);
            }
            repository.Save(jobs);
        }

        public override void Update(jobType item, string zone = null, string context = null)
        {
            throw new RejectedException();
        }

        public override void Update(IEnumerable<jobType> items, string zone = null, string context = null)
        {
            throw new RejectedException();
        }

        public override void Delete(Guid id, string zone = null, string context = null)
        {
            try
            {
                Job job = MapperFactory.CreateInstance<jobType, Job>(Retrieve(id, zone, context));
                checkJob(job);
                JobShutdown(job);
            }
            catch (Exception e)
            {
                throw new DeleteException("Unable to delete job with ID " + id, e);
            }
            base.Delete(id, zone, context);
        }

        public override void Delete(jobType item, string zone = null, string context = null)
        {
            try
            {
                Job job = MapperFactory.CreateInstance<jobType, Job>(Retrieve(Guid.Parse(item.id), zone, context));
                checkJob(job);
                JobShutdown(job);
            } catch(Exception e)
            {
                throw new DeleteException("Unable to delete job with ID " + item.id, e);
            }
            base.Delete(item, zone, context);
        }

        public override void Delete(IEnumerable<jobType> items, string zone = null, string context = null)
        {
            foreach (jobType item in items)
            {
                try
                {
                    Job job = MapperFactory.CreateInstance<jobType, Job>(Retrieve(Guid.Parse(item.id), zone, context));
                    checkJob(job);
                    JobShutdown(job);
                }
                catch (Exception e)
                {
                    throw new DeleteException("Unable to delete job with ID " + item.id, e);
                }
            }
            base.Delete(items, zone, context);
        }

        public virtual string CreateToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null)
        {
            Job job = MapperFactory.CreateInstance<jobType, Job>(Retrieve(id, zone, context));
            Phase phase = getPhase(job, phaseName);

            checkRight(phase.Rights, RightType.CREATE);

            IPhaseActions action = getActions(phaseName);
            string result = action.Create(job, phase, body, contentType, accept);
            repository.Save(job);
            return result;
        }

        public virtual string RetrieveToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null)
        {
            Job job = MapperFactory.CreateInstance<jobType, Job>(Retrieve(id, zone, context));
            Phase phase = getPhase(job, phaseName);

            checkRight(phase.Rights, RightType.QUERY);

            IPhaseActions action = getActions(phaseName);
            string result = action.Retrieve(job, phase, body, contentType, accept);
            repository.Save(job);
            return result;
        }

        public virtual string UpdateToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null)
        {
            Job job = MapperFactory.CreateInstance<jobType, Job>(Retrieve(id, zone, context));
            Phase phase = getPhase(job, phaseName);

            checkRight(phase.Rights, RightType.UPDATE);

            IPhaseActions action = getActions(phaseName);
            string result = action.Update(job, phase, body, contentType, accept);
            repository.Save(job);
            return result;
        }
        
        public virtual string DeleteToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null)
        {
            Job job = MapperFactory.CreateInstance<jobType, Job>(Retrieve(id, zone, context));
            Phase phase = getPhase(job, phaseName);

            checkRight(phase.Rights, RightType.DELETE);
            
            IPhaseActions action = getActions(phaseName);
            string result = action.Delete(job, phase, body, contentType, accept);
            repository.Save(job);
            return result;
        }

        public virtual stateType CreateToState(Guid id, string phaseName, stateType item = null, string zone = null, string context = null)
        {
            Job job = MapperFactory.CreateInstance<jobType, Job>(Retrieve(id, zone, context));
            State state = MapperFactory.CreateInstance<stateType, State>(item);
            Phase phase = getPhase(job, phaseName);

            checkRight(phase.StatesRights, RightType.CREATE);

            job.updatePhaseState(phaseName, state.Type, state.Description);
            repository.Save(job);

            return MapperFactory.CreateInstance<State, stateType>(phase.getCurrentState());
        }

        /// <summary>
        /// Override this method to perform actions when a job of this type is deleted. Should throw a exception if it cannot be deleted.
        /// </summary>
        protected virtual void JobShutdown(Job job)
        {
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
                throw new ArgumentException("Unknown phase action");
            }
            return actions;
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

            if (!getServiceName().Equals(job.Name + "s"))
            {
                throw new ArgumentException("Unsupported job name '" + job.Name + "', expected " + getServiceName().Substring(0, getServiceName().Length - 1) + ".");
            }
		}

        private void checkRight(IDictionary<string, Right> rights, RightType type)
        {
            if (!rights.ContainsKey(type.ToString()))
            {
                throw new RejectedException("Insufficient rights for this operation, no right for " + type.ToString() + " given in the rights collection");
            }
            Right right = rights[type.ToString()];
            if (right == null || right.Value.Equals(RightValue.REJECTED.ToString()))
            {
                throw new RejectedException("Insufficient rights for this operation");
            }
        }
    }
}
