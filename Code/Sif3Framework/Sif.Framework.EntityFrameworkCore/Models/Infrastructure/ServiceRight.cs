namespace Sif.Framework.Models.Infrastructure
{
    public partial class ServiceRight
    {
        public long ServiceId { get; set; }
        public long RightId { get; set; }
        public string Type { get; set; } = null!;

        public virtual Right1 Right { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
    }
}
