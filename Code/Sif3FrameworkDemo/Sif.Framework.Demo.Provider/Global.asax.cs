using System.Net.Http.Formatting;
using System.Web.Http;

namespace Sif.Framework.Demo.Provider
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            XmlMediaTypeFormatter formatter = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            formatter.UseXmlSerializer = true;
        }
    }
}
