using Sif.Framework.Controllers;
using Sif.Framework.Extensions;
using Sif.Framework.Providers;
using Sif.Framework.Service;
using Sif.Framework.Service.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Sif.Framework.Utils
{
    class ProviderUtils
    {
        public static Boolean isFunctionalService(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type.IsClass &&
                type.IsVisible &&
                !type.IsAbstract &&
                type.IsAssignableToGenericType(typeof(IFunctionalService));
        }

        public static Boolean isDataService(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type.IsClass &&
                type.IsVisible &&
                !type.IsAbstract &&
                type.IsAssignableToGenericType(typeof(IDataModelService<,,>));
        }

        public static Boolean isController(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type.IsClass &&
                type.IsVisible &&
                !type.IsAbstract &&
                (type.IsAssignableToGenericType(typeof(IProvider<,,>)) ||
                type.IsAssignableToGenericType(typeof(SifController<,>)) ||
                type.IsAssignableToGenericType(typeof(ServiceController))) &&
                typeof(IHttpController).IsAssignableFrom(type) &&
                type.Name.EndsWith("Provider");
        }
    }
}
