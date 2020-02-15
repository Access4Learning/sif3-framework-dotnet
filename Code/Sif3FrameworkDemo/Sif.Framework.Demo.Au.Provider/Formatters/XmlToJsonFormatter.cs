using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Sif.Framework.Demo.Au.Provider.Formatters
{
    public class XmlToJsonFormatter : XmlMediaTypeFormatter
    {
        public XmlToJsonFormatter()
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            return base.ReadFromStreamAsync(type, readStream, content, formatterLogger);
        }

        /// <summary>
        /// <see cref="MediaTypeFormatter.WriteToStreamAsync(Type, object, Stream, HttpContent, TransportContext)"/>
        /// </summary>
        public override Task WriteToStreamAsync(
            Type type,
            object value,
            Stream writeStream,
            HttpContent content,
            TransportContext transportContext)
        {
            string xml;

            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
                {
                    // Serialise the object into an XML string.
                    XmlSerializer xmlSerialiser = (XmlSerializer)GetSerializer(type, value, content);
                    xmlSerialiser.Serialize(xmlWriter, value);
                    xml = stringWriter.ToString();
                }
            }

            // Parse the XML string and remove empty elements (defined by the "xsi:nil" element).
            XElement xElement = XElement.Parse(xml);
            xElement
                .Descendants()
                .Where(x =>
                    string.IsNullOrEmpty(x.Value) &&
                    x.Attributes().Where(y => y.Name.LocalName == "nil" && y.Value == "true").Count() > 0)
                .Remove();
            xml = xElement.ToString();

            // Convert the XML into JSON.
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            string json = JsonConvert.SerializeXmlNode(xmlDocument);

            // Write the JSON to the stream.
            byte[] buf = System.Text.Encoding.Default.GetBytes(json);
            writeStream.Write(buf, 0, buf.Length);
            writeStream.Flush();

            return Task.FromResult(writeStream);
        }
    }
}