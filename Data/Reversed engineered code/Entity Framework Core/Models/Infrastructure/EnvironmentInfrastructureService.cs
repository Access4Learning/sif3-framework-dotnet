namespace Sif.Framework.Models.Infrastructure
{
    public partial class EnvironmentInfrastructureService
    {
        public byte[] EnvironmentId { get; set; } = null!;
        public long InfrastructureServiceId { get; set; }
        public long Name { get; set; }

        public virtual Environment Environment { get; set; } = null!;
        public virtual InfrastructureService InfrastructureService { get; set; } = null!;
    }
}
