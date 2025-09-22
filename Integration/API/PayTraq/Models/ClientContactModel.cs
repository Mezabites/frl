using eFlex.Index.Base.Models.Admin;
using System.Xml.Serialization;

namespace frlnet.Integration.API.PayTraq.Models
{
    [XmlRoot("Contacts/Contact")]
    public class ClientContactModel : frlnet.Models.frl.ClientContactModel
    {
        [XmlElement("ContactID")]
        [Map(Unique = true)]
        public override string ExternalId { get; set; }

        [XmlElement]
        public override string Name { get; set; }

        [XmlElement]
        public override string Email { get; set; }

        [XmlElement]
        public override string? Phone { get; set; }

        [XmlElement] 
        public override string? AdditionalInfo { get; set; }

        [XmlElement]
        public override bool IsDefault { get; set; }
    }
}