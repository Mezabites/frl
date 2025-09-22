using frlnet.Controllers.frl;
using frlnet.Integration.API;

namespace frlnet.Models.frl
{
    [MapSource("frl", "Client")]
    public class ClientModel : ApiModelBase
    {

        [Show]
        [Map]
        public virtual string? RegNumber { get; set; }

        [Show(In.Create)]
        [ButtonSettings]
        public Link Search { get; set; } = new Link(nameof(ClientController.Search), typeof(ClientController));

        [Map(Unique = true)] public override string? ExternalId { get; set; }

        [Show]
        [Map]
        public virtual string Name { get; set; }

        [Show(In.Filter)]
        [LabelGroupMark(nameof(DetailedFilter), false)]
        public object DetailedFilter { get; }

        [Show]
        [Map]
        [TextBoxSettings(Regex = Constants.RegExtEmail)]
        public virtual string? Email { get; set; }

        [Show][Map] public virtual eType Type { get; set; } = eType.Corporate;
        public enum eType : byte
        {
            Individual = 1,
            Corporate = 2,
        }

        [Show][Map] public virtual eStatus Status { get; set; }
        public enum eStatus : byte
        {
            Prospective = 1,
            Active = 2,
            Inactive = 3,
        }


        [ShowMark(In.Filter | In.Grid | In.Edit | In.Create)]

        [Show]
        [Map]
        public virtual string? WatNumber { get; set; }

        [Show]
        [Map]
        public virtual string? Phone { get; set; }

        [Show]
        [Map]
        public virtual string? InvoiceInfo { get; set; }

        [Map]
        public override Guid? LinkId { get; set; }

        [Show(In.Edit)]
        public bool IsLinked => LinkId.HasValue;

        [Map("OrganizationId")]
        public override Guid ParentId { get; set; }

        [ShowMark(In.Edit)]

        [Show]
        [LabelTabMark]
        [DynamicUpdateIgnore]
        public Link BankInfo { get; } = Link.ToIndex<ClientBankInfoController>();

        [Show]
        [LabelTabMark]
        [DynamicUpdateIgnore]
        public Link Contact { get; } = Link.ToIndex<ClientContactController>();

        public override ApiChildModelMeta[] ChildModels => [
            new(typeof(ClientContactModel), nameof(ClientContactModel.Email)),
            new (typeof(ClientBankInfoModel), nameof(ClientBankInfoModel.BankAccount))
            ];

        public override Guid OrganizationId { get => ParentId; set => ParentId = value; }



        // [LabelGroupMark]
        //[Show]public object? Address { get; set; }

        public override string ToString()
        {
            return $"{Name} {RegNumber}";
        }

    }


    //public abstract class ClientProcedure : AutoProcedure<ClientModel>
    //{
    //    public override void Update(ClientModel model)
    //    {
    //        var apiCom = ApiSync.GetApiCom(model.OrganizationId!.Value);
    //        if (apiCom is not null)
    //        {
    //            var orgModel = Of<OrganizationModel>().Get(model.OrganizationId!.Value);
    //            apiCom.UpdateSingle<ClientModel>(model, null);
    //            ApiSync.Sync<ClientModel>(apiCom.SyncModel, orgModel!);
    //        }
    //        else
    //        {
    //            base.Update(model);
    //        }
    //    }

    //    public override void Insert(IEnumerable<ClientModel> models)
    //    {
    //        foreach (var model in models)
    //        {
    //            var apiCom = ApiSync.GetApiCom(model.OrganizationId!.Value);
    //            if (apiCom is not null)
    //            {
    //                var orgModel = Of<OrganizationModel>().Get(model.OrganizationId!.Value);
    //                apiCom.InsertSingle<ClientModel>(model, null);
    //                ApiSync.Sync<ClientModel>(apiCom.SyncModel, orgModel!);
    //            }
    //            else
    //            {
    //                base.Insert(models);
    //            }
    //        }
    //    }
    //}

}
