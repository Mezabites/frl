using eFlex.Index.Base.Models.Admin;
using System.Xml.Serialization;

namespace frlnet.Integration.API.PayTraq.Models
{

    [XmlRoot("BankDetails/BankInfo")]
    public class ClientBankInfoModel : frlnet.Models.frl.ClientBankInfoModel
    {
        [Map(Unique = true)]
        [XmlElement("BankInfoID")]
        public override string? ExternalId { get => base.ExternalId; set => base.ExternalId = value; }

        [XmlElement]
        public override string BankAccount { get; set; }

        [XmlElement]
        public override string BankName { get; set; }

        [XmlElement]
        public override string BankCode { get; set; }

        [XmlElement]
        public override string? PaymentNote { get; set; }

        [XmlElement]
        public override bool IsDefault { get; set; }
    }
}