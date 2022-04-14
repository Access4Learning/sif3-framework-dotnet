namespace Sif.Framework.EntityFrameworkCore.Tests.Models.Infrastructure
{
    public partial class EnvironmentProvisionedZone
    {
        public byte[] EnvironmentId { get; set; } = null!;
        public long ProvisionedZoneId { get; set; }
        public string SifId { get; set; } = null!;

        public virtual Environment Environment { get; set; } = null!;
        public virtual ProvisionedZone ProvisionedZone { get; set; } = null!;
    }
}
