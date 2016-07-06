using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Exceptions;

namespace Sif.Framework.Service.Functional
{
    public class PhaseActions : IPhaseActions
    {
        public virtual string Create(Job job, Phase phase, string body = null, string contentType = null, string accept = null)
        {
            throw new RejectedException("Action not supported");
        }

        public virtual string Retrieve(Job job, Phase phase, string body = null, string contentType = null, string accept = null)
        {
            throw new RejectedException("Action not supported");
        }

        public virtual string Update(Job job, Phase phase, string body = null, string contentType = null, string accept = null)
        {
            throw new RejectedException("Action not supported");
        }

        public virtual string Delete(Job job, Phase phase, string body = null, string contentType = null, string accept = null)
        {
            throw new RejectedException("Action not supported");
        }
    }
}
