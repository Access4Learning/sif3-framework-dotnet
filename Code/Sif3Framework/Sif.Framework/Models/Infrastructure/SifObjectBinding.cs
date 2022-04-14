namespace Sif.Framework.Models.Infrastructure
{
    public partial class SifObjectBinding
    {
        public long SifObjectBindingId { get; set; }
        public byte[] RefId { get; set; }
        public string OwnerId { get; set; }
    }
}
