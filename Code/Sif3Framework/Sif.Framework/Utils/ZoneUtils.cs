using Sif.Framework.Model.Infrastructure;
using System.Linq;

namespace Sif.Framework.Utils
{
    static class ZoneUtils
    {
        public static Model.Infrastructure.Service GetService(ProvisionedZone zone, string name, ServiceType type)
        {
            return (from service in zone.Services
                where service.Type.Equals(type.ToString()) && service.Name.Equals(name)
                select service).FirstOrDefault();
        }
    }
}
