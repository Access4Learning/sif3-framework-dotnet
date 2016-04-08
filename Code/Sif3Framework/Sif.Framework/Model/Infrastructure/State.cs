using Sif.Framework.Model.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sif.Framework.Model.Infrastructure
{
    public class State : IPersistable<long>
    {
        public virtual long Id { get; set; }

        public virtual PhaseStateType Type { get; set; }

        public virtual DateTime? Created { get; set; }

        public virtual DateTime? LastModified { get; set; }

        public virtual string Description { get; set; }

        public State() {
            Type = PhaseStateType.NOTSTARTED;
            Created = DateTime.UtcNow;
            LastModified = Created;
        }

        public State(PhaseStateType type, string description = null) : this()
        {
            Type = type;
            Description = description;
        }
    }
}
