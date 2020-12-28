using Sif.Framework.Demo.Au.Provider.Models;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Settings;
using Sif.Framework.Utils;
using Sif.Framework.WebApi;
using Sif.Framework.WebApi.MediaTypeFormatters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Xml.Serialization;
using Tardigrade.Framework.Configurations;
using Tardigrade.Framework.EntityFramework.Configurations;

namespace Sif.Framework.Demo.Au.Provider
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private IRegistrationService registrationService;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            IFrameworkSettings settings = GetSettings();

            // URL Postfix Extension: Update the configuration to recognise postfix extensions and map known
            // extensions to MIME Types. Additional changes to WebApiConfig.cs are required to fully enable this
            // feature.
            GlobalConfiguration.Configuration.Formatters.JsonFormatter
                .AddUriPathExtensionMapping("json", "application/json");
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.AddUriPathExtensionMapping("xml", "text/xml");

            // XML Serialisation: Define the specific XML serialiser to use to ensure that SIF Data Model Objects (as
            // defined by the SIF Specification with XML Schema Definitions (XSDs)) are serialised correctly.
            XmlMediaTypeFormatter xmlFormatter = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            xmlFormatter.UseXmlSerializer = true;

            // XML Serialisation: For each SIF Data Model Object used by each SIF Provider, the following entries are
            // required to define the root element for each collection object.
            var schoolInfosXmlRootAttribute = new XmlRootAttribute("SchoolInfos")
            { Namespace = settings.DataModelNamespace, IsNullable = false };
            ISerialiser<List<SchoolInfo>> schoolInfosSerialiser =
                SerialiserFactory.GetXmlSerialiser<List<SchoolInfo>>(schoolInfosXmlRootAttribute);
            xmlFormatter.SetSerializer<List<SchoolInfo>>((XmlSerializer)schoolInfosSerialiser);

            var studentPersonalsXmlRootAttribute = new XmlRootAttribute("StudentPersonals")
            { Namespace = settings.DataModelNamespace, IsNullable = false };
            ISerialiser<List<StudentPersonal>> studentPersonalsSerialiser =
                SerialiserFactory.GetXmlSerialiser<List<StudentPersonal>>(studentPersonalsXmlRootAttribute);
            xmlFormatter.SetSerializer<List<StudentPersonal>>((XmlSerializer)studentPersonalsSerialiser);

            var studentSchoolEnrollmentsXmlRootAttribute = new XmlRootAttribute("StudentSchoolEnrollments")
            { Namespace = settings.DataModelNamespace, IsNullable = false };
            ISerialiser<List<StudentSchoolEnrollment>> studentSchoolEnrollmentsSerialiser =
                SerialiserFactory.GetXmlSerialiser<List<StudentSchoolEnrollment>>(
                    studentSchoolEnrollmentsXmlRootAttribute);
            xmlFormatter.SetSerializer<List<StudentSchoolEnrollment>>(
                (XmlSerializer)studentSchoolEnrollmentsSerialiser);

            // Replacement custom JSON formatter (compliant with Goessner notation).
            var xmlToJsonFormatter = new XmlToJsonFormatter { UseXmlSerializer = true };
            xmlToJsonFormatter.AddUriPathExtensionMapping("json", "application/json");
            xmlToJsonFormatter.SetSerializer<List<SchoolInfo>>((XmlSerializer)schoolInfosSerialiser);
            xmlToJsonFormatter.SetSerializer<List<StudentPersonal>>((XmlSerializer)studentPersonalsSerialiser);
            xmlToJsonFormatter.SetSerializer<List<StudentSchoolEnrollment>>(
                (XmlSerializer)studentSchoolEnrollmentsSerialiser);
            GlobalConfiguration.Configuration.Formatters.Add(xmlToJsonFormatter);
            GlobalConfiguration.Configuration.Formatters.Remove(
                GlobalConfiguration.Configuration.Formatters.JsonFormatter);

            // Alternative 1.
            //var xmlRootAttribute = new XmlRootAttribute("StudentPersonals")
            //{ Namespace = settings.DataModelNamespace, IsNullable = false };
            //xmlFormatter.SetSerializer<List<StudentPersonal>>(
            //    new XmlSerializer(typeof(List<StudentPersonal>), xmlRootAttribute));

            // Alternative 2.
            //var attributes = new XmlAttributes();
            //attributes.XmlRoot = new XmlRootAttribute("StudentPersonals")
            //{ Namespace = settings.DataModelNamespace, IsNullable = false };
            //var overrides = new XmlAttributeOverrides();
            //overrides.Add(typeof(List<StudentPersonal>), attributes);
            //xmlFormatter
            //    .SetSerializer<List<StudentPersonal>>(new XmlSerializer(typeof(List<StudentPersonal>), overrides));

            // Configure global exception loggers for unexpected errors.
            GlobalConfiguration.Configuration.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());

            // Configure a global exception handler for unexpected errors.
            GlobalConfiguration.Configuration.Services
                .Replace(typeof(IExceptionHandler), new GlobalUnexpectedExceptionHandler());

            Trace.TraceInformation("********** Application_Start **********");
            Register(settings);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Trace.TraceInformation("********** Application_End **********");
            Unregister();
        }

        private IFrameworkSettings GetSettings()
        {
            IFrameworkSettings settings;
            string settingsSource = System.Configuration.ConfigurationManager.AppSettings["demo.settingsSource"];

            if ("Database".Equals(settingsSource, StringComparison.InvariantCultureIgnoreCase))
            {
                settings = new ProviderSettings(new ApplicationConfiguration(new AppSettingsConfigurationSource("name=SettingsDb")));
            }
            else if ("File".Equals(settingsSource, StringComparison.InvariantCultureIgnoreCase))
            {
                settings = SettingsManager.ProviderSettings;
            }
            else
            {
                settings = SettingsManager.ProviderSettings;
            }

            return settings;
        }

        /// <summary>
        /// Register this SIF Provider with the EnvironmentProvider based upon settings defined in the SIF 3.0
        /// Framework configuration, e.g. SifFramework.config.
        /// </summary>
        private void Register(IFrameworkSettings settings)
        {
            registrationService = RegistrationManager.GetProviderRegistrationService(
                settings,
                SessionsManager.ProviderSessionService);
            registrationService.Register();
        }

        /// <summary>
        /// Unregister this SIF Provider from the EnvironmentProvider.
        /// </summary>
        private void Unregister()
        {
            registrationService.Unregister();
        }
    }
}