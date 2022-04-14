using System.Collections.Generic;

namespace Sif.Framework.Models.Infrastructure
{
    public partial class Service
    {
        public Service()
        {
            ServiceRights = new HashSet<ServiceRight>();
            ProvisionedZones = new HashSet<ProvisionedZone>();
        }

        public long ServiceId { get; set; }
        public string Contextid { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public virtual ICollection<ServiceRight> ServiceRights { get; set; }

        public virtual ICollection<ProvisionedZone> ProvisionedZones { get; set; }
    }
}
