using Sif.Framework.WebApi;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Sif.Framework.Demo.Us.Provider
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MessageHandlers.Add(new MethodOverrideHandler());

            config.Services.Replace(typeof(IHttpControllerTypeResolver), new ServiceProviderHttpControllerTypeResolver());

            FieldInfo suffix = typeof(DefaultHttpControllerSelector).GetField("ControllerSuffix", BindingFlags.Static | BindingFlags.Public);
            if (suffix != null) suffix.SetValue(null, "Provider");
        }
    }
}
