using Sif.Framework.WebApi;
using Sif.Framework.WebApi.ControllerSelectors;
using Sif.Framework.WebApi.RouteConstraints;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace Sif.Framework.Demo.Broker
{

    public static class WebApiConfig
    {

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Matrix Parameters: Register the SegmentPrefixConstraint route constraint for matching an exact segment
            // prefix.
            DefaultInlineConstraintResolver constraintResolver = new DefaultInlineConstraintResolver();
            constraintResolver.ConstraintMap.Add("SegmentPrefix", typeof(SegmentPrefixConstraint));

            // Web API routes
            config.MapHttpAttributeRoutes(constraintResolver);

            // URL Postfix Extension: The following route configures a REST endpoint (SIF Provider) to accept the MIME
            // Type for a response using a postfix extension on the URL request. Additional changes to Global.asax.cs
            // are required to fully enable this feature.
            config.Routes.MapHttpRoute(
                name: "UriPathExtensionApi",
                routeTemplate: "api/{controller}.{ext}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Service Paths: The following routes will handle SIF Service Paths to a deth of 3 conditions.
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

            // Functional Services: The following routes will enable to the use of Functional Services.
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

            // Method Override: Add a handler to allow for the override of the POST and PUT methods to enable Query by
            // Example and multiple object deletes respectively.
            config.MessageHandlers.Add(new MethodOverrideHandler());

            // Provider Suffix: Configuration which allows ASP.NET Web API to recognise SIF Providers as REST
            // Controllers.
            config.Services.Replace(typeof(IHttpControllerTypeResolver), new ServiceProviderHttpControllerTypeResolver());
            FieldInfo suffix = typeof(DefaultHttpControllerSelector).GetField("ControllerSuffix", BindingFlags.Static | BindingFlags.Public);
            if (suffix != null) suffix.SetValue(null, "Provider");

            // Matrix Parameters: Replace the default controller selector with a custom one which recognises Matrix Parameters.
            config.Services.Replace(typeof(IHttpControllerSelector), new ServiceProviderHttpControllerSelector(config));
        }

    }

}
