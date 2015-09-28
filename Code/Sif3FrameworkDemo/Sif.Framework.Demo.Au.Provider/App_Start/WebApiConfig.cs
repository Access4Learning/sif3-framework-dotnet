using Sif.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Sif.Framework.Demo.Au.Provider
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "UriPathExtensionApi",
                routeTemplate: "api/{controller}.{ext}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ServicePathApi3",
                routeTemplate: "api/{object1}/{id1}/{object2}/{id2}/{object3}/{id3}/{controller}"
            );

            config.Routes.MapHttpRoute(
                name: "ServicePathApi2",
                routeTemplate: "api/{object1}/{id1}/{object2}/{id2}/{controller}"
            );

            config.Routes.MapHttpRoute(
                name: "ServicePathApi1",
                routeTemplate: "api/{object1}/{id1}/{controller}"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MessageHandlers.Add(new MethodOverrideHandler());
        }
    }
}
