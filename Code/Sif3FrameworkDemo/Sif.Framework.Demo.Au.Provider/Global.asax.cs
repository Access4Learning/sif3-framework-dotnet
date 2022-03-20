using Sif.Framework.AspNet.Handlers;
using Sif.Framework.AspNet.Loggers;
using Sif.Framework.AspNet.MediaTypeFormatters;
using Sif.Framework.Demo.Au.Provider.Models;
using Sif.Framework.Demo.Au.Provider.Utils;
using Sif.Framework.Model.Settings;
using Sif.Framework.Service.Registration;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Service.Sessions;
using Sif.Framework.Utils;
using System;
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

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            IFrameworkSettings settings = FrameworkConfigFactory.CreateSettings();

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
            Register(settings, FrameworkConfigFactory.CreateSessionService());
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Trace.TraceInformation("********** Application_End **********");
            Unregister();
        }

        /// <summary>
        /// Register this SIF Provider with the EnvironmentProvider.
        /// </summary>
        /// <param name="settings">Application settings associated with the Provider.</param>
        /// <param name="sessionService">Service associated with Provider sessions.</param>
        private void Register(IFrameworkSettings settings, ISessionService sessionService)
        {
            registrationService = RegistrationManager.GetProviderRegistrationService(settings, sessionService);
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