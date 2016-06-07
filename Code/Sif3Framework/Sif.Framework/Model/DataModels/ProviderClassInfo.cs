using Sif.Framework.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sif.Framework.Model.DataModels
{
    public class ProviderClassInfo
    {
        private ConstructorInfo constructor = null;

        public ProviderClassInfo(Type clazz, Type[] paramTypes)
        {
            SetConstructor(clazz.GetConstructor(paramTypes));
        }

        public IService GetClassInstance(Object[] args)
        {
            return GetConstructor().Invoke(args) as IService;
        }

        public ConstructorInfo GetConstructor()
        {
            return constructor;
        }

        public void SetConstructor(ConstructorInfo constructor)
        {
            this.constructor = constructor;
        }
    }
}
