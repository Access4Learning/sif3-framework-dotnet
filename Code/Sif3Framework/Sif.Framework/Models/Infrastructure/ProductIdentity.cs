using System.Collections.Generic;

namespace Sif.Framework.Models.Infrastructure
{
    public partial class ProductIdentity
    {
        public ProductIdentity()
        {
            ApplicationInfoAdapterProducts = new HashSet<ApplicationInfo>();
            ApplicationInfoApplicationProducts = new HashSet<ApplicationInfo>();
        }

        public long ProductIdentityId { get; set; }
        public string VendorName { get; set; }
        public string ProductName { get; set; }
        public string ProductVersion { get; set; }
        public string IconUri { get; set; }

        public virtual ICollection<ApplicationInfo> ApplicationInfoAdapterProducts { get; set; }
        public virtual ICollection<ApplicationInfo> ApplicationInfoApplicationProducts { get; set; }
    }
}
