using Sif.Framework.Demo.Au.DataModel;
using Sif.Framework.Service.Serialisation;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Xml.Serialization;

namespace Sif.Framework.Demo.Au.Provider
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            XmlMediaTypeFormatter formatter = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            formatter.UseXmlSerializer = true;

            ISerialiser<List<StudentPersonal>> studentPersonalsSerialiser = SerialiserFactory.GetXmlSerialiser<List<StudentPersonal>>(new XmlRootAttribute("StudentPersonals"));
            formatter.SetSerializer<List<StudentPersonal>>((XmlSerializer)studentPersonalsSerialiser);

            ISerialiser<List<Sbp.Framework.Model.SchoolInfo>> schoolInfosSerialiser = SerialiserFactory.GetXmlSerialiser<List<Sbp.Framework.Model.SchoolInfo>>(new XmlRootAttribute("SchoolInfos"));
            formatter.SetSerializer<List<Sbp.Framework.Model.SchoolInfo>>((XmlSerializer)schoolInfosSerialiser);

            // Alternative 1.
            //formatter.SetSerializer<List<StudentPersonal>>(new XmlSerializer(typeof(List<StudentPersonal>), new XmlRootAttribute("StudentPersonals")));

            // Alternative 2.
            //XmlAttributes attributes = new XmlAttributes();
            //attributes.XmlRoot = new XmlRootAttribute("StudentPersonals");
            //XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            //overrides.Add(typeof(List<StudentPersonal>), attr);
            //formatter.SetSerializer<List<StudentPersonal>>(new XmlSerializer(typeof(List<StudentPersonal>), overrides));
        }
    }
}
