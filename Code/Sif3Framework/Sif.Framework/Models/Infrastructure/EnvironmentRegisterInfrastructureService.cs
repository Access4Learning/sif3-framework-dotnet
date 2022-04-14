namespace Sif.Framework.Models.Infrastructure
{
    public partial class EnvironmentRegisterInfrastructureService
    {
        public long EnvironmentRegisterId { get; set; }
        public long InfrastructureServiceId { get; set; }
        public long Name { get; set; }

        public virtual EnvironmentRegister EnvironmentRegister { get; set; } = null;
        public virtual InfrastructureService InfrastructureService { get; set; } = null;
    }
}
