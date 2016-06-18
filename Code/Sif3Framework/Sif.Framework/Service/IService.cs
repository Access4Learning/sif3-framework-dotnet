using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Query;
using System.Collections.Generic;

namespace Sif.Framework.Service
{
    public interface IService
    {
        ServiceType getServiceType();

        /// <summary>
        /// Name of the Object (or Functional Service name) that the Provider is based on
        /// </summary>
        string getServiceName();

        void Run();

        void Finalise();
    }
}
