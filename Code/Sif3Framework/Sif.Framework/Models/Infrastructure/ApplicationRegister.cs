using System.Collections.Generic;

namespace Sif.Framework.Models.Infrastructure
{
    public partial class ApplicationRegister
    {
        public ApplicationRegister()
        {
            EnvironmentRegisters = new HashSet<EnvironmentRegister>();
        }

        public long ApplicationRegisterId { get; set; }
        public string ApplicationKey { get; set; }
        public string SharedSecret { get; set; }

        public virtual ICollection<EnvironmentRegister> EnvironmentRegisters { get; set; }
    }
}
