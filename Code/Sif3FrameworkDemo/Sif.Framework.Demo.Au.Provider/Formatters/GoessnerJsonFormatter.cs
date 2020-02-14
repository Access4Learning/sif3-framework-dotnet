using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sif.Framework.Service.Serialisation;
using Sif.Framework.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Sif.Framework.Demo.Au.Provider.Formatters
{
    public class GoessnerJsonFormatter : MediaTypeFormatter
    {
        public GoessnerJsonFormatter()
        {
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        }

        public override bool CanWriteType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return true;
        }

        public override bool CanReadType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return true;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var task = Task<object>.Factory.StartNew(() =>
            {
                var settings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };

                var sr = new StreamReader(readStream);
                var jreader = new JsonTextReader(sr);

                var ser = new JsonSerializer();
                ser.Converters.Add(new IsoDateTimeConverter());

                object val = ser.Deserialize(jreader, type);
                return val;
            });

            return task;
        }

        //protected override System.Threading.Tasks.Task<object> OnReadFromStreamAsync(Type type, System.IO.Stream stream, System.Net.Http.Headers.HttpContentHeaders contentHeaders, FormatterContext formatterContext)
        //{
        //    var task = Task<object>.Factory.StartNew(() =>
        //    {
        //        var settings = new JsonSerializerSettings()
        //        {
        //            NullValueHandling = NullValueHandling.Ignore,
        //        };

        //        var sr = new StreamReader(stream);
        //        var jreader = new JsonTextReader(sr);

        //        var ser = new JsonSerializer();
        //        ser.Converters.Add(new IsoDateTimeConverter());

        //        object val = ser.Deserialize(jreader, type);
        //        return val;
        //    });

        //    return task;
        //}

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var task = Task.Factory.StartNew(() =>
            {
                XmlRootAttribute studentPersonalsXmlRootAttribute = new XmlRootAttribute($"StudentPersonals") { Namespace = SettingsManager.ProviderSettings.DataModelNamespace, IsNullable = false };
                ISerialiser<List<Type>> studentPersonalsSerialiser = SerialiserFactory.GetXmlSerialiser<List<Type>>(studentPersonalsXmlRootAttribute);
                //formatter.SetSerializer<List<StudentPersonal>>((XmlSerializer)studentPersonalsSerialiser);

                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                string xml;
                XmlSerializer xmlSerialiser = new XmlSerializer(type);
                //XmlSerializer xmlSerialiser = (XmlSerializer)studentPersonalsSerialiser;
                using (var stringWriter = new StringWriter())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings() { OmitXmlDeclaration = true }))
                    {
                        xmlSerialiser.Serialize(xmlWriter, value, namespaces);
                        xml = stringWriter.ToString();
                    }
                }
                XElement xElement = XElement.Parse(xml);
                xElement.Descendants().Where(x => string.IsNullOrEmpty(x.Value) && x.Attributes().Where(y => y.Name.LocalName == "nil" && y.Value == "true").Count() > 0).Remove();
                xml = xElement.ToString();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                string json = JsonConvert.SerializeXmlNode(xmlDocument);

                //var settings = new JsonSerializerSettings()
                //{
                //    NullValueHandling = NullValueHandling.Ignore,
                //};

                //string json = JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented,
                //                                          new JsonConverter[1] { new IsoDateTimeConverter() });

                byte[] buf = System.Text.Encoding.Default.GetBytes(json);
                writeStream.Write(buf, 0, buf.Length);
                writeStream.Flush();
            });

            return task;
        }

        //protected override System.Threading.Tasks.Task OnWriteToStreamAsync(Type type, object value, System.IO.Stream stream, System.Net.Http.Headers.HttpContentHeaders contentHeaders, FormatterContext formatterContext, System.Net.TransportContext transportContext)
        //{
        //    var task = Task.Factory.StartNew(() =>
        //    {
        //        var settings = new JsonSerializerSettings()
        //        {
        //            NullValueHandling = NullValueHandling.Ignore,
        //        };

        //        string json = JsonConvert.SerializeObject(value, Formatting.Indented,
        //                                                  new JsonConverter[1] { new IsoDateTimeConverter() });

        //        byte[] buf = System.Text.Encoding.Default.GetBytes(json);
        //        stream.Write(buf, 0, buf.Length);
        //        stream.Flush();
        //    });

        //    return task;
        //}
    }
}