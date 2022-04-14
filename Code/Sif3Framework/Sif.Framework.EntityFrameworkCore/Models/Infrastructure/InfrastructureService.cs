using System.Collections.Generic;

namespace Sif.Framework.Models.Infrastructure
{
    public partial class InfrastructureService
    {
        public InfrastructureService()
        {
            EnvironmentInfrastructureServices = new HashSet<EnvironmentInfrastructureService>();
            EnvironmentRegisterInfrastructureServices = new HashSet<EnvironmentRegisterInfrastructureService>();
        }

        public long InfrastructureServiceId { get; set; }
        public long? Name { get; set; }
        public string? Value { get; set; }

        public virtual ICollection<EnvironmentInfrastructureService> EnvironmentInfrastructureServices { get; set; }
        public virtual ICollection<EnvironmentRegisterInfrastructureService> EnvironmentRegisterInfrastructureServices { get; set; }
    }
}
