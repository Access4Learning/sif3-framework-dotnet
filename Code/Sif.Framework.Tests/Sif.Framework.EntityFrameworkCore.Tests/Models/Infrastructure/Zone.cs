using System.Collections.Generic;

namespace Sif.Framework.EntityFrameworkCore.Tests.Models.Infrastructure
{
    public partial class Zone
    {
        public Zone()
        {
            EnvironmentRegisters = new HashSet<EnvironmentRegister>();
            Environments = new HashSet<Environment>();
            ZoneProperties = new HashSet<ZoneProperty>();
        }

        public long ZoneId { get; set; }
        public string? SifId { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<EnvironmentRegister> EnvironmentRegisters { get; set; }
        public virtual ICollection<Environment> Environments { get; set; }
        public virtual ICollection<ZoneProperty> ZoneProperties { get; set; }
    }
}
