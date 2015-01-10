using Sif.Framework.Demo.Us.Provider.Models;
using Sif.Framework.Service.Serialisation;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Xml.Serialization;

namespace Sif.Framework.Demo.Us.Provider
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            XmlMediaTypeFormatter formatter = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            formatter.UseXmlSerializer = true;

            ISerialiser<List<K12Student>> k12StudentsSerialiser = SerialiserFactory.GetXmlSerialiser<List<K12Student>>(new XmlRootAttribute("K12Students"));
            formatter.SetSerializer<List<K12Student>>((XmlSerializer)k12StudentsSerialiser);
        }
    }
}
