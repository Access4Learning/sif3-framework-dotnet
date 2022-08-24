﻿using Sif.Framework.Models.Exceptions;
using Sif.Framework.Models.Infrastructure;

namespace Sif.Framework.Services.Functional
{
    /// <summary>
    /// <see cref="IPhaseActions"/>
    /// </summary>
    public class PhaseActions : IPhaseActions
    {
        /// <summary>
        /// <see cref="IPhaseActions.Create(Job, Phase, string, string, string)"/>
        /// </summary>
        public virtual string Create(Job job, Phase phase, string body = null, string contentType = null, string accept = null)
        {
            throw new RejectedException("Action not supported");
        }

        /// <summary>
        /// <see cref="IPhaseActions.Retrieve(Job, Phase, string, string, string)"/>
        /// </summary>
        public virtual string Retrieve(Job job, Phase phase, string body = null, string contentType = null, string accept = null)
        {
            throw new RejectedException("Action not supported");
        }

        /// <summary>
        /// <see cref="IPhaseActions.Update(Job, Phase, string, string, string)"/>
        /// </summary>
        public virtual string Update(Job job, Phase phase, string body = null, string contentType = null, string accept = null)
        {
            throw new RejectedException("Action not supported");
        }

        /// <summary>
        /// <see cref="IPhaseActions.Delete(Job, Phase, string, string, string)"/>
        /// </summary>
        public virtual string Delete(Job job, Phase phase, string body = null, string contentType = null, string accept = null)
        {
            throw new RejectedException("Action not supported");
        }
    }
}