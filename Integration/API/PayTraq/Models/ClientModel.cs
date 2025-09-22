using DocumentFormat.OpenXml.EMMA;
using eFlex.Index.Base.Models.Admin;
using System.Xml.Serialization;

namespace frlnet.Integration.API.PayTraq.Models
{

    //[XmlRoot("Clients")]
    //public class ClinetRootModel : ApiModel<frlnet.Models.frl.ClientModel>
    //{
    //    [XmlElement("Client")]
    //    public ClientModel[]? Clients { get; set; }

    //    public override IEnumerable<frlnet.Models.frl.ClientModel> Parse(Guid? parentId)
    //    {
    //        return Clients?.SelectMany(f => f.Parse(parentId))??Enumerable.Empty<frlnet.Models.frl.ClientModel>();
    //    }
    //}

    [XmlRoot("Clients/Client")]
    public class ClientModel : frlnet.Models.frl.ClientModel
    {
        [Map(Unique = true)]
        [XmlElement("ClientID")]
        public override string? ExternalId { get; set; }
        
        public Guid? ParentId { get; set; }

        [XmlElement()]
        public override string Name { get => base.Name; set => base.Name = value; }

        [XmlElement()]
        public override string? Email { get; set; }

        [XmlElement()]
        public new byte Type { get => (byte)base.Type; set => base.Type = (eType)value; }

        [XmlElement()]
        public new byte Status { get => (byte)base.Status; set => base.Status = (eStatus)value; }

        [XmlElement()]
        public override string? RegNumber { get => base.RegNumber; set => base.RegNumber = string.IsNullOrEmpty(value) ? null : value; }

        [XmlElement()]
        public override string? WatNumber { get => base.WatNumber; set => base.WatNumber = value; }

        [XmlElement()]
        public override string? Phone { get => base.Phone; set => base.Phone = value; }

        [XmlElement()]
        public override string? InvoiceInfo { get => base.InvoiceInfo; set => base.InvoiceInfo = value; }

        [XmlElement()]
        public AddressModel? LegalAddress { get; set; }

        [XmlElement()]
        public GroupModel? ClientGroup { get; set; }

        //private TimeStampsModel? _TimeStamps;
        //[XmlElement()]
        //public TimeStampsModel TimeStamps
        //{
        //    get => _TimeStamps!; set
        //    {
        //        _TimeStamps = value;

        //        RowCreated = DateTimeOffset.Parse(TimeStamps.Created);
        //        if (string.IsNullOrEmpty(TimeStamps.Updated))
        //            TimeStamps.Updated = TimeStamps.Created;
        //        RowModified = DateTimeOffset.Parse(TimeStamps.Updated);
        //    }
        //}



    }

    public class AddressModel
    {
        [XmlElement()] public string? Address { get; set; }
        [XmlElement()] public string? Zip { get; set; }
        [XmlElement()] public string? Country { get; set; }
    }

    public class GroupModel
    {
        [XmlElement()] public string? GroupID { get; set; }
        [XmlElement()] public string? GroupName { get; set; }
    }

    public class TimeStampsModel
    {
        [XmlElement()] public string Created { get; set; }
        [XmlElement()] public string Updated { get; set; }
    }
}