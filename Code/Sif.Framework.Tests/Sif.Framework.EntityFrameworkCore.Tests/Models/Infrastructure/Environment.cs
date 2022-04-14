using System.Collections.Generic;

namespace Sif.Framework.EntityFrameworkCore.Tests.Models.Infrastructure
{
    public partial class Environment
    {
        public Environment()
        {
            EnvironmentInfrastructureServices = new HashSet<EnvironmentInfrastructureService>();
            EnvironmentProvisionedZones = new HashSet<EnvironmentProvisionedZone>();
        }

        public byte[] EnvironmentId { get; set; } = null!;
        public string? AuthenticationMethod { get; set; }
        public string? ConsumerName { get; set; }
        public string? InstanceId { get; set; }
        public string? SessionToken { get; set; }
        public string? SolutionId { get; set; }
        public long? Type { get; set; }
        public string? UserToken { get; set; }
        public long? ApplicationInfoId { get; set; }
        public long? ZoneId { get; set; }

        public virtual ApplicationInfo? ApplicationInfo { get; set; }
        public virtual Zone? Zone { get; set; }
        public virtual ICollection<EnvironmentInfrastructureService> EnvironmentInfrastructureServices { get; set; }
        public virtual ICollection<EnvironmentProvisionedZone> EnvironmentProvisionedZones { get; set; }
    }
}
