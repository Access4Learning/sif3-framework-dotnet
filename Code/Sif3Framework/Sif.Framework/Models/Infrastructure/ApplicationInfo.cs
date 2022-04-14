namespace Sif.Framework.Models.Infrastructure
{
    public partial class ApplicationInfo
    {
        public long ApplicationInfoId { get; set; }
        public string ApplicationKey { get; set; }
        public string SupportedInfrastructureVersion { get; set; }
        public string DataModelNamespace { get; set; }
        public string Transport { get; set; }
        public long? ApplicationProductId { get; set; }
        public long? AdapterProductId { get; set; }

        public virtual ProductIdentity AdapterProduct { get; set; }
        public virtual ProductIdentity ApplicationProduct { get; set; }
        public virtual Environment Environment { get; set; }
    }
}
