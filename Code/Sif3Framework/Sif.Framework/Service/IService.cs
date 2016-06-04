using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Query;
using System.Collections.Generic;

namespace Sif.Framework.Service
{
    public interface IService : ModelLink
    {
        ServiceType getServiceType();

        void Run();

        void Finalise();
    }
}
