using System.Collections.Generic;

namespace Sif.Framework.Models.Infrastructure
{
    public partial class ProvisionedZone
    {
        public ProvisionedZone()
        {
            EnvironmentProvisionedZones = new HashSet<EnvironmentProvisionedZone>();
            EnvironmentRegisterProvisionedZones = new HashSet<EnvironmentRegisterProvisionedZone>();
            Services = new HashSet<Service>();
        }

        public long ProvisionedZoneId { get; set; }
        public string SifId { get; set; }

        public virtual ICollection<EnvironmentProvisionedZone> EnvironmentProvisionedZones { get; set; }
        public virtual ICollection<EnvironmentRegisterProvisionedZone> EnvironmentRegisterProvisionedZones { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}
