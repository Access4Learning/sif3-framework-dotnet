using Sif.Framework.Utils;
using Sif.Framework.WebApi;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Sif.Framework.EnvironmentProvider
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            XmlMediaTypeFormatter formatter = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            formatter.UseXmlSerializer = true;

            // Configure global exception loggers for unexpected errors.
            GlobalConfiguration.Configuration.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());

            // Configure a global exception handler for unexpected errors.
            GlobalConfiguration.Configuration.Services.Replace(typeof(IExceptionHandler), new GlobalUnexpectedExceptionHandler());
        }
    }
}
