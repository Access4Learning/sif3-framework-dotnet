using Sif.Framework.Model.DataModels;
using Sif.Framework.Model.Persistence;
using System;

namespace Sif.Framework.Model.Infrastructure
{
    class SifObjectBinding : IPersistable<long>, ISifRefId<Guid>
    {
        public virtual long Id { get; set; }

        public virtual Guid RefId { get; set; }

        public virtual string SessionToken { get; set; }
    }
}
