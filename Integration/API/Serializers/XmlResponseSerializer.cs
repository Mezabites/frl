using System.Text;
using System.Xml.Serialization;

namespace frlnet.Integration.API.Serializers
{
    public class XmlResponseSerializer : ResponseSerializer
    {
        // Serialize an object to XML string
        public override string Serialize<T>(IEnumerable<T> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (StringWriter stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }

        // Deserialize an XML string to an object
        public override IEnumerable<T> Deserialize<T>(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(nameof(str));
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (StringReader stringReader = new StringReader(str))
            {
                return (T[])xmlSerializer.Deserialize(stringReader)!;
            }
        }

        public override HttpContent SerializeToContent<T>(T obj) => new StringContent(Serialize([obj]), Encoding.UTF8, "application/xml");

        public override string Serialize<T>(T obj)
        {
            throw new NotImplementedException();
        }
    }
}
