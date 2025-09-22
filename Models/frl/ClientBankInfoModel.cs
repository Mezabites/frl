using frlnet.Integration.API;

namespace frlnet.Models.frl
{
    [MapSource("frl", "ClientBankInfo")]
    [MapJoin("frl", "Client")]
    public class ClientBankInfoModel : ApiModelBase
    {
        [Map("ClientId")] 
        public override Guid ParentId { get; set; }

        [Map(Unique = true)] public override string? ExternalId { get; set; }

        [TextBoxSettings(Regex = Constants.RegExtIban)]
        [Show][Map] public virtual string BankAccount { get; set; }

        [Show][Map] public virtual string BankName { get; set; }

        [TextBoxSettings(Regex = Constants.RegExtSwift)]
        [Show][Map] public virtual string BankCode { get; set; }

        [Show][Map] public virtual string? PaymentNote { get; set; }

        [Show][Map] public virtual bool IsDefault { get; set; } = true;
        public bool IsDefaultVisible
        {
            get
            {
                var count = AutoProcedure.Of<ClientBankInfoModel>().GetRange(new SqlWhereCondition(nameof(ParentId), ParentId)).Count();
                if (Id.HasValue) return count > 1;
                return count > 0;
            }
        }

        [Map]
        public override Guid? LinkId { get; set; }

        public override ApiChildModelMeta[] ChildModels => [];

        [Map("ClientOrganizationId")]
        public override Guid OrganizationId { get; set; }
    }
}