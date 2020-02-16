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
    /// <summary>
    /// XML-based MediaTypeFormatter that serialises an object to XML and then JSON, and deserialises JSON to XML and
    /// then an object.
    /// </summary>
    public class XmlToJsonFormatter : XmlMediaTypeFormatter
    {
        /// <summary>
        /// Create an instance of this formatter.
        /// </summary>
        public XmlToJsonFormatter()
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/json"));
        }

        /// <summary>
        /// <see cref="MediaTypeFormatter.ReadFromStreamAsync(Type, Stream, HttpContent, IFormatterLogger)"/>
        /// </summary>
        public override Task<object> ReadFromStreamAsync(
            Type type,
            Stream readStream,
            HttpContent content,
            IFormatterLogger formatterLogger)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (readStream == null) throw new ArgumentNullException(nameof(readStream));

            object value = null;

            using (StreamReader streamReader = new StreamReader(readStream))
            {
                string json = streamReader.ReadToEnd();

                // If there is JSON data, deserialise into an object.
                if (!string.IsNullOrWhiteSpace(json))
                {
                    // Convert the JSON string into an XML document.
                    XmlDocument xmlDocument = JsonConvert.DeserializeXmlNode(json);

                    using (XmlReader xmlReader = new XmlNodeReader(xmlDocument))
                    {
                        // Deserialise the XML document into an object.
                        XmlSerializer xmlSerializer = (XmlSerializer)GetDeserializer(type, content);
                        value = xmlSerializer.Deserialize(xmlReader);
                    }
                }
            }

            return Task.FromResult(value);
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
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (writeStream == null) throw new ArgumentNullException(nameof(writeStream));

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

            // Convert the XML document into a JSON string.
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            string json = JsonConvert.SerializeXmlNode(xmlDocument);

            // Write the JSON string to the stream.
            byte[] buf = System.Text.Encoding.Default.GetBytes(json);
            writeStream.Write(buf, 0, buf.Length);
            writeStream.Flush();

            return Task.FromResult(writeStream);
        }
    }
}