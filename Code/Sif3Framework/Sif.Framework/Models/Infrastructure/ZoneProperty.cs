namespace Sif.Framework.Models.Infrastructure
{
    public partial class ZoneProperty
    {
        public long ZoneId { get; set; }
        public long PropertyId { get; set; }
        public string Name { get; set; } = null;

        public virtual Property Property { get; set; } = null;
        public virtual Zone Zone { get; set; } = null;
    }
}
