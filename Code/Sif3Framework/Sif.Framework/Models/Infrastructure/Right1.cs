using System.Collections.Generic;

namespace Sif.Framework.Models.Infrastructure
{
    public partial class Right1
    {
        public Right1()
        {
            Rights = new HashSet<Right>();
            ServiceRights = new HashSet<ServiceRight>();
        }

        public long RightId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public virtual ICollection<Right> Rights { get; set; }
        public virtual ICollection<ServiceRight> ServiceRights { get; set; }
    }
}
