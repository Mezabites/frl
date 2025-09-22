using Serilog.Formatting.Elasticsearch;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace frlnet.Integration.API.Serializers
{
    public class DeepXmlResponseSerializer : ResponseSerializer
    {
        public override string Serialize<T>(IEnumerable<T> obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            var roots = GetRoots<T>();

            var xDocument = new XDocument();

            var rootElement = new XElement(roots.First());
            var contentElement = rootElement;
            foreach (var root in roots.Skip(1))
            {
                var newElement = new XElement(root);
                contentElement.Add(newElement);
                contentElement = newElement;
            }

            foreach (var f in obj)
            {
                if (f is null)
                    continue;
                var element = Serialize(f, contentElement.Name.LocalName);
                contentElement.Add(element);
            }

            xDocument.Add(rootElement);
            return xDocument.ToString();
        }

        public override string Serialize<T>(T obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            var roots = GetRoots<T>();

            var xDocument = new XDocument();
            var element = Serialize(obj, roots.Last());
            xDocument.Add(element);
            return xDocument.ToString();
        }

        public override IEnumerable<T> Deserialize<T>(string xml)
        {
            var roots = GetRoots<T>();

            XDocument xDocument = XDocument.Parse(xml);
            var rootElement = xDocument.Root!;
            if (rootElement.Name.LocalName == roots.FirstOrDefault() && roots.Length > 1)
                roots = roots.Skip(1).ToArray();

            var elements = new XElement[] { xDocument.Root! };
            foreach (var fRoot in roots)
                elements = elements.SelectMany(f => f.Elements(fRoot)).ToArray();

            var objects = new List<T>();

            foreach (var element in elements)
            {
                var obj = (T)Activator.CreateInstance(typeof(T))!;
                Deserialize(element, obj);
                objects.Add(obj);
            }

            return objects;
        }

        protected void Deserialize<T>(XElement element, T obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            foreach (var property in obj.GetType().GetProperties())
            {
                var xmlElement = GetElementName(property);
                if (xmlElement is null)
                    continue;

                var subElement = element.Element(xmlElement);
                if (subElement is null)
                    continue;

                var strValue = subElement.Value;

                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(obj, strValue);
                }
                else if (property.PropertyType.IsEnum)
                {
                    var value = Convert.ChangeType(strValue, Enum.GetUnderlyingType(property.PropertyType));
                    property.SetValue(obj, value);
                }
                else if (!property.PropertyType.IsClass)
                {
                    var value = Convert.ChangeType(strValue, property.PropertyType);
                    property.SetValue(obj, value);
                }
                else if (property.PropertyType.IsClass)
                {
                    var childElement = element.Element(xmlElement);
                    if (childElement != null)
                    {
                        var childObj = Activator.CreateInstance(property.PropertyType);
                        Deserialize(childElement, childObj);
                        property.SetValue(obj, childObj);
                    }
                }
            }
        }


        protected XElement Serialize(object obj, string elementName)
        {
            var element = new XElement(elementName);
            foreach (var property in obj.GetType().GetProperties())
            {
                var xmlElement = GetElementName(property);
                if (xmlElement is null)
                    continue;

                var value = property.GetValue(obj);
                if (value == null) continue;

                if (property.PropertyType == typeof(string) || !property.PropertyType.IsClass)
                {
                    element.Add(new XElement(xmlElement, value));
                }
                else if (property.PropertyType.IsClass)
                {
                    var childElement = Serialize(value, xmlElement);
                    element.Add(childElement);
                }
            }

            return element;
        }

        public override HttpContent SerializeToContent<T>(T obj) => new StringContent(Serialize(obj), Encoding.UTF8, "application/xml");

        private string? GetElementName(PropertyInfo property)
        {
            var xmlElement = property.GetCustomAttribute<XmlElementAttribute>()?.ElementName;
            if (xmlElement is null)
                return null;

            if (string.IsNullOrEmpty(xmlElement))
                xmlElement = property.Name;

            return xmlElement;
        }

        private static string[] GetRoots<T>()
        {
            var rootAttribute = typeof(T).GetCustomAttribute<XmlRootAttribute>();
            if (rootAttribute == null)
                return [];

            return rootAttribute.ElementName.Split('/').ToArray();
        }


    }


}
