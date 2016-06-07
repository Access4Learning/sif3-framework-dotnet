using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sif.Framework.Model;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Query;

namespace Sif.Framework.Service.Providers
{
    public abstract class BasicProviderService<T> : IBasicProviderService<T>
    {
        public abstract string getServiceName();

        public ServiceType getServiceType()
        {
            return ServiceType.OBJECT;
        }

        public abstract void Run();

        public abstract void Finalise();

        public abstract T Create(T obj, bool? mustUseAdvisory = default(bool?), string zone = null, string context = null);

        public abstract T Retrieve(string refId, string zone = null, string context = null);

        public abstract List<T> Retrieve(uint? pageIndex = default(uint?), uint? pageSize = default(uint?), string zone = null, string context = null);

        public abstract List<T> Retrieve(IEnumerable<EqualCondition> conditions, uint? pageIndex = default(uint?), uint? pageSize = default(uint?), string zone = null, string context = null);

        public abstract List<T> Retrieve(T obj, uint? pageIndex = default(uint?), uint? pageSize = default(uint?), string zone = null, string context = null);

        public abstract void Update(T obj, string zone = null, string context = null);

        public abstract void Delete(string refId, string zone = null, string context = null);
    }
}
