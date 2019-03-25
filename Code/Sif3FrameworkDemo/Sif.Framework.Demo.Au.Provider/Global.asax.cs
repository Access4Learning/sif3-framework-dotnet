using Sif.Framework.Demo.Au.Provider.Models;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using Sif.Framework.WebApi;
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

            XmlRootAttribute submissionsXmlRootAttribute = new XmlRootAttribute("FinancialQuestionnaireSubmissions") { Namespace = SettingsManager.ProviderSettings.DataModelNamespace, IsNullable = false };
            ISerialiser<List<FinancialQuestionnaireSubmission>> submissionsSerialiser = SerialiserFactory.GetXmlSerialiser<List<FinancialQuestionnaireSubmission>>(submissionsXmlRootAttribute);
            formatter.SetSerializer<List<FinancialQuestionnaireSubmission>>((XmlSerializer)submissionsSerialiser);

            XmlRootAttribute schoolInfosXmlRootAttribute = new XmlRootAttribute("SchoolInfos") { Namespace = SettingsManager.ProviderSettings.DataModelNamespace, IsNullable = false };
            ISerialiser<List<SchoolInfo>> schoolInfosSerialiser = SerialiserFactory.GetXmlSerialiser<List<SchoolInfo>>(schoolInfosXmlRootAttribute);
            formatter.SetSerializer<List<SchoolInfo>>((XmlSerializer)schoolInfosSerialiser);

            XmlRootAttribute studentPersonalsXmlRootAttribute = new XmlRootAttribute("StudentPersonals") { Namespace = SettingsManager.ProviderSettings.DataModelNamespace, IsNullable = false };
            ISerialiser<List<StudentPersonal>> studentPersonalsSerialiser = SerialiserFactory.GetXmlSerialiser<List<StudentPersonal>>(studentPersonalsXmlRootAttribute);
            formatter.SetSerializer<List<StudentPersonal>>((XmlSerializer)studentPersonalsSerialiser);

            XmlRootAttribute studentSchoolEnrollmentsXmlRootAttribute = new XmlRootAttribute("StudentSchoolEnrollments") { Namespace = SettingsManager.ProviderSettings.DataModelNamespace, IsNullable = false };
            ISerialiser<List<StudentSchoolEnrollment>> studentSchoolEnrollmentsSerialiser = SerialiserFactory.GetXmlSerialiser<List<StudentSchoolEnrollment>>(studentSchoolEnrollmentsXmlRootAttribute);
            formatter.SetSerializer<List<StudentSchoolEnrollment>>((XmlSerializer)studentSchoolEnrollmentsSerialiser);

            // Alternative 1.
            //XmlRootAttribute xmlRootAttribute = new XmlRootAttribute("StudentPersonals") { Namespace = SettingsManager.ProviderSettings.DataModelNamespace, IsNullable = false };
            //formatter.SetSerializer<List<StudentPersonal>>(new XmlSerializer(typeof(List<StudentPersonal>), xmlRootAttribute));

            // Alternative 2.
            //XmlAttributes attributes = new XmlAttributes();
            //attributes.XmlRoot = new XmlRootAttribute("StudentPersonals") { Namespace = SettingsManager.ProviderSettings.DataModelNamespace, IsNullable = false };
            //XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            //overrides.Add(typeof(List<StudentPersonal>), attributes);
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