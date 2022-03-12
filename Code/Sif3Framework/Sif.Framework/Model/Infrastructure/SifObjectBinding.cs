using Sif.Framework.Model.DataModels;
using System;
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.Model.Infrastructure
{
    internal class SifObjectBinding : IHasUniqueIdentifier<long>, ISifRefId<Guid>
    {
        /// <summary>
        /// Internal identifier used by hibernate. Do not use/alter this.
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        /// The ref id of the object to be bound
        /// </summary>
        public virtual Guid RefId { get; set; }

        /// <summary>
        /// The id of the owner of the object (application key, source id, etc.)
        /// </summary>
        public virtual string OwnerId { get; set; }
    }
}