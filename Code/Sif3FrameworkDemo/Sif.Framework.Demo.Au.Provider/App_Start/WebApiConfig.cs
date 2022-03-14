using Sif.Framework.AspNet.ControllerTypeResolvers;
using Sif.Framework.WebApi;
using Sif.Framework.WebApi.ControllerSelectors;
using Sif.Framework.WebApi.Handlers;
using Sif.Framework.WebApi.RouteConstraints;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace Sif.Framework.Demo.Au.Provider
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Register the SegmentPrefixConstraint for matching an exact segment prefix.
            var constraintResolver = new DefaultInlineConstraintResolver();
            constraintResolver.ConstraintMap.Add("SegmentPrefix", typeof(SegmentPrefixConstraint));

            // Web API routes
            config.MapHttpAttributeRoutes(constraintResolver);

            config.Routes.MapHttpRoute(
                "UriPathExtensionApi",
                "api/{controller}.{ext}/{id}",
                new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                "ServicePathApi3",
                "api/{object1}/{id1}/{object2}/{id2}/{object3}/{id3}/{controller}"
            );

            config.Routes.MapHttpRoute("ServicePathApi2", "api/{object1}/{id1}/{object2}/{id2}/{controller}");

            config.Routes.MapHttpRoute("ServicePathApi1", "api/{object1}/{id1}/{controller}");

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            config.MessageHandlers.Insert(0, new CompressionHandler());
            config.MessageHandlers.Add(new MethodOverrideHandler());

            config.Services.Replace(
                typeof(IHttpControllerTypeResolver),
                new ServiceProviderHttpControllerTypeResolver());

            FieldInfo suffix = typeof(DefaultHttpControllerSelector)
                .GetField("ControllerSuffix", BindingFlags.Static | BindingFlags.Public);

            if (suffix != null) suffix.SetValue(null, "Provider");

            // Replace the default controller selector with a custom one which recognises matrix parameters.
            config.Services.Replace(
                typeof(IHttpControllerSelector),
                new ServiceProviderHttpControllerSelector(config));
        }
    }
}