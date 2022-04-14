using System.Collections.Generic;

namespace Sif.Framework.Models.Infrastructure
{
    public partial class Property
    {
        public Property()
        {
            ZoneProperties = new HashSet<ZoneProperty>();
        }

        public long PropertyId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual ICollection<ZoneProperty> ZoneProperties { get; set; }
    }
}
