## Goessner notation configuration for a SIF Provider

The following sample entry needs to be added to the Global.asax.cs file of the SIF Provider. For a full example, refer to the Global.asax.cs file of the SifFramework.Demo.Au.Provider project.

    XmlToJsonFormatter xmlToJsonFormatter = new XmlToJsonFormatter
    {
        UseXmlSerializer = true
    };
    xmlToJsonFormatter.AddUriPathExtensionMapping("json", "application/json");
    xmlToJsonFormatter.SetSerializer<List<StudentPersonal>>((XmlSerializer)studentPersonalsSerialiser);
    GlobalConfiguration.Configuration.Formatters.Add(xmlToJsonFormatter);
    GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.JsonFormatter);


In addition, to enable JSON payloads for SIF Events, the following entries in the SifFramework.config file of the SIF Provider need to be configured to specify JSON.

    <add key="provider.payload.accept" value="JSON"/>
    <add key="provider.payload.contentType" value="JSON"/>
