using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Query;
using System.Collections.Generic;

namespace Sif.Framework.Service
{
    public interface IService
    {
        ServiceType GetServiceType();

        /// <summary>
        /// Name of the Object (or Functional Service name) that the Provider is based on
        /// </summary>
        string GetServiceName();

        /// <summary>
        /// Method that is run at specified intervals to broadcast events produced by this service.
        /// </summary>
        void BroadcastEvents();
    }
}
