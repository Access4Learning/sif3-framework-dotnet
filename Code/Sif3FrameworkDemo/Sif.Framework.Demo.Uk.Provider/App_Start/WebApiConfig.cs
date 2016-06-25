using Sif.Framework.WebApi;
using Sif.Framework.WebApi.ControllerSelectors;
using Sif.Framework.WebApi.RouteConstraints;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace Sif.Framework.Demo.Uk.Provider
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Register the SegmentPrefixConstraint for matching an exact segment prefix.
            DefaultInlineConstraintResolver constraintResolver = new DefaultInlineConstraintResolver();
            constraintResolver.ConstraintMap.Add("SegmentPrefix", typeof(SegmentPrefixConstraint));

            // Web API routes
            config.MapHttpAttributeRoutes(constraintResolver);

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

            /*
            config.Routes.MapHttpRoute(
                name: "FunctionalServicePathApi3",
                routeTemplate: "api/{controller}/{id}/phase/{name}"
            );

            config.Routes.MapHttpRoute(
                name: "FunctionalServicePathApi2",
                routeTemplate: "api/{controller}/{id}"
            );

            config.Routes.MapHttpRoute(
                name: "FunctionalServicePathApi1",
                routeTemplate: "api/{controller}"
            );
            */

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ServicesRoute",
                routeTemplate: "services/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ServiceStatesRoute",
                routeTemplate: "services/{controller}/{id}/{phaseName}/states/{stateId}",
                defaults: new { stateId = RouteParameter.Optional }
            );

            config.MessageHandlers.Add(new MethodOverrideHandler());

            config.Services.Replace(typeof(IHttpControllerTypeResolver), new ServiceProviderHttpControllerTypeResolver());

            FieldInfo suffix = typeof(DefaultHttpControllerSelector).GetField("ControllerSuffix", BindingFlags.Static | BindingFlags.Public);
            if (suffix != null) suffix.SetValue(null, "Provider");

            // Replace the default controller selector with a custom one which recognises matrix parameters.
            config.Services.Replace(typeof(IHttpControllerSelector), new ServiceProviderHttpControllerSelector(config));
        }
    }
}
