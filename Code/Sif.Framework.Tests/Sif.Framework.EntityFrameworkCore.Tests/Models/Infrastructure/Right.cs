namespace Sif.Framework.EntityFrameworkCore.Tests.Models.Infrastructure
{
    public partial class Right
    {
        public long PhaseId { get; set; }
        public long RightId { get; set; }
        public string Type { get; set; } = null!;

        public virtual Right1 RightNavigation { get; set; } = null!;
    }
}
