using System.Collections.Generic;

namespace Sif.Framework.Models.Infrastructure
{
    public partial class EnvironmentRegister
    {
        public EnvironmentRegister()
        {
            EnvironmentRegisterInfrastructureServices = new HashSet<EnvironmentRegisterInfrastructureService>();
            EnvironmentRegisterProvisionedZones = new HashSet<EnvironmentRegisterProvisionedZone>();
            ApplicationRegisters = new HashSet<ApplicationRegister>();
        }

        public long EnvironmentRegisterId { get; set; }
        public string? ApplicationKey { get; set; }
        public string? InstanceId { get; set; }
        public string? SolutionId { get; set; }
        public string? UserToken { get; set; }
        public long? ZoneId { get; set; }

        public virtual Zone? Zone { get; set; }
        public virtual ICollection<EnvironmentRegisterInfrastructureService> EnvironmentRegisterInfrastructureServices { get; set; }
        public virtual ICollection<EnvironmentRegisterProvisionedZone> EnvironmentRegisterProvisionedZones { get; set; }

        public virtual ICollection<ApplicationRegister> ApplicationRegisters { get; set; }
    }
}
