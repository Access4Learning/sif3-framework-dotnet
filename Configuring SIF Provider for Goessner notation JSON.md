## Goessner notation configuration

The following sample entry needs to be added to the Global.asax.cs file of the SIF Provider. For a full example, refer to the Global.asax.cs file of the SifFramework.Demo.Au.Provider project.

    XmlToJsonFormatter xmlToJsonFormatter = new XmlToJsonFormatter
    {
        UseXmlSerializer = true
    };
    xmlToJsonFormatter.AddUriPathExtensionMapping("json", "application/json");
    xmlToJsonFormatter.SetSerializer<List<StudentPersonal>>((XmlSerializer)studentPersonalsSerialiser);
    GlobalConfiguration.Configuration.Formatters.Add(xmlToJsonFormatter);
    GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.JsonFormatter);
