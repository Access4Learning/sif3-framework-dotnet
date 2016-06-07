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

namespace Sif.Framework.Service.Functional
{
    public abstract class BasicFunctionalService : SifService<jobType, Job>, IFunctionalService 
    {
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

        /// <summary>
        /// Method that must be extended to add phases to a given job when it has been created.
        /// </summary>
        protected abstract void addPhases(Job job);
        
        public override Guid Create(jobType item, string zone = null, string context = null)
        {
            Job job = MapperFactory.CreateInstance<jobType, Job>(item);
            checkJob(job);
            addPhases(job);
            return repository.Save(job);
        }
        
        public override void Create(IEnumerable<jobType> items, string zone = null, string context = null)
        {
            ICollection<Job> jobs = MapperFactory.CreateInstances<jobType, Job>(items);
            foreach (Job job in jobs)
            {
                checkJob(job);
                addPhases(job);
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
                JobShutdown(MapperFactory.CreateInstance<jobType, Job>(Retrieve(id, zone, context)));
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
            Right right = phase.Rights[RightType.CREATE.ToString()];
            if (right == null || right.Value.Equals(RightValue.REJECTED.ToString()))
            {
                string msg = "Insufficient rights for this operation";
                //job.updatePhaseState(phaseName, PhaseStateType.FAILED, msg);
                //repository.Save(job);
                throw new RejectedException(msg);
            }
            IPhaseActions action = getActions(phaseName);
            string result = action.Create(job, phase, body, contentType, accept);
            repository.Save(job);
            return result;
        }

        public virtual string RetrieveToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null)
        {
            Job job = MapperFactory.CreateInstance<jobType, Job>(Retrieve(id, zone, context));
            Phase phase = getPhase(job, phaseName);
            Right right = phase.Rights[RightType.QUERY.ToString()];
            if (right == null || right.Value.Equals(RightValue.REJECTED.ToString()))
            {
                string msg = "Insufficient rights for this operation";
                //job.updatePhaseState(phaseName, PhaseStateType.FAILED, msg);
                //repository.Save(job);
                throw new RejectedException(msg);
            }
            IPhaseActions action = getActions(phaseName);
            string result = action.Retrieve(job, phase, body, contentType, accept);
            repository.Save(job);
            return result;
        }

        public virtual string UpdateToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null)
        {
            Job job = MapperFactory.CreateInstance<jobType, Job>(Retrieve(id, zone, context));
            Phase phase = getPhase(job, phaseName);
            Right right = phase.Rights[RightType.UPDATE.ToString()];
            if (right == null || right.Value.Equals(RightValue.REJECTED.ToString()))
            {
                string msg = "Insufficient rights for this operation";
                //job.updatePhaseState(phaseName, PhaseStateType.FAILED, msg);
                //repository.Save(job);
                throw new RejectedException(msg);
            }
            IPhaseActions action = getActions(phaseName);
            string result = action.Update(job, phase, body, contentType, accept);
            repository.Save(job);
            return result;
        }
        
        public virtual string DeleteToPhase(Guid id, string phaseName, string body = null, string zone = null, string context = null, string contentType = null, string accept = null)
        {
            Job job = MapperFactory.CreateInstance<jobType, Job>(Retrieve(id, zone, context));
            Phase phase = getPhase(job, phaseName);
            Right right = phase.Rights[RightType.DELETE.ToString()];
            if (right == null || right.Value.Equals(RightValue.REJECTED.ToString()))
            {
                string msg = "Insufficient rights for this operation";
                //job.updatePhaseState(phaseName, PhaseStateType.FAILED, msg);
                //repository.Save(job);
                throw new RejectedException(msg);
            }
            IPhaseActions action = getActions(phaseName);
            string result = action.Delete(job, phase, body, contentType, accept);
            repository.Save(job);
            return result;
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

            if (!getServiceName().Equals(job.Name))
            {
                throw new ArgumentException("Unsupported job name '" + job.Name + "'.");
            }
		}

        public override void Finalise()
        {
        }

        public override void Run()
        {
        }
    }
}
