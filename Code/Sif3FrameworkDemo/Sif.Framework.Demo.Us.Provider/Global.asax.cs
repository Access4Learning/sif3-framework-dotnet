using Sif.Framework.Demo.Us.Provider.Models;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Xml.Serialization;

namespace Sif.Framework.Demo.Us.Provider
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private IRegistrationService registrationService;
        private void Register()
        {
            registrationService = RegistrationManager.ProviderRegistrationService;
            registrationService.Register();
        }
        private void Unregister()
        {
            registrationService.Unregister();
        }
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.AddUriPathExtensionMapping("json", "application/json");
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.AddUriPathExtensionMapping("xml", "text/xml");

            XmlMediaTypeFormatter formatter = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            formatter.UseXmlSerializer = true;

            XmlRootAttribute k12StudentsXmlRootAttribute = new XmlRootAttribute("K12Students") { Namespace = SettingsManager.ProviderSettings.DataModelNamespace, IsNullable = false };
            ISerialiser<List<K12Student>> k12StudentsSerialiser = SerialiserFactory.GetXmlSerialiser<List<K12Student>>(k12StudentsXmlRootAttribute);
            formatter.SetSerializer<List<K12Student>>((XmlSerializer)k12StudentsSerialiser);

            // Configure global exception loggers for unexpected errors.
            GlobalConfiguration.Configuration.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());

            // Configure a global exception handler for unexpected errors.
            GlobalConfiguration.Configuration.Services.Replace(typeof(IExceptionHandler), new GlobalUnexpectedExceptionHandler());

            Trace.TraceInformation("********** Application_Start **********");
            Register();
        }
        protected void Application_End(object sender, System.EventArgs e)
        {
            Trace.TraceInformation("********** Application_End **********");
            Unregister();
        }
    }
}
