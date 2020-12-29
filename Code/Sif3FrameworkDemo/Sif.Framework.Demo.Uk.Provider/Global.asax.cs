using Sif.Framework.Demo.Uk.Provider.Models;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using Sif.Framework.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Routing;
using System.Xml.Serialization;
using WebApiContrib.Formatting;

namespace Sif.Framework.Demo.Uk.Provider
{
    public class WebApiApplication : HttpApplication
    {
        private readonly slf4net.ILogger log =
            slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IRegistrationService registrationService;

        private void Register()
        {
            if (registrationService == null)
            {
                log.Info("Getting registration manager...");
                registrationService = RegistrationManager.GetProviderRegistrationService(
                    SettingsManager.ProviderSettings,
                    SessionsManager.ProviderSessionService);
            }

            if (!registrationService.Registered)
            {
                log.Info("Registering...");
                registrationService.Register();
            }
        }

        private void Unregister()
        {
            log.Info("Unregistering...");
            registrationService.Unregister();
        }

        public override void Init()
        {
            base.Init();
            AcquireRequestState += ShowRouteValues;
        }

        protected void ShowRouteValues(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            if (context == null) return;

            // Add a breakpoint here to debug routing
            RouteData _ = RouteTable.Routes.GetRouteData(new HttpContextWrapper(context));
        }

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            GlobalConfiguration.Configuration.Formatters.JsonFormatter
                .AddUriPathExtensionMapping("json", "application/json");
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.AddUriPathExtensionMapping("xml", "text/xml");

            // Add a text/plain formatter (WebApiContrib also contains CSV and other formatters)
            GlobalConfiguration.Configuration.Formatters.Add(new PlainTextFormatter());

            XmlMediaTypeFormatter formatter = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            formatter.UseXmlSerializer = true;

            // Set up serializer configuration for data object:
            var studentPersonalsXmlRootAttribute = new XmlRootAttribute("LearnerPersonals")
            { Namespace = SettingsManager.ProviderSettings.DataModelNamespace, IsNullable = false };
            ISerialiser<List<LearnerPersonal>> studentPersonalsSerialiser =
                SerialiserFactory.GetXmlSerialiser<List<LearnerPersonal>>(studentPersonalsXmlRootAttribute);
            formatter.SetSerializer<List<LearnerPersonal>>((XmlSerializer)studentPersonalsSerialiser);

            // Configure global exception loggers for unexpected errors.
            GlobalConfiguration.Configuration.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());

            // Configure a global exception handler for unexpected errors.
            GlobalConfiguration.Configuration.Services
                .Replace(typeof(IExceptionHandler), new GlobalUnexpectedExceptionHandler());

            log.Info("********** Application_Start **********");
            Register();
        }

        protected void Application_End(object sender, System.EventArgs e)
        {
            log.Info("********** Application_End **********");
            Unregister();
        }
    }
}