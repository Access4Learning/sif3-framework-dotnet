namespace Sif.Framework.Models.Infrastructure
{
    public partial class EnvironmentRegisterProvisionedZone
    {
        public long EnvironmentRegisterId { get; set; }
        public long ProvisionedZoneId { get; set; }
        public string SifId { get; set; } = null!;

        public virtual EnvironmentRegister EnvironmentRegister { get; set; } = null!;
        public virtual ProvisionedZone ProvisionedZone { get; set; } = null!;
    }
}
