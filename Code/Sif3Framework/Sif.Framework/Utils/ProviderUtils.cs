using log4net;
using Sif.Framework.Controllers;
using Sif.Framework.Extensions;
using Sif.Framework.Providers;
using Sif.Framework.Service;
using Sif.Framework.Service.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Sif.Framework.Utils
{
    class ProviderUtils
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static Boolean isFunctionalService(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("Argument type cannot be null");
            }

            Boolean isFService = type.IsClass &&
                type.IsVisible &&
                !type.IsAbstract &&
                typeof(IFunctionalService).IsAssignableFrom(type);
            /*
            if(isFService)
            {
                log.Debug("Found functional service: " + type.Name);
            }
            */
            return isFService;
        }

        public static Boolean isDataService(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("Argument type cannot be null");
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
                throw new ArgumentNullException("Argument type cannot be null");
            }

            Boolean iscontroller = type.IsClass &&
                type.IsVisible &&
                !type.IsAbstract &&
                (type.IsAssignableToGenericType(typeof(IProvider<,,>)) ||
                type.IsAssignableToGenericType(typeof(SifController<,>)) ||
                typeof(FunctionalServiceProvider).IsAssignableFrom(type)) &&
                typeof(IHttpController).IsAssignableFrom(type) &&
                type.Name.EndsWith("Provider");
            /*
            if (type.Name.Contains("FunctionalServiceProvider"))
            {
                log.Debug(type.Name + " (" + iscontroller + ")");
                log.Debug("isClass: " + type.IsClass);
                log.Debug("isVisible: " + type.IsVisible);
                log.Debug("isNotAbstract: " + !type.IsAbstract);
                log.Debug("isAssignable(IProvider): " + type.IsAssignableToGenericType(typeof(IProvider<,,>)));
                log.Debug("isAssignable(SifController): " + type.IsAssignableToGenericType(typeof(SifController<,>)));
                log.Debug("isAssignable(FSProvider): " + typeof(FunctionalServiceProvider).IsAssignableFrom(type));
                log.Debug("isAssignable: " + (type.IsAssignableToGenericType(typeof(IProvider<,,>)) ||
                type.IsAssignableToGenericType(typeof(SifController<,>)) ||
                typeof(FunctionalServiceProvider).IsAssignableFrom(type)));
                log.Debug("Is an IHttpController: " + typeof(IHttpController).IsAssignableFrom(type));
                log.Debug("Name ends with 'Provider': " + type.Name.EndsWith("Provider"));

            }
            */
            return iscontroller;
        }
    }
}
