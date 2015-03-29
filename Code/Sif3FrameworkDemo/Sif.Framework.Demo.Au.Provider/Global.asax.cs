using Sif.Framework.Demo.Au.Provider.Models;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Xml.Serialization;

namespace Sif.Framework.Demo.Au.Provider
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

            ISerialiser<List<StudentPersonal>> studentPersonalsSerialiser = SerialiserFactory.GetXmlSerialiser<List<StudentPersonal>>(new XmlRootAttribute("StudentPersonals"));
            formatter.SetSerializer<List<StudentPersonal>>((XmlSerializer)studentPersonalsSerialiser);

            ISerialiser<List<SchoolInfo>> schoolInfosSerialiser = SerialiserFactory.GetXmlSerialiser<List<SchoolInfo>>(new XmlRootAttribute("SchoolInfos"));
            formatter.SetSerializer<List<SchoolInfo>>((XmlSerializer)schoolInfosSerialiser);

            // Alternative 1.
            //formatter.SetSerializer<List<StudentPersonal>>(new XmlSerializer(typeof(List<StudentPersonal>), new XmlRootAttribute("StudentPersonals")));

            // Alternative 2.
            //XmlAttributes attributes = new XmlAttributes();
            //attributes.XmlRoot = new XmlRootAttribute("StudentPersonals");
            //XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            //overrides.Add(typeof(List<StudentPersonal>), attr);
            //formatter.SetSerializer<List<StudentPersonal>>(new XmlSerializer(typeof(List<StudentPersonal>), overrides));

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
